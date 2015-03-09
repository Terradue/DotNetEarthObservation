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

        public override void ApplyResultFilters(ref IOpenSearchResultCollection osr, NameValueCollection originalParameters, IOpenSearchable entity) {

            OpenSearchUrl corUrl = GetCorrelatedUrl(originalParameters);
            string function = GetFunction(originalParameters);

            if (function == "interferometry" && corUrl != null) {

                PerformInterferometryFunction(ref osr, originalParameters, entity);

            }

            if (function == "interfMaster" && corUrl != null) {

                PerformSlaveInterferometryFunction(ref osr, originalParameters, entity);

            }

        }

        protected virtual void PerformInterferometryFunction(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable entity) {

            AtomFeed feed;

            try {
                feed = (AtomFeed)osr;
            } catch (InvalidCastException) {
                throw new InvalidOperationException("Interferometric filter can be applied only on Atom Syndication Feed");
            }

            List<AtomItem> newitems = new List<AtomItem>();

            foreach (AtomItem item in feed.Items.ToArray()) {

                OpenSearchUrl slaveFeedUrl = GetFocusedSearchUrl(osr, item, parameters,masterEntity: entity);

                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("count", "0");

                IOpenSearchable slaveEntity = factory.Create(slaveFeedUrl);

                IOpenSearchResult slavesFeedResult0 = ose.Query(slaveEntity, nvc, typeof(AtomFeed));

                int count = int.Parse(((AtomFeed)slavesFeedResult0.Result).ElementExtensions.ReadElementExtensions<string>("totalResults", "http://a9.com/-/spec/opensearch/1.1/")[0]);

                if (count >= OpenSearchCorrelationFilter.GetMinimum(parameters)) {
                    newitems.Add(item);
                    item.Links.Add(new SyndicationLink(slaveFeedUrl, "related", "interferometry", "application/atom+xml", 0));
                } 

            }

            feed.Items = newitems;
        }

        protected virtual OpenSearchUrl GetFocusedSearchUrl(IOpenSearchResultCollection osr, AtomItem item, NameValueCollection searchParameters, IOpenSearchable masterEntity) {

            UriBuilder url = new UriBuilder(GetCorrelatedUrl(searchParameters));

            NameValueCollection nvc = HttpUtility.ParseQueryString(url.Query);
            NameValueCollection revOsParams = OpenSearchFactory.ReverseTemplateOpenSearchParameters(masterEntity.GetOpenSearchParameters(osr.ContentType));

            // - The track number, the sensor mode and the swath identifier of the master product
            nvc.Add(GetMasterParametersForSlaveFocusedSearch(osr, item, searchParameters, masterEntity));

            nvc.Add(revOsParams["cor:with"], HttpUtility.UrlEncode(item.Links.FirstOrDefault(l => l.RelationshipType == "self").Uri.ToString()));
            nvc.Add(revOsParams["cor:function"], "interfMaster");
            url.Query = nvc.ToString();

            return new OpenSearchUrl(url.Uri);

        }

        protected virtual NameValueCollection GetMasterParametersForSlaveFocusedSearch(IOpenSearchResultCollection osr, AtomItem item, NameValueCollection searchParameters, IOpenSearchable masterEntity) {

            var element = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);
            if ( element == null ) throw new InvalidOperationException("No EarthObservation SAR element found in master product to produce focus search for slaves");

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

            } else throw new InvalidOperationException("EarthObservation element found in master product to produce focus search for slaves is not SAR");

            if (string.IsNullOrEmpty(platformShortName) || string.IsNullOrEmpty(track) || string.IsNullOrEmpty(swath)) throw new InvalidOperationException(string.Format("Master SAR dataset [id:{0}] does not have all the attributes to filter slaves for interferometry", item.Id));

            NameValueCollection nvc = new NameValueCollection();
            NameValueCollection revOsParams = OpenSearchFactory.ReverseTemplateOpenSearchParameters(masterEntity.GetOpenSearchParameters(osr.ContentType));

            nvc.Add(revOsParams["cseop:platformShortName"], platformShortName);
            nvc.Add(revOsParams["cseop:wrsLongitudeGrid"], track);
            nvc.Add(revOsParams["cseop:swathIdentifier"], swath);

            return nvc;

        }

        protected virtual void PerformSlaveInterferometryFunction(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable slaveEntity) {

            AtomFeed feed;

            try {
                feed = (AtomFeed)osr;
            } catch (InvalidCastException) {
                throw new InvalidOperationException("Interferometric filter can be applied only on Atom Syndication Feed");
            }

            List<AtomItem> newitems = new List<AtomItem>();

            foreach (AtomItem slaveItem in feed.Items.ToArray()) {

                OpenSearchUrl masterFeedUrl = GetCorrelatedUrl(parameters);

                IOpenSearchable masterEntity = factory.Create(masterFeedUrl);

                IOpenSearchResult masterFeedResult = ose.Query(masterEntity, new NameValueCollection(), typeof(AtomFeed));

                AtomFeed masterFeed = (AtomFeed)masterFeedResult.Result;

                int count = int.Parse(masterFeed.ElementExtensions.ReadElementExtensions<string>("totalResults", "http://a9.com/-/spec/opensearch/1.1/")[0]);

                if (count > 1) {
                    throw new InvalidOperationException("There are multiples references to the master dataset. For this operation, a unique reference is mandatory");
                } 

                AtomItem masterItem = (AtomItem)masterFeed.Items.ElementAt(0);

                // - The ANX time period of the master product (i.e. the eop:startTimeFromAscendingNode 
                //   and eop:completionTimeFromAscendingNode attributes from the EO Product model 
                //   that must be overlapped
                var om = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(masterItem.ElementExtensions);
                if ( om == null ) throw new InvalidOperationException("No EarthObservation SAR element found in master product to produce slave interferometry");

                double masterAnxStart;
                double masterAnxStop;

                if (om is Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType) {
                    try { 
                        Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentType sarEquip = 
                            (Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentType)
                            ((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentPropertyType)
                                ((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType)om).procedure).EarthObservationEquipment;
                        masterAnxStart = sarEquip.acquisitionParameters.Acquisition.startTimeFromAscendingNode.Value;
                        masterAnxStop = sarEquip.acquisitionParameters.Acquisition.completionTimeFromAscendingNode.Value;
                    } catch (Exception e) {
                        throw new InvalidOperationException("missing information in master product to produce slave interferometry : " + e.Message);
                    }

                } else if (om is Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType) {
                    try { 
                        Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentType sarEquip = 
                            (Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentType)
                            ((Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentPropertyType)
                                ((Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType)om).procedure).EarthObservationEquipment;
                        masterAnxStart = sarEquip.acquisitionParameters.Acquisition.startTimeFromAscendingNode.Value;
                        masterAnxStop = sarEquip.acquisitionParameters.Acquisition.completionTimeFromAscendingNode.Value;
                    } catch (Exception e) {
                        throw new InvalidOperationException("missing information in master product to produce slave interferometry : " + e.Message);
                    }

                } else throw new InvalidOperationException("EarthObservation element found in master product to produce focus search for slaves is not SAR");
                    
                om = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(slaveItem.ElementExtensions);
                if ( om == null ) throw new InvalidOperationException("No EarthObservation SAR element found in master product to produce slave interferometry");

                double slaveAnxStart;
                double slaveAnxStop;

                if (om is Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType) {
                    try { 
                        Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentType sarEquip = 
                            (Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentType)
                            ((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentPropertyType)
                                ((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType)om).procedure).EarthObservationEquipment;
                        slaveAnxStart = sarEquip.acquisitionParameters.Acquisition.startTimeFromAscendingNode.Value;
                        slaveAnxStop = sarEquip.acquisitionParameters.Acquisition.completionTimeFromAscendingNode.Value;
                    } catch (Exception e) {
                        throw new InvalidOperationException("missing information in master product to produce slave interferometry : " + e.Message);
                    }

                } else if (om is Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType) {
                    try { 
                        Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentType sarEquip = 
                            (Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentType)
                            ((Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationEquipmentPropertyType)
                                ((Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType)om).procedure).EarthObservationEquipment;
                        slaveAnxStart = sarEquip.acquisitionParameters.Acquisition.startTimeFromAscendingNode.Value;
                        slaveAnxStop = sarEquip.acquisitionParameters.Acquisition.completionTimeFromAscendingNode.Value;
                    } catch (Exception e) {
                        throw new InvalidOperationException("missing information in master product to produce slave interferometry : " + e.Message);
                    }

                } else throw new InvalidOperationException("EarthObservation element found in master product to produce focus search for slaves is not SAR");



                //if (slaveAnxStart > masterAnxStop) continue;
                //if (slaveAnxStop > masterAnxStart) continue;

                // - the time period of the master product adapted according to the date difference min and 
                //   max offsets (taking respectively the beginning/end of mission dates whenever min and max offsets are absent)
                // ??



                newitems.Add(slaveItem);
            }

            feed.Items = newitems;
        }

        protected static UniqueValueDictionary<string,string> GetCorrelationOpenSearchParameters() {
            UniqueValueDictionary<string,string> osdic = OpenSearchCorrelationFilter.GetCorrelationOpenSearchParameters();

            osdic.Add("cor:normalBaseline", "baseline");
            osdic.Add("cor:burstSync", "burstSync");

            return osdic;
        }
    }
}

