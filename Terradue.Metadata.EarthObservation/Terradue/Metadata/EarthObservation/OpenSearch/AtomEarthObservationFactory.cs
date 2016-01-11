using System;
using Terradue.OpenSearch.Result;
using Terradue.Metadata.EarthObservation;
using Terradue.ServiceModel.Ogc.Eop21;
using Terradue.ServiceModel.Ogc.Sar21;
using System.Linq;
using System.IO;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using Terradue.ServiceModel.Ogc.Opt21;
using Terradue.ServiceModel.Ogc;

namespace Terradue.Metadata.EarthObservation.OpenSearch {
    
    public static class AtomEarthObservationFactory {
        
        public static AtomItem CreateAtomItemFromEarthObservationType(EarthObservationType eo) {

            if (eo is SarEarthObservationType) {

                SarEarthObservationType sarEo = (SarEarthObservationType)eo;
                return CreateAtomItemFromSarEarthObservationType(sarEo);
            } else if (eo is OptEarthObservationType) {

                OptEarthObservationType optEo = (OptEarthObservationType)eo;
                return CreateAtomItemFromOptEarthObservationType(optEo);

            } else {
                throw new NotImplementedException(string.Format("EO type {0} not implemented", eo.GetType().ToString()));
            }

        }

        public static AtomItem CreateAtomItemFromOptEarthObservationType(OptEarthObservationType optEo) {

            AtomItem item = null;

            try {

                item = new AtomItem(String.Format("{0} {1} {2} {3} {4} {5}-{6}",
                                                  optEo.procedure.Eop21EarthObservationEquipment.platform.Platform.shortName,
                                                  optEo.EopMetaDataProperty.EarthObservationMetaData.productType,
                                                  string.Join("/", optEo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.operationalMode.Text),
                                                  optEo.EopMetaDataProperty.EarthObservationMetaData.processing.First().ProcessingInformation.processingLevel,
                                                  optEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value,
                                                  DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.beginPosition.Value).ToUniversalTime().ToString("yyMMddThhmmss"),
                                                  DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.endPosition.Value).ToUniversalTime().ToString("yyMMddThhmmss")
                                                 ),
                                    "",
                                    null,
                                    optEo.EopMetaDataProperty.EarthObservationMetaData.identifier,
                                    DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.endPosition.Value)
                                   );



                item.Summary = new Terradue.ServiceModel.Syndication.TextSyndicationContent(
                    GetHtmlSummaryFromOptEarthObservationType(optEo),
                    Terradue.ServiceModel.Syndication.TextSyndicationContentKind.Html);

                item.ElementExtensions.Add(optEo.CreaterReader());

                item.ElementExtensions.Add("date", OgcHelpers.DC, string.Format("{0}/{1}", optEo.phenomenonTime.GmlTimePeriod.beginPosition.Value, optEo.phenomenonTime.GmlTimePeriod.endPosition.Value));

                item.ElementExtensions.Add("identifier", OgcHelpers.DC, optEo.EopMetaDataProperty.EarthObservationMetaData.identifier);


            } catch (NullReferenceException e) {

                throw new ArgumentNullException("sarEo", e.Source);

            }

            return item;

        }

        public static AtomItem CreateAtomItemFromSarEarthObservationType(SarEarthObservationType sarEo) {

            AtomItem item = null;

            try {

                item = new AtomItem(String.Format("{0} {1} {2} {3} {4} {7} {5}-{6}",
                                                  sarEo.procedure.Eop21EarthObservationEquipment.platform.Platform.shortName,
                                                  sarEo.EopMetaDataProperty.EarthObservationMetaData.productType,
                                                  string.Join("/", sarEo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.operationalMode.Text),
                                                  sarEo.EopMetaDataProperty.EarthObservationMetaData.processing.First().ProcessingInformation.processingLevel,
                                                  ((Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType)sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).polarisationChannels,
                                                  DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value).ToUniversalTime().ToString("yyMMddThhmmss"),
                                                  DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value).ToUniversalTime().ToString("yyMMddThhmmss"),
                                                  ((Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType)sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).wrsLongitudeGrid.Value
                ),
                                    "",
                                    null,
                                    sarEo.EopMetaDataProperty.EarthObservationMetaData.identifier,
                                    DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value)
                );

            

                item.Summary = new Terradue.ServiceModel.Syndication.TextSyndicationContent(
                    GetHtmlSummaryFromSarEarthObservationType(sarEo),
                    Terradue.ServiceModel.Syndication.TextSyndicationContentKind.Html);

                item.ElementExtensions.Add(sarEo.CreaterReader());

                item.ElementExtensions.Add("date", OgcHelpers.DC, string.Format("{0}/{1}", sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value, sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value));

                item.ElementExtensions.Add("identifier", OgcHelpers.DC, sarEo.EopMetaDataProperty.EarthObservationMetaData.identifier);

                                                                                 
            } catch (NullReferenceException e) {

                throw new ArgumentNullException("sarEo", e.Source);

            }

            return item;

        }

        public static string GetHtmlSummaryFromOptEarthObservationType(OptEarthObservationType optEo) {
            // Initialize StringWriter instance.
            StringWriter stringWriter = new StringWriter();

            // Put HtmlTextWriter in using block because it needs to call Dispose.
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter)) {

                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
                writer.RenderBeginTag(HtmlTextWriterTag.Table);

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Identifier");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(optEo.EopMetaDataProperty.EarthObservationMetaData.identifier);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Product Type");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(optEo.EopMetaDataProperty.EarthObservationMetaData.productType);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Orbit");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(optEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.orbitNumber);
                writer.Write(" ");
                writer.Write(optEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.orbitDirection.ToString());
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Track");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(optEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Start");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.beginPosition.Value).ToUniversalTime().ToString("O"));
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("End");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.endPosition.Value).ToUniversalTime().ToString("O"));
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();

            }

            // Return the result.
            return stringWriter.ToString();
        }

        public static string GetHtmlSummaryFromSarEarthObservationType(SarEarthObservationType sarEo) {
            // Initialize StringWriter instance.
            StringWriter stringWriter = new StringWriter();

            // Put HtmlTextWriter in using block because it needs to call Dispose.
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter)) {

                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
                writer.RenderBeginTag(HtmlTextWriterTag.Table);

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Identifier");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(sarEo.EopMetaDataProperty.EarthObservationMetaData.identifier);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Product Type");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(sarEo.EopMetaDataProperty.EarthObservationMetaData.productType);
                writer.RenderEndTag();
                writer.RenderEndTag();

                if (sarEo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.swathIdentifier != null && sarEo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.swathIdentifier.Text != null) {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write("Swath");
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(string.Join(" ", sarEo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.swathIdentifier.Text));
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
                writer.Write(((Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType)sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).orbitNumber);
                writer.Write(" ");
                writer.Write(((Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType)sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).orbitDirection);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Track");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(((Terradue.ServiceModel.Ogc.Sar21.SarAcquisitionType)sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition).wrsLongitudeGrid.Value);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Start");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value).ToUniversalTime().ToString("O"));
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("End");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value).ToUniversalTime().ToString("O"));
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

