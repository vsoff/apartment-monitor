using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Apartment.App.Common;
using Apartment.App.Models;
using Apartment.DataProvider;
using Apartment.DataProvider.Models;
using GMap.NET;

namespace Apartment.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IApartmentsProvider _apartmentsProvider;

        public MainWindowViewModel(IApartmentsProvider apartmentsProvider, MapViewModel mapViewModel)
        {
            MapViewModel = mapViewModel ?? throw new ArgumentNullException(nameof(mapViewModel));
            _apartmentsProvider = apartmentsProvider ?? throw new ArgumentNullException(nameof(apartmentsProvider));
            UpdateApartmentsListCommand = new RelayCommand(_ => UpdateApartmentsList(), x => true);
            DeleteSelectedRegionCommand = new RelayCommand(_ => DeleteSelectedRegion(), x => SelectedRegion != null);
        }

        private static readonly SizeLatLng MergeApartmentsClip = new SizeLatLng(0.0002, 0.0004);

        /// <summary>
        /// Обновляет актуальный список квартир.
        /// </summary>
        private void UpdateApartmentsList()
        {
            var actualApartments = _apartmentsProvider.GetApartments().DistinctBy(x => x.Id).OrderBy(x => x.Price).ToArray();
            MapViewModel.Apartments.Clear();
            var apartmentInRegions = MapViewModel.Regions.Count == 0
                ? actualApartments
                : actualApartments.Where(x => MapViewModel.Regions.Any(r => r.Contains(x.Location)));

            var groupedApartments = GroupNearestApartments(apartmentInRegions, MergeApartmentsClip);
            foreach (var apartmentGroup in groupedApartments)
                MapViewModel.Apartments.Add(apartmentGroup);
        }

        /// <summary>
        /// Группирует квартиры, которые находятся поблизости.
        /// </summary>
        private IEnumerable<ApartmentsGroup> GroupNearestApartments(IEnumerable<ApartmentData> apartments, SizeLatLng size)
        {
            double GetDiscreteValue(double value, double step) => (float) Math.Round(value / step) * step;

            //double GetDiscreteValue2(double value, double step) => (int)(value / step) * step;
            PointLatLng GetDiscrete(PointLatLng p) => new PointLatLng(GetDiscreteValue(p.Lat, size.HeightLat), GetDiscreteValue(p.Lng, size.WidthLng));

            var map = new Dictionary<PointLatLng, List<ApartmentData>>();
            foreach (var apartment in apartments)
            {
                var discretePoint = GetDiscrete(apartment.Location);
                if (!map.TryGetValue(discretePoint, out var list))
                {
                    list = new List<ApartmentData>();
                    map[discretePoint] = list;
                }

                list.Add(apartment);
            }

            return map.Select(x => new ApartmentsGroup(x.Value));
        }


        /// <summary>
        /// Удаляет выбранный регион.
        /// </summary>
        private void DeleteSelectedRegion()
        {
            if (SelectedRegion == null)
                return;

            MapViewModel.Regions.Remove(SelectedRegion);
        }

        #region Binding

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

        private ApartmentsRegion _selectedRegion;

        /// <summary>
        /// Выбранная квартира.
        /// </summary>
        public ApartmentsRegion SelectedRegion
        {
            get => _selectedRegion;
            set
            {
                _selectedRegion = value;

                // Центрируем карту по выбранной квартире.
                if (_selectedRegion != null)
                    MapViewModel.SetCurrentPosition(_selectedRegion.Center);

                OnPropertyChanged(nameof(SelectedApartmentGroup));
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

        #endregion
    }
}