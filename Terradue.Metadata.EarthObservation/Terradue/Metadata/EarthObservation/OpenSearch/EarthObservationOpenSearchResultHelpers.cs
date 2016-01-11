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
using Terradue.OpenSearch.Result;
using System.IO;
using System.Linq.Expressions;
using Terradue.Metadata.EarthObservation.Spatial;
using Terradue.ServiceModel.Ogc.Om;
using Terradue.ServiceModel.Ogc.Gml321;
using Terradue.GeoJson.GeoRss;
using Terradue.GeoJson.Gml321;
using Terradue.ServiceModel.Ogc;
using Terradue.GeoJson.GeoRss10;

namespace Terradue.Metadata.EarthObservation.OpenSearch {
    
    public static class EarthObservationOpenSearchResultHelpers {

        static SpatialHelper spatialHelper = new SpatialHelper();

        public static void RestoreEnclosureFromEopResult(IOpenSearchResultCollection result) {
            foreach (var item in result.Items) {
                RestoreEnclosureFromEopResult(item);
            }
        }

        public static void RestoreEnclosureFromEopResult(IOpenSearchResultItem item) {

            var matchLinks = item.Links.Where(l => l.RelationshipType == "enclosure").ToArray();
            if (matchLinks.Count() == 0) {
                var om = MetadataHelpers.GetEarthObservationFromIOpenSearchResultItem(item);
                foreach (var link in GetEnclosuresFromEopResult(om)) {
                    item.Links.Add(link);
                }
            }
        }

        public static SyndicationLink[] GetEnclosuresFromEopResult(OM_ObservationType om) {
            List<SyndicationLink> uris = new List<SyndicationLink>();
            if (om is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                var eo = (Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)om;
                try {
                    Terradue.ServiceModel.Ogc.Eop21.EarthObservationResultType result = eo.result.Eop21EarthObservationResult;
                    foreach (var pi in result.product) {
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
                    Terradue.ServiceModel.Ogc.Eop20.EarthObservationResultType result = eo.result.Eop20EarthObservationResult;
                    foreach (var pi in result.product) {
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

        public static void RestoreIdentifierFromEopMetadata(IOpenSearchResultCollection result) {
            foreach (var item in result.Items) {
                RestoreIdentifierFromEopMetadata(item);
            }
        }

        public static void RestoreIdentifierFromEopMetadata(IOpenSearchResultItem item) {
            var elements = item.ElementExtensions.ReadElementExtensions<string>("identifier", "http://purl.org/dc/elements/1.1/");
            if (elements.Count == 0) {
                var om = MetadataHelpers.GetEarthObservationFromIOpenSearchResultItem(item);
                string identifier = "";
                try {
                    if (om is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                        var eo = (Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)om;
                        identifier = eo.EopMetaDataProperty.EarthObservationMetaData.identifier;
                    }
                    if (om is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                        var eo = (Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)om;
                        identifier = eo.EopMetaDataProperty.EarthObservationMetaData.identifier;
                    }
                }
                catch(Exception) {
                }
                if (!string.IsNullOrEmpty(identifier)) {
                    item.Identifier = identifier;
                }
            }
        }

        public static void RestoreDcDateFromOmPhenomenon(IOpenSearchResultCollection result) {

            foreach (var item in result.Items) {
                RestoreDcDateFromOmPhenomenon(item);
            }

        }

        public static void RestoreDcDateFromOmPhenomenon(IOpenSearchResultItem item) {

            if (item.ElementExtensions.ReadElementExtensions<XmlElement>("date", OgcHelpers.DC).Count == 0) {

                var om = MetadataHelpers.GetEarthObservationFromIOpenSearchResultItem(item);

                var dcdate = GetDcDateElementExtensionFromOmPhenomenon(om);

                if (dcdate != null) {
                    item.ElementExtensions.Add(dcdate);
                }
            }
        }

        public static GeometryObject FindGeometryFromEarthObservation(IEarthObservationOpenSearchResultItem item) {

            if (item.EarthObservation != null) return MetadataHelpers.FindGeometryFromEarthObservation((OM_ObservationType)item.EarthObservation);

            return null;
        }

        public static GeometryObject FindGeometryFromIOpenSearchResultItem(IOpenSearchResultItem item) {

            var dctspatial = item.ElementExtensions.ReadElementExtensions<string>("spatial", "http://purl.org/dc/terms/");
            if (dctspatial.Count > 0) {
                return WktExtensions.WktToGeometry(dctspatial[0]);
            }

            foreach (SyndicationElementExtension ext in item.ElementExtensions.ToArray()) {

                if (ext.OuterNamespace == "http://www.georss.org/georss/10") {
                    var georss10obj = GeoRss10Helper.Deserialize(ext.GetReader());
                    return georss10obj.ToGeometry();
                }


                if (ext.OuterNamespace == "http://www.georss.org/georss") {
                    var georssobj = GeoRssHelper.Deserialize(ext.GetReader());
                    return georssobj.ToGeometry();
                }
                
            }

            var om = MetadataHelpers.GetEarthObservationFromIOpenSearchResultItem(item);
            if ( om != null )
                MetadataHelpers.FindGeometryFromEarthObservation(om);

            return null;
        }


        public static void RestoreGeoRssFromEopFeature(IOpenSearchResultCollection result) {

            foreach (var item in result.Items) {

                RestoreGeoRssFromEopFeature(item);

            }
        }

        public static void RestoreGeoRssFromEopFeature(IOpenSearchResultItem item) {

            Terradue.GeoJson.GeoRss.IGeoRSS georss = null;
            foreach (SyndicationElementExtension ext in item.ElementExtensions.ToArray()) {

                if (ext.OuterNamespace == "http://www.georss.org/georss") {
                    georss = GeoRssHelper.Deserialize(ext.GetReader());
                }

            }

            if (georss == null) {

                var geom = FindGeometryFromIOpenSearchResultItem(item);

                if (geom != null) {
                    var georssobj = geom.ToGeoRss();
                    if (georssobj != null)
                        item.ElementExtensions.Add(georssobj.CreateReader());
                }

            }
                
        }

        public static SyndicationElementExtension GetDcDateElementExtensionFromOmPhenomenon(OM_ObservationType eopElement) {

            DateTime start, stop, instant;

            start = MetadataHelpers.FindStartDateFromPhenomenonTime(eopElement);
            stop = MetadataHelpers.FindEndDateFromPhenomenonTime(eopElement);
            instant = MetadataHelpers.FindInstantFromResultTime(eopElement);

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

        public static SyndicationElementExtension GetGeoRssElementExtensionFromEarthObservation(OM_ObservationType eopElement) {

            XmlElement gml = null;

            GeometryObject geometry = MetadataHelpers.FindGeometryFromEarthObservation(eopElement);

            if (geometry != null) {
                
                return new SyndicationElementExtension(geometry.ToGeoRss().CreateReader());
            }

            return null;
        }

        public static void RestoreValidTimeFromOmPhenomenon(IOpenSearchResultCollection result) {

            foreach (var item in result.Items) {
                RestoreValidTimeFromOmPhenomenon(item);
            }

        }

        public static void RestoreValidTimeFromOmPhenomenon(IOpenSearchResultItem item) {

            if (item.ElementExtensions.ReadElementExtensions<XmlNode>("validTime", "http://www.opengis.net/gml/3.2").Count == 0) {
                var om = MetadataHelpers.GetEarthObservationFromIOpenSearchResultItem(item);
                try {
                    if (om.phenomenonTime.AbstractTimeObject is TimePeriodType) {
                        XmlSerializer ser = new XmlSerializer(typeof(TimePrimitivePropertyType));
                        TimePrimitivePropertyType validTime = new TimePrimitivePropertyType();
                        validTime.AbstractTimePrimitive = (TimePeriodType)om.phenomenonTime.AbstractTimeObject;
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

            var eo = MetadataHelpers.GetEarthObservationFromIOpenSearchResultItem(item);

            return MetadataHelpers.FindIdentifierFromEopMetadata(eo);

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
            string productGroupId = "";
            DateTime start = new DateTime();
            DateTime stop = new DateTime();

            var masterEO = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (masterEO != null)
                productGroupId = MetadataHelpers.FindProductGroupIdFromEopMetadata(masterEO);

            if (!string.IsNullOrEmpty(productGroupId)) {
                identifier = "";
                try {
                    start = MetadataHelpers.FindStartDateFromPhenomenonTime(masterEO);
                    stop = MetadataHelpers.FindEndDateFromPhenomenonTime(masterEO);
                } catch (Exception e) {
                    identifier = item.Identifier;
                }
            }

            NameValueCollection nvc = OpenSearchFactory.GetOpenSearchParameters(OpenSearchFactory.GetOpenSearchUrlByType(osd, mimeType));
            nvc.AllKeys.FirstOrDefault(k => {
                if (nvc[k] == "{geo:uid?}" && !string.IsNullOrEmpty(identifier)) {
                    nvc[k] = identifier;
                }
                if (nvc[k] == "{eop:productGroupId?}" && !string.IsNullOrEmpty(productGroupId)) {
                    nvc[k] = productGroupId;
                }
                if (nvc[k] == "{time:start?}" && start.Ticks != 0) {
                    nvc[k] = start.ToUniversalTime().ToString("O");
                }
                if (nvc[k] == "{time:end?}" && stop.Ticks != 0) {
                    nvc[k] = stop.ToUniversalTime().ToString("O");
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

        public static string FindProductTypeFromOpenSearchResultItem(IOpenSearchResultItem item) {

            OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return MetadataHelpers.FindProductTypeFromEopMetadata((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return MetadataHelpers.FindProductTypeFromEopMetadata((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindOrbitNumberFromOpenSearchResultItem(IOpenSearchResultItem item) {

            OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

            if (eo != null) {

                return MetadataHelpers.FindOrbitNumberFromEopMetadata(eo);
            }  

            return null;

        }

        public static string FindOrbitDirectionFromOpenSearchResultItem(IOpenSearchResultItem item) {

            OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

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

            OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

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

            OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

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

            OM_ObservationType eo = MetadataHelpers.GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

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
                return eo.procedure.Eop20EarthObservationEquipment.platform.Platform.shortName;
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
                return ((Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType)eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).polarisationChannels;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindPolarisationChannelsFromEarthObservation20(Terradue.ServiceModel.Ogc.Sar20.SarEarthObservationType eo) {
            try {
                return ((Terradue.ServiceModel.Ogc.Sar20.SarAcquisitionType)eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition).polarisationChannels;
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

