using System;
using System.Collections.Generic;
using System.Linq;
using Apartment.Common;
using Apartment.Common.Models;
using Apartment.Core.Extensions;
using GMap.NET;

namespace Apartment.App.Models
{
    public class ApartmentsGroup
    {
        public readonly bool HasNewest;
        public readonly bool AllIsOld;
        public readonly string Title;
        public readonly PointLatLng Location;
        public readonly IReadOnlyCollection<ApartmentInfo> Apartments;

        public ApartmentsGroup(IEnumerable<ApartmentInfo> apartments)
        {
            if (apartments == null) throw new ArgumentNullException(nameof(apartments));
            var apartmentList = new List<ApartmentInfo>(apartments);
            if (apartmentList.Count == 0) throw new ArgumentOutOfRangeException(nameof(apartments));
            Title = GetGroupTitle(apartmentList);
            Location = CoreModelsHelper.GetPointsRect(apartmentList.Select(x => x.Location).ToArray()).LocationMiddle;
            HasNewest = apartmentList.Any(x => x.IsNewest());
            AllIsOld = apartmentList.All(x => x.IsOld());
            Apartments = apartmentList;
        }

        private static string GetGroupTitle(ICollection<ApartmentInfo> apartments)
        {
            if (apartments.Count == 1)
                return $"{ToText(apartments.First().Price)}кк руб.";

            var orderedApartments = apartments.Where(x => x.Price.HasValue).OrderBy(x => x.Price).ToArray();
            if (orderedApartments.Length == 0)
                return $"[{apartments.Count} кв.] без цены";

            var priceMin = orderedApartments.First();
            var priceMax = orderedApartments.Last();

            return $"[{apartments.Count} кв.] {ToText(priceMin.Price)}-{ToText(priceMax.Price)}кк руб.";
        }

        private static string ToText(int? price) => price.HasValue ? (price.Value / (double) 1000000).ToString("F1") : "? ";

        public override string ToString() => Title;
    }
}