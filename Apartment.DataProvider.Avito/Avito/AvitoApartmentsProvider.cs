using System;
using System.Collections.Generic;
using System.Linq;
using Apartment.DataProvider.Avito.Common;
using Apartment.Options;
using Flurl.Http;
using GMap.NET;

namespace Apartment.DataProvider.Avito.Avito
{
    public class AvitoApartmentsProvider : IApartmentsProvider
    {
        private readonly ApplicationOptions _options;
        // URL для запроса подробной информации по ID объявления.
        // https://www.avito.ru/js/v2/map/pin/items?ids[0]=2042433758&ids[1]=2042569374&ids[2]=2035411698

        public AvitoApartmentsProvider(ApplicationOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IReadOnlyCollection<ApartmentData> GetApartments()
        {
            if (!Uri.IsWellFormedUriString(_options.AvitoUrl, UriKind.Absolute))
                return null;

            var content = _options.AvitoUrl.GetJsonAsync<MapApartmentsResponseWebModel>().GetAwaiter().GetResult();
            var apartments = content.rash.Select(x => new ApartmentData
            {
                Id = x.id,
                ItemsCount = x.itemsCount,
                Location = new PointLatLng
                {
                    Lat = x.coords.lat,
                    Lng = x.coords.lng,
                },
                PriceText = x.price.title,
                PriceValue = x.price.value
            }).ToArray();

            return apartments;
        }
    }
}