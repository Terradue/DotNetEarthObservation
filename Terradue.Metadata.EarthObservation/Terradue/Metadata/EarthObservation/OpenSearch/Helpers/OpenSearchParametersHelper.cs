using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Terradue.Metadata.EarthObservation.Helpers;
using Terradue.Metadata.EarthObservation.OpenSearch.Extensions;
using Terradue.OpenSearch;
using Terradue.OpenSearch.Result;
using Terradue.OpenSearch.Schema;

namespace Terradue.Metadata.EarthObservation.OpenSearch.Helpers
{
    public static class OpenSearchParametersHelper
    {

        static SpatialHelper spatialHelper = new SpatialHelper();

        public static bool FilterOnValue(KeyValuePair<string, string> param, double value)
        {

            // range
            if (param.Value.Contains("[") || param.Value.Contains("]"))
            {

                bool response = true;

                string[] limits = param.Value.Split(',');

                try
                {
                    if (limits[0].StartsWith("["))
                    {
                        response &= value >= double.Parse(limits[0].Trim('['));
                    }
                    else if (limits[0].StartsWith("]"))
                    {
                        response &= value > double.Parse(limits[0].Trim(']'));
                        //} else if (limits.Length > 1) {
                        //    throw new FormatException(string.Format("Wrong set format for param {0}={1} : {2}", param.Key, param.Value, GetRangeSetIntervalNotation())); 
                    }
                    else if (limits[0].EndsWith("["))
                    {
                        response &= value < double.Parse(limits[0].Trim('['));
                    }
                    else if (limits[0].EndsWith("]"))
                    {
                        response &= value <= double.Parse(limits[0].Trim(']'));
                    }

                    if (limits.Length > 1)
                    {
                        if (limits[1].EndsWith("]"))
                        {
                            response &= value <= double.Parse(limits[1].Trim(']'));
                        }
                        else if (limits[1].EndsWith("["))
                        {
                            response &= value < double.Parse(limits[1].Trim('['));
                        }
                    }
                }
                catch (FormatException e)
                {
                    throw new FormatException(string.Format("Wrong set format for param {0}={1} : {2}", param.Key, param.Value, GetRangeSetIntervalNotation()));
                }

                return response;

            }
            // set
            else if (param.Value.Contains("{") || param.Value.Contains("}"))
            {
                if (param.Value.StartsWith("{") && param.Value.EndsWith("}"))
                {
                    try
                    {
                        return param.Value.Trim(new char[] {
                            '{',
                            '}'
                        }).Split(',').Any(l => double.Parse(l) == value);
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException(string.Format("Wrong set format for param {0}={1} : {2}", param.Key, param.Value, GetRangeSetIntervalNotation()));
                    }
                }
                else
                {
                    throw new FormatException(string.Format("Wrong set format for param {0}={1} : {2}", param.Key, param.Value, GetRangeSetIntervalNotation()));
                }
            }
            // value
            else
            {
                try
                {
                    return value == double.Parse(param.Value);
                }
                catch (FormatException e)
                {
                    throw new FormatException(string.Format("Wrong set format for param {0}={1} : {2}", param.Key, param.Value, GetRangeSetIntervalNotation()));
                }
            }

        }

        public static string GetRangeSetIntervalNotation()
        {
            return "mathematical notation for ranges and sets to define the intervals with: 'n1' equal to field = n1; " +
            "'{n1,n2,…}' equals to field=n1 OR field=n2 OR ...; " +
            "'[n1,n2]' equal to n1 <= field <= n2; " +
            "'[n1,n2[' equals to n1 <= field < n2; " +
            "']n1,n2[' equals to n1 < field < n2; " +
            "']n1,n2]' equal to n1 < field  <= n2; " +
            "'[n1' equals to n1<= field " +
            "']n1' equals to n1 < field; " +
            "'n2]' equals to field <= n2; " +
            "'n2[' equals to field < n2.";
        }

        public static bool CoverLandMaskLimit(KeyValuePair<string, string> param, IOpenSearchResultItem item)
        {

            return FilterOnValue(param, spatialHelper.CalculateLandCover(item));

        }

        public static string EntrySelfLinkTemplate(IOpenSearchResultItem item, OpenSearchDescription osd, string mimeType)
        {

            if (item == null)
                return null;

            string identifier = item.Identifier;

            var masterEO = item.GetEarthObservationProfile();

            NameValueCollection nvc = OpenSearchFactory.GetOpenSearchParameters(OpenSearchFactory.GetOpenSearchUrlByType(osd, mimeType));
            nvc.AllKeys.FirstOrDefault(k => {
                if (nvc[k] == "{geo:uid?}" && !string.IsNullOrEmpty(identifier))
                {
                    nvc[k] = HttpUtility.UrlEncode(identifier);
                }
                Match matchParamDef = Regex.Match(nvc[k], @"^{([^?]+)\??}$");
                if (matchParamDef.Success)
                    nvc.Remove(k);
                return false;
            });
            UriBuilder template = new UriBuilder(OpenSearchFactory.GetOpenSearchUrlByType(osd, mimeType).Template);
            string[] queryString = Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", key, nvc[key]));
            template.Query = string.Join("&", queryString);
            return template.Uri.AbsoluteUri;

        }
    }
}
