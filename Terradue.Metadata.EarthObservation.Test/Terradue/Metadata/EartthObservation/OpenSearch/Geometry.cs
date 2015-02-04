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
    public class Geometry {


        [Test()]
        public void FindFromEOMetadata() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/search_s2.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var feature = OpenSearchMetadataHelpers.FindFeatureFromOpenSearchResultItem(afeed.Items.First());

            Assert.That(feature.Geometry is MultiPolygon);

        }

        [Test()]
        public void FindFromRDF() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/rdf.xml", FileMode.Open, FileAccess.Read));
            RdfXmlDocument rdf = RdfXmlDocument.Load(responseReader);

            var feature = OpenSearchMetadataHelpers.FindFeatureFromOpenSearchResultItem(rdf.Items.First());

            Assert.That(feature.Geometry is MultiPolygon);
            Assert.AreEqual("MULTIPOLYGON(((-129.968719 48.14193,-129.968719 48.14193,-131.056732 48.272541,-131.056732 48.272541,-130.657394 49.769352,-130.657394 49.769352,-129.536499 49.638176,-129.536499 49.638176,-129.968719 48.14193)))",
                            feature.ToWkt());

        }

        [Test()]
        public void FindFromAtomWithGeoRss() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/atom.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var feature = OpenSearchMetadataHelpers.FindFeatureFromOpenSearchResultItem(afeed.Items.First());

            Assert.That(feature.Geometry is Polygon);
            Assert.AreEqual("POLYGON((-22.068716 64.517059,-16.853634 64.967606,-16.260002 63.3451,-21.184656 62.907841,-22.068716 64.517059))",
                            feature.ToWkt());

        }
    }
}

