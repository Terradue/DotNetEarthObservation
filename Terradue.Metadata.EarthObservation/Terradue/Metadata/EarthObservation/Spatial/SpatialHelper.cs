using System;
using System.Linq;
using Terradue.OpenSearch.Result;
using Terradue.Metadata.EarthObservation.OpenSearch;
using System.Collections;
using System.Collections.Generic;
using Terradue.GeoJson.Geometry;
using System.Configuration;

namespace Terradue.Metadata.EarthObservation.Spatial {
    public class SpatialHelper {

        private static log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        NetTopologySuite.Geometries.GeometryCollection landMask;

        public SpatialHelper (){

            NetTopologySuite.Geometries.GeometryFactory gfactory = new NetTopologySuite.Geometries.GeometryFactory();
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var landMaskConfig = config.AppSettings.Settings["eo.landMask"];
            string landMaskPath = null;
            if (landMaskConfig == null)
                landMaskPath = "/usr/local/lib/ne_110m_land/ne_110m_land.shp";
            else
                landMaskPath = landMaskConfig.Value;

            log.DebugFormat("Opening land mask at {0}", landMaskPath);

            NetTopologySuite.IO.ShapefileDataReader landMaskShapeFileDataReader;

            try
            {
                landMaskShapeFileDataReader = new NetTopologySuite.IO.ShapefileDataReader(landMaskPath, gfactory);
            }
            catch(Exception e)
            {
                log.ErrorFormat("Error loading land mask at {0} : {1}", landMaskPath, e.Message);
                log.Debug(e.StackTrace);
                throw e;
            }

            List<GeoAPI.Geometries.IGeometry> geoms = new List<GeoAPI.Geometries.IGeometry>();

            while (landMaskShapeFileDataReader.Read()) {
                geoms.Add(landMaskShapeFileDataReader.Geometry);
            }

            landMask = new NetTopologySuite.Geometries.GeometryCollection(geoms.ToArray());
        }

        public double CalculateLandCover(IOpenSearchResultItem item) {

            var itemGeom = EarthObservationOpenSearchResultHelpers.FindGeometry(item);

            NetTopologySuite.IO.WKTReader wktReader = new NetTopologySuite.IO.WKTReader();
            var itemGeometry = wktReader.Read(itemGeom.ToWkt());

            if (itemGeometry.Area <= 0)
                return 0;

            var intersectionGeometryArea = landMask.Geometries.Sum(g => itemGeometry.Intersection(g).Area);

            return (intersectionGeometryArea / itemGeometry.Area) * 100;

        }
    }
}

