using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Apartment.DataProvider.Avito.Common;
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
            Apartments = new ObservableCollection<ApartmentData>();
            UpdateApartmentsListCommand = new RelayCommand(_ => UpdateApartmentsList(), x => true);
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

        #region Binding

        public MapViewModel MapViewModel { get; }

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

        /// <summary>
        /// Команда обновления актуального списка квартир.
        /// </summary>
        public ICommand UpdateApartmentsListCommand { get; }

        #endregion
    }
}