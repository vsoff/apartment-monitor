using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
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
            using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
            {
                streamGeometryContext.BeginFigure(localPath[0], true, true);
                streamGeometryContext.PolyLineTo((IList<Point>) localPath, true, true);
            }

            streamGeometry.Freeze();
            Path path = new Path {Data = streamGeometry};
            if (addBlurEffect)
                path.Effect = new BlurEffect()
                {
                    KernelType = KernelType.Gaussian,
                    Radius = 3.0,
                    RenderingBias = RenderingBias.Performance
                };

            path.Stroke = _polygonInfo.Stroke;
            path.StrokeThickness = _polygonInfo.StrokeThickness;
            path.StrokeLineJoin = _polygonInfo.StrokeLineJoin;
            path.StrokeStartLineCap = _polygonInfo.StrokeStartLineCap;
            path.StrokeEndLineCap = _polygonInfo.StrokeEndLineCap;
            path.Fill = _polygonInfo.Fill;
            path.Opacity = _polygonInfo.Opacity;
            path.IsHitTestVisible = false;

            return path;
        }
    }
}