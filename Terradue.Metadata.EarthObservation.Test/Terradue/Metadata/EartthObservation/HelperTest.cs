using System;
using NUnit.Framework;
using Terradue.Metadata.EarthObservation.OpenSearch;
using System.Linq;
using System.IO;
using Terradue.ServiceModel.Syndication;
using Terradue.ServiceModel.Ogc.Gml321;
using Terradue.ServiceModel.Ogc.Om;
using Terradue.ServiceModel.Ogc;
using System.Xml;
using Terradue.OpenSearch.Result;
using Terradue.GeoJson.Geometry;

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
            var sarAcquisition = new Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType();
            sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition = sarAcquisition;
            sarAcquisition.polarisationChannels = "test";
            sarAcquisition.wrsLongitudeGrid = new CodeWithAuthorityType();
            sarAcquisition.wrsLongitudeGrid.Value = "test";
            sarEo.phenomenonTime = new TimeObjectPropertyType();
            sarEo.phenomenonTime.GmlTimePeriod = new TimePeriodType();
            sarEo.phenomenonTime.GmlTimePeriod.beginPosition = new TimePositionType();
            sarEo.phenomenonTime.GmlTimePeriod.endPosition = new TimePositionType();
            sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value = "2015-07-18";
            sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value = "2015-07-18";

            var test = AtomEarthObservationFactory.CreateAtomItemFromEarthObservationType(sarEo);

            Assert.That(test.ElementExtensions.ReadElementExtensions<Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType>("EarthObservation", OgcHelpers.SAR21, OgcHelpers.Sar21Serializer).Count > 0);

        }

        [Test()]
        public void Sentinel1FromPeps() {

            var fs = new FileStream("../Samples/fa4fae6f-a856-5520-8b33-5c65f1e37b23.atom", FileMode.Open);

            XmlReader reader = XmlReader.Create(fs);

            Atom10FeedFormatter ff = new Atom10FeedFormatter();

            ff.ReadFrom(reader);

            reader.Close();

            AtomFeed feed = new AtomFeed(ff.Feed);

            Assert.AreEqual(new DateTimeOffset(DateTime.Parse("2016-01-18T13:12:36Z")), new DateTimeOffset(EarthObservationOpenSearchResultHelpers.FindStartDateFromOpenSearchResultItem(feed.Items.First())));
            Assert.AreEqual(new DateTimeOffset(DateTime.Parse("2016-01-18T13:13:37Z")), new DateTimeOffset(EarthObservationOpenSearchResultHelpers.FindEndDateFromOpenSearchResultItem(feed.Items.First())));
            Assert.AreEqual(null, EarthObservationOpenSearchResultHelpers.FindFrameFromOpenSearchResultItem(feed.Items.First()));
            Assert.AreEqual("fa4fae6f-a856-5520-8b33-5c65f1e37b23", EarthObservationOpenSearchResultHelpers.FindIdentifierFromOpenSearchResultItem(feed.Items.First()));
            Assert.AreEqual("POLYGON((-36.737206 83.462318,-58.498756 86.799332,-85.752228 83.767929,-61.68018 81.535675,-36.737206 83.462318))", EarthObservationOpenSearchResultHelpers.FindGeometryFromIOpenSearchResultItem(feed.Items.First()).ToWkt());

        }

        [Test()]
        public void Sar21() {

            var fs = new FileStream("../Samples/S1-20120407T205500910-20120407T211433040_A_T-XG0B.atom", FileMode.Open);

            XmlReader reader = XmlReader.Create(fs);

            Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType sarEO = (Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType)OgcHelpers.DeserializeEarthObservation(reader, OgcHelpers.SAR21);

            Assert.AreEqual("ESA.EECF.ENVISAT_ASA_GMI_1S", MetadataHelpers.FindParentIdentifierFromEopMetadata(sarEO));
            Assert.AreEqual(DateTime.Parse("2012-04-07T20:55:00.910000Z"), MetadataHelpers.FindStartDateFromPhenomenonTime(sarEO));
            Assert.AreEqual(DateTime.Parse("2012-04-07T21:14:33.040000Z"), MetadataHelpers.FindEndDateFromPhenomenonTime(sarEO));
            Assert.AreEqual("S1A", MetadataHelpers.FindPlatformShortNameFromEopMetadata(sarEO));
            Assert.AreEqual(DateTime.Parse("2012-04-07T21:14:33.040000Z"), MetadataHelpers.FindInstantFromResultTime(sarEO));
            Assert.AreEqual("SAR", MetadataHelpers.FindInstrumentShortNameFromEopMetadata(sarEO));
            Assert.AreEqual("GM", MetadataHelpers.FindOperationalModeFromEopMetadata(sarEO));
            Assert.AreEqual("swath182", MetadataHelpers.FindSwathIdentifierFromEopMetadata(sarEO));

        }
    }

}

