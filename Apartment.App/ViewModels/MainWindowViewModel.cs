﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Apartment.App.Common;
using Apartment.App.Models;
using Apartment.Common;
using Apartment.Common.Models;
using Apartment.Core.Services;
using Apartment.DataProvider;
using GMap.NET;
using Region = Apartment.Common.Models.Region;

namespace Apartment.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly SizeLatLng MergeApartmentsClip = new SizeLatLng(0.0002, 0.0004);

        private readonly IApartmentsProvider _apartmentsProvider;
        private readonly RegionsService _regionsService;
        private bool _isInitialized;

        public MainWindowViewModel(
            IApartmentsProvider apartmentsProvider,
            RegionsService regionsService,
            MapViewModel mapViewModel)
        {
            MapViewModel = mapViewModel ?? throw new ArgumentNullException(nameof(mapViewModel));
            _apartmentsProvider = apartmentsProvider ?? throw new ArgumentNullException(nameof(apartmentsProvider));
            _regionsService = regionsService ?? throw new ArgumentNullException(nameof(regionsService));
            InitializeCommand = new RelayCommand(x => Initialize(), x => !_isInitialized);
            UpdateApartmentsListCommand = new RelayCommand(x => UpdateApartmentsList(), x => true);
            SaveSelectedRegionChangesCommand = new RelayCommand(SaveSelectedRegion, x => SelectedRegion != null);
            DeleteSelectedRegionCommand = new RelayCommand(DeleteSelectedRegion, x => SelectedRegion != null);
            CancelSelectedRegionCommand = new RelayCommand(CancelSelectedRegion, x => SelectedRegion != null);
            mapViewModel.RegionCreated += (sender, locations) => OnRegionCreated(locations);
            MaxApartmentPrice = 3000000;
        }

        private async void Initialize()
        {
            if (_isInitialized) throw new InvalidOperationException($"Метод {nameof(Initialize)} был вызван повторно");

            _isInitialized = true;
            await UpdateRegionsList();
            await UpdateApartmentsList();
        }

        private async void OnRegionCreated(IEnumerable<PointLatLng> locations)
        {
            if (locations == null) throw new ArgumentNullException(nameof(locations));

            // TODO Вьюха для заполнения данных о новом регионе.
            var region = new Region(0, "Новый регион", "#0000FF", locations);
            var addedRegion = await _regionsService.AddRegionAsync(region);
            MapViewModel.Regions.Add(addedRegion);
        }

        /// <summary>
        /// Обновляет актуальный список регионов.
        /// </summary>
        private async Task UpdateRegionsList()
        {
            var regions = await _regionsService.GetAllRegionsAsync();
            MapViewModel.Regions.Clear();
            foreach (var region in regions)
                MapViewModel.Regions.Add(region);
        }

        /// <summary>
        /// Обновляет актуальный список квартир.
        /// </summary>
        private async Task UpdateApartmentsList()
        {
            // Получаем актуальные объявления.
            var actualApartments = (await _apartmentsProvider.GetApartmentsAsync())
                .DistinctBy(x => x.ExternalId)
                .OrderBy(x => x.Price)
                .Where(x => !x.Price.HasValue || x.Price <= MaxApartmentPrice)
                .ToArray();

            // Фильтруем по регионам.
            var apartmentInRegions = MapViewModel.Regions.Count == 0
                ? actualApartments
                : actualApartments.Where(x => MapViewModel.Regions.Any(r => r.Contains(x.Location)));

            // Группируем ближайшие объявления.
            var groupedApartments = GroupNearestApartments(apartmentInRegions, MergeApartmentsClip);

            // Добавляем на карту.
            MapViewModel.Apartments.Clear();
            foreach (var apartmentGroup in groupedApartments)
                MapViewModel.Apartments.Add(apartmentGroup);
        }

        /// <summary>
        /// Группирует квартиры, которые находятся поблизости.
        /// </summary>
        private IEnumerable<ApartmentsGroup> GroupNearestApartments(IEnumerable<ApartmentInfo> apartments, SizeLatLng size)
        {
            // TODO: Надо выбрать самый оптимальный метод.
            //double GetDiscreteValue2(double value, double step) => (int)(value / step) * step;
            double GetDiscreteValue(double value, double step) => (float) Math.Round(value / step) * step;

            PointLatLng GetDiscrete(PointLatLng p) => new PointLatLng(GetDiscreteValue(p.Lat, size.HeightLat), GetDiscreteValue(p.Lng, size.WidthLng));

            var map = new Dictionary<PointLatLng, List<ApartmentInfo>>();
            foreach (var apartment in apartments)
            {
                var discretePoint = GetDiscrete(apartment.Location);
                if (!map.TryGetValue(discretePoint, out var list))
                {
                    list = new List<ApartmentInfo>();
                    map[discretePoint] = list;
                }

                list.Add(apartment);
            }

            return map.Select(x => new ApartmentsGroup(x.Value));
        }


        private async void SaveSelectedRegion(object _)
        {
            if (SelectedRegion == null)
                return;

            var region = await _regionsService.UpdateRegionAsync(SelectedRegionViewModel.GetNewRegion());
            MapViewModel.Regions.Remove(SelectedRegion);
            MapViewModel.Regions.Add(region);
            SelectedRegion = null;
        }

        private void CancelSelectedRegion(object _)
        {
            SelectedRegion = null;
        }

        /// <summary>
        /// Удаляет выбранный регион.
        /// </summary>
        private async void DeleteSelectedRegion(object _)
        {
            if (SelectedRegion == null)
                return;

            await _regionsService.DeleteRegionAsync(SelectedRegion.Id);
            MapViewModel.Regions.Remove(SelectedRegion);
            SelectedRegion = null;
        }

        #region Binding

        public bool DisplayRegions { get; set; }
        public int MaxApartmentPrice { get; set; }

        public MapViewModel MapViewModel { get; }

        #region Apartments

        private ApartmentsGroup _selectedApartmentGroup;

        /// <summary>
        /// Выбранная квартира.
        /// </summary>
        public ApartmentsGroup SelectedApartmentGroup
        {
            get => _selectedApartmentGroup;
            set
            {
                _selectedApartmentGroup = value;

                // Центрируем карту по выбранной группе квартир.
                if (_selectedApartmentGroup != null)
                    MapViewModel.SetCurrentPosition(_selectedApartmentGroup.Location);

                OnPropertyChanged(nameof(SelectedApartmentGroup));
            }
        }

        #endregion

        #region Regions

        public RegionEditViewModel SelectedRegionViewModel { get; private set; }

        private Region _selectedRegion;

        /// <summary>
        /// Выбранная квартира.
        /// </summary>
        public Region SelectedRegion
        {
            get => _selectedRegion;
            set
            {
                _selectedRegion = value;

                // Центрируем карту по выбранной квартире.
                if (_selectedRegion != null)
                    MapViewModel.SetCurrentPosition(_selectedRegion.Center);

                SelectedRegionViewModel = _selectedRegion == null ? null : new RegionEditViewModel(_selectedRegion);
                OnPropertyChanged(nameof(SelectedRegion));
                OnPropertyChanged(nameof(SelectedRegionViewModel));
            }
        }

        #endregion

        /// <summary>
        /// Команда обновления актуального списка квартир.
        /// </summary>
        public ICommand UpdateApartmentsListCommand { get; }

        /// <summary>
        /// Команда удаления выделенного региона.
        /// </summary>
        public ICommand DeleteSelectedRegionCommand { get; }

        public ICommand SaveSelectedRegionChangesCommand { get; }

        public ICommand CancelSelectedRegionCommand { get; }

        /// <summary>
        /// Команда инициализации.
        /// </summary>
        public ICommand InitializeCommand { get; }

        #endregion
    }
}