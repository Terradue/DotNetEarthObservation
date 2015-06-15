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

        public override void ApplyResultFilters(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable entity) {

            OpenSearchUrl corUrl = GetCorrelatedUrl(parameters);
            string function = GetFunction(parameters);

            if (function == "interferometry" && corUrl != null) {

                PerformInterferometryFunction(ref osr, parameters, entity);

            }

            if (function == "interfMaster" && corUrl != null) {

                PerformSlaveInterferometryFunction(ref osr, parameters, entity);

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
                nvc.Add("count", "0");

                // create the opensearchable
                IOpenSearchable slaveEntity = factory.Create(slaveFeedUrl);

                // Query the slave url
                IOpenSearchResult slavesFeedResult0 = ose.Query(slaveEntity, nvc, "atom");

                // Remove the master from the results if any
                slavesFeedResult0.Result.Items = slavesFeedResult0.Result.Items.Where(i => i.Identifier != item.Identifier);

                // count the slaves
                int count = int.Parse(((AtomFeed)slavesFeedResult0.Result).ElementExtensions.ReadElementExtensions<string>("totalResults", "http://a9.com/-/spec/opensearch/1.1/")[0]);

                // if there a minimum of slaves, keep the master
                if (count >= OpenSearchCorrelationFilter.GetMinimum(parameters)) {
                    newitems.Add(item);
                    item.Links.Add(new SyndicationLink(slaveFeedUrl, "via", "interferometry", "application/atom+xml", 0));
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
            nvc.Add(revOsParams["cor:with"], HttpUtility.UrlEncode(item.Links.FirstOrDefault(l => l.RelationshipType == "self").Uri.ToString()));
            // Add the correlation function to the inverse function
            nvc.Add(revOsParams["cor:function"], "interfMaster");

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

            return nvc;

        }

        protected virtual void PerformSlaveInterferometryFunction(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable slaveEntity) {

            List<IOpenSearchResultItem> newitems = new List<IOpenSearchResultItem>();

            foreach (IOpenSearchResultItem slaveItem in osr.Items.ToArray()) {

                OpenSearchUrl masterFeedUrl = GetCorrelatedUrl(parameters);

                IOpenSearchable masterEntity = factory.Create(masterFeedUrl);

                IOpenSearchResult masterFeedResult = ose.Query(masterEntity, new NameValueCollection(), typeof(AtomFeed));

                IOpenSearchResultCollection masterFeed = (IOpenSearchResultCollection)masterFeedResult.Result;

                int count = int.Parse(masterFeed.ElementExtensions.ReadElementExtensions<string>("totalResults", "http://a9.com/-/spec/opensearch/1.1/")[0]);

                if (count > 1) {
                    throw new InvalidOperationException("There are multiples references to the master dataset. For this operation, a unique reference is mandatory");
                } 

                IOpenSearchResultItem masterItem = (IOpenSearchResultItem)masterFeed.Items.ElementAt(0);

                TimeSpan[] masterAnxTimes = GetStartAndStopTimeSpanFromAscendingNode(masterItem);
                TimeSpan[] slaveAnxTimes = GetStartAndStopTimeSpanFromAscendingNode(slaveItem);

                //if (slaveAnxStart > masterAnxStop) continue;
                //if (slaveAnxStop > masterAnxStart) continue;

                // - the time period of the master product adapted according to the date difference min and 
                //   max offsets (taking respectively the beginning/end of mission dates whenever min and max offsets are absent)
                // ??


                newitems.Add(slaveItem);
            }

            osr.Items = newitems;
        }


        protected virtual TimeSpan[] GetStartAndStopTimeSpanFromAscendingNode(IOpenSearchResultItem item) {

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
                    timeSpanAnxStart =  TimeSpan.FromMilliseconds(sarEquip.acquisitionParameters.Acquisition.startTimeFromAscendingNode.Value);
                    timeSpanAnxStop = TimeSpan.FromMilliseconds( sarEquip.acquisitionParameters.Acquisition.completionTimeFromAscendingNode.Value);
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
    }
}

