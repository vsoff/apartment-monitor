using System;
using System.Collections.Generic;
using System.Linq;
using GMap.NET;

namespace Apartment.Common.Models
{
    public class ApartmentsRegion
    {
        public string Name;
        public readonly PointLatLng Center;
        public IReadOnlyCollection<PointLatLng> Locations => _locations;

        private readonly PointLatLng[] _locations;
        private RectLatLng _rect;

        public ApartmentsRegion(string name, IEnumerable<PointLatLng> locations)
        {
            if (locations == null) throw new ArgumentNullException(nameof(locations));
            _locations = locations.ToArray();
            if (_locations.Length == 0) throw new ArgumentOutOfRangeException(nameof(locations));

            Name = name;
            _rect = CoreModelsHelper.GetPointsRect(_locations);
            Center = _rect.LocationMiddle;
        }

        public bool Contains(PointLatLng location) => _rect.Contains(location) && Contains(_locations, location);

        public override string ToString() => $"[{Locations.Count}] {Name}";

        /// <summary>
        /// Определяет находится ли точка внутри полигона описывающего регион.
        /// </summary>
        private static bool Contains(IList<PointLatLng> polygon, PointLatLng testPoint)
        {
            if (polygon == null) throw new ArgumentNullException(nameof(polygon));
            if (polygon.Count < 3) throw new ArgumentOutOfRangeException(nameof(polygon));

            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].Lat < testPoint.Lat && polygon[j].Lat >= testPoint.Lat
                    || polygon[j].Lat < testPoint.Lat && polygon[i].Lat >= testPoint.Lat)
                {
                    if (polygon[i].Lng + (testPoint.Lat - polygon[i].Lat) / (polygon[j].Lat - polygon[i].Lat) * (polygon[j].Lng - polygon[i].Lng) < testPoint.Lng)
                    {
                        result = !result;
                    }
                }

                j = i;
            }

            return result;
        }
    }
}