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

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture()]
    public class Geometry {


        [Test()]
        public void FindFromEOMetadata() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/search_s2.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var geometry = afeed.Items.First().FindGeometry();

            Assert.That(geometry is Polygon);

            OpenSearchDescription osd = new OpenSearchDescription();
            osd.Url = new OpenSearchDescriptionUrl[]{new OpenSearchDescriptionUrl("application/atom+xml", "http://localhost/search?q={searchTerms}", "search")};

            string template = OpenSearchParametersHelper.EntrySelfLinkTemplate(afeed.Items.First(), osd, "application/atom+xml");

        }

        [Test()]
        public void FindFromSpatial() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/spatial.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var geometry = afeed.Items.First().FindGeometry();

            Assert.That(geometry is MultiPolygon);

        }

		[Test()]
		public void FindGeometryFromS2()
		{

			XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/S2scihub.atom", FileMode.Open, FileAccess.Read));
			SyndicationFeed feed = SyndicationFeed.Load(responseReader);

			AtomFeed afeed = new AtomFeed(feed);

            var geometry = afeed.Items.First().FindGeometry();

			Assert.That(geometry is MultiPolygon);

		}

		[Test()]
		public void FindGeometryFromS1()
		{

			XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/S1scihub.atom", FileMode.Open, FileAccess.Read));
			SyndicationFeed feed = SyndicationFeed.Load(responseReader);

			AtomFeed afeed = new AtomFeed(feed);

            var geometry = afeed.Items.First().FindGeometry();

			Assert.That(geometry is MultiPolygon);

		}

    }
}

