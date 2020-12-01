using System.Collections.Generic;
using Apartment.DataProvider.Models;

namespace Apartment.DataProvider
{
    public interface IApartmentsProvider
    {
        IReadOnlyCollection<ApartmentData> GetApartments();
    }
}