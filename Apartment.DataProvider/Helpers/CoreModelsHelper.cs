using System;
using System.Collections.Generic;
using GMap.NET;

namespace Apartment.DataProvider.Helpers
{
    internal static class CoreModelsHelper
    {
        public static PointsCollectionInfo GetMiddle(ICollection<PointLatLng> points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));
            if (points.Count == 0) throw new ArgumentOutOfRangeException(nameof(points));

            double latMin = double.MaxValue;
            double latMax = double.MinValue;
            double lngMin = double.MaxValue;
            double lngMax = double.MinValue;
            foreach (var point in points)
            {
                latMin = Math.Min(latMin, point.Lat);
                latMax = Math.Max(latMax, point.Lat);
                lngMin = Math.Min(lngMin, point.Lng);
                lngMax = Math.Max(lngMax, point.Lng);
            }

            var rect = new RectLatLng(latMin, lngMin, latMax - latMin, lngMax - lngMin);
            var center = new PointLatLng((latMax + latMin) / 2, (lngMax + lngMin) / 2);
            return new PointsCollectionInfo(rect, center);
        }
    }
}