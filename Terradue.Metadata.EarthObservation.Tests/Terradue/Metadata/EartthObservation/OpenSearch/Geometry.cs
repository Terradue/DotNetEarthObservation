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
using Terradue.OpenSearch.RdfEO.Result;

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture()]
    public class Geometry {


        [Test()]
        public void FindFromEOMetadata() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/search_s2.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var geometry = EarthObservationOpenSearchResultHelpers.FindGeometryFromEarthObservation(afeed.Items.First());

            Assert.That(geometry is MultiPolygon);

            OpenSearchDescription osd = new OpenSearchDescription();
            osd.Url = new OpenSearchDescriptionUrl[]{new OpenSearchDescriptionUrl("application/atom+xml", "http://localhost/search?q={searchTerms}", "search")};

            string template = EarthObservationOpenSearchResultHelpers.EntrySelfLinkTemplate(afeed.Items.First(), osd, "application/atom+xml");

        }

        [Test()]
        public void FindFromRDF() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/rdf.xml", FileMode.Open, FileAccess.Read));
            RdfXmlDocument rdf = RdfXmlDocument.Load(responseReader);

            var geometry = EarthObservationOpenSearchResultHelpers.FindGeometryFromEarthObservation(rdf.Items.First());

            Assert.That(geometry is MultiPolygon);
            Assert.AreEqual("MULTIPOLYGON(((-129.968719 48.14193,-131.056732 48.272541,-130.657394 49.769352,-129.536499 49.638176,-129.968719 48.14193)))",
                            geometry.ToWkt());

        }

        [Test()]
        public void FindFromRDF2() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/rdf2.xml", FileMode.Open, FileAccess.Read));
            RdfXmlDocument rdf = RdfXmlDocument.Load(responseReader);

            var geometry = EarthObservationOpenSearchResultHelpers.FindGeometryFromEarthObservation(rdf.Items.First());

            Assert.That(geometry is Polygon);

        }
    }
}

