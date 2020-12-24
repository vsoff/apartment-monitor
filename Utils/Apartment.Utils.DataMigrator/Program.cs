using System;
using System.Threading.Tasks;
using Apartment.Data;
using Apartment.Data.Entities;
using Apartment.Data.Uow;
using Apartment.Options;

namespace Apartment.Utils.DataMigrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Нажмите enter, чтобы начать миграцию данных");
            Console.ReadLine();

            DatabaseOptions settingsFrom = new DatabaseOptions
            {
                DataBaseConnectionString = "Data Source=localhost;Initial Catalog=ApartmentsTest;Integrated Security=True;MultipleActiveResultSets=True",
                DataProviderType = DataProviderType.MsSql
            };

            DatabaseOptions settingsTo = new DatabaseOptions
            {
                DataBaseConnectionString = "server=mysql.arhpredator.myjino.ru;Uid=046723407_apart;Pwd=N9w?-2&+%K*FRC)w;Database=arhpredator_apartments;",
                DataProviderType = DataProviderType.MySql
            };

            if (string.IsNullOrEmpty(settingsFrom.DataBaseConnectionString) || string.IsNullOrEmpty(settingsTo.DataBaseConnectionString))
            {
                Console.WriteLine("Ошибка! Необходимо указать подключение к БД для обоих источников данных");
            }
            else
            {
                try
                {
                    Console.WriteLine();

                    await MigrateObjects(x => x.Apartments, settingsFrom, settingsTo);
                    await MigrateObjects(x => x.ItemsChanges, settingsFrom, settingsTo);
                    await MigrateObjects(x => x.Regions, settingsFrom, settingsTo);

                    Console.WriteLine("Миграцию данных успешно завершена");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Во время миграции данных произошла ошибка: {ex.Message}:\n{ex.StackTrace}");
                }
            }

            Console.WriteLine("Нажмите enter, чтобы завершить приложение...");
            Console.ReadLine();
        }

        /// <summary>
        /// Производит миграцию объектов для определённого репозитория.
        /// </summary>
        /// <typeparam name="T">Типа данных в репозитории</typeparam>
        /// <param name="getTargetField">Функция-указатель на поле репозитория.</param>
        /// <param name="settingsFrom">Настройки сервера с которого происходит миграция данных.</param>
        /// <param name="settingsTo">Настройки сервера на который происходит миграция данных.</param>
        private static async Task MigrateObjects<T>(Func<UnitOfWork, IRepository<T>> getTargetField, DatabaseOptions settingsFrom, DatabaseOptions settingsTo) where T : Entity
        {
            Console.WriteLine($"Миграция данных для типа {typeof(T)}...");

            using var uowTo = new UnitOfWork(new ApplicationContext(settingsTo.DataBaseConnectionString, settingsTo.DataProviderType));

            var repositoryTo = getTargetField(uowTo);

            // Проверяем что табличка пустая.
            if (await repositoryTo.AnyAsync())
                throw new InvalidOperationException($"В репозитории типа {typeof(T)} уже есть данные");

            using var uowFrom = new UnitOfWork(new ApplicationContext(settingsFrom.DataBaseConnectionString, settingsFrom.DataProviderType));

            // Получаем все объекты.
            var data = await getTargetField(uowFrom).GetAsync(x => x.CreatedAtUtc > DateTime.MinValue);
            Console.WriteLine($"Получено {data.Length} объектов типа {typeof(T)}.");

            // Добавляем и сохраняем данные.
            await repositoryTo.AddAsync(data);
            await uowTo.SaveChangesAsync();

            Console.WriteLine($"Миграция данных для типа {typeof(T)} закончена успешно!");
            Console.WriteLine();
        }
    }
}