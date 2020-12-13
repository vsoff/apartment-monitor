using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Apartment.Common;
using Apartment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apartment.Data.Uow
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly Expression<Func<T, bool>> _notDeleted = x => !x.IsDeleted;
        private readonly Expression<Func<T, bool>> _defaultExpression = x => true;

        protected readonly ApplicationContext Context;

        public Repository(ApplicationContext context)
        {
            Context = context;
        }

        public Task<T> GetAsync(int id)
            => Context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task DeleteAsync(int id)
        {
            var item = await GetAsync(id);
            item.IsDeleted = true;
            await UpdateAsync(item);
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> whereExpression = null)
            => Context.Set<T>().Where(_notDeleted).AnyAsync(whereExpression ?? _defaultExpression);

        public Task<T[]> GetAsync(PartitionRequest partitionRequest)
            => Context.Set<T>().Where(_notDeleted).Skip(partitionRequest.Skip).Take(partitionRequest.Take).ToArrayAsync();

        public Task<T[]> GetAsync(Expression<Func<T, bool>> whereExpression, PartitionRequest partitionRequest = null)
            => partitionRequest == null
                ? Context.Set<T>().Where(_notDeleted).Where(whereExpression).ToArrayAsync()
                : Context.Set<T>().Where(_notDeleted).Where(whereExpression).Skip(partitionRequest.Skip).Take(partitionRequest.Take).ToArrayAsync();

        public async Task<T> AddAsync(T item)
            => (await AddAsync(new[] {item})).First();

        public async Task<T> UpdateAsync(T item)
        {
            var oldObj = await Context.Set<T>().Where(_notDeleted).FirstOrDefaultAsync(x => item.Id == x.Id);
            if (oldObj == null)
                throw new NullReferenceException("Невозможно обновить объект, которого нету в БД");

            Context.Entry(oldObj).State = EntityState.Detached;
            item.CreatedAtUtc = oldObj.CreatedAtUtc;
            item.UpdatedAtUtc = DateTime.UtcNow;
            Context.Entry(item).State = EntityState.Modified;
            return item;
        }

        public async Task<T[]> UpdateAsync(IEnumerable<T> items)
        {
            var newItemsMap = items.ToDictionary(x => x.Id, x => x);
            var newItemsIds = newItemsMap.Keys.ToArray();
            var oldItemsMap = await Context.Set<T>()
                .Where(_notDeleted)
                .Where(x => newItemsIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x);

            foreach (var (newItemId, newItem) in newItemsMap)
            {
                if (!oldItemsMap.TryGetValue(newItemId, out var oldItem))
                {
                    await Context.AddAsync(newItem);
                    continue;
                }

                Context.Entry(oldItem).State = EntityState.Detached;
                newItem.CreatedAtUtc = oldItem.CreatedAtUtc;
                newItem.UpdatedAtUtc = DateTime.UtcNow;
                Context.Entry(newItem).State = EntityState.Modified;
            }

            return newItemsMap.Select(x => x.Value).ToArray();
        }

        public async Task<T[]> AddAsync(IEnumerable<T> items)
        {
            List<T> newItems = new List<T>();

            foreach (var item in items)
            {
                var now = DateTime.UtcNow;
                item.CreatedAtUtc = item.CreatedAtUtc == DateTime.MinValue ? now : item.CreatedAtUtc;
                item.UpdatedAtUtc = item.UpdatedAtUtc == DateTime.MinValue ? now : item.UpdatedAtUtc;
                var newItem = (await Context.Set<T>().AddAsync(item)).Entity;
                Context.Entry(newItem).State = EntityState.Added;

                newItems.Add(newItem);
            }

            return newItems.ToArray();
        }
    }
}