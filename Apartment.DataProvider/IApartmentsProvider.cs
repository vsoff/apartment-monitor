using System.Collections.Generic;
using System.Threading.Tasks;
using Apartment.Common.Models;

namespace Apartment.DataProvider
{
    public interface IApartmentsProvider
    {
        Task<ICollection<ApartmentInfo>> GetApartmentsAsync();
    }
}