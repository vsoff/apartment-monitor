using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Apartment.App.Components;
using Apartment.DataProvider.Avito.Common;
using Apartment.Options;
using GMap.NET;
using GMap.NET.WindowsPresentation;

namespace Apartment.App.ViewModels
{
    // TODO: В этой вьюмодели идёт нарушение MVVM, надо будет это как то по другому обыграть, когда станет понятен точный функционал.
    public class MapViewModel : ViewModelBase, IDisposable
    {
        public bool IsRegionEditingMode { get; set; }

        /// <summary>
        /// Вызывается, когда был создан новый регион.
        /// </summary>
        public event EventHandler<ApartmentsRegion> NewRegionCreated;

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

            _apartments = new List<ApartmentData>();
            _regions = new List<ApartmentsRegion>();
            _newRegionData = null;
            _newRegionPointsData = new List<NewRegionPoint>();
        }

        public void SetDrawableRegions(IEnumerable<ApartmentsRegion> regions)
        {
            if (regions == null) throw new ArgumentNullException(nameof(regions));

            Map.RemoveObjectIfExists(_regions);
            _regions.Clear();
            _regions.AddRange(regions);
            Map.AddObject(_regions, MapLayer.Apartments);
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
        private readonly List<ApartmentsRegion> _regions;
        private NewRegionData _newRegionData;

        private void FlushNewRegion()
        {
            var newRegion = new ApartmentsRegion($"test {DateTime.Now}", _newRegionData.Locations);

            // Чистим карту
            Map.RemoveObjectIfExists(_newRegionData);
            Map.RemoveObjectIfExists(_newRegionPointsData);
            _newRegionData = null;
            _newRegionPointsData.Clear();

            Map.AddObject(newRegion, MapLayer.Regions);
            _regions.Add(newRegion);
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

        #region Apartments

        private readonly List<ApartmentData> _apartments;

        public void SetDrawableApartments(IEnumerable<ApartmentData> apartments)
        {
            if (apartments == null) throw new ArgumentNullException(nameof(apartments));

            Map.RemoveObjectIfExists(_apartments);
            _apartments.Clear();
            _apartments.AddRange(apartments);
            Map.AddObject(_apartments, MapLayer.Apartments);
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