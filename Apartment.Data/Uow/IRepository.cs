using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Apartment.Common;
using Apartment.Data.Entities;

namespace Apartment.Data.Uow
{
    public interface IRepository<T> : IRepository where T : Entity
    {
        Task<T> GetAsync(int id);
        Task<T[]> GetAsync(PartitionRequest partitionRequest);
        Task<T[]> GetAsync(Expression<Func<T, bool>> whereExpression, PartitionRequest partitionRequest = null);
        Task<T> AddAsync(T item);
        Task<T> UpdateAsync(T item);
        Task<T[]> UpdateAsync(IEnumerable<T> items);
        Task<T[]> AddAsync(IEnumerable<T> items);
        Task DeleteAsync(int id);
        Task<bool> AnyAsync(Expression<Func<T, bool>> whereExpression = null);
    }

    public interface IRepository
    {
    }
}