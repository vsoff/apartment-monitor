using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apartment.Common.Models;
using Apartment.Core.Services;
using Apartment.DataProvider;

namespace Apartment.Core.Providers
{
    public class DatabaseApartmentsProvider : IApartmentsProvider
    {
        private readonly ApartmentService _apartmentService;

        public DatabaseApartmentsProvider(ApartmentService apartmentService)
        {
            _apartmentService = apartmentService ?? throw new ArgumentNullException(nameof(apartmentService));
        }

        public async Task<ICollection<ApartmentInfo>> GetApartmentsAsync()
        {
            return await _apartmentService.GetActuallyApartmentsAsync();
        }
    }
}