using System;
using NUnit.Framework;
using Terradue.Metadata.EarthObservation.OpenSearch;
using System.Linq;
using System.IO;
using Terradue.ServiceModel.Syndication;

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture()]
    public class HelperTest {

        [TestCase()]
        public void CreateEOext(){

            Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType sarEo = new Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType();

            sarEo.SarEarthObservationEquipment = new Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentPropertyType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment = new Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationEquipmentType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.platform = new Terradue.Metadata.EarthObservation.Ogc.Eop.PlatformPropertyType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.platform.Platform = new Terradue.Metadata.EarthObservation.Ogc.Eop.PlatformType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.platform.Platform.shortName = "Test";
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.instrument = new Terradue.Metadata.EarthObservation.Ogc.Eop.InstrumentPropertyType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.instrument.Instrument = new Terradue.Metadata.EarthObservation.Ogc.Eop.InstrumentType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.instrument.Instrument.shortName = "test";
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.sensor = new Terradue.Metadata.EarthObservation.Ogc.Eop.SensorPropertyType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.sensor.Sensor = new Terradue.Metadata.EarthObservation.Ogc.Eop.SensorType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.sensor.Sensor.operationalMode = new Terradue.GeoJson.Gml.CodeListType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.sensor.Sensor.operationalMode.Text = new string[]{"test"};
            sarEo.metaDataProperty1 = new Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationMetaDataPropertyType();
            sarEo.metaDataProperty1.EarthObservationMetaData = new Terradue.Metadata.EarthObservation.Ogc.Eop.EarthObservationMetaDataType();
            sarEo.metaDataProperty1.EarthObservationMetaData.processing = new Terradue.Metadata.EarthObservation.Ogc.Eop.ProcessingInformationPropertyType[1];
            sarEo.metaDataProperty1.EarthObservationMetaData.processing[0] = new Terradue.Metadata.EarthObservation.Ogc.Eop.ProcessingInformationPropertyType();
            sarEo.metaDataProperty1.EarthObservationMetaData.processing.First().ProcessingInformation = new Terradue.Metadata.EarthObservation.Ogc.Eop.ProcessingInformationType();
            sarEo.metaDataProperty1.EarthObservationMetaData.processing.First().ProcessingInformation.processingLevel = "test";
            sarEo.metaDataProperty1.EarthObservationMetaData.identifier = "test";
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters = new Terradue.Metadata.EarthObservation.Ogc.Sar.SarAcquisitionPropertyType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition = new Terradue.Metadata.EarthObservation.Ogc.Sar.SarAcquisitionType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition.polarisationChannels = "test";
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition.wrsLongitudeGrid = new Terradue.GeoJson.Gml.CodeWithAuthorityType();
            sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition.wrsLongitudeGrid.Value = "test";
            sarEo.phenomenonTime = new Terradue.Metadata.EarthObservation.Ogc.Om.TimeObjectPropertyType();
            sarEo.phenomenonTime.GmlTimePeriod = new Terradue.GeoJson.Gml.TimePeriodType();
            sarEo.phenomenonTime.GmlTimePeriod.beginPosition = new Terradue.GeoJson.Gml.TimePositionType();
            sarEo.phenomenonTime.GmlTimePeriod.endPosition = new Terradue.GeoJson.Gml.TimePositionType();
            sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value = "2015-07-18";
            sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value = "2015-07-18";

            var test = AtomEarthObservationFactory.CreateAtomItemFromEarthObservationType(sarEo);

            Assert.That(test.ElementExtensions.ReadElementExtensions<Terradue.Metadata.EarthObservation.Ogc.Sar.SarEarthObservationType>("EarthObservation", MetadataHelpers.SAR, MetadataHelpers.SarSerializer).Count > 0);

        }
    }
}

