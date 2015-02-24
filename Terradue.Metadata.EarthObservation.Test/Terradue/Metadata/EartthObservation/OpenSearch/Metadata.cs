using System;
using NUnit.Framework;
using System.Xml;
using System.IO;
using Terradue.ServiceModel.Syndication;
using Terradue.OpenSearch.Result;
using System.Xml.Serialization;
using Terradue.OpenSearch.Schema;
using System.Linq;
using Terradue.Metadata.EarthObservation.OpenSearch;
using Terradue.GeoJson.Geometry;
using Terradue.OpenSearch.GeoJson.Result;

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture()]
    public class Metadata {


        [Test()]
        public void FindFromEOMetadata() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/search_s2.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var identifier = OpenSearchMetadataHelpers.FindIdentifierFromOpenSearchResultItem(afeed.Items.First());

            Assert.AreEqual("S2A_OPER_REP_METARC_EPA__20150620T060607_20150620T010607_20150620T010607_L0_43_2", identifier);

            var start = OpenSearchMetadataHelpers.FindStartDateFromOpenSearchResultItem(afeed.Items.First());

            Assert.AreEqual("2015-06-20T01:06:07Z", start.ToString("u").Replace(" ", "T"));

            var stop = OpenSearchMetadataHelpers.FindEndDateFromOpenSearchResultItem(afeed.Items.First());

            Assert.AreEqual("2015-06-20T01:06:07Z", stop.ToString("u").Replace(" ", "T"));

        }

        [Test()]
        public void FindFromRDF() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/rdf.xml", FileMode.Open, FileAccess.Read));
            RdfXmlDocument rdf = RdfXmlDocument.Load(responseReader);

            var identifier = OpenSearchMetadataHelpers.FindIdentifierFromOpenSearchResultItem(rdf.Items.First());

            Assert.AreEqual("S1A_S4_GRDH_1SDH_20150106T145335_20150106T145400_004052_004E3B_E02B", identifier);

            var start = OpenSearchMetadataHelpers.FindStartDateFromOpenSearchResultItem(rdf.Items.First());

            Assert.AreEqual("2015-01-06T14:53:35Z", start.ToString("u").Replace(" ", "T"));

            var stop = OpenSearchMetadataHelpers.FindEndDateFromOpenSearchResultItem(rdf.Items.First());

            Assert.AreEqual("2015-01-06T14:54:00Z", stop.ToString("u").Replace(" ", "T"));
        }

        [Test()]
        public void FindFromAtomWithGeoRss() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/atom.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var identifier = OpenSearchMetadataHelpers.FindIdentifierFromOpenSearchResultItem(afeed.Items.First());

            Assert.AreEqual("S1A_IW_SLC__1SDH_20150129T185006_20150129T185034_004390_0055BD_FF5F", identifier);

            var start = OpenSearchMetadataHelpers.FindStartDateFromOpenSearchResultItem(afeed.Items.First());

            Assert.AreEqual("2015-01-29T18:50:06Z", start.ToString("u").Replace(" ", "T"));

            var stop = OpenSearchMetadataHelpers.FindEndDateFromOpenSearchResultItem(afeed.Items.First());

            Assert.AreEqual("2015-01-29T18:50:34Z", stop.ToString("u").Replace(" ", "T"));

        }

        [Test()]
        public void FindFromGeoJson() {

            var feed = FeatureCollectionResult.DeserializeFromStream(new FileStream("../Samples/S1.json", FileMode.Open, FileAccess.Read));

            var identifier = OpenSearchMetadataHelpers.FindIdentifierFromOpenSearchResultItem(feed.Items.First());

            Assert.AreEqual("S1A_IW_SLC__1SDV_20150124T165611_20150124T165638_004316_005423_20AC", identifier);

            var start = OpenSearchMetadataHelpers.FindStartDateFromOpenSearchResultItem(feed.Items.First());

            Assert.AreEqual("2015-01-24T16:56:11Z", start.ToString("u").Replace(" ", "T"));

            var stop = OpenSearchMetadataHelpers.FindEndDateFromOpenSearchResultItem(feed.Items.First());

            Assert.AreEqual("2015-01-24T16:56:38Z", stop.ToString("u").Replace(" ", "T"));

        }
    }
}

