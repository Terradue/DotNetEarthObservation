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

namespace Terradue.Metadata.EarthObservation.OpenSearch {
    public static class EarthObservationOpenSearchResultHelpers {



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
                        long.TryParse(pi.ProductInformation.size.Text[0], out size);
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
                foreach (var eo in item.ElementExtensions.ToList()) {
                    XElement eoElement = (XElement)XElement.ReadFrom(eo.GetReader());
                    if (eoElement.Name.LocalName == "EarthObservation") {
                        var result = eoElement.XPathSelectElement(string.Format("eop:metaDataProperty/eop:EarthObservationMetaData/eop:identifier", EONamespaces.TypeNamespaces[eo.OuterNamespace]), EONamespaces.GetXmlNamespaceManager(eoElement));
                        if (result != null) {
                            XElement identifier = new XElement(XName.Get("identifier", "http://purl.org/dc/elements/1.1/"), result.Value);
                            item.ElementExtensions.Add(identifier.CreateReader());
                        }
                    }
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
            foreach (SyndicationElementExtension ext in item.ElementExtensions.ToArray()) {

                if (ext.OuterName == "EarthObservation") {
                    Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType eopElement = 
                        (Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType)MetadataHelpers.DeserializeEarthObservation(ext.GetReader(), ext.OuterNamespace);

                    return MetadataHelpers.FindGeometryFromEarthObservation(eopElement);
                }


            }
            var dctspatial = item.ElementExtensions.ReadElementExtensions<string>("spatial", "http://purl.org/dc/terms/");
            if (dctspatial.Count > 0) {
                return GeometryFactory.WktToGeometry(dctspatial[0]);
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
                        item.ElementExtensions.Add(validTime, ser);
                    }
                } catch (Exception) {
                }
            }
        }

        /*public static XmlNode FindNodeByAttributeId(XmlElement elem, string attributeId) {

            string xpath;
            var namespaces = EONamespaces.TypeNamespaces;

            try { 
                switch (elem.NamespaceURI) {
                    case "http://www.opengis.net/eop/2.0":
                        xpath = EopEarthObservationSchema()[attributeId];
                        namespaces = EONamespaces.TypeNamespaces;
                        break;
                    case "http://www.opengis.net/sar/2.0":
                        xpath = SarEarthObservationSchema()[attributeId];
                        namespaces = EONamespaces.TypeNamespaces;
                        break;
                    case "http://www.opengis.net/opt/2.0":
                        xpath = OptEarthObservationSchema()[attributeId];
                        namespaces = EONamespaces.TypeNamespaces;
                        break;
                    case "http://www.opengis.net/alt/2.0":
                        xpath = AltEarthObservationSchema()[attributeId];
                        namespaces = EONamespaces.TypeNamespaces;
                        break;
                    case "http://www.opengis.net/eop/2.1":
                        xpath = EopEarthObservationSchema()[attributeId];
                        namespaces = EONamespaces.TypeNamespaces21;
                        break;
                    case "http://www.opengis.net/sar/2.1":
                        xpath = SarEarthObservationSchema()[attributeId];
                        namespaces = EONamespaces.TypeNamespaces21;
                        break;
                    case "http://www.opengis.net/opt/2.1":
                        xpath = OptEarthObservationSchema()[attributeId];
                        namespaces = EONamespaces.TypeNamespaces21;
                        break;
                    case "http://www.opengis.net/alt/2.1":
                        xpath = AltEarthObservationSchema()[attributeId];
                        namespaces = EONamespaces.TypeNamespaces21;
                        break;
                    default:
                        return null;
                }
            } catch (KeyNotFoundException) {
                return null;
            }

            XmlNamespaceManager xnsm = new XmlNamespaceManager(elem.OwnerDocument.NameTable);

            foreach (var key in namespaces.AllKeys) {
                xnsm.AddNamespace(namespaces[key], key);
            }
            return elem.SelectSingleNode(xpath, xnsm);

        }

        public static string FindValueByAttributeId(XmlElement elem, string attributeId) {

            XmlNode node = FindNodeByAttributeId(elem, attributeId);

            if (node == null)
                return null;

            return node.InnerText;

        }

        public static Dictionary<string, string> EopEarthObservationSchema() {

            Dictionary<string, string> dic = EopEarthObservation();
            EopEarthObservationEquipmentSchema().FirstOrDefault(kvp => {
                dic.Add(kvp.Key, "om:procedure/eop:EarthObservationEquipment/" + kvp.Value);
                return false;
            });
            EopFootprint().FirstOrDefault(kvp => {
                dic.Add(kvp.Key, "om:featureOfInterest/eop:Footprint/" + kvp.Value);
                return false;
            });
            EopEarthObservationResultSchema().FirstOrDefault(kvp => {
                dic.Add(kvp.Key, "om:result/eop:EarthObservationResult/" + kvp.Value);
                return false;
            });
            EopEarthObservationMetaDataSchema().FirstOrDefault(kvp => {
                dic.Add(kvp.Key, "eop:metaDataProperty/eop:EarthObservationMetaData/" + kvp.Value);
                return false;
            });

            return dic;

        }

        public static Dictionary<string, string> OptEarthObservationSchema() {

            Dictionary<string, string> dic = EopEarthObservationSchema();
            EopEarthObservationResultSchema().FirstOrDefault(kvp => {
                dic.Remove(kvp.Key);
                dic.Add(kvp.Key, "om:result/opt:EarthObservationResult/" + kvp.Value);
                return false;
            });
            OptEarthObservation().FirstOrDefault(kvp => {
                dic.Remove(kvp.Key);
                dic.Add(kvp.Key, kvp.Value);
                return false;
            });


            return dic;

        }

        public static Dictionary<string, string> AltEarthObservationSchema() {

            Dictionary<string, string> dic = EopEarthObservationSchema();
            AltEarthObservation().FirstOrDefault(kvp => {
                dic.Remove(kvp.Key);
                dic.Add(kvp.Key, kvp.Value);
                return false;
            });

            return dic;

        }


        public static Dictionary<string, string> SarEarthObservationSchema() {

            Dictionary<string, string> dic = EopEarthObservationSchema();
            SarEarthObservation().FirstOrDefault(kvp => {
                dic.Remove(kvp.Key);
                dic.Add(kvp.Key, kvp.Value);
                return false;
            });

            SarAcquisition().FirstOrDefault(kvp => {
                dic.Remove(kvp.Key);
                dic.Add(kvp.Key, "om:procedure/eop:EarthObservationEquipment/eop:acquisitionParameters/sar:Acquisition/" + kvp.Value);
                return false;
            });

            return dic;

        }

        public static Dictionary<string, string> EopEarthObservationEquipmentSchema() {

            Dictionary<string, string> dic = EopEarthObservationEquipment();
            EopAcquisition().FirstOrDefault(kvp => {
                dic.Add(kvp.Key, "eop:acquisitionParameters/eop:Acquisition/" + kvp.Value);
                return false;
            });

            return dic;

        }

        public static Dictionary<string, string> EopEarthObservationResultSchema() {

            Dictionary<string, string> dic = EopEarthObservationResult();
            EopProductInformation().FirstOrDefault(kvp => {
                dic.Add(kvp.Key, "eop:product/eop:ProductInformation/" + kvp.Value);
                return false;
            });

            return dic;

        }

        public static Dictionary<string, string> EopEarthObservationMetaDataSchema() {

            Dictionary<string, string> dic = EopEarthObservationMetaData();
            EopProcessingInformation().FirstOrDefault(kvp => {
                dic.Add(kvp.Key, "eop:processing/eop:ProcessingInformation/" + kvp.Value);
                return false;
            });

            return dic;

        }


        public static Dictionary<string, string> EopEarthObservation() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("beginAcquisition", "om:phenomenonTime/gml32:TimePeriod/gml32:beginPosition");
            dic.Add("endAcquisition", "om:phenomenonTime/gml32:TimePeriod/gml32:endPosition");
            dic.Add("availabilityTime", "om:resultTime/gml32:TimeInstant/gml32:timePosition");

            return dic;
        }

        public static Dictionary<string, string> EopEarthObservationEquipment() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("platformShortName", "eop:platform/eop:Platform/eop:shortName");
            dic.Add("platformSerialIdentifier", "eop:platform/eop:Platform/eop:serialIdentifier");
            dic.Add("orbitType", "eop:platform/eop:Platform/eop:orbitType");
            dic.Add("instrumentShortName", "eop:instrument/eop:Instrument/eop:shortName");
            dic.Add("instrumentDescription", "eop:instrument/eop:Instrument/eop:description");
            dic.Add("instrumentType", "eop:instrument/eop:Instrument/eop:instrumentType");
            dic.Add("sensorType", "eop:sensor/eop:Sensor/eop:sensorType");
            dic.Add("operationalMode", "eop:sensor/eop:Sensor/eop:operationalMode");
            dic.Add("swathIdentifier", "eop:sensor/eop:Sensor/eop:swathIdentifier");
            dic.Add("sensorResolution", "eop:sensor/eop:Sensor/eop:sensorResolution");

            return dic;
        }

        public static Dictionary<string, string> EopAcquisition() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("orbitNumber", "eop:orbitNumber");
            dic.Add("orbitDirection", "eop:orbitDirection");
            dic.Add("wrsLongitudeGrid", "eop:wrsLongitudeGrid");
            dic.Add("wrsLatitudeGrid", "eop:wrsLatitudeGrid");
            dic.Add("startTimeFromAscendingNode", "eop:startTimeFromAscendingNode");
            dic.Add("completionTimeFromAscendingNode", "eop:completionTimeFromAscendingNode");
            dic.Add("illuminationAzimuthAngle", "eop:illuminationAzimuthAngle");
            dic.Add("illuminationZenithAngle", "eop:illuminationZenithAngle");
            dic.Add("illuminationElevationAngle", "eop:illuminationElevationAngle");

            return dic;
        }

        public static Dictionary<string, string> EopFootprint() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("footprint", "eop:multiExtentOf");

            return dic;
        }

        public static Dictionary<string, string> EopEarthObservationResult() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            return dic;
        }

        public static Dictionary<string, string> EopProductInformation() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("productURI", "eop:fileName/ows:ServiceReference/@xlink:href");
            dic.Add("productVersion", "eop:version");
            dic.Add("productSize", "eop:size");

            return dic;
        }

        public static Dictionary<string, string> EopEarthObservationMetaData() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("productId", "eop:identifier");
            dic.Add("parentIdentifier", "eop:parentIdentifier");
            dic.Add("productType", "eop:productType");
            dic.Add("acquisitionType", "eop:acquisitionType");
            dic.Add("acquisitionSubType", "eop:acquisitionSubType");
            dic.Add("status", "eop:status");
            dic.Add("imageQualityDegradation", "eop:imageQualityDegradation");
            dic.Add("imageQualityStatus", "eop:imageQualityStatus");
            dic.Add("imageQualityDegradationTag", "eop:imageQualityDegradationTag");
            dic.Add("imageQualityReportURL", "eop:imageQualityReportURL");
            dic.Add("productGroupId", "eop:productGroupId");

            //TODO here system for additional attributes

            return dic;
        }

        public static Dictionary<string, string> EopProcessingInformation() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("processingMode", "eop:processingMode");

            return dic;
        }


        public static Dictionary<string, string> OptEarthObservation() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "om:procedure/eop:EarthObservationEquipment/eop:sensor/eop:Sensor/eop:sensorType");
            dic.Add("cloudCoverPercentage", "om:result/eop:EarthObservationResult/opt:cloudCoverPercentage");
            dic.Add("snowCoverPercentage", "om:result/eop:EarthObservationResult/opt:snowCoverPercentage");

            return dic;
        }

        public static Dictionary<string, string> SarEarthObservation() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "om:procedure/eop:EarthObservationEquipment/eop:sensor/eop:Sensor/eop:sensorType");

            return dic;
        }

        public static Dictionary<string, string> SarAcquisition() {

            Dictionary<string, string> dic = EopAcquisition();

            dic.Add("polarisationMode", "sar:polarisationMode");
            dic.Add("polarisationChannels", "sar:polarisationChannels");
            dic.Add("antennaLookDirection", "sar:antennaLookDirection");
            dic.Add("minimumIncidenceAngle", "sar:minimumIncidenceAngle");
            dic.Add("maximumIncidenceAngle", "sar:maximumIncidenceAngle");
            dic.Add("incidenceAngleVariation", "sar:incidenceAngleVariation");
            dic.Add("dopplerFrequency", "sar:dopplerFrequency");

            return dic;
        }

        public static Dictionary<string, string> AltEarthObservation() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "om:procedure/eop:EarthObservationEquipment/eop:sensor/eop:Sensor/eop:sensorType");

            return dic;
        }

        public static Dictionary<string, string> AltEarthObservationEquipment() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "eop:sensor/eop:Sensor/eop:sensorType");

            return dic;
        }

        public static Dictionary<string, string> AltFootprint() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("nominalTrack", "alt:nominalTrack");

            return dic;
        }

        public static Dictionary<string, string> LmbEarthObservation() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "om:procedure/eop:EarthObservationEquipment/eop:sensor/eop:Sensor/eop:sensorType");

            return dic;
        }

        public static Dictionary<string, string> LmbFootprint() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("nominalTrack", "lmb:nominalTrack");
            dic.Add("occultationPoints", "lmb:occultationPoints");

            return dic;
        }

        public static Dictionary<string, string> LmbSensor() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "eop:sensorType");

            return dic;
        }

        public static Dictionary<string, string> AtmEarthObservation() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "om:procedure/eop:EarthObservationEquipment/eop:sensor/eop:Sensor/eop:sensorType");
            dic.Add("cloudCoverPercentage", "atm:cloudCoverPercentage");
            dic.Add("snowCoverPercentage", "atm:snowCoverPercentage");

            return dic;
        }

        public static Dictionary<string, string> AtmAcquisition() {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("multiViewAngles", "atm:multiViewAngles");
            dic.Add("centreViewAngles", "atm:centreViewAngles");

            return dic;
        }*/

        public static string EntrySelfLinkTemplate(IOpenSearchResultItem item, OpenSearchDescription osd, string mimeType) {

            if (item == null)
                return null;

            string identifier = item.Identifier;
            string productGroupId = "";
            string start = "";
            string stop = "";

            var masterEO = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);
            if (masterEO == null)
                throw new InvalidOperationException("No EarthObservation element found in master product to find the identifier");

            productGroupId = MetadataHelpers.FindProductGroupId(masterEO);

            if (!string.IsNullOrEmpty(productGroupId)) {
                identifier = "";
                try {
                    start = MetadataHelpers.FindStart(masterEO);
                    stop = MetadataHelpers.FindStop(masterEO);
                } catch (Exception e){
                    identifier = item.Identifier;
                }
            }

            NameValueCollection nvc = OpenSearchFactory.GetOpenSearchParameters(OpenSearchFactory.GetOpenSearchUrlByType(osd, mimeType));
            nvc.AllKeys.FirstOrDefault(k => {
                if (nvc[k] == "{geo:uid?}" && !string.IsNullOrEmpty(identifier)) {
                    nvc[k] = identifier;
                }
                if (nvc[k] == "{cseop:productGroupId?}" && !string.IsNullOrEmpty(productGroupId)) {
                    nvc[k] = productGroupId;
                }
                if (nvc[k] == "{time:start?}" && !string.IsNullOrEmpty(start)) {
                    nvc[k] = start;
                }
                if (nvc[k] == "{time:end?}" && !string.IsNullOrEmpty(stop)) {
                    nvc[k] = stop;
                }
                Match matchParamDef = Regex.Match(nvc[k], @"^{([^?]+)\??}$");
                if (matchParamDef.Success)
                    nvc.Remove(k);
                return false;
            });
            UriBuilder template = new UriBuilder(OpenSearchFactory.GetOpenSearchUrlByType(osd, mimeType).Template);
            string[] queryString = Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", key, nvc[key]));
            template.Query = string.Join("&", queryString);
            return template.ToString();

        }
    }

}

