using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public class NewRegionPoint
    {
        public PointLatLng Location { get; }

        public NewRegionPoint(PointLatLng location)
        {
            Location = location;
        }
    }

    public class NewRegionData
    {
        public IReadOnlyCollection<PointLatLng> Locations { get; }

        public NewRegionData(IEnumerable<PointLatLng> locations)
        {
            Locations = new List<PointLatLng>(locations);
        }
    }

    public class ApartmentsRegion
    {
        public string Name { get; set; }
        public IReadOnlyCollection<PointLatLng> Locations { get; }

        public ApartmentsRegion(string name, IEnumerable<PointLatLng> locations)
        {
            Name = name;
            Locations = new List<PointLatLng>(locations);
        }

        public override string ToString() => $"[{Locations.Count}] {Name}";
    }
}