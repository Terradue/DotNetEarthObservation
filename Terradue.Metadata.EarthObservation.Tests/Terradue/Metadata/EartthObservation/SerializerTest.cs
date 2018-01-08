using System;
using NUnit.Framework;
using System.IO;
using Terradue.ServiceModel.Ogc;
using System.Xml;
using Terradue.OpenSearch.Result;

namespace Terradue.Metadata.EarthObservation.Test {

    [TestFixture]
    public class SerializerTest {

        [TestCase]
        public void DeserializeSar(){

            FileInfo s1 = new FileInfo("../Samples/S1-20120407T205500910-20120407T211433040_A_T-XG0B.atom");

            Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType sarEo = (Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType)OgcHelpers.DeserializeEarthObservation(XmlReader.Create(s1.OpenRead()));

            Assert.AreEqual("S1A", sarEo.procedure.Eop21EarthObservationEquipment.platform.Platform.shortName);

			Assert.AreEqual(Terradue.ServiceModel.Ogc.Eop21.OrbitDirectionValueType.DESCENDING, ((Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType)sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).orbitDirection);

            Assert.AreEqual("S", ((Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType)sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).polarisationMode);



        }

        [TestCase]
        public void DeserializeOpt(){

            FileInfo s2 = new FileInfo("../Samples/opt21.xml");

            Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType optEo = (Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType)OgcHelpers.DeserializeEarthObservation(XmlReader.Create(s2.OpenRead()));

            Assert.AreEqual("S2A_OPER_REP_METARC_EPA__20200620T190553_20200620T140553_20200620T140553_L1C_25_7", optEo.result.Opt21EarthObservationResult.id);



        }

        [TestCase]
        public void DeserializeEOS2(){

            FileInfo s2 = new FileInfo("../Samples/eos2.xml");

            Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType optEo = (Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType)OgcHelpers.DeserializeEarthObservation(XmlReader.Create(s2.OpenRead()));

            Assert.AreEqual("S2A_OPER_REP_METARC_EPA__20210204T120000_20210204T120000_20210204T130000_25TFJ_7", optEo.result.Opt21EarthObservationResult.id);



        }

        [TestCase]
        public void DeserializeEOS2T(){

            FileInfo s2 = new FileInfo("../Samples/S2MSI1CT.xml");

            Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType optEo = (Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType)OgcHelpers.DeserializeEarthObservation(XmlReader.Create(s2.OpenRead()));

            Assert.AreEqual("ID-24599", optEo.result.Opt21EarthObservationResult.id);



        }

		[TestCase]
		public void DeserializeAtom()
		{

			FileStream s1 = new FileStream("../Samples/S1_SAR_EW.atom", FileMode.Open, FileAccess.Read);

			var atom = AtomFeed.Load(XmlReader.Create(s1));

			s1.Close();


			FileStream s2 = new FileStream("../Samples/S2MSI1C.atom", FileMode.Open, FileAccess.Read);

			atom = AtomFeed.Load(XmlReader.Create(s2));

			s2.Close();


		}

    }
}

