using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Apartment.DataProvider.Models;
using Flurl;
using Flurl.Http;
using GMap.NET;
using Newtonsoft.Json;

namespace Apartment.DataProvider.Avito
{
    public class AvitoApartmentsProvider : IApartmentsProvider
    {
        private const string RequestUrl = @"https://www.avito.ru/js/v2/map/items";

        private const string MaxPriceTag = "priceMax";
        private const string LimitTag = "limit";
        private const string PageTag = "page";

        private static readonly string RequestQueryParameters = $@"
categoryId: 24
locationId: 648220
{MaxPriceTag}: 2500000
correctorMode: 0
{PageTag}: 1
map: eyJzZWFyY2hBcmVhIjp7ImxhdEJvdHRvbSI6NjEuNzEyMDc3MzAwOTA2MjIsImxhdFRvcCI6NjEuODMzMzc2MDU1NTM0MzQ1LCJsb25MZWZ0IjozNC4yNTkzNTE2ODY2MTM5NTQsImxvblJpZ2h0IjozNC41Mjg1MTY3MjU2NzY0NzV9fQ==
params[201]: 1059
params[497][from]: 5184
params[497][to]: 0
filterCategoryId: 24
searchArea[latBottom]: 61.71207730090622
searchArea[lonLeft]: 34.259351686613954
searchArea[latTop]: 61.833376055534345
searchArea[lonRight]: 34.528516725676475
viewPort[width]: 784
viewPort[height]: 747
{LimitTag}: 10";

        public IReadOnlyCollection<ApartmentData> GetApartments()
        {
            const int limit = 50;
            const int maxPrice = 2500000;
            var items = new List<Item>(128);

#if DEBUG
            // Пока что кешируем данные, чтобы каждый раз не дёргать авито.
            const string debugCacheFile = "AvitoCacheFile3.json.cache";
            string jsonContent;
            if (File.Exists(debugCacheFile))
            {
                jsonContent = File.ReadAllText(debugCacheFile);
                var cacheItems = JsonConvert.DeserializeObject<List<Item>>(jsonContent);
                items.AddRange(cacheItems);
            }
            else
            {
#endif
                for (int page = 1;; page++)
                {
                    var url = BuildUrl(page, limit, maxPrice);

                    var response = url.GetJsonAsync<AvitoApartmentsResponse>().GetAwaiter().GetResult();
                    items.AddRange(response.items);

                    if (page * limit >= response.count)
                        break;
                }
#if DEBUG

                jsonContent = JsonConvert.SerializeObject(items);
                File.WriteAllText(debugCacheFile, jsonContent);
            }
#endif

            return items.Select(x => new ApartmentData
            {
                Id = x.itemId.ToString(),
                Location = new PointLatLng(x.coords.lat, x.coords.lng),
                Address = x.geo.formattedAddress,
                ImageUrls = x.images.Select(i => i.SmallSize).ToArray(),
                Price = x.price,
                PublishingDate = DateTimeOffset.FromUnixTimeSeconds(x.time).UtcDateTime,
                Title = x.title,
                Url = Url.Combine("https://www.avito.ru", x.url)
            }).ToArray();
        }

        /// <summary>
        /// Создаёт URL для запроса списка квартир.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Объектов на одной странице.</param>
        /// <param name="maxPrice">Максимальная цена квартиры.</param>
        /// <returns>URL запроса</returns>
        private string BuildUrl(int page, int limit, int maxPrice)
        {
            // Парсим параметры.
            var parameters = new Dictionary<string, string>();
            var lines = RequestQueryParameters.Replace("\r", string.Empty).Split('\n');
            foreach (var line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var delimiterIndex = line.IndexOf(':');
                if (delimiterIndex == -1)
                    continue;

                var key = line.Substring(0, delimiterIndex);
                var value = line.Substring(delimiterIndex + 1).Trim();
                parameters[key] = value;
            }

            // Пересечиваем параметры.
            parameters[PageTag] = page.ToString();
            parameters[LimitTag] = limit.ToString();
            parameters[MaxPriceTag] = maxPrice.ToString();

            // Собираем URL.
            var queries = string.Join("&", parameters.Select(x => $"{Url.Encode(x.Key)}={Url.Encode(x.Value)}"));
            return $"{RequestUrl}?{queries}";
        }
    }
}