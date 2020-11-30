using System.Collections.Generic;
using System.Linq;
using GMap.NET;

namespace Apartment.DataProvider.Avito.Common
{
    public class ApartmentData
    {
        public string Id { get; set; }
        public PointLatLng Location { get; set; }
        internal int ItemsCount { get; set; }
        public int PriceValue { get; set; }
        public string PriceText { get; set; }

        public override string ToString() => $"{Id}. [{ItemsCount}]: {PriceText}";
    }

    public class ApartmentsRegion
    {
        public string Name { get; }
        public IReadOnlyCollection<PointLatLng> Locations { get; }

        public ApartmentsRegion(string name, IEnumerable<PointLatLng> locations)
        {
            Name = name;
            Locations = locations.ToArray();
        }
    }
}