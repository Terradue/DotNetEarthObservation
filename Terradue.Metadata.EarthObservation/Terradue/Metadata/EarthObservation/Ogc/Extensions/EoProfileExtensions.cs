using System;
using Terradue.GeoJson.Geometry;
using Terradue.ServiceModel.Syndication;
using Terradue.GeoJson.Gml321;
using Terradue.ServiceModel.Ogc;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace Terradue.Metadata.EarthObservation.Ogc.Extensions
{
    public static class EoProfileExtensions
    {

        /// <summary>
        /// Gets the earth observation profile.
        /// </summary>
        /// <returns>The earth observation profile.</returns>
        /// <param name="extensions">Extensions.</param>
        public static ServiceModel.Ogc.Om20.OM_ObservationType GetEarthObservationProfile(this SyndicationElementExtensionCollection extensions)
        {

            foreach (var ext in extensions)
            {
                if (ext.OuterName == "EarthObservation")
                {
                    return (ServiceModel.Ogc.Om20.OM_ObservationType)OgcHelpers.DeserializeEarthObservation(ext.GetReader());
                }
            }

            return null;

        }

        /// <summary>
        /// Finds the geometry.
        /// </summary>
        /// <returns>The geometry.</returns>
        /// <param name="earthObservation">Earth observation.</param>
        public static GeometryObject FindGeometry(this ServiceModel.Ogc.Om20.OM_ObservationType earthObservation)
        {


            if (earthObservation != null && earthObservation is ServiceModel.Ogc.Alt21.AltEarthObservationType)
            {
                return ((ServiceModel.Ogc.Alt21.AltEarthObservationType)earthObservation).FindGeometry();
            }

            if (earthObservation != null && earthObservation is ServiceModel.Ogc.Ssp21.SspEarthObservationType)
            {

                return ((ServiceModel.Ogc.Ssp21.SspEarthObservationType)earthObservation).FindGeometry();
            }

            if (earthObservation != null && earthObservation is ServiceModel.Ogc.Lmb21.LmbEarthObservationType)
            {

                return ((ServiceModel.Ogc.Lmb21.LmbEarthObservationType)earthObservation).FindGeometry();
            }

            if (earthObservation != null && earthObservation is ServiceModel.Ogc.Eop21.EarthObservationType)
            {

                return ((ServiceModel.Ogc.Eop21.EarthObservationType)earthObservation).FindGeometry();
            }

            if (earthObservation != null && earthObservation is ServiceModel.Ogc.Alt20.AltEarthObservationType)
            {

                return ((ServiceModel.Ogc.Alt20.AltEarthObservationType)earthObservation).FindGeometry();
            }

            if (earthObservation != null && earthObservation is ServiceModel.Ogc.Eop20.EarthObservationType)
            {

                return ((ServiceModel.Ogc.Eop20.EarthObservationType)earthObservation).FindGeometry();
            }

            if (earthObservation != null && earthObservation is ServiceModel.Ogc.Ssp20.SspEarthObservationType)
            {

                return ((ServiceModel.Ogc.Ssp20.SspEarthObservationType)earthObservation).FindGeometry();
            }

            if (earthObservation != null && earthObservation is ServiceModel.Ogc.Lmb20.LmbEarthObservationType)
            {

                return ((ServiceModel.Ogc.Lmb20.LmbEarthObservationType)earthObservation).FindGeometry();
            }

            return null;
        }

        /// <summary>
        /// Finds the geometry.
        /// </summary>
        /// <returns>The geometry.</returns>
        /// <param name="eo">Eo.</param>
        public static GeometryObject FindGeometry(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {

            if (eo.featureOfInterest != null)
            {

                if (eo.featureOfInterest.Eop21Footprint != null)
                {
                    try
                    {
                        return eo.featureOfInterest.Eop21Footprint.multiExtentOf.MultiSurface.ToGeometry();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return null;
        }

        public static GeometryObject FindGeometry(this ServiceModel.Ogc.Alt21.AltEarthObservationType eo)
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

        public static GeometryObject FindGeometry(this ServiceModel.Ogc.Ssp21.SspEarthObservationType eo)
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

        public static GeometryObject FindGeometry(this ServiceModel.Ogc.Lmb21.LmbEarthObservationType eo)
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

        public static GeometryObject FindGeometry(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {

            if (eo.featureOfInterest != null)
            {

                if (eo.featureOfInterest.Eop20Footprint != null)
                {
                    try
                    {
                        return eo.featureOfInterest.Eop20Footprint.multiExtentOf.MultiSurface.ToGeometry();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return null;
        }

        public static GeometryObject FindGeometry(this ServiceModel.Ogc.Alt20.AltEarthObservationType eo)
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

        public static GeometryObject FindGeometry(this ServiceModel.Ogc.Ssp20.SspEarthObservationType eo)
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

        public static GeometryObject FindGeometry(this ServiceModel.Ogc.Lmb20.LmbEarthObservationType eo)
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

        public static string FindIdentifier(this ServiceModel.Ogc.Om20.OM_ObservationType om)
        {

            if (om is ServiceModel.Ogc.Eop21.EarthObservationType)
            {
                var eo = (ServiceModel.Ogc.Eop21.EarthObservationType)om;
                try
                {
                    return eo.EopMetaDataProperty.EarthObservationMetaData.identifier;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            if (om is ServiceModel.Ogc.Eop20.EarthObservationType)
            {
                var eo = (ServiceModel.Ogc.Eop20.EarthObservationType)om;
                try
                {
                    return eo.EopMetaDataProperty.EarthObservationMetaData.identifier;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;

        }

        public static string FindProductGroupId(this ServiceModel.Ogc.Om20.OM_ObservationType om)
        {

            if (om is ServiceModel.Ogc.Eop21.EarthObservationType)
            {
                var eo = (ServiceModel.Ogc.Eop21.EarthObservationType)om;
                try
                {
                    return eo.EopMetaDataProperty.EarthObservationMetaData.productGroupId;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            if (om is ServiceModel.Ogc.Eop20.EarthObservationType)
            {
                return null;
            }

            return null;

        }

        public static DateTime FindRelevantDate(this ServiceModel.Ogc.Om20.OM_ObservationType om)
        {
            var date = om.FindBeginPosition();
            if (date.Ticks > 0) return date;
            date = om.FindInstantPosition();
            if (date.Ticks > 0) return date;

            return DateTime.MinValue;



        }


        public static DateTime FindBeginPosition(this ServiceModel.Ogc.Om20.OM_ObservationType om)
        {

            try
            {
                return DateTime.Parse(om.phenomenonTime.GmlTimePeriod.beginPosition.Value);
            }
            catch (Exception)
            {
                return new DateTime();
            }


        }

        public static DateTime FindEndPosition(this ServiceModel.Ogc.Om20.OM_ObservationType om)
        {

            try
            {
                return DateTime.Parse(om.phenomenonTime.GmlTimePeriod.endPosition.Value);
            }
            catch (Exception)
            {
                return new DateTime();
            }

        }

        public static DateTime FindInstantPosition(this ServiceModel.Ogc.Om20.OM_ObservationType om)
        {

            try
            {
                return DateTime.Parse(om.resultTime.TimeInstant.timePosition.Value);
            }
            catch (Exception)
            {
                return new DateTime();
            }

        }

        public static string FindProductType(this ServiceModel.Ogc.Om20.OM_ObservationType earthObservation)
        {

            if (earthObservation != null && earthObservation is ServiceModel.Ogc.Eop21.EarthObservationType)
            {

                return ((ServiceModel.Ogc.Eop21.EarthObservationType)earthObservation).FindProductType();
            }

            if (earthObservation != null && earthObservation is ServiceModel.Ogc.Eop20.EarthObservationType)
            {

                return ((ServiceModel.Ogc.Eop20.EarthObservationType)earthObservation).FindProductType();
            }

            return null;
        }

        public static string FindProductType(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.EopMetaDataProperty.EarthObservationMetaData.productType;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindProductType(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.EopMetaDataProperty.EarthObservationMetaData.productType;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindParentIdentifier(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindParentIdentifier();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindParentIdentifier();
                }
            }

            return null;

        }

        public static string FindParentIdentifier(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.EopMetaDataProperty.EarthObservationMetaData.parentIdentifier;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindParentIdentifier(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.EopMetaDataProperty.EarthObservationMetaData.parentIdentifier;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindOrbitNumber(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindOrbitNumber();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindOrbitNumber();
                }
            }

            return null;

        }

        public static string FindOrbitNumber(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.orbitNumber;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindOrbitNumber(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.orbitNumber;
            }
            catch (Exception)
            {
                return null;
            }
        }



        public static string FindOrbitDirection(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindOrbitDirection();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindOrbitDirection();
                }
            }

            return null;

        }

        public static string FindOrbitDirection(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.orbitDirection.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindOrbitDirection(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.orbitDirection.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindTrack(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindTrack();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindTrack();
                }
            }

            return null;

        }

        public static string FindTrack(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindTrack(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindFrame(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindFrame();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindFrame();
                }
            }

            return null;

        }

        public static string FindFrame(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLatitudeGrid.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

		public static string FindFrame(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLatitudeGrid.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindSwathIdentifier(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindSwathIdentifier();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindSwathIdentifier();
                }
            }

            return null;

        }

        public static string FindSwathIdentifier(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return string.Join(" ", eo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.swathIdentifier.Text);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindSwathIdentifier(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return string.Join(" ", eo.procedure.Eop20EarthObservationEquipment.sensor.Sensor.swathIdentifier.Text);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindPlatformShortName(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindPlatformShortName();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindPlatformShortName();
                }
            }

            return null;

        }

        public static string FindPlatformShortName(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop21EarthObservationEquipment.platform.Platform.shortName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindPlatformShortName(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop20EarthObservationEquipment.platform[0].Platform.shortName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindInstrumentShortName(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindInstrumentShortName();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindInstrumentShortName();
                }
            }

            return null;

        }

        public static string FindInstrumentShortName(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop21EarthObservationEquipment.instrument.Instrument.shortName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindInstrumentShortName(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop20EarthObservationEquipment.instrument[0].Instrument.shortName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindOperationalMode(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindOperationalMode();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindOperationalMode();
                }
            }

            return null;

        }

        public static string FindOperationalMode(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return string.Join(" ", eo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.operationalMode.Text);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindOperationalModeFrom(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return string.Join(" ", eo.procedure.Eop20EarthObservationEquipment.sensor.Sensor.operationalMode.Text);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindPolarisationChannels(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Sar21.SarEarthObservationType)
                {
                    return ((ServiceModel.Ogc.Sar21.SarEarthObservationType)eo).FindPolarisationChannels();
                }

                if (eo is ServiceModel.Ogc.Sar20.SarEarthObservationType)
                {
                    return ((ServiceModel.Ogc.Sar20.SarEarthObservationType)eo).FindPolarisationChannels();
                }
            }

            return null;

        }

        public static string FindPolarisationChannels(this ServiceModel.Ogc.Sar21.SarEarthObservationType eo)
        {
            try
            {
                return ((ServiceModel.Ogc.Sar21.SarAcquisitionType)eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).polarisationChannels;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindPolarisationChannels(this ServiceModel.Ogc.Sar20.SarEarthObservationType eo)
        {
            try
            {
                return ((ServiceModel.Ogc.Sar20.SarAcquisitionType)eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition).polarisationChannels;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindWrsLongitudeGrid(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindWrsLongitudeGrid();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindWrsLongitudeGrid();
                }
            }

            return null;

        }

        public static string FindWrsLongitudeGrid(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindWrsLongitudeGrid(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindWrsLatitudeGrid(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindWrsLatitudeGrid();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindWrsLatitudeGrid();
                }
            }

            return null;

        }

        public static string FindWrsLatitudeGrid(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLatitudeGrid.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindWrsLatitudeGrid(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.procedure.Eop20EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLatitudeGrid.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindProcessingLevel(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindProcessingLevel();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindProcessingLevel();
                }
            }

            return null;

        }

        public static string FindProcessingLevel(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            try
            {
                return eo.EopMetaDataProperty.EarthObservationMetaData.processing[0].ProcessingInformation.processingLevel;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FindProcessingLevel(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            try
            {
                return eo.EopMetaDataProperty.EarthObservationMetaData.processing[0].ProcessingInformation.processingLevel;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool IsOpticalDataset(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Opt21.OptEarthObservationType || eo is ServiceModel.Ogc.Opt20.OptEarthObservationType)
                {
                    return true;
                }
            }

            return false;

        }

        public static double FindCloudCoverPercentage(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Opt21.OptEarthObservationType)
                {
                    return ((ServiceModel.Ogc.Opt21.OptEarthObservationType)eo).FindCloudCoverPercentage();
                }

                if (eo is ServiceModel.Ogc.Opt20.OptEarthObservationType)
                {
                    return ((ServiceModel.Ogc.Opt20.OptEarthObservationType)eo).FindCloudCoverPercentage();
                }
            }

            return -1;

        }

        public static double FindCloudCoverPercentage(this ServiceModel.Ogc.Opt21.OptEarthObservationType eo)
        {
            try
            {
                return eo.result.Opt21EarthObservationResult.cloudCoverPercentage.Value;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static double FindCloudCoverPercentage(this ServiceModel.Ogc.Opt20.OptEarthObservationType eo)
        {
            try
            {
                return eo.result.Opt21EarthObservationResult.cloudCoverPercentage.Value;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static XmlReader CreateDcDateXmlReader(this ServiceModel.Ogc.Om20.OM_ObservationType om)
        {
            XElement xelement = new XElement(XName.Get("date", OgcHelpers.DC));
            var date = om.FindBeginPosition();
            if (date.Ticks == 0)
            {
                date = om.FindInstantPosition();
                if (date.Ticks == 0)
                {
                    xelement.Value = date.ToUniversalTime().ToString("O");
                    return xelement.CreateReader();
                }
                return null;
            }

            var date1 = om.FindEndPosition();
            xelement.Value = string.Format("{0}{1}", date.ToUniversalTime().ToString("O"), date1.Ticks > 0 ? "/" + date1.ToUniversalTime().ToString("O") : "");
            return xelement.CreateReader();
        }

        public static Uri FindBrowseUrl(this ServiceModel.Ogc.Om20.OM_ObservationType eo)
        {

            if (eo != null)
            {

                if (eo is ServiceModel.Ogc.Eop21.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop21.EarthObservationType)eo).FindBrowseUrl();
                }

                if (eo is ServiceModel.Ogc.Eop20.EarthObservationType)
                {
                    return ((ServiceModel.Ogc.Eop20.EarthObservationType)eo).FindBrowseUrl();
                }
            }

            return null;

        }

        public static Uri FindBrowseUrl(this ServiceModel.Ogc.Eop21.EarthObservationType eo)
        {
            ServiceModel.Ogc.Eop21.BrowseInformationPropertyType[] bi = null;
            if (eo.result != null && eo.result.Eop21EarthObservationResult.browse != null)
            {
                bi = eo.result.Eop21EarthObservationResult.browse;
            }
            if (bi != null && bi.Count() > 0)
            {
                var browse = bi.FirstOrDefault(b =>
                {
                    try
                    {
                        var uri = new Uri(b.BrowseInformation.fileName.ServiceReference.href);
                        return true;
                    }
                    catch {
                        return false;
                    }
                });

                if (browse != null)
                    return new Uri(browse.BrowseInformation.fileName.ServiceReference.href);
            }

            return null;
        }

        public static Uri FindBrowseUrl(this ServiceModel.Ogc.Eop20.EarthObservationType eo)
        {
            ServiceModel.Ogc.Eop20.BrowseInformationPropertyType[] bi = null;
            if (eo.result != null && eo.result.Eop20EarthObservationResult.browse != null)
            {
                bi = eo.result.Eop20EarthObservationResult.browse;
            }
            if (bi != null && bi.Count() > 0)
            {
                var browse = bi.FirstOrDefault(b =>
                {
                    try
                    {
                        var uri = new Uri(b.BrowseInformation.referenceSystemIdentifier.Value);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });

                if (browse != null)
                    return new Uri(browse.BrowseInformation.referenceSystemIdentifier.Value);
            }

            return null;
        }

    }
}

