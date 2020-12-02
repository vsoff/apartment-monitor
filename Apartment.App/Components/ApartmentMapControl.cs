using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Apartment.App.Models;
using Apartment.DataProvider;
using Apartment.DataProvider.Models;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace Apartment.App.Components
{
    public class ApartmentMapControl : GMapControl
    {
        public PointLatLng LastPosition;

        public event MouseButtonEventHandler MouseLeftButtonClick;
        public event MouseButtonEventHandler MouseRightButtonClick;

        public ApartmentMapControl(PointLatLng startPosition)
        {
            _markerByObjectMap = new Dictionary<object, GMapMarker>();

            #region Map settings

            MapProvider = OpenStreetMapProvider.Instance;
            MinZoom = 2;
            MaxZoom = 17;
            Position = startPosition;
            Zoom = 14;
            MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            CanDragMap = true;
            DragButton = MouseButton.Left;

            LastPosition = startPosition;

            #endregion

            #region Events

            OnPositionChanged += point => { LastPosition = point; };
            Point? leftPoint = null;
            MouseLeftButtonUp += (sender, args) =>
            {
                if (!leftPoint.HasValue)
                    return;

                var newPoint = args.GetPosition(this);
                if (Math.Abs(newPoint.X - leftPoint.Value.X) < double.Epsilon && Math.Abs(newPoint.Y - leftPoint.Value.Y) < double.Epsilon)
                {
                    MouseLeftButtonClick?.Invoke(this, args);
                }
            };
            MouseLeftButtonDown += (sender, args) => { leftPoint = args.GetPosition(this); };

            Point? rightPoint = null;
            MouseRightButtonUp += (sender, args) =>
            {
                if (!rightPoint.HasValue)
                    return;

                var newPoint = args.GetPosition(this);
                if (Math.Abs(newPoint.X - rightPoint.Value.X) < double.Epsilon && Math.Abs(newPoint.Y - rightPoint.Value.Y) < double.Epsilon)
                {
                    MouseRightButtonClick?.Invoke(this, args);
                }
            };
            MouseRightButtonDown += (sender, args) => { rightPoint = args.GetPosition(this); };

            #endregion
        }

        private int GetZIndex(MapLayer layer)
        {
            switch (layer)
            {
                case MapLayer.Apartments: return 10;
                case MapLayer.Regions: return 1;
                case MapLayer.NewRegion: return 2;
                case MapLayer.NewRegionPoints: return 3;
                default: return 0;
            }
        }

        private readonly Dictionary<object, GMapMarker> _markerByObjectMap;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private GMapMarker CreateMarker(object obj)
        {
            Type type = obj.GetType();

            if (type == typeof(NewRegionData))
            {
                var data = obj as NewRegionData;

                return new RegionPolygon(data.Locations, null, Colors.IndianRed);
            }

            if (type == typeof(NewRegionPoint))
            {
                var data = obj as NewRegionPoint;
                const double size = 8;
                return new GMapMarker(data.Location)
                {
                    Shape = new Rectangle
                    {
                        Stroke = Brushes.Red,
                        Fill = Brushes.White,
                        Width = size,
                        Height = size,
                        StrokeThickness = 1
                    },
                    Offset = new Point(-size / 2, -size / 2)
                };
            }

            if (type == typeof(ApartmentData))
            {
                var data = obj as ApartmentData;
                return new ApartmentMarker(data, x => MessageBox.Show(x.ToString(), x.Id));
            }

            if (type == typeof(ApartmentsRegion))
            {
                var data = obj as ApartmentsRegion;
                return new RegionPolygon(data.Locations, data.Name, Colors.LightSkyBlue);
            }

            throw new ArgumentException($"Неизвестный тип {type.FullName}", nameof(obj));
        }

        /// <summary>
        /// Добавляет объект на карту.
        /// </summary>
        public void AddObject(IEnumerable objects, MapLayer layer)
        {
            foreach (var obj in objects)
                AddObject(obj, layer);
        }

        /// <summary>
        /// Добавляет объект на карту.
        /// </summary>
        public void AddObject(object obj, MapLayer layer)
        {
            if (obj is IEnumerable enumerable)
            {
                AddObject(enumerable, layer);
                return;
            }

            if (_markerByObjectMap.TryGetValue(obj, out _))
                throw new ArgumentException("Этот объект уже добавлен на карту");

            var marker = CreateMarker(obj);
            var zIndex = GetZIndex(layer);
            marker.ZIndex = zIndex;

            _markerByObjectMap.Add(obj, marker);
            Markers.Add(marker);
        }

        /// <summary>
        /// Удаляет объект с карты.
        /// </summary>
        public void RemoveObjectIfExists(IEnumerable objects)
        {
            foreach (var obj in objects)
                RemoveObjectIfExists(obj);
        }

        /// <summary>
        /// Удаляет объект с карты.
        /// </summary>
        public void RemoveObjectIfExists(object obj)
        {
            if (obj is IEnumerable enumerable)
            {
                RemoveObjectIfExists(enumerable);
                return;
            }

            if (!_markerByObjectMap.TryGetValue(obj, out var marker))
                return;

            _markerByObjectMap.Remove(marker);
            Markers.Remove(marker);
        }
    }

    public enum MapLayer
    {
        Undefined,
        NewRegion,
        NewRegionPoints,
        Regions,
        Apartments
    }
}