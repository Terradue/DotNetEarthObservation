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

namespace Terradue.Metadata.EarthObservation {
    public static class EarthObservationOpenSearchResultHelpers {

        public static void RestoreEnclosure(IOpenSearchResultCollection result){
            foreach (var item in result.Items) {
                RestoreEnclosure(item);
            }
        }



        public static void RestoreEnclosure(IOpenSearchResultItem item){

            var matchLinks = item.Links.Where(l => l.RelationshipType == "enclosure").ToArray();
            if (matchLinks.Count() == 0) {
                foreach (var eo in item.ElementExtensions) {
                    XElement eoElement = (XElement)XElement.ReadFrom(eo.GetReader());
                    if (eoElement.Name.LocalName == "EarthObservation") {
                        var result = eoElement.XPathSelectElement(string.Format("om:result/eop20:EarthObservationResult/eop20:product/eop20:ProductInformation", EONamespaces.TypeNamespaces[eo.OuterNamespace]), EONamespaces.GetXmlNamespaceManager(eoElement));
                        if (result != null) {
                            var link = result.XPathSelectElement("eop20:fileName/ows:ServiceReference", EONamespaces.GetXmlNamespaceManager(result));
                            if (link != null && link.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink")) != null) {
                                long size = 0;
                                var sizeElement = result.XPathSelectElement("eop20:size", EONamespaces.GetXmlNamespaceManager(result));
                                if ( sizeElement != null )
                                    long.TryParse(sizeElement.Value, out size);
                                item.Links.Add(new SyndicationLink(new Uri(link.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink")).Value), "enclosure", "Product file", "application/x-binary", size));
                            }
                        }
                    }
                }
            }
        }

        public static void RestoreIdentifier(IOpenSearchResultCollection result){
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
                        var result = eoElement.XPathSelectElement(string.Format("eop20:metaDataProperty/eop20:EarthObservationMetaData/eop20:identifier", EONamespaces.TypeNamespaces[eo.OuterNamespace]), EONamespaces.GetXmlNamespaceManager(eoElement));
                        if (result != null) {
                            XElement identifier = new XElement(XName.Get("identifier", "http://purl.org/dc/elements/1.1/"), result.Value);
                            item.ElementExtensions.Add(identifier.CreateReader());
                        }
                    }
                }
            }

        }

        public static void RestoreGeoRss(IOpenSearchResultCollection result) {

            foreach (var item in result.Items) {
                RestoreGeoRss(item);
            }

        }

        public static void RestoreGeoRss(IOpenSearchResultItem item) {

            if (item.ElementExtensions.ReadElementExtensions<XmlNode>("where", "http://www.georss.org/georss").Count == 0) {
                foreach (SyndicationElementExtension ext in item.ElementExtensions.ToArray()) {
                    XmlReader reader;
                    try {
                        reader = ext.GetReader();
                    } catch {
                        return;
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.Load(reader);
                    foreach (XmlNode node in doc.ChildNodes) {
                        if (node.LocalName == "EarthObservation") {
                            XmlNode footprint = FindNodeByAttributeId((XmlElement)node, "footprint");
                            if (footprint != null && footprint.ChildNodes[0] is XmlElement) {
                                item.ElementExtensions.Add("where", "http://www.georss.org/georss", footprint.ChildNodes[0]);
                            }
                            footprint = FindNodeByAttributeId((XmlElement)node, "nominalTrack");
                            if (footprint != null && footprint.ChildNodes[0] is XmlElement) {
                                item.ElementExtensions.Add("where", "http://www.georss.org/georss", footprint.ChildNodes[0]);
                            }
                        }
                    }
                }
            }
        }

        public static XmlNode FindNodeByAttributeId(XmlElement elem, string attributeId){

            string xpath;

            try { 
                switch (elem.NamespaceURI) {
                    case "http://www.opengis.net/eop/2.0":
                        xpath = EopEarthObservationSchema()[attributeId];
                        break;
                    case "http://www.opengis.net/sar/2.0":
                        xpath = SarEarthObservationSchema()[attributeId];
                        break;
                    default:
                        return null;
                }
            }catch ( KeyNotFoundException ) {
                return null;
            }

            XmlNamespaceManager xnsm = new XmlNamespaceManager(elem.OwnerDocument.NameTable);
            xnsm.AddNamespace("om", "http://www.opengis.net/om/2.0");
            xnsm.AddNamespace("eop", "http://www.opengis.net/eop/2.0");
            xnsm.AddNamespace("alt", "http://www.opengis.net/alt/2.0");
            xnsm.AddNamespace("sar", "http://www.opengis.net/sar/2.0");
            xnsm.AddNamespace("opt", "http://www.opengis.net/opt/2.0");
            return elem.SelectSingleNode(xpath, xnsm);

        }

        public static string FindValueByAttributeId(XmlElement elem, string attributeId){

            XmlNode node = FindNodeByAttributeId(elem, attributeId);

            if (node == null) return null;

            return node.InnerText;

        }

        public static Dictionary<string, string> EopEarthObservationSchema () {

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

        public static Dictionary<string, string> OptEarthObservationSchema () {

            Dictionary<string, string> dic = EopEarthObservationSchema();
            OptEarthObservation().FirstOrDefault(kvp => {
                dic.Remove(kvp.Key);
                dic.Add( kvp.Key , kvp.Value);
                return false;
            });

            return dic;

        }

        public static Dictionary<string, string> SarEarthObservationSchema () {

            Dictionary<string, string> dic = EopEarthObservationSchema();
            SarEarthObservation().FirstOrDefault(kvp => {
                dic.Remove(kvp.Key);
                dic.Add( kvp.Key , kvp.Value);
                return false;
            });

            SarAcquisition().FirstOrDefault(kvp => {
                dic.Remove(kvp.Key);
                dic.Add(kvp.Key, "om:procedure/eop:EarthObservationEquipment/eop:acquisitionParameters/sar:Acquisition/" + kvp.Value);
                return false;
            });

            return dic;

        }

        public static Dictionary<string, string> EopEarthObservationEquipmentSchema () {

            Dictionary<string, string> dic = EopEarthObservationEquipment();
            EopAcquisition().FirstOrDefault(kvp => {
                dic.Add(kvp.Key, "eop:acquisitionParameters/eop:Acquisition/" + kvp.Value);
                return false;
            });

            return dic;

        }

        public static Dictionary<string, string> EopEarthObservationResultSchema () {

            Dictionary<string, string> dic = EopEarthObservationResult();
            EopProductInformation().FirstOrDefault(kvp => {
                dic.Add(kvp.Key, "eop:product/eop:ProductInformation/" + kvp.Value);
                return false;
            });

            return dic;

        }

        public static Dictionary<string, string> EopEarthObservationMetaDataSchema () {

            Dictionary<string, string> dic = EopEarthObservationMetaData();
            EopProcessingInformation().FirstOrDefault(kvp => {
                dic.Add(kvp.Key, "eop:processing/eop:ProcessingInformation/" + kvp.Value);
                return false;
            });

            return dic;

        }


        public static Dictionary<string, string> EopEarthObservation(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("beginAcquisition", "om:phenomenonTime/gml:TimePeriod/gml:beginPosition");
            dic.Add("endAcquisition", "om:phenomenonTime/gml:TimePeriod/gml:endPosition");
            dic.Add("availabilityTime", "om:resultTime/gml:TimeInstant/gml:timePosition");

            return dic;
        }

        public static Dictionary<string, string> EopEarthObservationEquipment(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("platformShortName", "eop:platform/eop:Platform/eop:shortName");
            dic.Add("platformSerialIdentifier", "eop:platform/eop:Platform/eop:serialIdentifier");
            dic.Add("instrumentShortName", "eop:instrument/eop:Instrument/eop:shortName");
            dic.Add("sensorType", "eop:sensor/eop:Sensor/eop:sensorType");
            dic.Add("operationalMode", "eop:sensor/eop:Sensor/eop:operationalMode");
            dic.Add("swathIdentifier", "eop:sensor/eop:Sensor/eop:swathIdentifier");

            return dic;
        }

        public static Dictionary<string, string> EopAcquisition (){

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

        public static Dictionary<string, string> EopFootprint (){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("footprint", "eop:multiExtentOf");

            return dic;
        }

        public static Dictionary<string, string> EopEarthObservationResult  (){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            return dic;
        }

        public static Dictionary<string, string> EopProductInformation (){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("productURI", "eop:filename/ows:ServiceReference/@xlink:href");
            dic.Add("productVersion", "eop:version");
            dic.Add("productSize", "eop:size");

            return dic;
        }

        public static Dictionary<string, string> EopEarthObservationMetaData (){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("productId", "eop:identifier");
            dic.Add("parentIdentifier", "eop:parentIdentifier");
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

        public static Dictionary<string, string> EopProcessingInformation (){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("processingMode", "eop:processingMode");

            return dic;
        }


        public static Dictionary<string, string> OptEarthObservation(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "om:procedure/eop:EarthObservationEquipment/eop:sensor/eop:Sensor/eop:sensorType");
            dic.Add("cloudCoverPercentage", "om:result/eop:EarthObservationResult/opt:cloudCoverPercentage");
            dic.Add("snowCoverPercentage", "om:result/eop:EarthObservationResult/opt:snowCoverPercentage");

            return dic;
        }

        public static Dictionary<string, string> SarEarthObservation(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "om:procedure/eop:EarthObservationEquipment/eop:sensor/eop:Sensor/eop:sensorType");

            return dic;
        }

        public static Dictionary<string, string> SarAcquisition (){

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

        public static Dictionary<string, string> AltEarthObservation(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "om:procedure/eop:EarthObservationEquipment/eop:sensor/eop:Sensor/eop:sensorType");

            return dic;
        }

        public static Dictionary<string, string> AltEarthObservationEquipment(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "eop:sensor/eop:Sensor/eop:sensorType");

            return dic;
        }

        public static Dictionary<string, string> AltFootprint(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("nominalTrack", "alt:nominalTrack");

            return dic;
        }

        public static Dictionary<string, string> LmbEarthObservation(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "om:procedure/eop:EarthObservationEquipment/eop:sensor/eop:Sensor/eop:sensorType");

            return dic;
        }

        public static Dictionary<string, string> LmbFootprint(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("nominalTrack", "lmb:nominalTrack");
            dic.Add("occultationPoints", "lmb:occultationPoints");

            return dic;
        }

        public static Dictionary<string, string> LmbSensor(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "eop:sensorType");

            return dic;
        }

        public static Dictionary<string, string> AtmEarthObservation(){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("sensorType", "om:procedure/eop:EarthObservationEquipment/eop:sensor/eop:Sensor/eop:sensorType");
            dic.Add("cloudCoverPercentage", "atm:cloudCoverPercentage");
            dic.Add("snowCoverPercentage", "atm:snowCoverPercentage");

            return dic;
        }

        public static Dictionary<string, string> AtmAcquisition (){

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("multiViewAngles", "atm:multiViewAngles");
            dic.Add("centreViewAngles", "atm:centreViewAngles");

            return dic;
        }

        public static string EntrySelfLinkTemplate(IOpenSearchResultItem item, OpenSearchDescription osd, string mimeType) {

            string identifier = item.Identifier;
            return EntrySelfLinkTemplate(identifier, osd, mimeType);

        }

        public static string EntrySelfLinkTemplate(string identifier, OpenSearchDescription osd, string mimeType) {

            if (identifier == null) return null;
            NameValueCollection nvc = OpenSearchFactory.GetOpenSearchParameters(OpenSearchFactory.GetOpenSearchUrlByType(osd,mimeType));
            nvc.AllKeys.FirstOrDefault(k => {
                if (nvc[k] == "{geo:uid?}") nvc[k]=identifier;
                Match matchParamDef = Regex.Match(nvc[k], @"^{([^?]+)\??}$");
                if (matchParamDef.Success) nvc.Remove(k);
                return false;
            });
            UriBuilder template = new UriBuilder(OpenSearchFactory.GetOpenSearchUrlByType(osd, mimeType).Template);
            string[] queryString = Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", key, nvc[key]));
            template.Query = string.Join("&", queryString);
            return template.ToString();

        }
    }

}

