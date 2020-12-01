using System;
using System.Linq;
using System.Windows.Input;
using Apartment.App.Common;
using Apartment.DataProvider;
using Apartment.DataProvider.Models;

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

        /// <summary>
        /// Обновляет актуальный список квартир.
        /// </summary>
        private void UpdateApartmentsList()
        {
            var actualApartments = _apartmentsProvider.GetApartments().DistinctBy(x => x.Id).OrderBy(x => x.PriceValue).ToArray();
            MapViewModel.Apartments.Clear();
            foreach (var apartment in actualApartments)
                MapViewModel.Apartments.Add(apartment);
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

        private ApartmentData _selectedApartment;

        /// <summary>
        /// Выбранная квартира.
        /// </summary>
        public ApartmentData SelectedApartment
        {
            get => _selectedApartment;
            set
            {
                _selectedApartment = value;

                // Центрируем карту по выбранной квартире.
                if (_selectedApartment != null)
                    MapViewModel.SetCurrentPosition(_selectedApartment.Location);

                OnPropertyChanged(nameof(SelectedApartment));
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

                OnPropertyChanged(nameof(SelectedApartment));
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