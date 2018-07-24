using System;
using NUnit.Framework;
using System.Xml;
using System.IO;
using Terradue.ServiceModel.Syndication;
using Terradue.OpenSearch.Result;
using Terradue.OpenSearch.Schema;
using System.Linq;
using Terradue.Metadata.EarthObservation.OpenSearch.Extensions;
using Terradue.Metadata.EarthObservation.Ogc.Extensions;
using Terradue.Metadata.EarthObservation.OpenSearch.Helpers;

namespace Terradue.Metadata.EarthObservation.Test
{

    [TestFixture()]
    public class Metadata {


        [Test()]
        public void FindFromEOMetadata() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/search_s2.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var om = afeed.Items.First().GetEarthObservationProfile();

            var identifier = om.FindIdentifier();

            Assert.AreEqual("S2A_OPER_REP_METARC_EPA__20150620T060607_20150620T010607_20150620T010607_L0_43_2", identifier);

            var start = om.FindBeginPosition();

            Assert.AreEqual(DateTime.Parse("2015-06-20T01:06:07Z"), start);

            var stop = om.FindEndPosition();

            Assert.AreEqual(DateTime.Parse("2015-06-20T01:06:07Z"), stop);

			Assert.AreEqual(9.94, om.FindCloudCoverPercentage());

            OpenSearchDescription osd = new OpenSearchDescription();
            osd.Url = new OpenSearchDescriptionUrl[]{new OpenSearchDescriptionUrl("application/atom+xml", "http://localhost/search?q={searchTerms?}&start={time:start?}&stop={time:end?}&bbox={geo:box?}&grp={eop:productGroupId?}&id={geo:uid?}", "search")};

            string template = OpenSearchParametersHelper.EntrySelfLinkTemplate(afeed.Items.First(), osd, "application/atom+xml");

        }

		[Test()]
		public void FindFromIT4IEOMetadata()
		{

			XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/optit4i.xml", FileMode.Open, FileAccess.Read));
			SyndicationFeed feed = SyndicationFeed.Load(responseReader);

			AtomFeed afeed = new AtomFeed(feed);

            var om = afeed.Items.First().GetEarthObservationProfile();

            var identifier = om.FindIdentifier();

            Assert.AreEqual("20111", identifier);

            var start = om.FindBeginPosition();

            Assert.AreEqual(DateTime.Parse("2015-01-01T01:00:00.000+01:00"), start);

            var stop = om.FindEndPosition();

            Assert.AreEqual(DateTime.Parse("2015-02-01T01:00:00.000+01:00"), stop);

			Assert.AreEqual(-1, om.FindCloudCoverPercentage());

			OpenSearchDescription osd = new OpenSearchDescription();
			osd.Url = new OpenSearchDescriptionUrl[] { new OpenSearchDescriptionUrl("application/atom+xml", "http://localhost/search?q={searchTerms?}&start={time:start?}&stop={time:end?}&bbox={geo:box?}&grp={eop:productGroupId?}&id={geo:uid?}", "search") };

            string template = OpenSearchParametersHelper.EntrySelfLinkTemplate(afeed.Items.First(), osd, "application/atom+xml");

		}

        [Test()]
        public void FindFromAtomWithGeoRss() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/atom.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var om = afeed.Items.First().GetEarthObservationProfile();

            var identifier = om.FindIdentifier();

            Assert.AreEqual("S1A_IW_SLC__1SDH_20150129T185006_20150129T185034_004390_0055BD_FF5F", identifier);

            var start = om.FindBeginPosition();

            Assert.AreEqual(DateTime.Parse("2015-01-29T18:50:06.82Z"), start);

            var stop = om.FindEndPosition();

            Assert.AreEqual(DateTime.Parse("2015-01-29T18:50:34.085Z"), stop);

        }

        [Test()]
        public void FindEOPFromFedeoTSX()
        {
            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/fedeo-tsx.atom", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var eo = afeed.Items.First().GetEarthObservationProfile();

            var identifier = eo.FindIdentifier();

            Assert.AreEqual("urn:eop:DLR:EOWEB:TerraSAR-X_SSC:/dims_nz_pl_dfd_XXXXB00000000361145447199/dims_op_pl_dfd_//TerraSAR-X_SSC", identifier);

            var start = eo.FindBeginPosition();
        }
    }
}

