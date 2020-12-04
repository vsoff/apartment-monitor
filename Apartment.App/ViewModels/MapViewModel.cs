using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Apartment.App.Common;
using Apartment.App.Components;
using Apartment.App.Models;
using Apartment.DataProvider;
using Apartment.DataProvider.Models;
using Apartment.Options;
using GMap.NET;

namespace Apartment.App.ViewModels
{
    // TODO: В этой вьюмодели идёт нарушение MVVM, надо будет это как то по другому обыграть, когда станет понятен точный функционал.
    public class MapViewModel : ViewModelBase, IDisposable
    {
        public bool IsRegionEditingMode { get; set; }

        public ObservableCollection<ApartmentsGroup> Apartments { get; }
        public ObservableCollection<ApartmentsRegion> Regions { get; }

        /// <summary>
        /// Объект карты.
        /// </summary>
        /// <remarks>Только для биндинга во вьюху, использоваться может исключительно в <see cref="MapViewModel"/>.</remarks>
        public ApartmentMapControl Map { get; }

        public MapViewModel(ApplicationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            Map = CreateMap(options);
            Map.MouseLeftButtonClick += Map_MouseLeftButtonClick;
            Map.MouseRightButtonClick += Map_MouseRightButtonClick;
            IsRegionEditingMode = false;

            _newRegionData = null;
            _newRegionPointsData = new List<NewRegionPoint>();

            var apartments = new FixedObservableCollection<ApartmentsGroup>();
            apartments.CollectionChanged += DrawableCollectionChanged;
            apartments.Clearing += Apartments_Clearing;
            Apartments = apartments;

            var regions = new FixedObservableCollection<ApartmentsRegion>();
            regions.CollectionChanged += DrawableCollectionChanged;
            regions.Clearing += Apartments_Clearing;
            Regions = regions;
        }

        private void Apartments_Clearing(object sender, EventArgs e) => Map.RemoveObjectIfExists(sender);

        private void DrawableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems?.Count > 0)
            {
                var items = e.NewItems;
                // TODO: Избавиться от слоёв.
                Map.AddObject(items, MapLayer.Apartments);
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems?.Count > 0)
            {
                var items = e.OldItems;
                Map.RemoveObjectIfExists(items);
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
                return;

            throw new NotImplementedException("Такой кейс не проверялся");
        }

        #region Mouse events

        private void Map_MouseRightButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsRegionEditingMode)
                return;

            FlushNewRegion();
        }

        private void Map_MouseLeftButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsRegionEditingMode)
                return;

            AddNewRegionPoint(Map.LastPosition);
        }

        #endregion

        #region New region editing

        private readonly List<NewRegionPoint> _newRegionPointsData;
        private NewRegionData _newRegionData;

        private void FlushNewRegion()
        {
            if (_newRegionData == null)
                return;

            var locations = _newRegionData.Locations;

            // Чистим карту
            Map.RemoveObjectIfExists(_newRegionData);
            Map.RemoveObjectIfExists(_newRegionPointsData);
            _newRegionData = null;
            _newRegionPointsData.Clear();

            // Добавляем новоиспечённый регион.
            if (locations.Count > 2)
                Regions.Add(new ApartmentsRegion($"Новый регион {DateTime.Now}", locations));
        }

        private void AddNewRegionPoint(PointLatLng point)
        {
            List<PointLatLng> locations;
            if (_newRegionData == null)
            {
                locations = new List<PointLatLng> {point};
            }
            else
            {
                locations = new List<PointLatLng>(_newRegionData.Locations.Count);
                locations.AddRange(_newRegionData.Locations);
                locations.Add(point);
                Map.RemoveObjectIfExists(_newRegionData);
            }

            var newPoint = new NewRegionPoint(point);
            _newRegionPointsData.Add(newPoint);
            Map.AddObject(newPoint, MapLayer.NewRegionPoints);

            _newRegionData = new NewRegionData(locations);
            Map.AddObject(_newRegionData, MapLayer.NewRegion);
        }

        #endregion

        /// <summary>
        /// Создаёт новый экземпляр карты.
        /// </summary>
        private ApartmentMapControl CreateMap(ApplicationOptions options)
        {
            var startPosition = new PointLatLng(options.StartPosition.Latitude, options.StartPosition.Longitude);
            var mapControl = new ApartmentMapControl(startPosition);
            return mapControl;
        }

        /// <summary>
        /// Центрирует карту по указанным координатам.
        /// </summary>
        public void SetCurrentPosition(PointLatLng position) => Map.Position = position;

        public void Dispose() => Map?.Dispose();
    }
}