using System;
using System.Linq;
using Terradue.OpenSearch.Result;
using Terradue.Metadata.EarthObservation.OpenSearch;
using System.Collections;
using System.Collections.Generic;
using Terradue.GeoJson.Geometry;

namespace Terradue.Metadata.EarthObservation.Spatial {
    public class SpatialHelper {


        NetTopologySuite.Geometries.GeometryCollection landMask;

        public SpatialHelper (){

            NetTopologySuite.Geometries.GeometryFactory gfactory = new NetTopologySuite.Geometries.GeometryFactory();
            NetTopologySuite.IO.ShapefileDataReader landMaskShapeFileDataReader = new NetTopologySuite.IO.ShapefileDataReader("/usr/local/lib/ne_110m_land/ne_110m_land.shp", gfactory);

            List<GeoAPI.Geometries.IGeometry> geoms = new List<GeoAPI.Geometries.IGeometry>();

            while (landMaskShapeFileDataReader.Read()) {
                geoms.Add(landMaskShapeFileDataReader.Geometry);
            }

            landMask = new NetTopologySuite.Geometries.GeometryCollection(geoms.ToArray());
        }

        public double CalculateLandCover(IOpenSearchResultItem item) {

            var itemGeom = EarthObservationOpenSearchResultHelpers.FindGeometryFromEarthObservation(item);

            NetTopologySuite.IO.WKTReader wktReader = new NetTopologySuite.IO.WKTReader();
            var itemGeometry = wktReader.Read(itemGeom.ToWkt());

            if (itemGeometry.Area <= 0)
                return 0;

            var intersectionGeometryArea = landMask.Geometries.Sum(g => itemGeometry.Intersection(g).Area);

            return (intersectionGeometryArea / itemGeometry.Area) * 100;

        }
    }
}

