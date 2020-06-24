using System;
using System.Linq;
using Terradue.OpenSearch.Result;
using Terradue.Metadata.EarthObservation.OpenSearch;
using System.Collections;
using System.Collections.Generic;
using Terradue.GeoJson.Geometry;
using System.Configuration;
using Terradue.Metadata.EarthObservation.OpenSearch.Extensions;
using NetTopologySuite.Geometries;
using System.Text;

namespace Terradue.Metadata.EarthObservation.Helpers
{
    public class SpatialHelper
    {

        private static log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        NetTopologySuite.Geometries.GeometryCollection landMask;

        public SpatialHelper()
        {

            NetTopologySuite.Geometries.GeometryFactory gfactory = new NetTopologySuite.Geometries.GeometryFactory();
            var landMaskConfig = System.Environment.GetEnvironmentVariable("EO_LANDMASK_DIRPATH");
            string landMaskPath = null;
            if (string.IsNullOrEmpty(landMaskConfig))
                landMaskPath = "/usr/local/lib/ne_110m_land/ne_110m_land.shp";
            else
                landMaskPath = landMaskConfig;

            log.DebugFormat("Opening land mask at {0}", landMaskPath);

            List<Geometry> geoms = new List<Geometry>();

            try
            {
                NetTopologySuite.IO.ShapefileDataReader landMaskShapeFileDataReader = new NetTopologySuite.IO.ShapefileDataReader(landMaskPath, gfactory, Encoding.Default);
                while (landMaskShapeFileDataReader.Read())
                {
                    geoms.Add(landMaskShapeFileDataReader.Geometry);
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error loading land mask at {0} : {1}", landMaskPath, e.Message);
                log.Debug(e.StackTrace);
                throw e;
            }

            landMask = new NetTopologySuite.Geometries.GeometryCollection(geoms.ToArray());
        }

        public double CalculateLandCover(IOpenSearchResultItem item)
        {

            var itemGeom = item.FindGeometry();

            NetTopologySuite.IO.WKTReader wktReader = new NetTopologySuite.IO.WKTReader();
            var itemGeometry = wktReader.Read(itemGeom.ToWkt());

            if (itemGeometry.Area <= 0)
                return 0;

            var intersectionGeometryArea = landMask.Geometries.Sum(land => {
                if ( land.Intersects(itemGeometry)  )
                    return itemGeometry.Intersection(land).Area;
                return 0;
            });

            return (intersectionGeometryArea / itemGeometry.Area) * 100;

        }
    }
}

