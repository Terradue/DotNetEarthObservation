using System;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Xml;
using Terradue.OpenSearch.Engine;
using Terradue.OpenSearch.Response;
using Terradue.OpenSearch.Result;
using Terradue.OpenSearch.Schema;
using Terradue.ServiceModel.Syndication;
using Terradue.Metadata.EarthObservation;
using Terradue.OpenSearch.Filters;
using Terradue.OpenSearch;
using Terradue.GeoJson.Geometry;
using System.Threading.Tasks;
using System.Threading;

namespace Terradue.Metadata.EarthObservation.OpenSearch.Filters {
    public abstract class OpenSearchInterferometryFilter : OpenSearchCorrelationFilter {

        private log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        protected IOpenSearchable correlatedEntity;

        public OpenSearchInterferometryFilter(OpenSearchEngine ose, IOpenSearchableFactory factory, IOpenSearchable correlatedEntity) : base(ose, factory) {
            this.correlatedEntity = correlatedEntity;
        }

        public override void AddSearchLink(ref IOpenSearchResultCollection osr, NameValueCollection originalParameters, IOpenSearchable entity, string with, string finalContentType) {
            foreach (IOpenSearchResultItem item in osr.Items) {

                var link = item.Links.FirstOrDefault(l => l.RelationshipType == "self");

                if (link == null)
                    continue;

                // Get the url with self parameters
                UriBuilder url = new UriBuilder(link.Uri);

                // Keep the url parameters if any
                NameValueCollection nvc = HttpUtility.ParseQueryString(url.Query);
                // Get the opensearch parameters
                NameValueCollection revOsParams = OpenSearchFactory.ReverseTemplateOpenSearchParameters(entity.GetOpenSearchParameters(osr.ContentType));

                // Add the master self link as the correlation pair
                nvc.Set(revOsParams["cor:with"], with);
                // Add the correlation function to the inverse function
                nvc.Set(revOsParams["cor:function"], "interferometry");

                if (!string.IsNullOrEmpty(originalParameters["format"])) {
                    nvc.Set("format", originalParameters["format"]);
                }

                url.Query = nvc.ToString();
                item.Links.Add(new SyndicationLink(url.Uri, "search", "interferometry", finalContentType, 0));
                 

            }
        }

        public override void ApplyResultFilters(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable entity) {

            OpenSearchUrl corUrl = GetCorrelatedUrl(parameters);
            string function = GetFunction(parameters);

            if (function == "interferometry" && corUrl != null) {

                PerformInterferometryFunction(ref osr, parameters, entity);

            }

            if (function == "interfMaster" && corUrl != null) {

                PerformSlaveInterferometryFunction(ref osr, parameters, entity);
                osr.Title = new TextSyndicationContent(GetSlavesInfo(osr));


            }

        }

        protected virtual void PerformInterferometryFunction(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable entity) {

            // prpepare the contianer for the new list of slaves
            List<IOpenSearchResultItem> newitems = new List<IOpenSearchResultItem>();

            //List<Task> tasks = new List<Task>();

            int workerThreads, completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

            log.DebugFormat("Evaluate slaves for each master...");

            IOpenSearchResultCollection osr2 = osr;


            try {
                Parallel.ForEach(osr.Items, item => PerformInterferometryFunctionBySlave(newitems, item, osr2, parameters, entity));
            } catch (AggregateException e) {
                foreach (var e1 in e.InnerExceptions) {
                    log.Error(e1.Message);
                }
            }

            osr.Items = newitems.OrderByDescending(i => i.SortKey );

        }

        void PerformInterferometryFunctionBySlave(List<IOpenSearchResultItem> newitems, IOpenSearchResultItem item, IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable entity) {

            log.DebugFormat("Evaluate slave for {0} ...", item.Identifier);

            // Get Focused Search Url
            OpenSearchUrl slaveFeedUrl = GetFocusedSearchUrl(osr, item, parameters, entity);

            // make the OpenSearch over the slave url
            NameValueCollection nvc = new NameValueCollection(HttpUtility.ParseQueryString(slaveFeedUrl.Query));
            nvc.Set("count", OpenSearchCorrelationFilter.GetMinimum(parameters).ToString());


            // Query the slave url
            IOpenSearchResultCollection slavesFeedResult0 = ose.Query(correlatedEntity, nvc);


            // Remove the master from the results if any
            slavesFeedResult0.Items = slavesFeedResult0.Items.Where(i => i.Identifier != item.Identifier);

            // if there a minimum of slaves, keep the master
            if (slavesFeedResult0.Count >= OpenSearchCorrelationFilter.GetMinimum(parameters)) {
                newitems.Add(item);
                item.Links = new System.Collections.ObjectModel.Collection<SyndicationLink>(item.Links.Where(l => !(l.RelationshipType == "related" && l.Title == "interferometry")).ToList());
                item.Links.Add(new SyndicationLink(slaveFeedUrl, "related", "interferometry", osr.ContentType, 0));
            } 
           

        }


        /// <summary>
        /// Gets the focused search URL.
        /// </summary>
        /// <returns>The focused search URL.</returns>
        /// <param name="osr">Osr.</param>
        /// <param name="item">Item.</param>
        /// <param name="searchParameters">Search parameters.</param>
        /// <param name="masterEntity">Master entity.</param>
        protected virtual OpenSearchUrl GetFocusedSearchUrl(IOpenSearchResultCollection osr, IOpenSearchResultItem item, NameValueCollection searchParameters, IOpenSearchable masterEntity) {

            // Get the url with correlation parameters
            UriBuilder url = new UriBuilder(GetCorrelatedUrl(searchParameters));

            // Keep the slave url parameters if any
            NameValueCollection nvc = HttpUtility.ParseQueryString(url.Query);
            // Get the master opensearch parameters
            NameValueCollection revOsParams = OpenSearchFactory.ReverseTemplateOpenSearchParameters(masterEntity.GetOpenSearchParameters(osr.ContentType));

            // Add the track number, the sensor mode and the swath identifier of the master product
            nvc.Add(GetMasterParametersForSlaveFocusedSearch(osr, item, searchParameters, masterEntity));

            // Add the master self link as the correlation pair
            nvc.Set(revOsParams["cor:with"], item.Links.FirstOrDefault(l => l.RelationshipType == "self").Uri.ToString());
            // Add the correlation function to the inverse function
            nvc.Set(revOsParams["cor:function"], "interfMaster");

            url.Query = nvc.ToString();

            return new OpenSearchUrl(url.Uri);

        }

        /// <summary>
        /// Gets the master parameters for slave focused search.
        /// </summary>
        /// <returns>The master parameters for slave focused search.</returns>
        /// <param name="osr">Osr.</param>
        /// <param name="item">Item.</param>
        /// <param name="searchParameters">Search parameters.</param>
        /// <param name="masterEntity">Master entity.</param>
        protected virtual NameValueCollection GetMasterParametersForSlaveFocusedSearch(IOpenSearchResultCollection osr, IOpenSearchResultItem item, NameValueCollection searchParameters, IOpenSearchable masterEntity) {

            var element = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);
            if (element == null)
                throw new InvalidOperationException("No EarthObservation SAR element found in master product to produce focus search for slaves");

            string platformShortName = "";
            string track = "";
            string swath = "";

            if (element is Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType) {
                try { 
                    Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentType sarEquip = 
                        (Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentType)
                        ((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentPropertyType)
                            ((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType)element).procedure).EarthObservationEquipment;
                    platformShortName = sarEquip.platform.Platform.shortName;
                    track = sarEquip.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
                    swath = sarEquip.sensor.Sensor.swathIdentifier.Text[0];
                } catch (Exception e) {
                    throw new InvalidOperationException("missing information in master product to produce focus search for slaves : " + e.Message);
                }

            } else if (element is Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType) {
                try { 
                    Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentType sarEquip = 
                        (Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentType)
                        ((Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentPropertyType)
                            ((Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType)element).procedure).EarthObservationEquipment;
                    platformShortName = sarEquip.platform.Platform.shortName;
                    track = sarEquip.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
                    swath = sarEquip.sensor.Sensor.swathIdentifier.Text[0];
                } catch (Exception e) {
                    throw new InvalidOperationException("missing information in master product to produce focus search for slaves : " + e.Message);
                }

            } else
                throw new InvalidOperationException("EarthObservation element found in master product to produce focus search for slaves is not SAR");

            GeometryObject geometry = EarthObservationOpenSearchResultHelpers.FindGeometryFromEarthObservation(item);

            if (string.IsNullOrEmpty(platformShortName) || string.IsNullOrEmpty(track) || string.IsNullOrEmpty(swath) || geometry == null)
                throw new InvalidOperationException(string.Format("Master SAR dataset [id:{0}] does not have all the attributes to filter slaves for interferometry", item.Id));

            NameValueCollection nvc = new NameValueCollection();
            NameValueCollection revOsParams = OpenSearchFactory.ReverseTemplateOpenSearchParameters(masterEntity.GetOpenSearchParameters(osr.ContentType));

            if (!string.IsNullOrEmpty(revOsParams["eop:platformShortName"]))
                nvc.Set(revOsParams["eop:platformShortName"], platformShortName);
            
            if (string.IsNullOrEmpty(revOsParams["eop:wrsLongitudeGrid"])) throw new ImpossibleSearchException("Interferometry search requires that slave sries can be searched by track (missing eop:wrsLongitudeGrid in OSDD)");
            nvc.Set(revOsParams["eop:wrsLongitudeGrid"], track);

            if (string.IsNullOrEmpty(revOsParams["eop:swathIdentifier"])) throw new ImpossibleSearchException("Interferometry search requires that slave sries can be searched by swath id (missing eop:swathIdentifier in OSDD)");
            nvc.Set(revOsParams["eop:swathIdentifier"], swath);

            if (!string.IsNullOrEmpty(revOsParams["geo:geometry"]))
                nvc.Set(revOsParams["geo:geometry"], geometry.ToWkt());

            var timeCoverage = GetTimeCoverage(searchParameters, item);

            if (timeCoverage != null) {
                if (!string.IsNullOrEmpty(revOsParams["time:start"]) || !string.IsNullOrEmpty(revOsParams["time:end"])) throw new ImpossibleSearchException("Interferometry search with time parameter requires that slave sries can be searched by time (missing time:start and/or time:end in OSDD)");
                nvc.Set(revOsParams["time:start"], timeCoverage[0].ToUniversalTime().ToString("O"));
                nvc.Set(revOsParams["time:end"], timeCoverage[1].ToUniversalTime().ToString("O"));
            }

            if (!string.IsNullOrEmpty(searchParameters["spatialCover"])) {
                nvc.Set("spatialCover", searchParameters["spatialCover"]);
            }

            return nvc;

        }

        protected virtual void PerformSlaveInterferometryFunction(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable slaveEntity) {

            List<IOpenSearchResultItem> newitems = new List<IOpenSearchResultItem>();

            OpenSearchUrl masterFeedUrl = GetCorrelatedUrl(parameters);

            var nvcMaster = new NameValueCollection(HttpUtility.ParseQueryString(masterFeedUrl.Query));
            nvcMaster.Set("count", "1");

            IOpenSearchResultCollection masterFeed = ose.Query(correlatedEntity, nvcMaster);

            long count = masterFeed.TotalResults;

            if (count < 1) throw new ImpossibleSearchException("There is no reference to the master dataset. For this operation, one reference is mandatory");

            if (count > 1) {
                throw new ImpossibleSearchException("There are multiples references to the master dataset. For this operation, a unique reference is mandatory");
            } 


            try {
                Parallel.ForEach(osr.Items.ToArray(), slaveItem => PerformSlaveInterferometryFunctionBySlave(slaveItem, masterFeed, parameters, newitems));
            } catch (AggregateException e) {
                foreach (var e1 in e.InnerExceptions) {
                    log.Error(e1.Message);

                }
            }

            /*
            foreach (IOpenSearchResultItem slaveItem in osr.Items.ToArray()) {

                PerformSlaveInterferometryFunctionBySlave(slaveItem, masterFeed, parameters, newitems);


            }*/

            osr.Items = newitems.OrderByDescending(i => i.SortKey );

        }

        public virtual void PerformSlaveInterferometryFunctionBySlave(IOpenSearchResultItem slaveItem, IOpenSearchResultCollection masterFeed, NameValueCollection parameters, List<IOpenSearchResultItem> newitems) {

            if (masterFeed.Items.Count() < 1) throw new ImpossibleSearchException("Impossible to perform interferometry function by slave. There is no master associated");

            log.DebugFormat("evaluating slave {0}...", slaveItem.Identifier);

            IOpenSearchResultItem masterItem = (IOpenSearchResultItem)masterFeed.Items.ElementAt(0);

            if (slaveItem.Identifier == masterItem.Identifier) {
                log.DebugFormat("Master is slave: evicted");
                return;
            }

            if (!string.IsNullOrEmpty(parameters["spatialCover"])) {
                int spatialCover = OpenSearchCorrelationFilter.GetSpatialCover(parameters);
                var spatialOverlap = CalculateSpatialOverlap(masterItem, slaveItem, parameters["geom"]);
                if (spatialOverlap < spatialCover) {
                    log.DebugFormat("not enough spatial overlap : {0}/{1} {2} evicted", spatialOverlap, spatialCover, slaveItem.Identifier);
                    return;
                }
            }

            // Calculate perpendicular baseline
            double perpendicularBaseline = 0;
            try {
                perpendicularBaseline = GetPerpendicularBaseline(masterItem, slaveItem);
            } catch (Exception e) {
                log.ErrorFormat("Error calculating baseline : {0}", e.Message);
                log.Debug(e.StackTrace);
                return;
            }

            slaveItem.ElementExtensions.Add("baseline", MetadataHelpers.EOP, Math.Round(perpendicularBaseline, 2));
            slaveItem.Title = new TextSyndicationContent("[" + Math.Round(perpendicularBaseline, 2) + "] " + slaveItem.Title.Text);

            newitems.Add(slaveItem);

        }


        protected override DateTime[] GetStartAndStopTime(IOpenSearchResultItem item) {

            return new DateTime[] {
                Terradue.Metadata.EarthObservation.OpenSearch.EarthObservationOpenSearchResultHelpers.FindStartDateFromOpenSearchResultItem(item),
                Terradue.Metadata.EarthObservation.OpenSearch.EarthObservationOpenSearchResultHelpers.FindEndDateFromOpenSearchResultItem(item)
            };
        }

        protected static UniqueValueDictionary<string,string> GetCorrelationOpenSearchParameters() {
            UniqueValueDictionary<string,string> osdic = OpenSearchCorrelationFilter.GetCorrelationOpenSearchParameters();

            osdic.Add("cor:normalBaseline", "baseline");
            osdic.Add("cor:burstSync", "burstSync");

            return osdic;
        }

        protected virtual DateTime GetAscendingNodeDateTime(IOpenSearchResultItem item) {

            // - The ANX time period of the master product (i.e. the eop:startTimeFromAscendingNode 
            //   and eop:completionTimeFromAscendingNode attributes from the EO Product model 
            //   that must be overlapped
            var om = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);
            if (om == null)
                throw new InvalidOperationException("No EarthObservation SAR element found in master product to produce slave interferometry");

            if (om is Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType) {
                try { 
                    return ((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType)om).SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.Acquisition.ascendingNodeDate;
                } catch (Exception e) {
                    throw new InvalidOperationException("missing information in master product to produce slave interferometry : " + e.Message);
                }
            } else
                throw new InvalidOperationException("EarthObservation element found in master product to produce focus search for slaves is not SAR");

        }

        public string GetReadableTimeSpan(TimeSpan value) {
            string duration;

            if (value.TotalMinutes < 1)
                duration = value.Seconds + " Seconds";
            else if (value.TotalHours < 1)
                duration = value.Minutes + " Minutes, " + value.Seconds + " Seconds";
            else if (value.TotalDays < 1)
                duration = value.Hours + " Hours, " + value.Minutes + " Minutes";
            else
                duration = value.Days + " Days, " + value.Hours + " Hours";

            if (duration.StartsWith("1 Seconds") || duration.EndsWith(" 1 Seconds"))
                duration = duration.Replace("1 Seconds", "1 Second");

            if (duration.StartsWith("1 Minutes") || duration.EndsWith(" 1 Minutes"))
                duration = duration.Replace("1 Minutes", "1 Minute");

            if (duration.StartsWith("1 Hours") || duration.EndsWith(" 1 Hours"))
                duration = duration.Replace("1 Hours", "1 Hour");

            if (duration.StartsWith("1 Days"))
                duration = duration.Replace("1 Days", "1 Day");

            return duration;
        }

        public TimeSpan GetMaxTimeGap(IOpenSearchResultCollection osr) {

            TimeSpan maxTimeGap = new TimeSpan(0);
            IOpenSearchResultItem prev = null;

            foreach (IOpenSearchResultItem item in osr.Items.ToArray()) {

                if (prev != null && EarthObservationOpenSearchResultHelpers.FindStartDateFromOpenSearchResultItem(prev).Subtract(EarthObservationOpenSearchResultHelpers.FindStartDateFromOpenSearchResultItem(item)) > maxTimeGap)
                    maxTimeGap = EarthObservationOpenSearchResultHelpers.FindStartDateFromOpenSearchResultItem(prev).Subtract(EarthObservationOpenSearchResultHelpers.FindStartDateFromOpenSearchResultItem(item));


                prev = item;
            }

            return maxTimeGap;
                
        }

        public TimeSpan GetTimeSpan(IOpenSearchResultCollection osr) {

            if (osr.Count > 0)
                return EarthObservationOpenSearchResultHelpers.FindStartDateFromOpenSearchResultItem(osr.Items.First()).Subtract(EarthObservationOpenSearchResultHelpers.FindStartDateFromOpenSearchResultItem((osr.Items.Last())));

            return new TimeSpan(0);

        }

        public string GetSlavesInfo(IOpenSearchResultCollection osr) {

            if (osr.Count <= 0)
                return "";
            return string.Format("Stack image: {0} | Baseline Min.: {1}, Max.: {2} | Time gap max.: {3} | Time span: {4}", osr.Count, osr.Items.Min(i => Math.Abs(i.ElementExtensions.ReadElementExtensions<double>("baseline", MetadataHelpers.EOP)[0])), osr.Items.Max(i => Math.Abs(i.ElementExtensions.ReadElementExtensions<double>("baseline", MetadataHelpers.EOP)[0])), GetReadableTimeSpan(GetMaxTimeGap(osr)), GetReadableTimeSpan(GetTimeSpan(osr)));
        }

        public abstract double GetPerpendicularBaseline(IOpenSearchResultItem master, IOpenSearchResultItem slave);

        public double CalculateSpatialOverlap(IOpenSearchResultItem master, IOpenSearchResultItem slave, string bbox = null) {

            var masterGeom = EarthObservationOpenSearchResultHelpers.FindGeometryFromEarthObservation(master);
            var slaveGeom = EarthObservationOpenSearchResultHelpers.FindGeometryFromEarthObservation(slave);

            NetTopologySuite.IO.WKTReader wktReader = new NetTopologySuite.IO.WKTReader();
            var masterGeometry = wktReader.Read(masterGeom.ToWkt());
            var slaveGeometry = wktReader.Read(slaveGeom.ToWkt());

            if (masterGeometry.Area <= 0)
                return 0;

            if (!string.IsNullOrEmpty(bbox)) {
                var bboxGeometry = wktReader.Read(bbox);
                masterGeometry = bboxGeometry.Intersection(masterGeometry);
                slaveGeometry = slaveGeometry.Intersection(masterGeometry);
            }

            if (masterGeometry.Area <= 0)
                return 0;

            var intersectionGeometry = masterGeometry.Intersection(slaveGeometry);

            return (intersectionGeometry.Area / masterGeometry.Area) * 100;

        }



    }
}

