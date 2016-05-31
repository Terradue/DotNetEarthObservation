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
using Terradue.ServiceModel.Ogc;
using Terradue.GeoJson.GeoRss;
using Terradue.GeoJson.GeoRss10;

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

        public static SyndicationLink[] GetEnclosuresFromEarthObservation(Terradue.ServiceModel.Ogc.Om.OM_ObservationType om) {
            List<SyndicationLink> uris = new List<SyndicationLink>();
            if (om is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                var eo = (Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)om;
                try {
                    foreach (var pi in eo.result.Eop21EarthObservationResult.product) {
                        long size = 0;
                        long.TryParse(pi.ProductInformation.size.Text[0], out size);
                        if (size < 0)
                            size = 0;
                        uris.Add(new SyndicationLink(new Uri(pi.ProductInformation.fileName.ServiceReference.href), "enclosure", eo.EopMetaDataProperty.EarthObservationMetaData.identifier, "application/x-binary", size));
                    }
                } catch (Exception) {
                }
            }
            if (om is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                var eo = (Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)om;
                try {
                    foreach (var pi in eo.result.Eop20EarthObservationResult.product) {
                        long size = 0;
                        long.TryParse(pi.ProductInformation.size.Text, out size);
                        if (size < 0)
                            size = 0;
                        uris.Add(new SyndicationLink(new Uri(pi.ProductInformation.fileName.ServiceReference.href), "enclosure", eo.EopMetaDataProperty.EarthObservationMetaData.identifier, "application/x-binary", size));
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
                if (om != null)
                    identifier = MetadataHelpers.FindIdentifierFromEopMetadata(om);
                if (!string.IsNullOrEmpty(identifier)) {
                    if (om != null)
                        identifier = MetadataHelpers.FindIdentifierFromEopMetadata(om);
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

            if (item.ElementExtensions.ReadElementExtensions<XmlElement>("date", OgcHelpers.DC).Count == 0) {

                SyndicationElementExtension georrs = null;

                if (item is IEarthObservationOpenSearchResultItem) {

                    georrs = GetDcDateElementExtensionFromEarthObservation(((IEarthObservationOpenSearchResultItem)item).EarthObservation);

                } else {

                    foreach (SyndicationElementExtension ext in item.ElementExtensions.ToArray()) {

                        if (ext.OuterName == "EarthObservation") {
                            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eopElement = 
                                (Terradue.ServiceModel.Ogc.Om.OM_ObservationType)OgcHelpers.DeserializeEarthObservation(ext.GetReader());

                            georrs = GetDcDateElementExtensionFromEarthObservation(eopElement);
                            if (ext != null)
                                break;

                        }

                    }
                }
                if (georrs != null) {
                    item.ElementExtensions.Add(georrs);
                }
            }
        }

        public static GeometryObject FindGeometry(IOpenSearchResultItem item){

            var geom = FindGeometryFromGeoTime(item);
            if (geom != null)
                return geom;
            return FindGeometryFromEarthObservation(item);

        }

        public static GeometryObject FindGeometryFromGeoTime(IOpenSearchResultItem item) {

            GeometryObject savegeom = null;

            if (item.ElementExtensions != null && item.ElementExtensions.Count > 0) {

                foreach (var ext in item.ElementExtensions) {

                    XmlReader xr = ext.GetReader();

                    switch (xr.NamespaceURI) {
                    // 1) search for georss
                        case "http://www.georss.org/georss":
                            savegeom = Terradue.GeoJson.GeoRss.GeoRssHelper.Deserialize(xr).ToGeometry();
                            if (xr.LocalName != "box" || xr.LocalName != "point") {
                                return savegeom;
                            }
                            break;
                    // 2) search for georss10
                        case "http://www.georss.org/georss/10":
                            savegeom = GeoRss10Extensions.ToGeometry(GeoRss10Helper.Deserialize(xr));
                            if (xr.LocalName != "box" || xr.LocalName != "point") {
                                return savegeom;
                            }
                            break;
                    // 3) search for dct:spatial
                        case "http://purl.org/dc/terms/":
                            if (xr.LocalName == "spatial")
                                savegeom = WktExtensions.WktToGeometry(xr.ReadContentAsString());
                            if (!(savegeom is Point)) {
                                return savegeom;
                            }
                            break;
                        default:
                            continue;
                    }

                }

            }

            return savegeom;

        }

        public static GeometryObject FindGeometryFromEarthObservation(IOpenSearchResultItem item) {

            GeometryObject geom = null;

            foreach (SyndicationElementExtension ext in item.ElementExtensions.ToArray()) {

                if (ext.OuterName == "EarthObservation") {
                    Terradue.ServiceModel.Ogc.Om.OM_ObservationType eopElement = 
                        (Terradue.ServiceModel.Ogc.Om.OM_ObservationType)OgcHelpers.DeserializeEarthObservation(ext.GetReader());
                    geom = MetadataHelpers.FindGeometryFromEarthObservation(eopElement);
                }

            }
            return geom;
        }

        public static void RestoreGeoRss(IOpenSearchResultCollection result) {

            foreach (var item in result.Items) {


                RestoreGeoRss(item);

            }

        }


        public static void RestoreGeoRss(IOpenSearchResultItem item) {

            var geom = FindGeometryFromGeoTime(item);

            if (geom != null)
                return;

            geom = FindGeometryFromEarthObservation(item);

            if (geom != null) {
                item.ElementExtensions.Add(geom.ToGeoRss().CreateReader());
            }
        }

        public static SyndicationElementExtension GetDcDateElementExtensionFromEarthObservation(Terradue.ServiceModel.Ogc.Om.OM_ObservationType eopElement) {

            DateTime start = new DateTime(), stop = new DateTime(), instant = new DateTime();

            try {
                if (eopElement.phenomenonTime.AbstractTimeObject is Terradue.ServiceModel.Ogc.Gml321.TimePeriodType &&
                    ((Terradue.ServiceModel.Ogc.Gml321.TimePeriodType)eopElement.phenomenonTime.AbstractTimeObject).Item is Terradue.ServiceModel.Ogc.Gml321.TimePositionType &&
                    ((Terradue.ServiceModel.Ogc.Gml321.TimePeriodType)eopElement.phenomenonTime.AbstractTimeObject).Item1 is Terradue.ServiceModel.Ogc.Gml321.TimePositionType)
                    start = DateTime.Parse(
                        ((Terradue.ServiceModel.Ogc.Gml321.TimePositionType)((Terradue.ServiceModel.Ogc.Gml321.TimePeriodType)eopElement.phenomenonTime.AbstractTimeObject).Item).Value);
                stop = DateTime.Parse(
                    ((Terradue.ServiceModel.Ogc.Gml321.TimePositionType)((Terradue.ServiceModel.Ogc.Gml321.TimePeriodType)eopElement.phenomenonTime.AbstractTimeObject).Item1).Value);
            } catch (Exception) {
            }

            try {
                instant = start = DateTime.Parse(eopElement.resultTime.TimeInstant.timePosition.Value);
            } catch (Exception) {
            }

            if (start.Ticks == 0) {
                string date = string.Format("{0}{1}", start.ToString("yyyy-MM-ddThh:mm:ss.fffZ"), stop.Ticks == 0 ? "" : "/" + stop.ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
                return new SyndicationElementExtension("date", OgcHelpers.DC, date);
            }

            if (instant.Ticks == 0) {
                string date = string.Format("{0}", instant.ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
                return new SyndicationElementExtension("date", OgcHelpers.DC, date);
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
                    if (om.phenomenonTime.AbstractTimeObject is Terradue.ServiceModel.Ogc.Gml321.TimePeriodType) {
                        XmlSerializer ser = new XmlSerializer(typeof(Terradue.ServiceModel.Ogc.Gml321.TimePrimitivePropertyType));
                        Terradue.ServiceModel.Ogc.Gml321.TimePrimitivePropertyType validTime = new Terradue.ServiceModel.Ogc.Gml321.TimePrimitivePropertyType();
                        validTime.AbstractTimePrimitive = (Terradue.ServiceModel.Ogc.Gml321.TimePeriodType)om.phenomenonTime.AbstractTimeObject;
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
            if (elements.Count > 0)
                return elements[0];

            var eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {
                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    try {
                        return ((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo).EopMetaDataProperty.EarthObservationMetaData.identifier;
                    } catch (Exception) {
                        return null;
                    }
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    try {
                        return ((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo).EopMetaDataProperty.EarthObservationMetaData.identifier;
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
                } else
                    return DateTime.Parse(elements[0]).ToUniversalTime();
            }

            elements = item.ElementExtensions.ReadElementExtensions<string>("date", "");
            if (elements.Count > 0) {
                if (elements[0].Contains('/')) {
                    return DateTime.Parse(elements[0].Split('/').First()).ToUniversalTime();
                } else
                    return DateTime.Parse(elements[0]).ToUniversalTime();
            }

            var eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {
                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    try {
                        return DateTime.Parse(((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo).phenomenonTime.GmlTimePeriod.beginPosition.Value);
                    } catch (Exception) {
                        return DateTime.MinValue;
                    }
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    try {
                        return DateTime.Parse(((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo).phenomenonTime.GmlTimePeriod.beginPosition.Value);
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
                if (elements[0].Contains('/'))
                    return DateTime.Parse(elements[0].Split('/').Last()).ToUniversalTime();
            }

            elements = item.ElementExtensions.ReadElementExtensions<string>("dtend", "http://www.w3.org/2002/12/cal/ical#");
            if (elements.Count > 0)
                return DateTime.Parse(elements[0]).ToUniversalTime();

            elements = item.ElementExtensions.ReadElementExtensions<string>("date", "");
            if (elements.Count > 0) {
                if (elements[0].Contains('/'))
                    return DateTime.Parse(elements[0].Split('/').Last()).ToUniversalTime();
            }

            var eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {
                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    try {
                        return DateTime.Parse(((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo).phenomenonTime.GmlTimePeriod.endPosition.Value);
                    } catch (Exception) {
                        return DateTime.MaxValue;
                    }
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    try {
                        return DateTime.Parse(((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo).phenomenonTime.GmlTimePeriod.endPosition.Value);
                    } catch (Exception) {
                        return DateTime.MaxValue;
                    }
                }
            }

            return DateTime.MaxValue;

        }

        public static string EntrySelfLinkTemplate(IOpenSearchResultItem item, OpenSearchDescription osd, string mimeType) {

            if (item == null)
                return null;

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
                if (matchParamDef.Success)
                    nvc.Remove(k);
                return false;
            });
            UriBuilder template = new UriBuilder(OpenSearchFactory.GetOpenSearchUrlByType(osd, mimeType).Template);
            string[] queryString = Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", key, nvc[key]));
            template.Query = string.Join("&", queryString);
            return template.Uri.AbsoluteUri;

        }

        public static string FindProductTypeFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindProductTypeFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindProductTypeFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindProductTypeFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.EopMetaDataProperty.EarthObservationMetaData.productType;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindProductTypeFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopMetaDataProperty.EarthObservationMetaData.productType;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindParentIdentifierFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindParentIdentifierFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindParentIdentifierFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindParentIdentifierFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.EopMetaDataProperty.EarthObservationMetaData.parentIdentifier;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindParentIdentifierFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopMetaDataProperty.EarthObservationMetaData.parentIdentifier;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOrbitNumberFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindOrbitNumberFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindOrbitNumberFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindOrbitNumberFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.orbitNumber;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOrbitNumberFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.orbitNumber;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOrbitDirectionFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindOrbitDirectionFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindOrbitDirectionFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindOrbitDirectionFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.orbitDirection.ToString();
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOrbitDirectionFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.orbitDirection.ToString();
            } catch (Exception) {
                return null;
            }
        }

        public static string FindTrackFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindTrackFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindTrackFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindTrackFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindTrackFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindFrameFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindFrameFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindFrameFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindFrameFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLatitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindFrameFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLatitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindSwathIdentifierFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindSwathIdentifierFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindSwathIdentifierFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindSwathIdentifierFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return string.Join(" ", eo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.swathIdentifier.Text);
            } catch (Exception) {
                return null;
            }
        }

        public static string FindSwathIdentifierFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return string.Join(" ", eo.procedure.Eop20EarthObservationEquipment.sensor.Sensor.swathIdentifier.Text);
            } catch (Exception) {
                return null;
            }
        }

        public static string FindPlatformShortNameFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindPlatformNameFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindPlatformNameFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindPlatformNameFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.procedure.Eop21EarthObservationEquipment.platform.Platform.shortName;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindPlatformNameFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.procedure.Eop20EarthObservationEquipment.platform[0].Platform.shortName;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOperationalModeFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindOperationalModeFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindOperationalModeFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindOperationalModeFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return string.Join(" ", eo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.operationalMode.Text);
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOperationalModeFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return string.Join(" ", eo.procedure.Eop20EarthObservationEquipment.sensor.Sensor.operationalMode.Text);
            } catch (Exception) {
                return null;
            }
        }

        public static string FindPolarisationChannelsFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType) {
                    return FindPolarisationChannelsFromEarthObservation((Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Sar20.SarEarthObservationType) {
                    return FindPolarisationChannelsFromEarthObservation20((Terradue.ServiceModel.Ogc.Sar20.SarEarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindPolarisationChannelsFromEarthObservation(Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType eo) {
            try {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.polarisationChannels;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindPolarisationChannelsFromEarthObservation20(Terradue.ServiceModel.Ogc.Sar20.SarEarthObservationType eo) {
            try {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.SarAcquisition.polarisationChannels;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindWrsLongitudeGridFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindWrsLongitudeGridFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindWrsLongitudeGridFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindWrsLongitudeGridFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindWrsLongitudeGridFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindProcessingLevelFromOpenSearchResultItem(IOpenSearchResultItem item) {

            Terradue.ServiceModel.Ogc.Om.OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindProcessingLevelFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindProcessingLevelFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindProcessingLevelFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.EopMetaDataProperty.EarthObservationMetaData.processing[0].ProcessingInformation.processingLevel;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindProcessingLevelFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopMetaDataProperty.EarthObservationMetaData.processing[0].ProcessingInformation.processingLevel;
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

