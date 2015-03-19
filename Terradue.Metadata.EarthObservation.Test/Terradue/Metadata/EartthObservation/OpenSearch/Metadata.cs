﻿using System;
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

            var om = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(afeed.Items.First().ElementExtensions);

            var identifier = MetadataHelpers.FindIdentifier(om);

            Assert.AreEqual("S2A_OPER_REP_METARC_EPA__20150620T060607_20150620T010607_20150620T010607_L0_43_2", identifier);

            var start = MetadataHelpers.FindStart(om);

            Assert.AreEqual("2015-06-20T01:06:07Z", start.Replace(" ", "T"));

            var stop = MetadataHelpers.FindStop(om);

            Assert.AreEqual("2015-06-20T01:06:07Z", stop.Replace(" ", "T"));

        }

        [Test()]
        public void FindFromAtomWithGeoRss() {

            XmlReader responseReader = XmlReader.Create(new FileStream("../Samples/atom.xml", FileMode.Open, FileAccess.Read));
            SyndicationFeed feed = SyndicationFeed.Load(responseReader);

            AtomFeed afeed = new AtomFeed(feed);

            var om = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(afeed.Items.First().ElementExtensions);

            var identifier = MetadataHelpers.FindIdentifier(om);

            Assert.AreEqual("S1A_IW_SLC__1SDH_20150129T185006_20150129T185034_004390_0055BD_FF5F", identifier);

            var start = MetadataHelpers.FindStart(om);

            Assert.AreEqual("2015-01-29T18:50:06.82Z", start.Replace(" ", "T"));

            var stop = MetadataHelpers.FindStop(om);

            Assert.AreEqual("2015-01-29T18:50:34.085Z", stop.Replace(" ", "T"));

        }
    }
}

