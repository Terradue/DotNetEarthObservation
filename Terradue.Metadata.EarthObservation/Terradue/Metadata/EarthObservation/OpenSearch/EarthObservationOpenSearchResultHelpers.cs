using System;
using Terradue.OpenSearch.Result;
using System.Xml.XPath;
using Terradue.ServiceModel.Syndication;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Collections.Specialized;
using Terradue.OpenSearch.Schema;
using Terradue.OpenSearch;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Terradue.GeoJson.Geometry;
using System.IO;
using System.Linq.Expressions;
using Terradue.Metadata.EarthObservation.Spatial;
using System.Web;

namespace Terradue.Metadata.EarthObservation.OpenSearch {
    public static class EarthObservationOpenSearchResultHelpers {

        static SpatialHelper spatialHelper = new SpatialHelper();

        public static void RestoreEnclosure(IOpenSearchResultCollection result) {
            foreach (var item in result.Items) {
                RestoreEnclosure(item);
            }
        }

        public static void RestoreEnclosure(IOpenSearchResultItem item) {

            var matchLinks = item.Links.Where(l => l.RelationshipType == "enclosure").ToArray();
            if (matchLinks.Count() == 0) {
                var om = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);
                foreach (var link in GetEnclosuresFromEarthObservation(om)) {
                    item.Links.Add(link);
                }
            }
        }

        public static SyndicationLink[] GetEnclosuresFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType om) {
            List<SyndicationLink> uris = new List<SyndicationLink>();
            if (om is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                var eo = (Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)om;
                try {
                    Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationResultType result = ((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationResultPropertyType)eo.result).EarthObservationResult;
                    foreach (var pi in result.product) {
                        long size = 0;
                        long.TryParse(pi.ProductInformation.size.Text[0], out size);
                        if (size < 0) size = 0;
                        uris.Add(new SyndicationLink(new Uri(pi.ProductInformation.fileName.ServiceReference.href), "enclosure", eo.metaDataProperty1.EarthObservationMetaData.identifier, "application/x-binary", size));
                    }
                } catch (Exception) {
                }
            }
            if (om is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                var eo = (Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)om;
                try {
                    Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationResultType result = ((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationResultPropertyType)eo.result).EarthObservationResult;
                    foreach (var pi in result.product) {
                        long size = 0;
                        long.TryParse(pi.ProductInformation.size.Text, out size);
                        if (size < 0) size = 0;
                        uris.Add(new SyndicationLink(new Uri(pi.ProductInformation.fileName.ServiceReference.href), "enclosure", eo.metaDataProperty1.EarthObservationMetaData.identifier, "application/x-binary", size));
                    }
                } catch (Exception) {
                }
            }
            return uris.ToArray();
        }

        public static void RestoreIdentifier(IOpenSearchResultCollection result) {
            foreach (var item in result.Items) {
                RestoreIdentifier(item);
            }
        }

        public static void RestoreIdentifier(IOpenSearchResultItem item) {
            var elements = item.ElementExtensions.ReadElementExtensions<string>("identifier", "http://purl.org/dc/elements/1.1/");
            if (elements.Count == 0) {
                var identifier = "";
                var om = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);
                if (om != null) identifier = MetadataHelpers.FindIdentifier(om);
                if (!string.IsNullOrEmpty(identifier)) {
                    if (om != null) identifier = MetadataHelpers.FindIdentifier(om);
                    XElement identifier1 = new XElement(XName.Get("identifier", "http://purl.org/dc/elements/1.1/"), identifier);
                    item.ElementExtensions.Add(identifier1.CreateReader());
                }
            }

        }

        public static void RestoreDcDate(IOpenSearchResultCollection result) {

            foreach (var item in result.Items) {


                RestoreDcDate(item);

            }

        }

        public static void RestoreDcDate(IOpenSearchResultItem item) {

            if (item.ElementExtensions.ReadElementExtensions<XmlElement>("date", MetadataHelpers.DC).Count == 0) {

                SyndicationElementExtension georrs = null;

                if (item is IEarthObservationOpenSearchResultItem) {

                    georrs = GetDcDateElementExtensionFromEarthObservation(((IEarthObservationOpenSearchResultItem)item).EarthObservation);

                } else {

                    foreach (SyndicationElementExtension ext in item.ElementExtensions.ToArray()) {

                        if (ext.OuterName == "EarthObservation") {
                            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eopElement = 
                                (Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType)MetadataHelpers.DeserializeEarthObservation(ext.GetReader(), ext.OuterNamespace);

                            georrs = GetDcDateElementExtensionFromEarthObservation(eopElement);
                            if (ext != null) break;

                        }

                    }
                }
                if (georrs != null) {
                    item.ElementExtensions.Add(georrs);
                }
            }
        }

        public static GeometryObject FindGeometryFromEarthObservation(IOpenSearchResultItem item) {

            var dctspatial = item.ElementExtensions.ReadElementExtensions<string>("spatial", "http://purl.org/dc/terms/");
            if (dctspatial.Count > 0) {
                return GeometryFactory.WktToGeometry(dctspatial[0]);
            }

            SyndicationElementExtension georss = null;
            SyndicationElementExtension eo = null;
            foreach (SyndicationElementExtension ext in item.ElementExtensions.ToArray()) {

                if (ext.OuterNamespace == "http://www.georss.org/georss/10" || ext.OuterNamespace == "http://www.georss.org/georss") georss = ext;

                if (ext.OuterName == "EarthObservation") {
                    eo = ext;
                }


            }

            if (georss != null) {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(georss.GetReader());
                return GeometryFactory.GeoRSSToGeometry(xdoc.DocumentElement);
            }

            if (eo != null) {
                Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eopElement = 
                    (Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType)MetadataHelpers.DeserializeEarthObservation(eo.GetReader(), eo.OuterNamespace);

                return MetadataHelpers.FindGeometryFromEarthObservation(eopElement);
            }

            return null;
        }

        public static void RestoreGeoRss(IOpenSearchResultCollection result) {

            foreach (var item in result.Items) {


                RestoreGeoRss(item);

            }

        }


        public static void RestoreGeoRss(IOpenSearchResultItem item) {

            if (item.ElementExtensions.ReadElementExtensions<XmlElement>("where", "http://www.georss.org/georss").Count == 0 &&
                item.ElementExtensions.ReadElementExtensions<XmlElement>("point", "http://www.georss.org/georss").Count == 0 &&
                item.ElementExtensions.ReadElementExtensions<XmlElement>("line", "http://www.georss.org/georss").Count == 0 &&
                item.ElementExtensions.ReadElementExtensions<XmlElement>("box", "http://www.georss.org/georss").Count == 0 &&
                item.ElementExtensions.ReadElementExtensions<XmlElement>("polygon", "http://www.georss.org/georss").Count == 0 &&
                item.ElementExtensions.ReadElementExtensions<XmlElement>("where", "http://www.georss.org/georss/10").Count == 0 &&
                item.ElementExtensions.ReadElementExtensions<XmlElement>("point", "http://www.georss.org/georss/10").Count == 0 &&
                item.ElementExtensions.ReadElementExtensions<XmlElement>("line", "http://www.georss.org/georss/10").Count == 0 &&
                item.ElementExtensions.ReadElementExtensions<XmlElement>("box", "http://www.georss.org/georss/10").Count == 0 &&
                item.ElementExtensions.ReadElementExtensions<XmlElement>("polygon", "http://www.georss.org/georss/10").Count == 0) {


                SyndicationElementExtension georrs = null;

                if (item is IEarthObservationOpenSearchResultItem) {

                    georrs = GetGeoRssElementExtensionFromEarthObservation(((IEarthObservationOpenSearchResultItem)item).EarthObservation);

                } else {

                    foreach (SyndicationElementExtension ext in item.ElementExtensions.ToArray()) {

                        if (ext.OuterName == "EarthObservation") {
                            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eopElement = 
                                (Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType)MetadataHelpers.DeserializeEarthObservation(ext.GetReader(), ext.OuterNamespace);

                            georrs = GetGeoRssElementExtensionFromEarthObservation(eopElement);
                            if (ext != null) break;

                        }

                    }
                }
                if (georrs != null) {
                    item.ElementExtensions.Add(georrs);
                }
            }
        }

        public static SyndicationElementExtension GetDcDateElementExtensionFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eopElement) {

            DateTime start = new DateTime(), stop = new DateTime(), instant = new DateTime();

            try {
                if (eopElement.phenomenonTime.AbstractTimeObject is Terradue.GeoJson.Gml.TimePeriodType &&
                    ((Terradue.GeoJson.Gml.TimePeriodType)eopElement.phenomenonTime.AbstractTimeObject).Item is Terradue.GeoJson.Gml.TimePositionType &&
                    ((Terradue.GeoJson.Gml.TimePeriodType)eopElement.phenomenonTime.AbstractTimeObject).Item1 is Terradue.GeoJson.Gml.TimePositionType) start = DateTime.Parse(
                        ((Terradue.GeoJson.Gml.TimePositionType)((Terradue.GeoJson.Gml.TimePeriodType)eopElement.phenomenonTime.AbstractTimeObject).Item).Value);
                stop = DateTime.Parse(
                    ((Terradue.GeoJson.Gml.TimePositionType)((Terradue.GeoJson.Gml.TimePeriodType)eopElement.phenomenonTime.AbstractTimeObject).Item1).Value);
            } catch (Exception) {
            }

            try {
                instant = start = DateTime.Parse(eopElement.resultTime.TimeInstant.timePosition.Value);
            } catch (Exception) {
            }

            if (start.Ticks == 0) {
                string date = string.Format("{0}{1}", start.ToString("yyyy-MM-ddThh:mm:ss.fffZ"), stop.Ticks == 0 ? "" : "/" + stop.ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
                return new SyndicationElementExtension("date", MetadataHelpers.DC, date);
            }

            if (instant.Ticks == 0) {
                string date = string.Format("{0}", instant.ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
                return new SyndicationElementExtension("date", MetadataHelpers.DC, date);
            }

            return null;
        }

        public static SyndicationElementExtension GetGeoRssElementExtensionFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eopElement) {

            XmlElement gml = null;

            GeometryObject geometry = MetadataHelpers.FindGeometryFromEarthObservation(eopElement);

            if (geometry != null) {
                return new SyndicationElementExtension("where", "http://www.georss.org/georss", XElement.Parse(geometry.ToGml().OuterXml).CreateReader());
            }

            return null;
        }

        public static void RestoreValidTime(IOpenSearchResultCollection result) {

            foreach (var item in result.Items) {
                RestoreValidTime(item);
            }

        }

        public static void RestoreValidTime(IOpenSearchResultItem item) {

            if (item.ElementExtensions.ReadElementExtensions<XmlNode>("validTime", "http://www.opengis.net/gml/3.2").Count == 0) {
                var om = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);
                try {
                    if (om.phenomenonTime.AbstractTimeObject is Terradue.GeoJson.Gml.TimePeriodType) {
                        XmlSerializer ser = new XmlSerializer(typeof(Terradue.GeoJson.Gml.TimePrimitivePropertyType));
                        Terradue.GeoJson.Gml.TimePrimitivePropertyType validTime = new Terradue.GeoJson.Gml.TimePrimitivePropertyType();
                        validTime.AbstractTimePrimitive = (Terradue.GeoJson.Gml.TimePeriodType)om.phenomenonTime.AbstractTimeObject;
                        MemoryStream stream = new MemoryStream();
                        ser.Serialize(stream, validTime);
                        stream.Seek(0, SeekOrigin.Begin);
                        item.ElementExtensions.Add(XmlReader.Create(stream));
                    }
                } catch (Exception) {
                }
            }
        }

        public static string FindIdentifierFromOpenSearchResultItem(IOpenSearchResultItem item) {

            var elements = item.ElementExtensions.ReadElementExtensions<string>("identifier", "http://purl.org/dc/elements/1.1/");
            if (elements.Count > 0) return elements[0];

            var eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {
                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    try {
                        return ((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo).metaDataProperty1.EarthObservationMetaData.identifier;
                    } catch (Exception) {
                        return null;
                    }
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    try {
                        return ((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo).metaDataProperty1.EarthObservationMetaData.identifier;
                    } catch (Exception) {
                        return null;
                    }
                }
            }

            return null;

        }

        public static DateTime FindStartDateFromOpenSearchResultItem(IOpenSearchResultItem item) {

            var elements = item.ElementExtensions.ReadElementExtensions<string>("date", "http://purl.org/dc/elements/1.1/");
            if (elements.Count > 0) {
                if (elements[0].Contains('/')) {
                    return DateTime.Parse(elements[0].Split('/').First()).ToUniversalTime();
                } else return DateTime.Parse(elements[0]).ToUniversalTime();
            }

            elements = item.ElementExtensions.ReadElementExtensions<string>("date", "");
            if (elements.Count > 0) {
                if (elements[0].Contains('/')) {
                    return DateTime.Parse(elements[0].Split('/').First()).ToUniversalTime();
                } else return DateTime.Parse(elements[0]).ToUniversalTime();
            }

            var eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {
                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    try {
                        return DateTime.Parse(((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo).phenomenonTime.GmlTimePeriod.beginPosition.Value);
                    } catch (Exception) {
                        return DateTime.MinValue;
                    }
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    try {
                        return DateTime.Parse(((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo).phenomenonTime.GmlTimePeriod.beginPosition.Value);
                    } catch (Exception) {
                        return DateTime.MinValue;
                    }
                }
            }

            return DateTime.MinValue;

        }

        public static DateTime FindEndDateFromOpenSearchResultItem(IOpenSearchResultItem item) {

            var elements = item.ElementExtensions.ReadElementExtensions<string>("date", "http://purl.org/dc/elements/1.1/");
            if (elements.Count > 0) {
                if (elements[0].Contains('/')) return DateTime.Parse(elements[0].Split('/').Last()).ToUniversalTime();
            }

            elements = item.ElementExtensions.ReadElementExtensions<string>("dtend", "http://www.w3.org/2002/12/cal/ical#");
            if (elements.Count > 0) return DateTime.Parse(elements[0]).ToUniversalTime();

            elements = item.ElementExtensions.ReadElementExtensions<string>("date", "");
            if (elements.Count > 0) {
                if (elements[0].Contains('/')) return DateTime.Parse(elements[0].Split('/').Last()).ToUniversalTime();
            }

            var eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {
                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    try {
                        return DateTime.Parse(((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo).phenomenonTime.GmlTimePeriod.endPosition.Value);
                    } catch (Exception) {
                        return DateTime.MaxValue;
                    }
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    try {
                        return DateTime.Parse(((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo).phenomenonTime.GmlTimePeriod.endPosition.Value);
                    } catch (Exception) {
                        return DateTime.MaxValue;
                    }
                }
            }

            return DateTime.MaxValue;

        }

        public static string EntrySelfLinkTemplate(IOpenSearchResultItem item, OpenSearchDescription osd, string mimeType) {

            if (item == null) return null;

            string identifier = item.Identifier;
//            string productGroupId = "";
//            string start = "";
//            string stop = "";

            var masterEO = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

//            if (masterEO != null) productGroupId = MetadataHelpers.FindProductGroupId(masterEO);

            // removed because ngeo does not need it anymore
//            if (!string.IsNullOrEmpty(productGroupId)) {
//                try {
//                    start = MetadataHelpers.FindStart(masterEO);
//                    stop = MetadataHelpers.FindStop(masterEO);
//                    identifier = item.Identifier;
//                } catch (Exception e) {
//                    identifier = item.Identifier;
//                }
//            }

            NameValueCollection nvc = OpenSearchFactory.GetOpenSearchParameters(OpenSearchFactory.GetOpenSearchUrlByType(osd, mimeType));
            nvc.AllKeys.FirstOrDefault(k => {
                if (nvc[k] == "{geo:uid?}" && !string.IsNullOrEmpty(identifier)) {
                    nvc[k] = HttpUtility.UrlEncode(identifier);
                }
                // removed because ngeo does not need it anymore
//                if (nvc[k] == "{eop:productGroupId?}" && !string.IsNullOrEmpty(productGroupId)) {
//                    nvc[k] = HttpUtility.UrlEncode(productGroupId);
//                }
//                if (nvc[k] == "{time:start?}" && !string.IsNullOrEmpty(start)) {
//                    nvc[k] = start;
//                }
//                if (nvc[k] == "{time:end?}" && !string.IsNullOrEmpty(stop)) {
//                    nvc[k] = stop;
//                }
                Match matchParamDef = Regex.Match(nvc[k], @"^{([^?]+)\??}$");
                if (matchParamDef.Success) nvc.Remove(k);
                return false;
            });
            UriBuilder template = new UriBuilder(OpenSearchFactory.GetOpenSearchUrlByType(osd, mimeType).Template);
            string[] queryString = Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", key, nvc[key]));
            template.Query = string.Join("&", queryString);
            return template.Uri.AbsoluteUri;

        }

        public static string FindProductTypeFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindProductTypeFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindProductTypeFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindProductTypeFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return eo.metaDataProperty1.EarthObservationMetaData.productType;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindProductTypeFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.metaDataProperty1.EarthObservationMetaData.productType;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindParentIdentifierFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindParentIdentifierFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindParentIdentifierFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindParentIdentifierFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return eo.metaDataProperty1.EarthObservationMetaData.parentIdentifier;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindParentIdentifierFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.metaDataProperty1.EarthObservationMetaData.parentIdentifier;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOrbitNumberFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindOrbitNumberFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindOrbitNumberFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindOrbitNumberFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.orbitNumber;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOrbitNumberFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.orbitNumber;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOrbitDirectionFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindOrbitDirectionFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindOrbitDirectionFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindOrbitDirectionFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.orbitDirection.ToString();
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOrbitDirectionFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.orbitDirection.ToString();
            } catch (Exception) {
                return null;
            }
        }

        public static string FindTrackFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindTrackFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindTrackFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindTrackFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindTrackFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindFrameFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindFrameFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindFrameFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindFrameFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLatitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindFrameFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLatitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindSwathIdentifierFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindSwathIdentifierFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindSwathIdentifierFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindSwathIdentifierFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return string.Join(" ", eo.EopProcedure.EarthObservationEquipment.sensor.Sensor.swathIdentifier.Text);
            } catch (Exception) {
                return null;
            }
        }

        public static string FindSwathIdentifierFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return string.Join(" ", eo.EopProcedure.EarthObservationEquipment.sensor.Sensor.swathIdentifier.Text);
            } catch (Exception) {
                return null;
            }
        }

        public static string FindPlatformShortNameFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindPlatformNameFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindPlatformNameFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindPlatformNameFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.platform.Platform.shortName;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindPlatformNameFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.platform.Platform.shortName;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOperationalModeFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindOperationalModeFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindOperationalModeFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindOperationalModeFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return string.Join(" ", eo.EopProcedure.EarthObservationEquipment.sensor.Sensor.operationalMode.Text);
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOperationalModeFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return string.Join(" ", eo.EopProcedure.EarthObservationEquipment.sensor.Sensor.operationalMode.Text);
            } catch (Exception) {
                return null;
            }
        }

        public static string FindPolarisationChannelsFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType) {
                    return FindPolarisationChannelsFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType) {
                    return FindPolarisationChannelsFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindPolarisationChannelsFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType eo) {
            try {
                return eo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition.polarisationChannels;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindPolarisationChannelsFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType eo) {
            try {
                throw new NotImplementedException();
            } catch (Exception) {
                return null;
            }
        }

        public static string FindWrsLongitudeGridFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindWrsLongitudeGridFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindWrsLongitudeGridFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindWrsLongitudeGridFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindWrsLongitudeGridFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindProcessingLevelFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                    return FindProcessingLevelFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)eo);
                }

                if (eo is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                    return FindProcessingLevelFromEarthObservation20((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindProcessingLevelFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {
            try {
                return eo.metaDataProperty1.EarthObservationMetaData.processing[0].ProcessingInformation.processingLevel;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindProcessingLevelFromEarthObservation20(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.metaDataProperty1.EarthObservationMetaData.processing[0].ProcessingInformation.processingLevel;
            } catch (Exception) {
                return null;
            }
        }

        public static bool FilterOnValue(KeyValuePair<string, string> param, double value) {

            // range
            if (param.Value.Contains("[") || param.Value.Contains("]")) {

                bool response = true;

                string[] limits = param.Value.Split(',');

                try {
                    if (limits[0].StartsWith("[")) {
                        response &= value >= double.Parse(limits[0].Trim('['));
                    } else if (limits[0].StartsWith("]")) {
                        response &= value > double.Parse(limits[0].Trim(']'));
                        //} else if (limits.Length > 1) {
                        //    throw new FormatException(string.Format("Wrong set format for param {0}={1} : {2}", param.Key, param.Value, GetRangeSetIntervalNotation())); 
                    } else if (limits[0].EndsWith("[")) {
                        response &= value < double.Parse(limits[0].Trim('['));
                    } else if (limits[0].EndsWith("]")) {
                        response &= value <= double.Parse(limits[0].Trim(']'));
                    }

                    if (limits.Length > 1) {
                        if (limits[1].EndsWith("]")) {
                            response &= value <= double.Parse(limits[1].Trim(']'));
                        } else if (limits[1].EndsWith("[")) {
                            response &= value < double.Parse(limits[1].Trim('['));
                        }
                    }
                } catch (FormatException e) {
                    throw new FormatException(string.Format("Wrong set format for param {0}={1} : {2}", param.Key, param.Value, GetRangeSetIntervalNotation())); 
                }

                return response;

            }
            // set
            else if (param.Value.Contains("{") || param.Value.Contains("}")) {
                if (param.Value.StartsWith("{") && param.Value.EndsWith("}")) {
                    try {
                        return param.Value.Trim(new char[] {
                            '{',
                            '}'
                        }).Split(',').Any(l => double.Parse(l) == value);
                    } catch (FormatException e) {
                        throw new FormatException(string.Format("Wrong set format for param {0}={1} : {2}", param.Key, param.Value, GetRangeSetIntervalNotation())); 
                    }
                } else {
                    throw new FormatException(string.Format("Wrong set format for param {0}={1} : {2}", param.Key, param.Value, GetRangeSetIntervalNotation())); 
                }
            } 
            // value
            else {
                try {
                    return value == double.Parse(param.Value);
                } catch (FormatException e) {
                    throw new FormatException(string.Format("Wrong set format for param {0}={1} : {2}", param.Key, param.Value, GetRangeSetIntervalNotation())); 
                }
            }


        }

        public static string GetRangeSetIntervalNotation() {
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

        public static bool CoverLandMaskLimit(KeyValuePair<string, string> param, IOpenSearchResultItem item) {

            return FilterOnValue(param, spatialHelper.CalculateLandCover(item));

        }
    }

}

