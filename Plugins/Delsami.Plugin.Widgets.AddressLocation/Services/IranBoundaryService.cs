using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using NetTopologySuite.IO;
using Nop.Core.Infrastructure;
using System;
using System.IO;
using System.Linq;

namespace Delsami.Plugin.Widgets.AddressLocation.Services
{
    public class IranBoundaryService
    {
        private readonly IPreparedGeometry _geometry;

        public IranBoundaryService(INopFileProvider fileProvider)
        {
            var filePath = fileProvider.MapPath(
                "~/Plugins/Delsami.Widgets.AddressLocation/Contents/iran.geojson");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Iran boundary file was not found.", filePath);

            var json = File.ReadAllText(filePath);
            var reader = new GeoJsonReader();
            var featureCollection = reader.Read<FeatureCollection>(json);

            if (featureCollection.Count == 0)
                throw new InvalidOperationException(
                    "Iran boundary GeoJSON must contain at least one feature.");

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            // همه هندسه‌ها را در یک MultiPolygon واحد ترکیب می‌کنیم
            var polygons = featureCollection
                .Select(f => f.Geometry)
                .Where(g => g != null)
                .SelectMany(g => Flatten(g))
                .ToArray();

            if (polygons.Length == 0)
                throw new InvalidOperationException(
                    "Iran boundary GeoJSON has no valid polygon geometries.");

            var combined = geometryFactory.CreateMultiPolygon(polygons);

            // آماده‌سازی برای جستجوی سریع نقطه
            _geometry = PreparedGeometryFactory.Prepare(combined);
        }

        /// <summary>
        /// هر Polygon یا MultiPolygon را به لیست Polygon تخت تبدیل می‌کند.
        /// </summary>
        private static System.Collections.Generic.IEnumerable<Polygon> Flatten(Geometry geometry)
        {
            switch (geometry)
            {
                case Polygon polygon:
                    yield return polygon;
                    break;

                case MultiPolygon multiPolygon:
                    foreach (var g in multiPolygon.Geometries)
                    {
                        if (g is Polygon p)
                            yield return p;
                    }
                    break;

                case GeometryCollection collection:
                    foreach (var g in collection.Geometries)
                    {
                        foreach (var p in Flatten(g))
                            yield return p;
                    }
                    break;

                default:
                    // انواع دیگر (LineString, Point و...) نادیده گرفته می‌شوند
                    break;
            }
        }

        public bool IsInside(double latitude, double longitude)
        {
            var point = new Point(longitude, latitude) { SRID = 4326 };
            return _geometry.Covers(point);
        }
    }
}