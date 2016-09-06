using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace Terradue.Metadata.EarthObservation
{
    public static class GeoTimeOpenSearchHelper
    {
        public static NameValueCollection MergeGeoTimeFilters(NameValueCollection parameters, NameValueCollection overriders)
        {

            NameValueCollection nvc = new NameValueCollection(parameters);

            foreach (var key in overriders.AllKeys)
            {
                if (!string.IsNullOrEmpty(overriders[key]))
                {
                    switch (key)
                    {
                        case "bbox":

                            if (!string.IsNullOrEmpty(parameters["bbox"]))
                            {
                                string geom = IntersectBboxFilters(parameters[key], overriders[key]);
                                nvc.Remove("bbox");
                                nvc.Set("geom", geom);
                            }
                            else if (!string.IsNullOrEmpty(parameters["geom"]))
                            {
                                string geom = IntersectBboxAndGeomFilters(overriders[key], parameters[key]);
                                nvc.Set("geom", geom);
                            }
                            else
                            {
                                nvc.Set("bbox", overriders[key]);
                            }
                            break;
                        case "geom":
                            if (!string.IsNullOrEmpty(parameters["bbox"]))
                            {
                                string geom = IntersectBboxAndGeomFilters(parameters[key], overriders[key]);
                                nvc.Remove("bbox");
                                nvc.Set("geom", geom);
                            }
                            else if (!string.IsNullOrEmpty(parameters["geom"]))
                            {
                                string geom = IntersectGeomFilters(parameters[key], overriders[key]);
                                nvc.Set("geom", geom);
                            }
                            else
                            {
                                nvc.Set("bbox", overriders[key]);
                            }
                            break;
                        case "start":
                            if (!string.IsNullOrEmpty(parameters["start"]))
                            {
                                var paramDate = DateTime.Parse(parameters["start"]).ToUniversalTime();
                                var overrideDate = DateTime.Parse(overriders["start"]).ToUniversalTime();
                                if (overrideDate > paramDate)
                                    nvc.Set(key, overriders[key]);
                            }
                            else {
                                nvc.Set("start", overriders[key]);
                            }
                            break;
                        case "stop":
                            if (!string.IsNullOrEmpty(parameters["stop"]))
                            {
                                var paramDate = DateTime.Parse(parameters["stop"]).ToUniversalTime();
                                var overrideDate = DateTime.Parse(overriders["stop"]).ToUniversalTime();
                                if (overrideDate < paramDate)
                                    nvc.Set(key, overriders[key]);
                            }
                            else {
                                nvc.Set("stop", overriders[key]);
                            }
                            break;
                        default:
                            parameters.Set(key, overriders[key]);
                        break;
                    }
                }

            }

            return nvc;

        }

        static string IntersectBboxFilters(string bbox1, string bbox2)
        {
            Polygon poly1 = GetPolygonFromBbox(bbox1);
            Polygon poly2 = GetPolygonFromBbox(bbox2);

            var inters =poly1.Intersection(poly2);
            NetTopologySuite.IO.WKTWriter wktwriter = new NetTopologySuite.IO.WKTWriter();
            return wktwriter.Write(inters);
        }

        static string IntersectBboxAndGeomFilters(string bbox, string wkt)
        {
            Polygon poly = GetPolygonFromBbox(bbox);
            IGeometry geom = GetPolygonFromWkt(wkt);

            var inters = poly.Intersection(geom);
            NetTopologySuite.IO.WKTWriter wktwriter = new NetTopologySuite.IO.WKTWriter();
            return wktwriter.Write(inters);
        }

        static string IntersectGeomFilters(string wkt1, string wkt2)
        {
            IGeometry geom1 = GetPolygonFromWkt(wkt1);
            IGeometry geom2 = GetPolygonFromWkt(wkt2);

            var inters = geom1.Intersection(geom2);
            NetTopologySuite.IO.WKTWriter wktwriter = new NetTopologySuite.IO.WKTWriter();
            return wktwriter.Write(inters);
        }

        public static NetTopologySuite.Geometries.Polygon GetPolygonFromBbox(string bbox)
        {
            Regex re = new Regex(@"^\ *(?<west>[-+]?[0-9]*\.?[0-9]+\ *),\ *(?'south'[-+]?[0-9]*\.?[0-9]+)\ *,\ *(?'east'[-+]?[0-9]*\.?[0-9]+)\ *,\ *(?'north'[-+]?[0-9]*\.?[0-9]+)\ *$");
            MatchCollection match = re.Matches(bbox); 
            if (match.Count == 0 || !match[0].Success)
                throw new FormatException("bbox is not well formed :" + bbox);

            try
            {
                return new NetTopologySuite.Geometries.Polygon(
                    new LinearRing(
                        new Coordinate[5]{
                        GetCoordinate(match[0].Groups["west"].Value, match[0].Groups["south"].Value),
                        GetCoordinate(match[0].Groups["east"].Value, match[0].Groups["south"].Value),
                         GetCoordinate(match[0].Groups["east"].Value, match[0].Groups["north"].Value),
                         GetCoordinate(match[0].Groups["west"].Value, match[0].Groups["north"].Value),
                         GetCoordinate(match[0].Groups["west"].Value, match[0].Groups["south"].Value)
                    }
                    )
                );
            }
            catch (FormatException e)
            {
                throw new FormatException("bbox is not well formed :" + bbox + e.Message);
            }
        }

        static IGeometry GetPolygonFromWkt(string wkt)
        {
            NetTopologySuite.IO.WKTReader reader = new NetTopologySuite.IO.WKTReader();
            return reader.Read(wkt);
        }

        public static Coordinate GetCoordinate(string x, string y, string z = null)
        {
            return new Coordinate(double.Parse(x),
                           double.Parse(y),
                           string.IsNullOrEmpty(z) ? 0 : double.Parse(z));
        }
    }
}

