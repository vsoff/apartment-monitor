using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apartment.Data.Entities;

namespace Apartment.Data.Uow
{
    public class UnitOfWork : IDisposable
    {
        private readonly ApplicationContext _context;
        private readonly IReadOnlyDictionary<Type, IRepository> _repositoriesMap;

        public UnitOfWork(ApplicationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _repositoriesMap = new Dictionary<Type, IRepository>
            {
                [typeof(ApartmentEntity)] = new Repository<ApartmentEntity>(context),
                [typeof(ItemChangeEntity)] = new Repository<ItemChangeEntity>(context)
            };
        }

        public IRepository<T> Set<T>() where T : Entity
        {
            if (!_repositoriesMap.TryGetValue(typeof(T), out var repository))
                throw new ArgumentException($"Нет репозитория для типа {typeof(T)}");

            return (IRepository<T>) repository;
        }

        public IRepository<ApartmentEntity> Apartments => Set<ApartmentEntity>();
        public IRepository<ItemChangeEntity> ItemsChanges => Set<ItemChangeEntity>();

        public void Dispose() => _context.Dispose();
        public void SaveChanges() => _context.SaveChanges();
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}