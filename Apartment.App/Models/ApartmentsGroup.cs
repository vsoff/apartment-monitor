using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;
using Apartment.DataProvider.Helpers;
using Apartment.DataProvider.Models;
using GMap.NET;

namespace Apartment.App.Models
{
    public class ApartmentsGroup
    {
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
            Apartments = apartmentList;
        }

        private static string GetGroupTitle(ICollection<ApartmentInfo> apartments)
        {
            if (apartments.Count == 1)
                return ToText(apartments.First().Price);

            var orderedApartments = apartments.OrderBy(x => x.Price).ToArray();
            var priceMin = orderedApartments.First();
            var priceMax = orderedApartments.Last();

            return $"[{apartments.Count} кв.] {ToText(priceMin.Price)}-{ToText(priceMax.Price)}кк руб.";
        }

        private static string ToText(int price) => (price / (double) 1000000).ToString("F1");

        public override string ToString() => Title;
    }
}