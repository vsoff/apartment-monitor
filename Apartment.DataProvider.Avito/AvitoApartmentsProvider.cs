using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Apartment.Common.Models;
using Apartment.Options;
using Flurl;
using Flurl.Http;
using GMap.NET;
using Newtonsoft.Json;

namespace Apartment.DataProvider.Avito
{
    public class AvitoApartmentsProvider : IApartmentsProvider
    {
        private readonly DebugOptions _options;
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

        public AvitoApartmentsProvider(DebugOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<ICollection<ApartmentInfo>> GetApartmentsAsync()
        {
            const int limit = 50;
            const int maxPrice = 2500000;
            var items = new List<Item>(128);

            if (_options.UseProviderCache)
            {
                string cacheFile = "AvitoCacheFile.json.cache";
                string jsonContent;
                if (File.Exists(cacheFile))
                {
                    jsonContent = File.ReadAllText(cacheFile);
                    var cacheItems = JsonConvert.DeserializeObject<List<Item>>(jsonContent);
                    items.AddRange(cacheItems);
                }
                else
                {
                    for (int page = 1;; page++)
                    {
                        var url = BuildUrl(page, limit, maxPrice);

                        var response = await url.GetJsonAsync<AvitoApartmentsResponse>();
                        items.AddRange(response.items);

                        if (page * limit >= response.count)
                            break;
                    }

                    jsonContent = JsonConvert.SerializeObject(items);
                    File.WriteAllText(cacheFile, jsonContent);
                }
            }
            else
            {
                for (int page = 1;; page++)
                {
                    var url = BuildUrl(page, limit, maxPrice);

                    var response = await url.GetJsonAsync<AvitoApartmentsResponse>();
                    items.AddRange(response.items);

                    if (page * limit >= response.count)
                        break;
                }
            }

            return items.Select(x =>
            {
                var stringArea = x.ext.area?.Length > 3
                    ? x.ext.area.Replace(".", ",").Substring(0, x.ext.area.Length - 3)
                    : null;
                // TODO: Этот кусок кода будет зависеть от локали, надо исправить.
                double.TryParse(stringArea, out var area);
                int.TryParse(x.ext.nomer_kvartiry, out var apartNumber);
                int.TryParse(x.ext.floor, out var floor);
                int.TryParse(x.ext.floors_count, out var floorsCount);
                int.TryParse(x.ext.rooms, out var roomsCount);

                return new ApartmentInfo
                {
                    ExternalId = x.itemId.ToString(),
                    Location = new PointLatLng(x.coords.lat, x.coords.lng),
                    Price = x.price,
                    Url = Url.Combine("https://www.avito.ru", x.url),
                    ApartmentNumber = apartNumber,
                    Floor = floor,
                    FloorsCount = floorsCount,
                    RoomsCount = roomsCount,
                    Area = area,
                    Title = x.title,
                    Address = x.geo.formattedAddress,
                    PublishingDate = DateTimeOffset.FromUnixTimeSeconds(x.time).UtcDateTime,
                    DisappearedDate = null,
                    ImageUrls = x.images.Select(i => i.SmallSize).ToArray()
                };
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