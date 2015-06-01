using System;
using NUnit.Framework;
using System.IO;
using Terradue.Metadata.EarthObservation.Ogc.Sar;

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture]
    public class SerializerTest {

        [TestCase]
        public void DeserializeSar(){

            FileInfo s1 = new FileInfo("../Samples/S1-20120407T205500910-20120407T211433040_A_T-XG0B.atom");

            SarEarthObservationType sarEo = (SarEarthObservationType)Terradue.Metadata.EarthObservation.MetadataHelpers.SarSerializer.Deserialize(s1.OpenRead());

            Assert.AreEqual("S1A", sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.platform.Platform.shortName);



        }

    }
}

