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
using Terradue.Metadata.EarthObservation.OpenSearch.Extensions;
using Terradue.Metadata.EarthObservation.OpenSearch.Helpers;

namespace Terradue.Metadata.EarthObservation.Test
{

    [TestFixture()]
    public class Geometry
    {


        [Test()]
        public void FindFromEOMetadata()
        {

            XmlReader responseReader = XmlReader.Create(new FileStream(Util.TestBaseDir + "/Samples/search_s2.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var geometry = afeed.Items.First().FindGeometry();

            Assert.That(geometry is Polygon);

            OpenSearchDescription osd = new OpenSearchDescription();
            osd.Url = new OpenSearchDescriptionUrl[] { new OpenSearchDescriptionUrl("application/atom+xml", "http://localhost/search?q={searchTerms}", "search", osd.ExtraNamespace) };

            string template = OpenSearchParametersHelper.EntrySelfLinkTemplate(afeed.Items.First(), osd, "application/atom+xml");

        }

        [Test()]
        public void FindFromSpatial()
        {

            XmlReader responseReader = XmlReader.Create(new FileStream(Util.TestBaseDir + "/Samples/spatial.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var geometry = afeed.Items.First().FindGeometry();

            Assert.That(geometry is MultiPolygon);

        }

        [Test()]
        public void FindGeometryFromS2()
        {

            XmlReader responseReader = XmlReader.Create(new FileStream(Util.TestBaseDir + "/Samples/S2scihub.atom", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var geometry = afeed.Items.First().FindGeometry();

            Assert.That(geometry is MultiPolygon);

        }

        [Test()]
        public void FindGeometryFromS1()
        {

            XmlReader responseReader = XmlReader.Create(new FileStream(Util.TestBaseDir + "/Samples/S1scihub.atom", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var geometry = afeed.Items.First().FindGeometry();

            Assert.That(geometry is MultiPolygon);

        }

        [Test()]
        public void FindGeometryFromWB()
        {

            XmlReader responseReader = XmlReader.Create(new FileStream(Util.TestBaseDir + "/Samples/mwa.atom", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var geometry = afeed.Items.First().FindGeometry();

            Assert.That(geometry is Polygon);

            Assert.That(((Polygon)geometry).Coordinates[0].Count() > 5);

        }

        [Test()]
        public void FindFromMundi()
        {

            XmlReader responseReader = XmlReader.Create(new FileStream(Util.TestBaseDir + "/Samples/mundiresponse.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            foreach (var item in afeed.Items)
            {

                var geometry = item.FindGeometry();

                Assert.That(geometry is Polygon);
            }

        }


        [Test()]
        public void FindFromNewMundi()
        {

            XmlReader responseReader = XmlReader.Create(new FileStream(Util.TestBaseDir + "/Samples/gml_new_record_example.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            foreach (var item in afeed.Items)
            {

                var geometry = item.FindGeometry();

                Assert.That(geometry is MultiPolygon);
            }

        }

    }
}

