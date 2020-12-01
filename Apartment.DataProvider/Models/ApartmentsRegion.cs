using System;
using System.Collections.Generic;
using System.Linq;
using Apartment.DataProvider.Helpers;
using GMap.NET;

namespace Apartment.DataProvider.Models
{
    public class ApartmentsRegion
    {
        public string Name { get; set; }
        public PointLatLng Center { get; }
        private RectLatLng Rect { get; }
        public IReadOnlyCollection<PointLatLng> Locations { get; }

        public ApartmentsRegion(string name, IEnumerable<PointLatLng> locations)
        {
            if (locations == null) throw new ArgumentNullException(nameof(locations));
            if (!locations.Any()) throw new ArgumentOutOfRangeException(nameof(locations));

            var locationsList = new List<PointLatLng>(locations);
            var locationsInfo = CoreModelsHelper.GetMiddle(locationsList);

            Name = name;
            Locations = locationsList;
            Center = locationsInfo.Center;
            Rect = locationsInfo.Rect;
        }

        public bool Contains(PointLatLng location) => Rect.Contains(location);

        public override string ToString() => $"[{Locations.Count}] {Name}";
    }
}