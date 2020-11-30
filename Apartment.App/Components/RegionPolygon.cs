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
        private readonly Polygon _polygonInfo;

        public RegionPolygon(IEnumerable<PointLatLng> points, Polygon polygonInfo) : base(points)
        {
            _polygonInfo = polygonInfo ?? throw new ArgumentNullException(nameof(polygonInfo));
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
                Stroke = _polygonInfo.Stroke,
                StrokeThickness = _polygonInfo.StrokeThickness,
                StrokeLineJoin = _polygonInfo.StrokeLineJoin,
                StrokeStartLineCap = _polygonInfo.StrokeStartLineCap,
                StrokeEndLineCap = _polygonInfo.StrokeEndLineCap,
                Fill = _polygonInfo.Fill,
                Opacity = _polygonInfo.Opacity,
                IsHitTestVisible = false
            };

            return path;
        }
    }
}