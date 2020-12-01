using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Apartment.App.Common;
using Apartment.DataProvider.Avito.Common;

namespace Apartment.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IApartmentsProvider _apartmentsProvider;

        public MainWindowViewModel(IApartmentsProvider apartmentsProvider, MapViewModel mapViewModel)
        {
            MapViewModel = mapViewModel ?? throw new ArgumentNullException(nameof(mapViewModel));
            _apartmentsProvider = apartmentsProvider ?? throw new ArgumentNullException(nameof(apartmentsProvider));
            Apartments = new ObservableCollection<ApartmentData>();
            UpdateApartmentsListCommand = new RelayCommand(_ => UpdateApartmentsList(), x => true);
            DeleteSelectedRegionCommand = new RelayCommand(_ => DeleteSelectedRegion(), x => SelectedRegion != null);
        }

        /// <summary>
        /// Обновляет актуальный список квартир.
        /// </summary>
        private void UpdateApartmentsList()
        {
            var actualApartments = _apartmentsProvider.GetApartments().DistinctBy(x => x.Id).OrderBy(x => x.PriceValue).ToArray();
            Apartments = new ObservableCollection<ApartmentData>(actualApartments);
            MapViewModel.SetDrawableApartments(actualApartments);
        }

        private void DeleteSelectedRegion()
        {
            if (SelectedRegion == null)
                return;

            Regions.Remove(SelectedRegion);
        }

        #region Binding

        public bool IsRegionEditingMode
        {
            get => MapViewModel.IsRegionEditingMode;
            set => MapViewModel.IsRegionEditingMode = value;
        }

        public MapViewModel MapViewModel { get; }

        #region Apartments

        private ObservableCollection<ApartmentData> _apartments;

        /// <summary>
        /// Список квартир.
        /// </summary>
        public ObservableCollection<ApartmentData> Apartments
        {
            get => _apartments;
            set
            {
                _apartments = value;
                OnPropertyChanged(nameof(Apartments));
            }
        }

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

        private ObservableCollection<ApartmentsRegion> _regions;

        /// <summary>
        /// Список квартир.
        /// </summary>
        public ObservableCollection<ApartmentsRegion> Regions
        {
            get => _regions;
            set
            {
                _regions = value;
                OnPropertyChanged(nameof(_regions));
            }
        }

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
                    // TODO: Добавить отчку центра для регионов.
                    MapViewModel.SetCurrentPosition(_selectedRegion.Locations.First());

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