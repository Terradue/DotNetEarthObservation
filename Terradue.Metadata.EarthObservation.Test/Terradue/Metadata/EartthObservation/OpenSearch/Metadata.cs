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

        }

        [Test()]
        public void FindFromRDF() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/rdf.xml", FileMode.Open, FileAccess.Read));
            RdfXmlDocument rdf = RdfXmlDocument.Load(responseReader);

            var identifier = OpenSearchMetadataHelpers.FindIdentifierFromOpenSearchResultItem(rdf.Items.First());

            Assert.AreEqual("S1A_S4_GRDH_1SDH_20150106T145335_20150106T145400_004052_004E3B_E02B", identifier);

        }

        [Test()]
        public void FindFromAtomWithGeoRss() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/atom.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var identifier = OpenSearchMetadataHelpers.FindIdentifierFromOpenSearchResultItem(afeed.Items.First());

            Assert.AreEqual("S1A_IW_SLC__1SDH_20150129T185006_20150129T185034_004390_0055BD_FF5F", identifier);

        }
    }
}

