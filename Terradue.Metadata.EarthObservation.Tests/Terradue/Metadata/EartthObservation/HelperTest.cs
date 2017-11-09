using NUnit.Framework;
using System.Collections.Specialized;
using System.Reflection;
using Terradue.Metadata.EarthObservation.Helpers;

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


    }
}

