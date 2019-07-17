using NUnit.Framework;
using System;
using Terradue.OpenSearch.Engine.Extensions;
using Terradue.OpenSearch.Request;
using Terradue.OpenSearch.Result;
using System.IO;
using System.Xml;
using Terradue.ServiceModel.Syndication;
using Terradue.OpenSearch;
using System.Xml.Serialization;
using Terradue.OpenSearch.Schema;
using System.Linq;

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture()]
    public class Test {

        [Test()]
        public void TestCase() {

            XmlReader responseReader = XmlReader.Create(new FileStream(Util.TestBaseDir + "/Samples/search_s2.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(OpenSearchDescription));
            XmlReader xmlReader = XmlReader.Create(new FileStream(Util.TestBaseDir + "/Samples/S2_OSDD.xml", FileMode.Open, FileAccess.Read));
            OpenSearchDescription osd = (OpenSearchDescription)xmlSerializer.Deserialize(xmlReader);

            var self = Terradue.Metadata.EarthObservation.OpenSearch.Helpers.OpenSearchParametersHelper.EntrySelfLinkTemplate(afeed.Items.Cast<AtomItem>().First(), osd, "application/atom+xml");

            //Assert.That(self.Contains("pgrpi="));

        }
    }
}

