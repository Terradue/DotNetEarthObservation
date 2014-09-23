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

/*!
\defgroup modules_interferometrySearch Interferometry Search Module
@{
This module implements a search engine to perform the interferometry search. Since it uses similar paradigm of correlation in the algorithm, it relies on the \ref modules_correlationSearch for its implementation.
In addition, the module also stores and loads dataset’s valid combinations for interferometry in the database.

\image latex "graphics/component/interferometry.eps" "Interferometry search module component" width=17cm

\ingroup modules_correlationSearch

\section sec_modules_interferometrySearchPurpose Purpose

| Requirements  | Abstract | Purpose |
| ------------- | -------- | ------- |
| \req{ngEO-SUB-136-WEBS-FUN} | Grouping of Search Results | The Web server groups the results by master with a link to the slaves results |
| \req{ngEO-SUB-152-WEBS-FUN} | Dataset’s interferometry | The Web server stores datasets’ valid combinations for interferometry. |
| \req{ngEO-SUB-154-WEBS-FUN} \req{ngEO-SUB-160-WEBS-FUN} | Interferometry Search Support | The ngEO Web Server supports Interferometry Searches as defined in \docref{ngEO-14-SRD-ELC-006} |
| \req{ngEO-SUB-156-WEBS-FUN} | Interferometry Search Response - Grouping | ngEO Web Server answers to an Interferometry Search with a set of results groups, each one identified uniquely. |

\section sec_modules_interferometrySearchDependencies Dependencies

- \ref modules_openSearchEngine, used as the main engine for searching
- \ref modules_correlationSearch, used to exploit grouping algorithmic.

\section sec_modules_interferometrySearchInterfaces Interfaces 

This component implements those interfaces

\ref IOpenSearchEngineExtension

@}



*/
using Terradue.ServiceModel.Syndication;
using Terradue.Metadata.EarthObservation;

namespace Terradue.OpenSearch.Filters {
    public class OpenSearchInterferometryFilter : OpenSearchCorrelationFilter {
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

        void PerformInterferometryFunction(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable entity) {

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

        OpenSearchUrl GetFocusedSearchUrl(IOpenSearchResultCollection osr, AtomItem item, NameValueCollection searchParameters, IOpenSearchable masterEntity) {

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

        public NameValueCollection GetMasterParametersForSlaveFocusedSearch(IOpenSearchResultCollection osr, SyndicationItem item, NameValueCollection searchParameters, IOpenSearchable masterEntity) {

            var element = item.ElementExtensions.ReadElementExtensions<XmlElement>("EarthObservation", "http://www.opengis.net/sar/2.0");
            if (element.Count == 0) throw new InvalidOperationException("No EarthObservation SAR element found in master product to produce focus search for slaves");

            string platformShortName = EarthObservationOpenSearchResultHelpers.FindValueByAttributeId(element[0], "platformShortName");
            string track = EarthObservationOpenSearchResultHelpers.FindValueByAttributeId(element[0], "wrsLongitudeGrid");
            string swath = EarthObservationOpenSearchResultHelpers.FindValueByAttributeId(element[0], "swathIdentifier");

            if (string.IsNullOrEmpty(platformShortName) || string.IsNullOrEmpty(track) || string.IsNullOrEmpty(swath)) throw new InvalidOperationException(string.Format("Master SAR dataset [id:{0}] does not have all the attributes to filter slaves for interferometry", item.Id));

            NameValueCollection nvc = new NameValueCollection();
            NameValueCollection revOsParams = OpenSearchFactory.ReverseTemplateOpenSearchParameters(masterEntity.GetOpenSearchParameters(osr.ContentType));

            nvc.Add(revOsParams["cseop:platformShortName"], platformShortName);
            nvc.Add(revOsParams["cseop:wrsLongitudeGrid"], track);
            nvc.Add(revOsParams["cseop:swathIdentifier"], swath);

            return nvc;

        }

        public void PerformSlaveInterferometryFunction(ref IOpenSearchResultCollection osr, NameValueCollection parameters, IOpenSearchable slaveEntity) {

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
                //   and eop:completionTimeFromAscendingNode attributes from the EO Product model presented
                //   in [ngEO-MGICD]) that must be overlapped
                var masterEO = masterItem.ElementExtensions.ReadElementExtensions<XmlElement>("EarthObservation", "http://www.opengis.net/sar/2.0");
                if (masterEO.Count == 0) throw new InvalidOperationException("No EarthObservation SAR element found in master product to produce focus search for slaves");

                string masterAnxStart = EarthObservationOpenSearchResultHelpers.FindValueByAttributeId(masterEO[0], "startTimeFromAscendingNode");
                string masterAnxStop = EarthObservationOpenSearchResultHelpers.FindValueByAttributeId(masterEO[0], "completionTimeFromAscendingNode");

                if (string.IsNullOrEmpty(masterAnxStart) || string.IsNullOrEmpty(masterAnxStart)) throw new InvalidOperationException(string.Format("Master SAR dataset [id:{0}] does not have ANX time attributes to perform interferometry", masterItem.Id));

                var slaveEO = slaveItem.ElementExtensions.ReadElementExtensions<XmlElement>("EarthObservation", "http://www.opengis.net/sar/2.0");
                if (slaveEO.Count == 0) throw new InvalidOperationException("No EarthObservation SAR element found in slave product to produce focus search");

                string slaveAnxStart = EarthObservationOpenSearchResultHelpers.FindValueByAttributeId(slaveEO[0], "startTimeFromAscendingNode");
                string slaveAnxStop = EarthObservationOpenSearchResultHelpers.FindValueByAttributeId(slaveEO[0], "completionTimeFromAscendingNode");

                if (string.IsNullOrEmpty(slaveAnxStart) || string.IsNullOrEmpty(slaveAnxStop)) throw new InvalidOperationException(string.Format("Master SAR dataset [id:{0}] does not have ANX time attributes to perform interferometry", slaveItem.Id));


                //if (slaveAnxStart > masterAnxStop) continue;
                //if (slaveAnxStop > masterAnxStart) continue;

                // - the time period of the master product adapted according to the date difference min and 
                //   max offsets (taking respectively the beginning/end of mission dates whenever min and max offsets are absent)
                // ??



                newitems.Add(slaveItem);
            }

            feed.Items = newitems;
        }

        public new static UniqueValueDictionary<string,string> GetCorrelationOpenSearchParameters() {
            UniqueValueDictionary<string,string> osdic = OpenSearchCorrelationFilter.GetCorrelationOpenSearchParameters();

            osdic.Add("cor:normalBaseline", "baseline");
            osdic.Add("cor:burstSync", "burstSync");

            return osdic;
        }
    }
}

