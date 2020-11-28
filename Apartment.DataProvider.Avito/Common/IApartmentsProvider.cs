using System.Collections.Generic;

namespace Apartment.DataProvider.Avito.Common
{
    public interface IApartmentsProvider
    {
        IReadOnlyCollection<ApartmentData> GetApartments();
    }
}