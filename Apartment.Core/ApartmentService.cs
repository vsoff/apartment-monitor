using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apartment.Common;
using Apartment.Data;
using Apartment.Data.Entities;
using Apartment.Data.Uow;
using Apartment.DataProvider.Models;
using DifferencesSearch;
using Newtonsoft.Json;

namespace Apartment.Core
{
    public class ApartmentService
    {
        /// <summary>
        /// Время подтверждённого исчезновения объявления.
        /// </summary>
        /// <remarks>
        /// Приходится делать так, потому что avito возвращает абсолютно рандомный список объявлений.
        /// Они могут появляться и пропадать.
        /// </remarks>
        private static readonly TimeSpan DisappearedTimeout = TimeSpan.FromDays(31);

        private readonly DifferenceController _differenceController;
        private readonly IDatabaseContextProvider _contextProvider;
        private readonly ILogger _logger;

        public ApartmentService(IDatabaseContextProvider contextProvider, ILogger logger)
        {
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _differenceController = new DifferenceController();
            _differenceController.AutoBuilder<ApartmentEntity>()
                .Ignore(x => x.Id)
                .Ignore(x => x.CreatedAtUtc)
                .Ignore(x => x.UpdatedAtUtc)
                .Ignore(x => x.DisappearedDate)
                .Ignore(x => x.Title)
                .Build();
        }

        public async Task UpdateDisappearedStatusAsync()
        {
            using var uow = new UnitOfWork(_contextProvider.Create());
            var activityExpiredDate = DateTime.UtcNow - DisappearedTimeout;
            var apartments = await uow.Apartments
                .GetAsync(x => x.DisappearedDate == null && x.PublishingDate < activityExpiredDate);

            var apartmentsChanges = new List<ItemChangeEntity>();
            foreach (var apartment in apartments)
            {
                var disappearedDate = DateTime.UtcNow;
                apartmentsChanges.Add(new ItemChangeEntity
                {
                    Table = ApartmentEntity.TableName,
                    ObjectId = apartment.Id,
                    PropertyName = nameof(ApartmentEntity.DisappearedDate),
                    PropertyTypeFullName = typeof(DateTime?).FullName,
                    OldValueJson = JsonConvert.SerializeObject(apartment.DisappearedDate),
                    NewValueJson = JsonConvert.SerializeObject(disappearedDate)
                });
                apartment.DisappearedDate = disappearedDate;
            }

            if (apartments.Length > 0)
                _logger.Trace($"{GetType().Name}: объявления со следующими Ids теперь помечены как устаревшие: " +
                              string.Join(", ", apartments.Select(x => x.Id)));

            await uow.Apartments.UpdateAsync(apartments);
            await uow.ItemsChanges.AddAsync(apartmentsChanges);
            await uow.SaveChangesAsync();
        }

        public async Task AddOrUpdateAsync(IEnumerable<ApartmentInfo> data)
        {
            var newEntitiesMap = data.Select(x => x.ToEntity())
                .DistinctBy(x => x.ExternalId)
                .ToDictionary(x => x.ExternalId, x => x);

            using var uow = new UnitOfWork(_contextProvider.Create());

            var existsApartmentsMap = (await uow.Apartments
                    .GetAsync(x => newEntitiesMap.Keys.Contains(x.ExternalId)))
                .ToDictionary(x => x.ExternalId, x => x);

            // Сохраняем новые объявления.
            var newItems = newEntitiesMap.Values.Where(x => !existsApartmentsMap.Keys.Contains(x.ExternalId)).ToArray();
            _logger.Trace($"{GetType().Name}: Всего на запись отправлено: {newEntitiesMap.Count};" +
                          $" уже существует в БД: {existsApartmentsMap.Count}; новых записей: {newItems.Length}");

            await uow.Apartments.AddAsync(newItems);
            await uow.SaveChangesAsync();

            // Теперь пишем историю для всех объявлений.
            var apartmentsChanges = new List<ItemChangeEntity>();
            var updatedApartments = new List<ApartmentEntity>(existsApartmentsMap.Count);
            foreach (var oldApartment in existsApartmentsMap.Values)
            {
                // Если объявления нету в списке новых - значит оно устарело и исчезло.
                if (!newEntitiesMap.TryGetValue(oldApartment.ExternalId, out var newApartment))
                    continue;

                // Если не было изменений, то элемент нас не интересует.
                var diffs = _differenceController.GetAutoDifferences(oldApartment, newApartment);
                if (diffs.Length == 0)
                    continue;

                _logger.Trace($"{GetType().Name}: Объявление с Id == {oldApartment.Id} (ExternalId == {oldApartment.ExternalId}) имеет {diffs.Length} изменений");

                // Сохраняем изменения объекта.
                apartmentsChanges.AddRange(diffs.Select(diff => new ItemChangeEntity
                {
                    Table = ApartmentEntity.TableName,
                    ObjectId = oldApartment.Id,
                    PropertyName = diff.PropertyName,
                    PropertyTypeFullName = diff.PropertyType.FullName,
                    OldValueJson = JsonConvert.SerializeObject(diff.ValueLeft),
                    NewValueJson = JsonConvert.SerializeObject(diff.ValueRight)
                }));

                // Сохраянем результат.
                newApartment.Id = oldApartment.Id;
                updatedApartments.Add(newApartment);
            }

            _logger.Trace($"{GetType().Name}: Всего было изменено {updatedApartments.Count} апартаметов и {apartmentsChanges.Count} полей");
            await uow.Apartments.UpdateAsync(updatedApartments);
            await uow.ItemsChanges.AddAsync(apartmentsChanges);
            await uow.SaveChangesAsync();
        }

        public async Task<ICollection<ApartmentInfo>> GetActuallyApartmentsAsync()
        {
            using var uow = new UnitOfWork(_contextProvider.Create());
            var apartments = await uow.Apartments.GetAsync(x => x.DisappearedDate == null);
            return apartments.Select(x => x.ToCore()).ToList();
        }

        private async Task<ICollection<DataWithHistory<ApartmentInfo>>> GetActuallyApartmentsWithHistoryAsync()
        {
            using var uow = new UnitOfWork(_contextProvider.Create());
            var apartments = await uow.Apartments.GetAsync(x => x.DisappearedDate == null);
            // TODO: Подгружать историю
            throw new NotImplementedException();
        }
    }
}