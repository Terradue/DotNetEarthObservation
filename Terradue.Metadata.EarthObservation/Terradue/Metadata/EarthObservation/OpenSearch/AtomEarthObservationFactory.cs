using System;
using Terradue.OpenSearch.Result;
using Terradue.Metadata.EarthObservation;
using Terradue.Metadata.EarthObservation.Ogc.Eop;
using Terradue.Metadata.EarthObservation.Ogc.Sar;
using System.Linq;
using System.IO;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;

namespace Terradue.Metadata.EarthObservation.OpenSearch {
    public class AtomEarthObservationFactory {
        
        public static AtomItem CreateAtomItemFromEarthObservationType(EarthObservationType eo) {

            if (eo is SarEarthObservationType) {

                SarEarthObservationType sarEo = (SarEarthObservationType)eo;
                return CreateAtomItemFromSarEarthObservationType(sarEo);


            } else {
                throw new NotImplementedException(string.Format("EO type {0} not implemented", eo.GetType().ToString()));
            }

        }

        public static AtomItem CreateAtomItemFromSarEarthObservationType(SarEarthObservationType sarEo) {

            AtomItem item = null;

            try {

                item = new AtomItem(String.Format("{0} {1} {2} {3} {4} {5}-{6}",
                                                  sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.platform.Platform.shortName,
                                                  sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.instrument.Instrument.shortName,
                                                  string.Join("/", sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.sensor.Sensor.operationalMode.Text),
                                                  sarEo.metaDataProperty1.EarthObservationMetaData.processing.First().ProcessingInformation.processingLevel,
                                                  sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition.polarisationChannels,
                                                  DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value).ToString("yyMMddThhmmss"),
                                                  DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value).ToString("yyMMddThhmmss")
                ),
                                    "",
                                    null,
                                    sarEo.metaDataProperty1.EarthObservationMetaData.identifier,
                                    DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value)
                );

            

                item.Summary = new Terradue.ServiceModel.Syndication.TextSyndicationContent(
                    GetHtmlSummaryFromSarEarthObservationType(sarEo),
                    Terradue.ServiceModel.Syndication.TextSyndicationContentKind.Html);

                MemoryStream stream = new MemoryStream();
                MetadataHelpers.SarSerializer.Serialize(stream, sarEo);
                stream.Seek(0, SeekOrigin.Begin);

                item.ElementExtensions.Add(XmlReader.Create(stream));

                item.ElementExtensions.Add("date", MetadataHelpers.DC, string.Format("{0}/{1}", sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value, sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value));

                item.ElementExtensions.Add("identifier", MetadataHelpers.DC, sarEo.metaDataProperty1.EarthObservationMetaData.identifier);

                                                                                 
            } catch (NullReferenceException e) {

                throw new ArgumentNullException("sarEo", e.Source);

            }

            return item;

        }

        public static string GetHtmlSummaryFromSarEarthObservationType(SarEarthObservationType sarEo) {
            // Initialize StringWriter instance.
            StringWriter stringWriter = new StringWriter();

            // Put HtmlTextWriter in using block because it needs to call Dispose.
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter)) {

                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
                writer.RenderBeginTag(HtmlTextWriterTag.Table);

                if (sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.sensor.Sensor.swathIdentifier != null && sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.sensor.Sensor.swathIdentifier.Text != null) {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write("Swath");
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(string.Join(" ", sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.sensor.Sensor.swathIdentifier.Text));
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Orbit");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition.orbitNumber);
                writer.Write(" ");
                writer.Write(sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition.orbitDirection);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Track");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition.wrsLongitudeGrid.Value);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();

            }

            // Return the result.
            return stringWriter.ToString();
        }

    }
}

