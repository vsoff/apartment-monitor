using System;
using System.Collections.Generic;
using GMap.NET;

namespace Apartment.DataProvider.Models
{
    public class ApartmentData
    {
        public string Id { get; set; }
        public PointLatLng Location { get; set; }
        public int Price { get; set; }
        public string Url { get; set; }
        public DateTime PublishingDate { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public IReadOnlyCollection<string> ImageUrls { get; set; }

        public string PriceText => $"{Price / (double) 1000000:F1}кк руб.";

        public override string ToString() => $"{Title} ({PriceText})";
    }
}