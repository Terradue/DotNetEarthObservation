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
                        // there is a bbox defined
                        case "bbox":
                            // bbox/bbox merge
                            if (!string.IsNullOrEmpty(parameters["bbox"]))
                            {
                                string geom = MergeBboxFilters(parameters[key], overriders[key], parameters["rel"], overriders["rel"]);
                                nvc.Remove("bbox");
                                if (geom == null)
                                {
                                    nvc.Set("bbox", "-180,-90,180,90");
                                    nvc.Set("rel", "disjoint");
                                }
                                else
                                    nvc.Set("geom", geom);
                            }
                            // bbox/geom intersection
                            else if (!string.IsNullOrEmpty(parameters["geom"]))
                            {
                                string geom = MergeBboxAndGeomFilters(overriders[key], parameters["geom"], parameters["rel"], overriders["rel"]);
                                if (geom == null)
                                {
                                    nvc.Set("bbox", "-180,-90,180,90");
                                    nvc.Set("rel", "disjoint");
                                }
                                else
                                    nvc.Set("geom", geom);
                            }
                            // no intersection
                            else
                            {
                                nvc.Set("bbox", overriders[key]);
                            }
                            break;
                        case "geom":
                            if (!string.IsNullOrEmpty(parameters["bbox"]))
                            {
                                string geom = MergeBboxAndGeomFilters(parameters["bbox"], overriders[key], parameters["rel"], overriders["rel"]);
                                if (geom == null)
                                {
                                    nvc.Set("bbox", "-180,-90,180,90");
                                    nvc.Set("rel", "disjoint");
                                }
                                else
                                    nvc.Set("geom", geom);
                            }
                            else if (!string.IsNullOrEmpty(parameters["geom"]))
                            {
                                nvc.Remove("geom");
                                string geom = MergeGeomFilters(parameters[key], overriders[key], parameters["rel"], overriders["rel"]);
                                if (geom == null)
                                {
                                    nvc.Set("bbox", "-180,-90,180,90");
                                    nvc.Set("rel", "disjoint");
                                }
                            }
                            else
                            {
                                nvc.Set("geom", overriders[key]);
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
                            else
                            {
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
                            else
                            {
                                nvc.Set("stop", overriders[key]);
                            }
                            break;
                        case "uid":
                            if (!string.IsNullOrEmpty(parameters["uid"]))
                            {
                                if (parameters["uid"] != overriders[key])
                                {
                                    nvc.Set("uid", "__null");
                                    break;
                                }
                            }
                            nvc.Set("uid", overriders[key]);
                            break;
                        default:
                            nvc.Set(key, overriders[key]);
                            break;
                    }
                }

            }

            return nvc;

        }

        static string MergeBboxFilters(string bbox1, string bbox2, string bbox1rel, string bbox2rel)
        {
            Polygon poly1 = GetPolygonFromBbox(bbox1);
            Polygon poly2 = GetPolygonFromBbox(bbox2);

            IGeometry merge = null;
            if (string.IsNullOrEmpty(bbox1rel) || bbox1rel == "intersects" || bbox1rel == "contains")
            {
                if (string.IsNullOrEmpty(bbox2rel) || bbox2rel == "intersects")
                {
                    merge = poly1.Intersection(poly2);
                }
                else if (bbox2rel == "disjoint")
                {
                    merge = poly1.Difference(poly2);
                }
            }
            else if (bbox1rel == "disjoint")
            {
                if (string.IsNullOrEmpty(bbox2rel) || bbox2rel == "intersects")
                {
                    merge = poly2.Difference(poly1);
                }
                else if (bbox2rel == "disjoint")
                {
                    merge = poly1.Intersection(poly2);
                }
            }
            else
                throw new FormatException("Unknown geometric relation : " + bbox1rel);
            if (merge.IsEmpty)
                return "";
            NetTopologySuite.IO.WKTWriter wktwriter = new NetTopologySuite.IO.WKTWriter();
            return wktwriter.Write(merge);
        }

        static string MergeBboxAndGeomFilters(string bbox, string wkt, string bboxrel, string wktrel)
        {
            Polygon poly = GetPolygonFromBbox(bbox);
            IGeometry geom = GetPolygonFromWkt(wkt);

            IGeometry merge = null;
            if (string.IsNullOrEmpty(bboxrel) || bboxrel == "intersects" || bboxrel == "contains")
            {
                if (string.IsNullOrEmpty(wktrel) || wktrel == "intersects")
                {
                    merge = poly.Intersection(geom);
                }
                else if (wktrel == "disjoint")
                {
                    merge = poly.Difference(geom);
                }
            }
            else if (bboxrel == "disjoint")
            {
                if (string.IsNullOrEmpty(wktrel) || wktrel == "intersects")
                {
                    merge = geom.Difference(poly);
                }
                else if (wktrel == "disjoint")
                {
                    merge = poly.Intersection(geom);
                }
            }
            else
                throw new FormatException("Unknown geometric relation : " + bboxrel);
            if (merge.IsEmpty)
                return null;
            NetTopologySuite.IO.WKTWriter wktwriter = new NetTopologySuite.IO.WKTWriter();
            return wktwriter.Write(merge);
        }

        static string MergeGeomFilters(string wkt1, string wkt2, string wkt1rel, string wkt2rel)
        {
            IGeometry geom1 = GetPolygonFromWkt(wkt1);
            IGeometry geom2 = GetPolygonFromWkt(wkt2);

            IGeometry merge = null;
            if (string.IsNullOrEmpty(wkt1rel) || wkt1rel == "intersects" || wkt1rel == "contains")
            {
                if (string.IsNullOrEmpty(wkt2rel) || wkt2rel == "intersects")
                {
                    merge = geom1.Intersection(geom2);
                }
                else if (wkt2rel == "disjoint")
                {
                    merge = geom1.Difference(geom2);
                }
            }
            else if (wkt1rel == "disjoint")
            {
                if (string.IsNullOrEmpty(wkt2rel) || wkt2rel == "intersects")
                {
                    merge = geom2.Difference(geom1);
                }
                else if (wkt2rel == "disjoint")
                {
                    merge = geom1.Intersection(geom2);
                }
            }
            else
                throw new FormatException("Unknown geometric relation : " + wkt1rel);
            if (merge.IsEmpty)
                return null;
            NetTopologySuite.IO.WKTWriter wktwriter = new NetTopologySuite.IO.WKTWriter();
            return wktwriter.Write(merge);
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

