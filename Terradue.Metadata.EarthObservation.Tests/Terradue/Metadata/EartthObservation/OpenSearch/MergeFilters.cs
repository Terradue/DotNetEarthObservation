using NUnit.Framework;
using System;
using Terradue.OpenSearch.Engine;
using Terradue.OpenSearch.Result;
using Terradue.OpenSearch;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Terradue.Metadata.EarthObservation.Tests
{
    [TestFixture()]
    public class MergeFilters
    {
        [Test()]
        public void TestCase()
        {
            IOpenSearchResultCollection result = null;
            try
            {
                OpenSearchEngine ose = new OpenSearchEngine();
                ose.LoadPlugins();

                var settings = new OpenSearchableFactorySettings(ose);
                settings.MergeFilters = Terradue.Metadata.EarthObservation.Helpers.GeoTimeOpenSearchHelper.MergeGeoTimeFilters;

                System.Collections.Generic.List<IOpenSearchable> osentities = new List<IOpenSearchable>();
                osentities.Add(OpenSearchFactory.FindOpenSearchable(settings, new Uri("https://catalog.terradue.com:443//ingv-stemp/search?format=json&uid=LC08_L1GT_052210_20171129_20171129_01_RT_B10_TEMP-etna.tif"), "application/atom+xml"));

                var parameters = new NameValueCollection();
                parameters.Set("format", "json");

                MultiGenericOpenSearchable multiOSE = new MultiGenericOpenSearchable(osentities, settings);
                result = ose.Query(multiOSE, parameters, typeof(AtomFeed));
            }
            catch (Exception e)
            {
                throw e;
            }

            var test = result.SerializeToString();
        }
    }
}
