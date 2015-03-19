using System;
using Terradue.GeoJson.Geometry;
using Terradue.OpenSearch.Result;
using System.Xml;
using Terradue.ServiceModel.Syndication;
using System.Linq;
using Terradue.GeoJson.Feature;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Specialized;
using System.Xml.Serialization;
using Terradue.Metadata.EarthObservation.OpenSearch;

namespace Terradue.Metadata.EarthObservation {
    public class MetadataHelpers {


        public const string EOP20 = "http://www.opengis.net/eop/2.0";
        public const string OPT20 = "http://www.opengis.net/opt/2.0";
        public const string SAR20 = "http://www.opengis.net/sar/2.0";
        public const string ATM20 = "http://www.opengis.net/atm/2.0";
        public const string ALT20 = "http://www.opengis.net/alt/2.0";
        public const string LMB20 = "http://www.opengis.net/lmb/2.0";
        public const string SSP20 = "http://www.opengis.net/ssp/2.0";

        public const string EOP = "http://www.opengis.net/eop/2.1";
        public const string OPT = "http://www.opengis.net/opt/2.1";
        public const string SAR = "http://www.opengis.net/sar/2.1";
        public const string ATM = "http://www.opengis.net/atm/2.1";
        public const string ALT = "http://www.opengis.net/alt/2.1";
        public const string LMB = "http://www.opengis.net/lmb/2.1";
        public const string SSP = "http://www.opengis.net/ssp/2.1";

        public const string DC = "http://purl.org/dc/elements/1.1/";
        public const string GML = "http://www.opengis.net/gml/3.2";

        public static XNamespace Eop {
            get {
                return EOP;
            }
        }

        public static XNamespace Opt {
            get {
                return OPT;
            }
        }

        public static XNamespace Sar {
            get {
                return SAR;
            }
        }

        public static XNamespace Atm {
            get {
                return ATM;
            }
        }

        public static XNamespace Alt {
            get {
                return ALT;
            }
        }

        public static XNamespace Lmb {
            get {
                return LMB;
            }
        }

        public static XNamespace Ssp {
            get {
                return SSP;
            }
        }

        static XmlSerializer eopSerializer;
        static XmlSerializer eopSerializer20;
        static XmlSerializer sarSerializer;
        static XmlSerializer sarSerializer20;
        static XmlSerializer optSerializer;
        static XmlSerializer optSerializer20;
        static XmlSerializer altSerializer;
        static XmlSerializer altSerializer20;
        static XmlSerializer multiCurveSerializer;
        static XmlSerializer multiSurfaceSerializer;


        public static XmlSerializer EopSerializer {
            get {

                if (eopSerializer == null) eopSerializer = new XmlSerializer(typeof(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType));
                return eopSerializer;

            }
        }

        public static XmlSerializer SarSerializer {
            get {

                if (sarSerializer == null) sarSerializer = new XmlSerializer(typeof(Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType));
                return sarSerializer;

            }
        }

        public static XmlSerializer OptSerializer {
            get {

                if (optSerializer == null) optSerializer = new XmlSerializer(typeof(Terradue.Metadata.EarthObservation.Ogc.Opt.OptEarthObservationType));
                return optSerializer;

            }
        }

        public static XmlSerializer AltSerializer {
            get {

                if (altSerializer == null) altSerializer = new XmlSerializer(typeof(Terradue.Metadata.EarthObservation.Ogc.Alt.AltEarthObservationType));
                return altSerializer;

            }
        }

        public static XmlSerializer EopSerializer20 {
            get {

                if (eopSerializer20 == null) eopSerializer20 = new XmlSerializer(typeof(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType));
                return eopSerializer20;

            }
        }

        public static XmlSerializer SarSerializer20 {
            get {

                if (sarSerializer20 == null) sarSerializer20 = new XmlSerializer(typeof(Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType));
                return sarSerializer20;

            }
        }

        public static XmlSerializer OptSerializer20 {
            get {

                if (optSerializer20 == null) optSerializer20 = new XmlSerializer(typeof(Terradue.Metadata.EarthObservation.Ogc.Opt20.OptEarthObservationType));
                return optSerializer20;

            }
        }

        public static XmlSerializer AltSerializer20 {
            get {

                if (altSerializer20 == null) altSerializer20 = new XmlSerializer(typeof(Terradue.Metadata.EarthObservation.Ogc.Alt20.AltEarthObservationType));
                return altSerializer20;

            }
        }

        public static XmlSerializer MultiCurveSerializer {
            get {

                if (multiCurveSerializer == null) multiCurveSerializer = new XmlSerializer(typeof(Terradue.GeoJson.Gml.MultiCurveType));
                return multiCurveSerializer;

            }
        }

        public static XmlSerializer MultiSurfaceSerializer {
            get {

                if (multiSurfaceSerializer == null) multiSurfaceSerializer = new XmlSerializer(typeof(Terradue.GeoJson.Gml.MultiSurfaceType));
                return multiSurfaceSerializer;

            }
        }

        public static XmlElement SerializeToXmlElement(object obj) {
            XmlDocument xdoc = new XmlDocument();

            Type type = obj.GetType();

            using (XmlWriter writer = xdoc.CreateNavigator().AppendChild()) {

                XmlSerializer ser = GetXmlSerializerFromType(type);

                ser.Serialize(writer, obj);
            }

            return xdoc.DocumentElement;
        }

        public static XmlSerializer GetXmlSerializerFromType(Type type) {
            if (type == typeof(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)) return MetadataHelpers.EopSerializer;

            if (type == typeof(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)) return MetadataHelpers.EopSerializer20;

            if (type == typeof(Terradue.GeoJson.Gml.MultiCurveType)) return MetadataHelpers.MultiCurveSerializer;

            if (type == typeof(Terradue.GeoJson.Gml.MultiSurfaceType)) return MetadataHelpers.MultiSurfaceSerializer;

            if (type == typeof(Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType)) return MetadataHelpers.SarSerializer;

            if (type == typeof(Terradue.Metadata.EarthObservation.Ogc.Sar20.SarEarthObservationType)) return MetadataHelpers.SarSerializer20;

            if (type == typeof(Terradue.Metadata.EarthObservation.Ogc.Opt.OptEarthObservationType)) return MetadataHelpers.OptSerializer;

            if (type == typeof(Terradue.Metadata.EarthObservation.Ogc.Opt20.OptEarthObservationType)) return MetadataHelpers.OptSerializer20;

            if (type == typeof(Terradue.Metadata.EarthObservation.Ogc.Alt.AltEarthObservationType)) return MetadataHelpers.AltSerializer;

            if (type == typeof(Terradue.Metadata.EarthObservation.Ogc.Alt20.AltEarthObservationType)) return MetadataHelpers.AltSerializer20;
            return new XmlSerializer(type);
        }

        public static Object DeserializeEarthObservation(XmlReader reader, string nspace) {

            XmlSerializer ser = null;

            switch (nspace) {

                case SSP20:
                case LMB20:
                case EOP20:
                    ser = EopSerializer20;
                    break;
                case SAR20:
                    ser = SarSerializer20;
                    break;
                case OPT20:
                    ser = OptSerializer20;
                    break;
                case ALT20:
                    ser = AltSerializer20;
                    break;
                case ATM20:
                    ser = EopSerializer20;
                    break;
                case EOP:
                case LMB:
                case SSP:
                    ser = EopSerializer;
                    break;
                case SAR:
                    ser = SarSerializer;
                    break;
                case OPT:
                    ser = OptSerializer;
                    break;
                case ALT:
                    ser = AltSerializer;
                    break;
                case ATM:
                    ser = EopSerializer;
                    break;
            }

            return ser.Deserialize(reader);
        }


        public static Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType GetEarthObservationFromSyndicationElementExtensionCollection(SyndicationElementExtensionCollection extensions) {

            foreach (var ext in extensions) {

                if (ext.OuterName == "EarthObservation") {
                    return (Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType)DeserializeEarthObservation(ext.GetReader(), ext.OuterNamespace);
                }

            }

            return null;

        }


        public static GeometryObject FindGeometryFromEarthObservation(IEarthObservationOpenSearchResultItem item) {

            if (item.EarthObservation != null) return FindGeometryFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType)item.EarthObservation);

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType earthObservation) {

            if (earthObservation != null && earthObservation is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {

                return FindGeometryFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)earthObservation);
            }

            if (earthObservation != null && earthObservation is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {

                return FindGeometryFromEarthObservation((Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)earthObservation);
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType eo) {

            if (eo.featureOfInterest != null && eo.featureOfInterest is Terradue.Metadata.EarthObservation.Ogc.Eop.FootprintPropertyType) {

                try {
                    return GeometryFactory.GmlToGeometry((XmlElement)SerializeToXmlElement(
                        ((Terradue.Metadata.EarthObservation.Ogc.Eop.FootprintPropertyType)eo.featureOfInterest).Footprint.multiExtentOf.MultiSurface));
                } catch (Exception) {
                }
            }

            if (eo.featureOfInterest != null && eo.featureOfInterest is Terradue.Metadata.EarthObservation.Ogc.Alt.AltFootprintPropertyType) {

                try {
                    return GeometryFactory.GmlToGeometry((XmlElement)SerializeToXmlElement(
                        ((Terradue.Metadata.EarthObservation.Ogc.Alt.AltFootprintPropertyType)eo.featureOfInterest).Footprint.nominalTrack.MultiCurve));
                } catch (Exception) {
                }
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType eo) {

            if (eo.featureOfInterest != null && eo.featureOfInterest is Terradue.Metadata.EarthObservation.Ogc.Eop20.FootprintPropertyType) {

                try {
                    return GeometryFactory.GmlToGeometry((XmlElement)SerializeToXmlElement(
                        ((Terradue.Metadata.EarthObservation.Ogc.Eop20.FootprintPropertyType)eo.featureOfInterest).Footprint.multiExtentOf.MultiSurface));
                } catch (Exception) {
                }
            }

            if (eo.featureOfInterest != null && eo.featureOfInterest is Terradue.Metadata.EarthObservation.Ogc.Alt20.AltFootprintPropertyType) {

                try {
                    return GeometryFactory.GmlToGeometry((XmlElement)SerializeToXmlElement(
                        ((Terradue.Metadata.EarthObservation.Ogc.Alt20.AltFootprintPropertyType)eo.featureOfInterest).Footprint.nominalTrack.MultiCurve));
                } catch (Exception) {
                }
            }

            return null;
        }

        public static string FindIdentifier(Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType om) {

            if (om is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                var eo = (Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)om;
                try {
                    return eo.metaDataProperty1.EarthObservationMetaData.identifier;
                } catch (Exception) {
                    return null;
                }
            }
            if (om is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                var eo = (Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType)om;
                try {
                    return eo.metaDataProperty1.EarthObservationMetaData.identifier;
                } catch (Exception) {
                    return null;
                }
            }

            return null;

        }

        public static string FindProductGroupId(Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType om) {

            if (om is Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType) {
                var eo = (Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationType)om;
                try {
                    return eo.metaDataProperty1.EarthObservationMetaData.productGroupId;
                } catch (Exception) {
                    return null;
                }
            }
            if (om is Terradue.Metadata.EarthObservation.Ogc.Eop20.EarthObservationType) {
                return null;
            }

            return null;

        }

        public static string FindStart(Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType om) {

            if (om.phenomenonTime != null && om.phenomenonTime.AbstractTimeObject is Terradue.GeoJson.Gml.TimePeriodType) {
                try {
                    var timePeriod = (Terradue.GeoJson.Gml.TimePeriodType)om.phenomenonTime.AbstractTimeObject;
                    return ((Terradue.GeoJson.Gml.TimePositionType)timePeriod.Item).Value;
                } catch (Exception) {
                    return null;
                }
            }

            return null;

        }

        public static string FindStop(Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType om) {

            if (om.phenomenonTime != null && om.phenomenonTime.AbstractTimeObject is Terradue.GeoJson.Gml.TimePeriodType) {
                try {
                    var timePeriod = (Terradue.GeoJson.Gml.TimePeriodType)om.phenomenonTime.AbstractTimeObject;
                    return ((Terradue.GeoJson.Gml.TimePositionType)timePeriod.Item1).Value;
                    } catch (Exception) {
                    return null;
                }
            }

            return null;

        }


        /*public static string FindIdentifierFromOpenSearchResultItem(IOpenSearchResultItem item) {

            var elements = item.ElementExtensions.ReadElementExtensions<string>("identifier", "http://purl.org/dc/elements/1.1/");
            if (elements.Count > 0)
                return elements[0];

            foreach (var eo in item.ElementExtensions.ToList()) {
                XmlElement eoElement = (XmlElement)eo.GetObject<XmlElement>();
                if (eoElement.LocalName == "EarthObservation") {
                    var result = EarthObservationOpenSearchResultHelpers.FindValueByAttributeId(eoElement, "productId");
                    if (!string.IsNullOrEmpty(result)) {
                        return result;
                    }
                }
            }

            elements = item.ElementExtensions.ReadElementExtensions<string>("identifier", "");
            if (elements.Count > 0)
                return elements[0];

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

            foreach (var eo in item.ElementExtensions.ToList()) {
                XmlElement eoElement = (XmlElement)eo.GetObject<XmlElement>();
                if (eoElement.LocalName == "EarthObservation") {
                    var start = EarthObservationOpenSearchResultHelpers.FindValueByAttributeId(eoElement, "beginAcquisition");
                    if (!string.IsNullOrEmpty(start))
                        return DateTime.Parse(start).ToUniversalTime();
                }
            }

            elements = item.ElementExtensions.ReadElementExtensions<string>("dtstart", "http://www.w3.org/2002/12/cal/ical#");
            if (elements.Count > 0)
                return DateTime.Parse(elements[0]).ToUniversalTime();

            elements = item.ElementExtensions.ReadElementExtensions<string>("date", "");
            if (elements.Count > 0) {
                if (elements[0].Contains('/')) {
                    return DateTime.Parse(elements[0].Split('/').First()).ToUniversalTime();
                } else
                    return DateTime.Parse(elements[0]).ToUniversalTime();
            }

            return DateTime.MinValue;

        }

        public static DateTime FindEndDateFromOpenSearchResultItem(IOpenSearchResultItem item) {

            var elements = item.ElementExtensions.ReadElementExtensions<string>("date", "http://purl.org/dc/elements/1.1/");
            if (elements.Count > 0) {
                if (elements[0].Contains('/'))
                    return DateTime.Parse(elements[0].Split('/').Last()).ToUniversalTime();
            }

            foreach (var eo in item.ElementExtensions.ToList()) {
                XmlElement eoElement = (XmlElement)eo.GetObject<XmlElement>();
                if (eoElement.LocalName == "EarthObservation") {
                    var stop = EarthObservationOpenSearchResultHelpers.FindValueByAttributeId(eoElement, "endAcquisition");
                    if (!string.IsNullOrEmpty(stop))
                        return DateTime.Parse(stop).ToUniversalTime();
                }
            }

            elements = item.ElementExtensions.ReadElementExtensions<string>("dtend", "http://www.w3.org/2002/12/cal/ical#");
            if (elements.Count > 0)
                return DateTime.Parse(elements[0]).ToUniversalTime();

            elements = item.ElementExtensions.ReadElementExtensions<string>("date", "");
            if (elements.Count > 0) {
                if (elements[0].Contains('/'))
                    return DateTime.Parse(elements[0].Split('/').Last()).ToUniversalTime();
            }
            return DateTime.MaxValue;

        }*/

    }
}

