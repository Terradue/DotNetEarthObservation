using System;
using NUnit.Framework;
using System.IO;
using Terradue.Metadata.EarthObservation.Ogc.Sar;
using Terradue.Metadata.EarthObservation.Ogc.Opt;

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture]
    public class SerializerTest {

        [TestCase]
        public void DeserializeSar(){

            FileInfo s1 = new FileInfo("../Samples/S1-20120407T205500910-20120407T211433040_A_T-XG0B.atom");

            SarEarthObservationType sarEo = (SarEarthObservationType)Terradue.Metadata.EarthObservation.MetadataHelpers.SarSerializer.Deserialize(s1.OpenRead());

            Assert.AreEqual("S1A", sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.platform.Platform.shortName);



        }

        [TestCase]
        public void DeserializeOpt(){

            FileInfo s2 = new FileInfo("../Samples/opt21.xml");

            OptEarthObservationType optEo = (OptEarthObservationType)Terradue.Metadata.EarthObservation.MetadataHelpers.OptSerializer.Deserialize(s2.OpenRead());

            Assert.AreEqual("S2A_OPER_REP_METARC_EPA__20200620T190553_20200620T140553_20200620T140553_L1C_25_7", optEo.Optresult.OptEarthObservationResult.id);



        }

    }
}

