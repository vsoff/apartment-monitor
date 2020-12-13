using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apartment.Core;
using Apartment.DataProvider.Models;

namespace Apartment.DataProvider.Avito
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