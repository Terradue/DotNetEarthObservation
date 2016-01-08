using System;
using NUnit.Framework;
using Terradue.Metadata.EarthObservation.OpenSearch;
using System.Linq;
using System.IO;
using Terradue.ServiceModel.Syndication;
using Terradue.ServiceModel.Ogc.Gml321;
using Terradue.ServiceModel.Ogc.Om;

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture()]
    public class HelperTest {

        [TestCase()]
        public void CreateEOext(){

            Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType sarEo = new Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType();
            sarEo.procedure = new OM_ProcessPropertyType();
            sarEo.procedure.Eop21EarthObservationEquipment = new Terradue.ServiceModel.Ogc.Eop21.EarthObservationEquipmentType();
            sarEo.procedure.Eop21EarthObservationEquipment.platform = new Terradue.ServiceModel.Ogc.Eop21.PlatformPropertyType();
            sarEo.procedure.Eop21EarthObservationEquipment.platform.Platform = new Terradue.ServiceModel.Ogc.Eop21.PlatformType();
            sarEo.procedure.Eop21EarthObservationEquipment.platform.Platform.shortName = "Test";
            sarEo.procedure.Eop21EarthObservationEquipment.instrument = new Terradue.ServiceModel.Ogc.Eop21.InstrumentPropertyType();
            sarEo.procedure.Eop21EarthObservationEquipment.instrument.Instrument = new Terradue.ServiceModel.Ogc.Eop21.InstrumentType();
            sarEo.procedure.Eop21EarthObservationEquipment.instrument.Instrument.shortName = "test";
            sarEo.procedure.Eop21EarthObservationEquipment.sensor = new Terradue.ServiceModel.Ogc.Eop21.SensorPropertyType();
            sarEo.procedure.Eop21EarthObservationEquipment.sensor.Sensor = new Terradue.ServiceModel.Ogc.Eop21.SensorType();
            sarEo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.operationalMode = new CodeListType();
            sarEo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.operationalMode.Text = "test";
            sarEo.EopMetaDataProperty = new Terradue.ServiceModel.Ogc.Eop21.EarthObservationMetaDataPropertyType();
            sarEo.EopMetaDataProperty.EarthObservationMetaData = new Terradue.ServiceModel.Ogc.Eop21.EarthObservationMetaDataType();
            sarEo.EopMetaDataProperty.EarthObservationMetaData.processing = new Terradue.ServiceModel.Ogc.Eop21.ProcessingInformationPropertyType[1];
            sarEo.EopMetaDataProperty.EarthObservationMetaData.processing[0] = new Terradue.ServiceModel.Ogc.Eop21.ProcessingInformationPropertyType();
            sarEo.EopMetaDataProperty.EarthObservationMetaData.processing.First().ProcessingInformation = new Terradue.ServiceModel.Ogc.Eop21.ProcessingInformationType();
            sarEo.EopMetaDataProperty.EarthObservationMetaData.processing.First().ProcessingInformation.processingLevel = "test";
            sarEo.EopMetaDataProperty.EarthObservationMetaData.identifier = "test";
            sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters = new Terradue.ServiceModel.Ogc.Eop21.AcquisitionPropertyType();
            sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition = new Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType();
            sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.polarisationChannels = "test";
            sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.wrsLongitudeGrid = new CodeWithAuthorityType();
            sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.wrsLongitudeGrid.Value = "test";
            sarEo.phenomenonTime = new TimeObjectPropertyType();
            sarEo.phenomenonTime.GmlTimePeriod = new TimePeriodType();
            sarEo.phenomenonTime.GmlTimePeriod.beginPosition = new TimePositionType();
            sarEo.phenomenonTime.GmlTimePeriod.endPosition = new TimePositionType();
            sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value = "2015-07-18";
            sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value = "2015-07-18";

            var test = AtomEarthObservationFactory.CreateAtomItemFromEarthObservationType(sarEo);

            Assert.That(test.ElementExtensions.ReadElementExtensions<Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType>("EarthObservation", MetadataHelpers.SAR, MetadataHelpers.SarSerializer).Count > 0);

        }
    }
}

