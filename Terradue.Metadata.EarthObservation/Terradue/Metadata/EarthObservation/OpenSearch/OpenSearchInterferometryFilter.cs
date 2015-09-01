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

namespace Terradue.Metadata.EarthObservation.OpenSearch.Filters {
    public abstract class OpenSearchInterferometryFilter : OpenSearchCorrelationFilter {
        public OpenSearchInterferometryFilter(OpenSearchEngine ose, IOpenSearchableFactory factory) : base(ose, factory) {

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
                var count = OpenSearchFactory.GetCount(parameters);
                if (osr.TotalResults >= count && count != 0 && osr.TotalResults <= osr.Count)
                    osr.TotalResults = count + 1;
                else
                    osr.TotalResults = osr.Count;

            }

            if (function == "interfMaster" && corUrl != null) {

                PerformSlaveInterferometryFunction(ref osr, parameters, entity);
                osr.Title = new TextSyndicationContent(GetSlavesInfo(osr));
                var count = OpenSearchFactory.GetCount(parameters);
                if (osr.TotalResults >= count && count != 0 && osr.TotalResults <= osr.Count)
                    osr.TotalResults = count + 1;
                else
                    osr.TotalResults = osr.Count;
                osr.Items = osr.Items.OrderBy(i => Math.Abs(i.ElementExtensions.ReadElementExtensions<double>("baseline", MetadataHelpers.EOP)[0]));

            }

        }

        protected virtual void PerformInterferometryFunction(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable entity) {

            // prpepare the contianer for the new list of slaves
            List<IOpenSearchResultItem> newitems = new List<IOpenSearchResultItem>();

            // Test each item to see if it fits the filters
            foreach (IOpenSearchResultItem item in osr.Items) {

                // Get Focused Search Url
                OpenSearchUrl slaveFeedUrl = GetFocusedSearchUrl(osr, item, parameters, entity);

                // make the OpenSearch over the slave url
                NameValueCollection nvc = new NameValueCollection();

                // create the opensearchable
                IOpenSearchable slaveEntity = factory.Create(slaveFeedUrl);

                // Query the slave url
                IOpenSearchResultCollection slavesFeedResult0 = ose.Query(slaveEntity, nvc);

                // Remove the master from the results if any
                slavesFeedResult0.Items = slavesFeedResult0.Items.Where(i => i.Identifier != item.Identifier);


                // if there a minimum of slaves, keep the master
                if (slavesFeedResult0.TotalResults >= OpenSearchCorrelationFilter.GetMinimum(parameters)) {
                    newitems.Add(item);
                    item.Links.Add(new SyndicationLink(slaveFeedUrl, "related", "interferometry", osr.ContentType, 0));
                } 

            }

            osr.Items = newitems;

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

            if (!string.IsNullOrEmpty(searchParameters["format"])) {
                nvc.Set("format", searchParameters["format"]);
            }

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

            if (string.IsNullOrEmpty(platformShortName) || string.IsNullOrEmpty(track) || string.IsNullOrEmpty(swath))
                throw new InvalidOperationException(string.Format("Master SAR dataset [id:{0}] does not have all the attributes to filter slaves for interferometry", item.Id));

            NameValueCollection nvc = new NameValueCollection();
            NameValueCollection revOsParams = OpenSearchFactory.ReverseTemplateOpenSearchParameters(masterEntity.GetOpenSearchParameters(osr.ContentType));

            nvc.Add(revOsParams["eop:platformShortName"], platformShortName);
            nvc.Add(revOsParams["eop:wrsLongitudeGrid"], track);
            nvc.Add(revOsParams["eop:swathIdentifier"], swath);

            TimeSpan[] masterAnxTimes = GetStartAndStopTimeSpanFromAscendingNode(item);
            var timeCoverage = GetTimeCoverage(searchParameters, item);


            nvc.Add(revOsParams["eop:startTimeFromAscendingNode"], string.Format("[{0},{1}]", masterAnxTimes[0].Subtract(TimeSpan.FromSeconds(timeCoverage[0])).TotalMilliseconds, masterAnxTimes[1].TotalMilliseconds));
            nvc.Add(revOsParams["eop:completionTimeFromAscendingNode"], string.Format("[{0},{1}]", masterAnxTimes[0].TotalMilliseconds, masterAnxTimes[1].Add(TimeSpan.FromSeconds(timeCoverage[1])).TotalMilliseconds));

            return nvc;

        }

        protected virtual void PerformSlaveInterferometryFunction(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable slaveEntity) {

            List<IOpenSearchResultItem> newitems = new List<IOpenSearchResultItem>();

            foreach (IOpenSearchResultItem slaveItem in osr.Items.ToArray()) {

                OpenSearchUrl masterFeedUrl = GetCorrelatedUrl(parameters);

                IOpenSearchable masterEntity = factory.Create(masterFeedUrl);

                IOpenSearchResultCollection masterFeed = ose.Query(masterEntity, new NameValueCollection());

                long count = masterFeed.TotalResults;

                if (count > 1) {
                    throw new InvalidOperationException("There are multiples references to the master dataset. For this operation, a unique reference is mandatory");
                } 

                IOpenSearchResultItem masterItem = (IOpenSearchResultItem)masterFeed.Items.ElementAt(0);

                if (slaveItem.Identifier == masterItem.Identifier)
                    continue;

                // Calculate perpendicular baseline
                double perpendicularBaseline = 0;
                try {
                    perpendicularBaseline = GetPerpendicularBaseline(masterItem, slaveItem);
                } catch (Exception e) {
                    continue;
                }

                slaveItem.ElementExtensions.Add("baseline", MetadataHelpers.EOP, Math.Round(perpendicularBaseline, 2));
                slaveItem.Title = new TextSyndicationContent("[" + Math.Round(perpendicularBaseline, 2) + "] " + slaveItem.Title.Text);

                newitems.Add(slaveItem);


            }

            osr.Items = newitems;

        }


        protected override TimeSpan[] GetStartAndStopTimeSpanFromAscendingNode(IOpenSearchResultItem item) {

            // - The ANX time period of the master product (i.e. the eop:startTimeFromAscendingNode 
            //   and eop:completionTimeFromAscendingNode attributes from the EO Product model 
            //   that must be overlapped
            var om = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);
            if (om == null)
                throw new InvalidOperationException("No EarthObservation SAR element found in master product to produce slave interferometry");

            TimeSpan timeSpanAnxStart;
            TimeSpan timeSpanAnxStop;

            if (om is Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType) {
                try { 
                    Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentType sarEquip = 
                        (Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentType)
                            ((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentPropertyType)
                                ((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType)om).procedure).EarthObservationEquipment;
                    timeSpanAnxStart = TimeSpan.FromMilliseconds(sarEquip.acquisitionParameters.Acquisition.startTimeFromAscendingNode.Value);
                    timeSpanAnxStop = TimeSpan.FromMilliseconds(sarEquip.acquisitionParameters.Acquisition.completionTimeFromAscendingNode.Value);
                } catch (Exception e) {
                    throw new InvalidOperationException("missing information in master product to produce slave interferometry : " + e.Message);
                }

            } else if (om is Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType) {
                try { 
                    Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentType sarEquip = 
                        (Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentType)
                            ((Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentPropertyType)
                                ((Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType)om).procedure).EarthObservationEquipment;
                    timeSpanAnxStart = TimeSpan.FromMilliseconds(sarEquip.acquisitionParameters.Acquisition.startTimeFromAscendingNode.Value);
                    timeSpanAnxStop = TimeSpan.FromMilliseconds(sarEquip.acquisitionParameters.Acquisition.completionTimeFromAscendingNode.Value);
                } catch (Exception e) {
                    throw new InvalidOperationException("missing information in master product to produce slave interferometry : " + e.Message);
                }

            } else
                throw new InvalidOperationException("EarthObservation element found in master product to produce focus search for slaves is not SAR");
                    
            return new TimeSpan[]{ timeSpanAnxStart, timeSpanAnxStop };
        }

        protected static UniqueValueDictionary<string,string> GetCorrelationOpenSearchParameters() {
            UniqueValueDictionary<string,string> osdic = OpenSearchCorrelationFilter.GetCorrelationOpenSearchParameters();

            osdic.Add("cor:normalBaseline", "baseline");
            osdic.Add("cor:burstSync", "burstSync");

            return osdic;
        }

        protected virtual DateTime GetAcquisitionStartTime(IOpenSearchResultItem item) {

            // - The ANX time period of the master product (i.e. the eop:startTimeFromAscendingNode 
            //   and eop:completionTimeFromAscendingNode attributes from the EO Product model 
            //   that must be overlapped
            var om = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);
            if (om == null)
                throw new InvalidOperationException("No EarthObservation SAR element found in master product to produce slave interferometry");

            if (om is Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType) {
                try { 
                    return DateTime.Parse(((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType)om).phenomenonTime.GmlTimePeriod.beginPosition.Value);
                } catch (Exception e) {
                    throw new InvalidOperationException("missing information in master product to produce slave interferometry : " + e.Message);
                }

            } else if (om is Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType) {
                try { 
                    return DateTime.Parse(((Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType)om).phenomenonTime.GmlTimePeriod.beginPosition.Value);
                } catch (Exception e) {
                    throw new InvalidOperationException("missing information in master product to produce slave interferometry : " + e.Message);
                }

            } else
                throw new InvalidOperationException("EarthObservation element found in master product to produce focus search for slaves is not SAR");

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

                if (prev != null && GetAcquisitionStartTime(prev).Subtract(GetAcquisitionStartTime(item)) > maxTimeGap)
                    maxTimeGap = GetAcquisitionStartTime(prev).Subtract(GetAcquisitionStartTime(item));


                prev = item;
            }

            return maxTimeGap;
                
        }

        public TimeSpan GetTimeSpan(IOpenSearchResultCollection osr) {

            if (osr.Count > 0)
                return GetAcquisitionStartTime(osr.Items.First()).Subtract(GetAcquisitionStartTime(osr.Items.Last()));

            return new TimeSpan(0);

        }

        public string GetSlavesInfo(IOpenSearchResultCollection osr) {

            if (osr.Count <= 0)
                return "";
            return string.Format("Stack image: {0} | Baseline Min.: {1}, Max.: {2} | Time gap max.: {3} | Time span: {4}", osr.Count, osr.Items.Min(i => Math.Abs(i.ElementExtensions.ReadElementExtensions<double>("baseline", MetadataHelpers.EOP)[0])), osr.Items.Max(i => Math.Abs(i.ElementExtensions.ReadElementExtensions<double>("baseline", MetadataHelpers.EOP)[0])), GetReadableTimeSpan(GetMaxTimeGap(osr)), GetReadableTimeSpan(GetTimeSpan(osr)));
        }

        public abstract double GetPerpendicularBaseline(IOpenSearchResultItem master, IOpenSearchResultItem slave);

    }
}

