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
using Terradue.ServiceModel.Ogc.Om;
using Terradue.ServiceModel.Ogc.Gml321;
using Terradue.GeoJson.Gml321;

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


        public static XmlSerializer EopSerializer {
            get {

                if (eopSerializer == null) eopSerializer = new XmlSerializer(typeof(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType));
                return eopSerializer;

            }
        }

        public static XmlSerializer SarSerializer {
            get {

                if (sarSerializer == null) sarSerializer = new XmlSerializer(typeof(Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType));
                return sarSerializer;

            }
        }

        public static XmlSerializer OptSerializer {
            get {

                if (optSerializer == null) optSerializer = new XmlSerializer(typeof(Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType));
                return optSerializer;

            }
        }

        public static XmlSerializer AltSerializer {
            get {

                if (altSerializer == null) altSerializer = new XmlSerializer(typeof(Terradue.ServiceModel.Ogc.Alt21.AltEarthObservationType));
                return altSerializer;

            }
        }

        public static XmlSerializer EopSerializer20 {
            get {

                if (eopSerializer20 == null) eopSerializer20 = new XmlSerializer(typeof(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType));
                return eopSerializer20;

            }
        }

        public static XmlSerializer SarSerializer20 {
            get {

                if (sarSerializer20 == null) sarSerializer20 = new XmlSerializer(typeof(Terradue.ServiceModel.Ogc.Sar20.SarEarthObservationType));
                return sarSerializer20;

            }
        }

        public static XmlSerializer OptSerializer20 {
            get {

                if (optSerializer20 == null) optSerializer20 = new XmlSerializer(typeof(Terradue.ServiceModel.Ogc.Opt20.OptEarthObservationType));
                return optSerializer20;

            }
        }

        public static XmlSerializer AltSerializer20 {
            get {

                if (altSerializer20 == null) altSerializer20 = new XmlSerializer(typeof(Terradue.ServiceModel.Ogc.Alt20.AltEarthObservationType));
                return altSerializer20;

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
            if (type == typeof(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)) return MetadataHelpers.EopSerializer;

            if (type == typeof(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)) return MetadataHelpers.EopSerializer20;

            if (type == typeof(Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType)) return MetadataHelpers.SarSerializer;

            if (type == typeof(Terradue.ServiceModel.Ogc.Sar20.SarEarthObservationType)) return MetadataHelpers.SarSerializer20;

            if (type == typeof(Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType)) return MetadataHelpers.OptSerializer;

            if (type == typeof(Terradue.ServiceModel.Ogc.Opt20.OptEarthObservationType)) return MetadataHelpers.OptSerializer20;

            if (type == typeof(Terradue.ServiceModel.Ogc.Alt21.AltEarthObservationType)) return MetadataHelpers.AltSerializer;

            if (type == typeof(Terradue.ServiceModel.Ogc.Alt20.AltEarthObservationType)) return MetadataHelpers.AltSerializer20;
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

            object eo = null;

            try {
                eo = ser.Deserialize(reader);
            } catch (Exception e){
                throw e;
            }

            return eo;
        }


        public static OM_ObservationType GetEarthObservationFromSyndicationElementExtensionCollection(SyndicationElementExtensionCollection extensions) {

            foreach (var ext in extensions) {

                if (ext.OuterName == "EarthObservation") {
                    return (OM_ObservationType)DeserializeEarthObservation(ext.GetReader(), ext.OuterNamespace);
                }

            }

            return null;

        }


        public static GeometryObject FindGeometryFromEarthObservation(IEarthObservationOpenSearchResultItem item) {

            if (item.EarthObservation != null) return FindGeometryFromEarthObservation((OM_ObservationType)item.EarthObservation);

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(OM_ObservationType earthObservation) {

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {

                return FindGeometryFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)earthObservation);
            }

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {

                return FindGeometryFromEarthObservation((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)earthObservation);
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {

            if (eo.featureOfInterest != null && eo.featureOfInterest is Terradue.ServiceModel.Ogc.Eop21.FootprintPropertyType) {

                try {
                    ((Terradue.ServiceModel.Ogc.Eop21.FootprintPropertyType)eo.featureOfInterest).Footprint.multiExtentOf.MultiSurface.ToGeometry();
                } catch (Exception) {
                }
            }

            if (eo.featureOfInterest != null && eo.featureOfInterest is Terradue.ServiceModel.Ogc.Alt21.AltFootprintPropertyType) {

                try {
                    ((Terradue.ServiceModel.Ogc.Alt21.AltFootprintPropertyType)eo.featureOfInterest).Footprint.nominalTrack.MultiCurve.ToGeometry();
                } catch (Exception) {
                }
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {

            if (eo.featureOfInterest != null && eo.featureOfInterest is Terradue.ServiceModel.Ogc.Eop20.FootprintPropertyType) {

                try {
                    ((Terradue.ServiceModel.Ogc.Eop20.FootprintPropertyType)eo.featureOfInterest).Footprint.multiExtentOf.MultiSurface.ToGeometry();
                } catch (Exception) {
                }
            }

            if (eo.featureOfInterest != null && eo.featureOfInterest is Terradue.ServiceModel.Ogc.Alt20.AltFootprintPropertyType) {

                try {
                    ((Terradue.ServiceModel.Ogc.Alt20.AltFootprintPropertyType)eo.featureOfInterest).Footprint.nominalTrack.MultiCurve.ToGeometry();
                } catch (Exception) {
                }
            }

            return null;
        }

        public static string FindIdentifier(OM_ObservationType om) {

            if (om is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                var eo = (Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)om;
                try {
                    return eo.EopMetaDataProperty.EarthObservationMetaData.identifier;
                } catch (Exception) {
                    return null;
                }
            }
            if (om is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                var eo = (Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)om;
                try {
                    return eo.EopMetaDataProperty.EarthObservationMetaData.identifier;
                } catch (Exception) {
                    return null;
                }
            }

            return null;

        }

        public static string FindProductGroupId(OM_ObservationType om) {

            if (om is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                var eo = (Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)om;
                try {
                    return eo.EopMetaDataProperty.EarthObservationMetaData.productGroupId;
                } catch (Exception) {
                    return null;
                }
            }
            if (om is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                return null;
            }

            return null;

        }

        public static string FindStart(OM_ObservationType om) {

            if (om.phenomenonTime != null && om.phenomenonTime.AbstractTimeObject is TimePeriodType) {
                try {
                    var timePeriod = (TimePeriodType)om.phenomenonTime.AbstractTimeObject;
                    return ((TimePositionType)timePeriod.Item).Value;
                } catch (Exception) {
                    return null;
                }
            }

            return null;

        }

        public static string FindStop(OM_ObservationType om) {

            if (om.phenomenonTime != null && om.phenomenonTime.AbstractTimeObject is TimePeriodType) {
                try {
                    var timePeriod = (TimePeriodType)om.phenomenonTime.AbstractTimeObject;
                    return ((TimePositionType)timePeriod.Item1).Value;
                    } catch (Exception) {
                    return null;
                }
            }

            return null;

        }


    }
}

