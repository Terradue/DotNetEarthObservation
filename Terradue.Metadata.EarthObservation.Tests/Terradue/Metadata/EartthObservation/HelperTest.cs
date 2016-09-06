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
using System.Collections.Specialized;

namespace Terradue.Metadata.EarthObservation.Test {

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

       
    }
}

