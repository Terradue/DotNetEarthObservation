using System;
using NUnit.Framework;
using System.IO;
using Terradue.ServiceModel.Ogc.Sar21;
using Terradue.ServiceModel.Ogc.Eop21;

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture]
    public class SerializerTest {

        [TestCase]
        public void DeserializeSar(){

            FileInfo s1 = new FileInfo("../Samples/S1-20120407T205500910-20120407T211433040_A_T-XG0B.atom");

            SarEarthObservationType sarEo = (SarEarthObservationType)Terradue.Metadata.EarthObservation.MetadataHelpers.SarSerializer.Deserialize(s1.OpenRead());

            Assert.AreEqual("S1A", sarEo.procedure.Eop21EarthObservationEquipment.platform.Platform.shortName);

            Assert.AreEqual(OrbitDirectionValueType.DESCENDING, sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.orbitDirection);

            Assert.AreEqual("S", sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.polarisationMode);

        }

    }
}

