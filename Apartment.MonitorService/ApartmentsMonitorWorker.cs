using System;
using System.Threading.Tasks;
using Apartment.Common;
using Apartment.Common.Loggers;
using Apartment.Common.Workers;
using Apartment.Core;
using Apartment.Core.Services;
using Apartment.DataProvider;
using Apartment.Options;

namespace Apartment.MonitorService
{
    internal class ApartmentsMonitorWorker : IWorker
    {
        /// <summary>
        /// Минимальный интервал работы воркера.
        /// </summary>
        private static readonly TimeSpan WorkerMinimumInterval = TimeSpan.FromMinutes(5);

        private IWorker _worker;
        private readonly IApartmentsProvider _apartmentsProvider;
        private readonly IWorkerController _workerController;
        private readonly ApartmentService _apartmentService;
        private readonly MonitoringServiceOptions _options;
        private readonly ILogger _logger;

        public ApartmentsMonitorWorker(
            IApartmentsProvider apartmentsProvider,
            IWorkerController workerController,
            ApartmentService apartmentService,
            MonitoringServiceOptions options,
            ILogger logger)
        {
            _apartmentsProvider = apartmentsProvider ?? throw new ArgumentNullException(nameof(apartmentsProvider));
            _workerController = workerController ?? throw new ArgumentNullException(nameof(workerController));
            _apartmentService = apartmentService ?? throw new ArgumentNullException(nameof(apartmentService));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task CheckApartments()
        {
            try
            {
                _logger.Info($"{GetType().Name}: Производится получение данных от провайдера...");
                var actualApartments = await _apartmentsProvider.GetApartmentsAsync();

                _logger.Info($"{GetType().Name}: от провайдера получено {actualApartments.Count} объявлений");
                await _apartmentService.AddOrUpdateAsync(actualApartments);
                await _apartmentService.UpdateDisappearedStatusAsync();
                _logger.Info($"{GetType().Name}: Данные успешно получены и записаны");
            }
            catch (Exception ex)
            {
                _logger.Error($"{GetType().Name}: Во время получения данных произошла ошибка", ex);
            }
        }

        public void Start()
        {
            Stop();
            var interval = new TimeSpan(Math.Max(_options.WorkerInterval.Ticks, WorkerMinimumInterval.Ticks));
            _logger.Info($"{GetType().Name}: запускается воркер с интервалом {interval} (минимально возможный интервал: {WorkerMinimumInterval})");
            _worker = _workerController.StartWorker(() => CheckApartments().GetAwaiter().GetResult(), interval);
        }

        public void Stop() => _worker?.Stop();

        public void Dispose() => Stop();
    }
}