using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apartment.Common.Loggers;
using Apartment.Common.Models;
using Apartment.Core.Mappers;
using Apartment.Data;
using Apartment.Data.Uow;

namespace Apartment.Core.Services
{
    public class RegionsService
    {
        private readonly IDatabaseContextProvider _contextProvider;

        public RegionsService(IDatabaseContextProvider contextProvider)
        {
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public async Task<Region> AddRegionAsync(Region region)
        {
            using var uow = new UnitOfWork(_contextProvider.Create());
            var newRegion = region.ToEntity();
            await uow.Regions.AddAsync(newRegion);
            await uow.SaveChangesAsync();
            return newRegion.ToCore();
        }

        public async Task<ICollection<Region>> GetAllRegionsAsync()
        {
            using var uow = new UnitOfWork(_contextProvider.Create());
            var regions = await uow.Regions.GetAsync(x => !x.IsDeleted);
            return regions.Select(x => x.ToCore()).ToArray();
        }

        public async Task<Region> UpdateRegionAsync(Region region)
        {
            using var uow = new UnitOfWork(_contextProvider.Create());
            var entity = region.ToEntity();
            await uow.Regions.UpdateAsync(entity);
            await uow.SaveChangesAsync();
            return entity.ToCore();
        }

        public async Task DeleteRegionAsync(int regionId)
        {
            using var uow = new UnitOfWork(_contextProvider.Create());
            await uow.Regions.DeleteAsync(regionId);
            await uow.SaveChangesAsync();
        }
    }
}