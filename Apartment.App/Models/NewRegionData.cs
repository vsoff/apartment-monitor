using System.Collections.Generic;
using GMap.NET;

namespace Apartment.App.Models
{
    public class NewRegionData
    {
        public IReadOnlyCollection<PointLatLng> Locations { get; }

        public NewRegionData(IEnumerable<PointLatLng> locations)
        {
            Locations = new List<PointLatLng>(locations);
        }
    }
}