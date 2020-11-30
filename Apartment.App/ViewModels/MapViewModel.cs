using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Вызывается, когда был создан новый регион.
        /// </summary>
        public event EventHandler<ApartmentsRegion> NewRegionCreated;

        /// <summary>
        /// Объект карты.
        /// </summary>
        /// <remarks>Только для биндинга во вьюху, использоваться может исключительно в <see cref="MapViewModel"/>.</remarks>
        public ApartmentMapControl Map { get; }

        public bool IsRegionEditingMode { get; set; }

        public MapViewModel(ApplicationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            Map = CreateMap(options);
            Map.MouseLeftButtonClick += Map_MouseLeftButtonClick;
            Map.MouseRightButtonClick += Map_MouseRightButtonClick;
            IsRegionEditingMode = false;
        }

        private void Map_MouseRightButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsRegionEditingMode)
                return;

            var region = Map.FlushRegion("region");
            NewRegionCreated?.Invoke(null, region);
        }

        private void Map_MouseLeftButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsRegionEditingMode)
                return;

            Map.AddNewRegionMarker(Map.LastPosition);
        }

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

        public void SetDrawableApartments(IEnumerable<ApartmentData> apartments)
        {
            Map.ClearLayer(MapLayer.Apartments);
            Map.AddApartmentMarkers(apartments);
        }

        public void SetDrawableRegions(IEnumerable<ApartmentsRegion> regions)
        {
            Map.ClearLayer(MapLayer.Regions);
            Map.AddRegionMarkers(regions);
        }

        public void Dispose() => Map?.Dispose();
    }
}