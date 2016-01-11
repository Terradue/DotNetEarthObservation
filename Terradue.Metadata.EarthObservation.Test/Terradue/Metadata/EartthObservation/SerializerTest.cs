using System;
using NUnit.Framework;
using System.IO;
using Terradue.ServiceModel.Ogc.Sar21;
using Terradue.ServiceModel.Ogc.Eop21;
using Terradue.ServiceModel.Ogc;
using System.Xml;

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture]
    public class SerializerTest {

        [TestCase]
        public void DeserializeSar(){

            FileStream s1 = new FileStream("../Samples/S1-20120407T205500910-20120407T211433040_A_T-XG0B.atom", FileMode.Open);

            var xr = XmlReader.Create(s1);

            SarEarthObservationType sarEo = (SarEarthObservationType)OgcHelpers.DeserializeEarthObservation(xr, OgcHelpers.SAR21);

            Assert.AreEqual("S1A", sarEo.procedure.Eop21EarthObservationEquipment.platform.Platform.shortName);

            Assert.AreEqual(OrbitDirectionValueType.DESCENDING, ((Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType)sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).orbitDirection);

            Assert.AreEqual("S", ((Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType)sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).polarisationMode);

        }

    }
}

