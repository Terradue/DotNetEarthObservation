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
using Terradue.ServiceModel.Ogc;

namespace Terradue.Metadata.EarthObservation {
    public class MetadataHelpers {


        public static OM_ObservationType GetEarthObservationFromIOpenSearchResultItem(IOpenSearchResultItem item) {

            if (item is IEarthObservationOpenSearchResultItem)
                return ((IEarthObservationOpenSearchResultItem)item).EarthObservation;

            return GetEarthObservationFromSyndicationElementExtensionCollection(item.ElementExtensions);

        }

        public static OM_ObservationType GetEarthObservationFromSyndicationElementExtensionCollection(SyndicationElementExtensionCollection extensions) {

            foreach (var ext in extensions) {
                if (ext.OuterName == "EarthObservation") {
                    return (OM_ObservationType)OgcHelpers.DeserializeEarthObservation(ext.GetReader());
                }
            }

            return null;

        }

        public static GeometryObject FindGeometryFromEarthObservation(OM_ObservationType earthObservation) {


            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Alt21.AltEarthObservationType)
            {

                return FindGeometryFromEarthObservation((Terradue.ServiceModel.Ogc.Alt21.AltEarthObservationType)earthObservation);
            }

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Ssp21.SspEarthObservationType)
            {

                return FindGeometryFromEarthObservation((Terradue.ServiceModel.Ogc.Ssp21.SspEarthObservationType)earthObservation);
            }

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Lmb21.LmbEarthObservationType)
            {

                return FindGeometryFromEarthObservation((Terradue.ServiceModel.Ogc.Lmb21.LmbEarthObservationType)earthObservation);
            }

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {

                return FindGeometryFromEarthObservation((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)earthObservation);
            }

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Alt20.AltEarthObservationType)
            {

                return FindGeometryFromEarthObservation((Terradue.ServiceModel.Ogc.Alt20.AltEarthObservationType)earthObservation);
            }

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {

                return FindGeometryFromEarthObservation((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)earthObservation);
            }

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Ssp20.SspEarthObservationType)
            {

                return FindGeometryFromEarthObservation((Terradue.ServiceModel.Ogc.Ssp20.SspEarthObservationType)earthObservation);
            }

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Lmb20.LmbEarthObservationType)
            {

                return FindGeometryFromEarthObservation((Terradue.ServiceModel.Ogc.Lmb20.LmbEarthObservationType)earthObservation);
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {

            if (eo.featureOfInterest != null) {

                if (eo.featureOfInterest.Eop21Footprint != null) {
                    try {
                        return eo.featureOfInterest.Eop21Footprint.multiExtentOf.MultiSurface.ToGeometry();
                    } catch (Exception e) {
                    }
                }
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.ServiceModel.Ogc.Alt21.AltEarthObservationType eo)
        {

            if (eo.featureOfInterest != null)
            {

                if (eo.featureOfInterest.Alt21Footprint != null)
                {
                    try
                    {
                        return eo.featureOfInterest.Alt21Footprint.nominalTrack.MultiCurve.ToGeometry();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.ServiceModel.Ogc.Ssp21.SspEarthObservationType eo)
        {

            if (eo.featureOfInterest != null)
            {

                if (eo.featureOfInterest.Ssp21Footprint != null)
                {
                    try
                    {
                        return eo.featureOfInterest.Ssp21Footprint.nominalTrack.MultiCurve.ToGeometry();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.ServiceModel.Ogc.Lmb21.LmbEarthObservationType eo)
        {

            if (eo.featureOfInterest != null)
            {

                if (eo.featureOfInterest.Lmb21Footprint != null)
                {
                    try
                    {
                        return eo.featureOfInterest.Lmb21Footprint.occultationPoints.MultiPoint.ToGeometry();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {

            if (eo.featureOfInterest != null) {

                if (eo.featureOfInterest.Eop20Footprint != null) {
                    try {
                        return eo.featureOfInterest.Eop20Footprint.multiExtentOf.MultiSurface.ToGeometry();
                    } catch (Exception) {
                    }
                }
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.ServiceModel.Ogc.Alt20.AltEarthObservationType eo)
        {

            if (eo.featureOfInterest != null)
            {

                if (eo.featureOfInterest.Alt20Footprint != null)
                {
                    try
                    {
                        return eo.featureOfInterest.Alt20Footprint.nominalTrack.MultiCurve.ToGeometry();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.ServiceModel.Ogc.Ssp20.SspEarthObservationType eo)
        {

            if (eo.featureOfInterest != null)
            {

                if (eo.featureOfInterest.Ssp20Footprint != null)
                {
                    try
                    {
                        return eo.featureOfInterest.Ssp20Footprint.multiExtentOf.MultiSurface.ToGeometry();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return null;
        }

        public static GeometryObject FindGeometryFromEarthObservation(Terradue.ServiceModel.Ogc.Lmb20.LmbEarthObservationType eo)
        {

            if (eo.featureOfInterest != null)
            {

                if (eo.featureOfInterest.Lmb20Footprint != null)
                {
                    try
                    {
                        return eo.featureOfInterest.Lmb20Footprint.occultationPoints.MultiPoint.ToGeometry();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return null;
        }

        public static string FindIdentifierFromEopMetadata(OM_ObservationType om) {

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

        public static string FindProductGroupIdFromEopMetadata(OM_ObservationType om) {

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

        public static DateTime FindStartDateFromPhenomenonTime(OM_ObservationType om) {

            try {
                return DateTime.Parse(om.phenomenonTime.GmlTimePeriod.beginPosition.Value);
            } catch (Exception) {
                return new DateTime();
            }


        }

        public static DateTime FindEndDateFromPhenomenonTime(OM_ObservationType om) {

            try {
                return DateTime.Parse(om.phenomenonTime.GmlTimePeriod.endPosition.Value);
            } catch (Exception) {
                return new DateTime();
            }

        }

        public static DateTime FindInstantFromResultTime(OM_ObservationType om) {

            try {
                return DateTime.Parse(om.resultTime.TimeInstant.timePosition.Value);
            } catch (Exception) {
                return new DateTime();
            }

        }

        public static string FindProductTypeFromEopMetadata(OM_ObservationType earthObservation) {

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {

                return FindProductTypeFromEop21Metadata((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)earthObservation);
            }

            if (earthObservation != null && earthObservation is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {

                return FindProductTypeFromEop20Metadata((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)earthObservation);
            }

            return null;
        }

        public static string FindProductTypeFromEop21Metadata(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.EopMetaDataProperty.EarthObservationMetaData.productType;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindProductTypeFromEop20Metadata(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopMetaDataProperty.EarthObservationMetaData.productType;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindParentIdentifierFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindParentIdentifierFromEop21Metadata((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindParentIdentifierFromEop20Metadata((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindParentIdentifierFromEop21Metadata(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.EopMetaDataProperty.EarthObservationMetaData.parentIdentifier;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindParentIdentifierFromEop20Metadata(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.EopMetaDataProperty.EarthObservationMetaData.parentIdentifier;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOrbitNumberFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindOrbitNumberFromEop21Metadata((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindOrbitNumberFromEop20Metadata((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindOrbitNumberFromEop21Metadata(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.orbitNumber;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOrbitNumberFromEop20Metadata(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.orbitNumber;
            } catch (Exception) {
                return null;
            }
        }



        public static string FindOrbitDirectionFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindOrbitDirectionFromEarthObservation21((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindOrbitDirectionFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindOrbitDirectionFromEarthObservation21(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
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

        public static string FindTrackFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindTrackFromEarthObservation21((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindTrackFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindTrackFromEarthObservation21(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
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

        public static string FindFrameFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindFrameFromEarthObservation21((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindFrameFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindFrameFromEarthObservation21(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
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

        public static string FindSwathIdentifierFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindSwathIdentifierFromEarthObservation21((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindSwathIdentifierFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindSwathIdentifierFromEarthObservation21(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
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

        public static string FindPlatformShortNameFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindPlatformNameFromEarthObservation21((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindPlatformNameFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindPlatformNameFromEarthObservation21(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
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

        public static string FindInstrumentShortNameFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindInstrumentNameFromEarthObservation21((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindInstrumentNameFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindInstrumentNameFromEarthObservation21(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
            try {
                return eo.procedure.Eop21EarthObservationEquipment.instrument.Instrument.shortName;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindInstrumentNameFromEarthObservation20(Terradue.ServiceModel.Ogc.Eop20.EarthObservationType eo) {
            try {
                return eo.procedure.Eop20EarthObservationEquipment.instrument[0].Instrument.shortName;
            } catch (Exception) {
                return null;
            }
        }

        public static string FindOperationalModeFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindOperationalModeFromEarthObservation21((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindOperationalModeFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindOperationalModeFromEarthObservation21(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
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

        public static string FindPolarisationChannelsFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType) {
                    return FindPolarisationChannelsFromEarthObservation21((Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Sar20.SarEarthObservationType) {
                    return FindPolarisationChannelsFromEarthObservation20((Terradue.ServiceModel.Ogc.Sar20.SarEarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindPolarisationChannelsFromEarthObservation21(Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType eo) {
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

        public static string FindWrsLongitudeGridFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindWrsLongitudeGridFromEarthObservation21((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindWrsLongitudeGridFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindWrsLongitudeGridFromEarthObservation21(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
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

        public static string FindProcessingLevelFromEopMetadata(OM_ObservationType eo) {

            if (eo != null) {

                if (eo is Terradue.ServiceModel.Ogc.Eop21.EarthObservationType) {
                    return FindProcessingLevelFromEarthObservation21((Terradue.ServiceModel.Ogc.Eop21.EarthObservationType)eo);
                }

                if (eo is Terradue.ServiceModel.Ogc.Eop20.EarthObservationType) {
                    return FindProcessingLevelFromEarthObservation20((Terradue.ServiceModel.Ogc.Eop20.EarthObservationType)eo);
                }
            }

            return null;

        }

        public static string FindProcessingLevelFromEarthObservation21(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {
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

    }
}

