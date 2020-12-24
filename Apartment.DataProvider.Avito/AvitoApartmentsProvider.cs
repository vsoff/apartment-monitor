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
correctorMode: 0
{PageTag}: 1
map: eyJzZWFyY2hBcmVhIjp7ImxhdEJvdHRvbSI6NjEuNjg0MDc0NTU4MzYwMzgsImxhdFRvcCI6NjEuODEyNDU3NjkwNzQwNCwibG9uTGVmdCI6MzQuMjgxMzI5OTQ4MzE0NTg1LCJsb25SaWdodCI6MzQuNDY4Nzg0MTcxOTQ3NH19
params[201]: 1059
params[2952][from]: 3
filterCategoryId: 24
verticalCategoryId: 1
rootCategoryId: 4
searchArea[latBottom]: 61.68407455836038
searchArea[lonLeft]: 34.281329948314585
searchArea[latTop]: 61.8124576907404
searchArea[lonRight]: 34.4687841719474
viewPort[width]: 546
viewPort[height]: 790
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
                    jsonContent = await File.ReadAllTextAsync(cacheFile);
                    var cacheItems = JsonConvert.DeserializeObject<List<Item>>(jsonContent);
                    items.AddRange(cacheItems);
                }
                else
                {
                    items = await GetApartmentsFromAllPages(limit, null);

                    jsonContent = JsonConvert.SerializeObject(items);
                    File.WriteAllText(cacheFile, jsonContent);
                }
            }
            else
            {
                items = await GetApartmentsFromAllPages(limit, null);
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
                    PublishingDateUtc = x.time.HasValue ? DateTimeOffset.FromUnixTimeSeconds(x.time.Value).UtcDateTime : DateTime.MinValue,
                    DisappearedDate = null,
                    ImageUrls = x.images.Select(i => i.SmallSize).ToArray()
                };
            }).ToArray();
        }

        private async Task<List<Item>> GetApartmentsFromAllPages(int partitionSize, int? maxPrice)
        {
            var items = new List<Item>();
            for (int page = 1;; page++)
            {
                var url = BuildUrl(page, partitionSize, maxPrice);

                var responseJson = await url.GetStringAsync();
                var response = JsonConvert.DeserializeObject<AvitoApartmentsResponse>(responseJson);
                if (response.items == null)
                    throw new AvitoException("Авито вернули ошибку вместо ответа");

                items.AddRange(response.items);

                if (page * partitionSize >= response.count)
                    break;
            }

            return items;
        }

        /// <summary>
        /// Создаёт URL для запроса списка квартир.
        /// </summary>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Объектов на одной странице.</param>
        /// <param name="maxPrice">Максимальная цена квартиры.</param>
        /// <returns>URL запроса</returns>
        private string BuildUrl(int page, int limit, int? maxPrice)
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
            if (maxPrice.HasValue)
                parameters[MaxPriceTag] = maxPrice.ToString();

            // Собираем URL.
            var queries = string.Join("&", parameters.Select(x => $"{Url.Encode(x.Key)}={Url.Encode(x.Value)}"));
            return $"{RequestUrl}?{queries}";
        }
    }

    public class AvitoException : Exception
    {
        public AvitoException(string message) : base(message)
        {
        }
    }
}