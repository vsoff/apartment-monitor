using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.WindowsPresentation;

namespace Apartment.App.Components
{
    public class RegionPolygon : GMapPolygon
    {
        private readonly string _name;
        private const double StrokeThickness = 2;
        private const double Opacity = 0.6;
        private readonly Brush _strokeBrush;
        private readonly Brush _fillBrush;

        public RegionPolygon(IEnumerable<PointLatLng> points, string name, Color color) : base(points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));
            _name = name;
            const byte min = 0;
            const byte delta = 40;
            _fillBrush = new SolidColorBrush(color);
            _strokeBrush = new SolidColorBrush(Color.FromRgb(
                (byte) Math.Max(min, color.R - delta),
                (byte) Math.Max(min, color.G - delta),
                (byte) Math.Max(min, color.B - delta)));
        }

        public override Path CreatePath(List<Point> localPath, bool addBlurEffect)
        {
            StreamGeometry streamGeometry = new StreamGeometry();
            using (var streamGeometryContext = streamGeometry.Open())
            {
                streamGeometryContext.BeginFigure(localPath[0], true, true);
                streamGeometryContext.PolyLineTo(localPath, true, true);
            }

            streamGeometry.Freeze();
            var path = new Path
            {
                Data = streamGeometry,
                StrokeThickness = StrokeThickness,
                Stroke = _strokeBrush,
                Fill = _fillBrush,
                Opacity = Opacity,
                IsHitTestVisible = false
            };

            return path;
        }
    }
}