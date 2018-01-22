using NUnit.Framework;
using System.Collections.Specialized;
using System.Reflection;
using Terradue.Metadata.EarthObservation.Helpers;
using System.IO;
using Terradue.Metadata.EarthObservation.OpenSearch;
using Terradue.OpenSearch.Result;
using Terradue.ServiceModel.Ogc;
using System.Xml;
using Terradue.Metadata.EarthObservation.Ogc.Extensions;

namespace Terradue.Metadata.EarthObservation.Test
{

    [TestFixture()]
    public class HelperTests {


        [Test()]
        public void MergeGeoTime() {

            NameValueCollection parameters = new NameValueCollection();
            parameters.Set("bbox", "-10,-20,10,20");
            NameValueCollection overriders = new NameValueCollection();
            overriders.Set("bbox", "0,-10,20,10");

            var newParams = GeoTimeOpenSearchHelper.MergeGeoTimeFilters(parameters, overriders);

            Assert.AreEqual("POLYGON ((10 10, 10 -10, 0 -10, 0 10, 10 10))", newParams["geom"]);
        }


        [Test()]
        public void MergeGeoTime2()
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Set("bbox", "-10,-20,10,20");
            NameValueCollection overriders = new NameValueCollection();
            overriders.Set("geom", "POLYGON((6.372 47.01, 19.028 47.01, 18.896 36.527, 6.46 36.598, 6.372 47.01))");

            var newParams = GeoTimeOpenSearchHelper.MergeGeoTimeFilters(parameters, overriders);

            Assert.IsNullOrEmpty(newParams["geom"]);
            Assert.AreEqual("-180,-90,180,90", newParams["bbox"]);
            Assert.AreEqual("disjoint", newParams["rel"]);
        }


        [Test()]
        public void Encoding()
        {
            Assembly.Load("I18N, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
        }

        [Test()]
        public void CreateAtomItemFromEopProfile()
        {
            FileInfo s1 = new FileInfo("../Samples/S1EOP.xml");

            Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType sarEo = (Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType)OgcHelpers.DeserializeEarthObservation(XmlReader.Create(s1.OpenRead()));

            AtomItem item =  AtomEarthObservationFactory.CreateEarthObservationAtomItem(sarEo);

            AtomFeed feed = new AtomFeed();

            feed.Items = new AtomItem[1] { item };

            FileStream s1out = new FileStream("../out/S1EOP.atom", FileMode.Create, FileAccess.Write);

            feed.SerializeToStream(s1out);
        }
        
        
        [Test()]
        public void GetTrackFromEopProfileExtensions()
        {
            FileInfo s1 = new FileInfo("../Samples/S1AEOP.xml");

            Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType sarEo = (Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType)OgcHelpers.DeserializeEarthObservation(XmlReader.Create(s1.OpenRead()));

            Assert.AreEqual(EoProfileExtensions.FindTrack(sarEo),"147");
        }
        

        [Test()]
        public void CreateAtomItemFromEopProfileBrowse()
        {
            FileInfo s1 = new FileInfo("../Samples/S1EOPwbrowse.xml");

            Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType sarEo = (Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType)OgcHelpers.DeserializeEarthObservation(XmlReader.Create(s1.OpenRead()));

            AtomItem item = AtomEarthObservationFactory.CreateEarthObservationAtomItem(sarEo);

            AtomFeed feed = new AtomFeed();

            feed.Items = new AtomItem[1] { item };

            FileStream s1out = new FileStream("../out/S1EOPwbrowse.atom", FileMode.Create, FileAccess.Write);

            feed.SerializeToStream(s1out);
        }
    }
}

