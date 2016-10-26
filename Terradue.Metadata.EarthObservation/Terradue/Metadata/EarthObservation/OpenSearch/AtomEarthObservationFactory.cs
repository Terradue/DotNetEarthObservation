using System;
using Terradue.OpenSearch.Result;
using Terradue.Metadata.EarthObservation;
using System.Linq;
using System.IO;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using Terradue.ServiceModel.Syndication;
using System.Xml.Serialization;
using Terradue.GeoJson.Geometry;
using Terradue.ServiceModel.Ogc;
using Terradue.GeoJson.GeoRss;
using Terradue.ServiceModel.Ogc.Owc.AtomEncoding;

namespace Terradue.Metadata.EarthObservation.OpenSearch {
    public class AtomEarthObservationFactory {
        
        public static AtomItem CreateAtomItemFromEarthObservationType(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo) {

            if (eo is Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType) {

                Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType sarEo = (Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType)eo;
                var item = CreateAtomItemFromSarEarthObservationType(sarEo);
                return item;
            } else if (eo is Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType) {

                Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType optEo = (Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType)eo;
                var item = CreateAtomItemFromOptEarthObservationType(optEo);
                AddWMSOffering(optEo, item);
                AddGeoRss(optEo, item);
                return item;

            } else {
                throw new NotImplementedException(string.Format("EO type {0} not implemented", eo.GetType().ToString()));
            }

        }

        public static AtomItem CreateAtomItemFromOptEarthObservationType(Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType optEo) {

            AtomItem item = null;

            try {

                item = new AtomItem(String.Format("{0} {1} {2} {3} {4} {5}-{6}",
                                                  optEo.procedure.Eop21EarthObservationEquipment.platform.Platform.shortName,
                                                  optEo.EopMetaDataProperty.EarthObservationMetaData.productType,
                                                  string.Join("/", optEo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.operationalMode.Text),
                                                  optEo.EopMetaDataProperty.EarthObservationMetaData.processing.First().ProcessingInformation.processingLevel,
                                                  optEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value,
                                                  DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.beginPosition.Value).ToUniversalTime().ToString("yyMMddTHHmmss"),
                                                  DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.endPosition.Value).ToUniversalTime().ToString("yyMMddTHHmmss")
                                                 ),
                                    "",
                                    null,
                                    optEo.EopMetaDataProperty.EarthObservationMetaData.identifier,
                                    DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.endPosition.Value)
                                   );



                item.Summary = new Terradue.ServiceModel.Syndication.TextSyndicationContent(
                    GetHtmlSummaryFromOptEarthObservationType(optEo),
                    Terradue.ServiceModel.Syndication.TextSyndicationContentKind.Html);

                MemoryStream stream = new MemoryStream();
                OgcHelpers.Opt21Serializer.Serialize(stream, optEo);
                stream.Seek(0, SeekOrigin.Begin);

                item.ElementExtensions.Add(XmlReader.Create(stream));

                item.ElementExtensions.Add("date", OgcHelpers.DC, string.Format("{0}/{1}", optEo.phenomenonTime.GmlTimePeriod.beginPosition.Value, optEo.phenomenonTime.GmlTimePeriod.endPosition.Value));

                item.ElementExtensions.Add("identifier", OgcHelpers.DC, optEo.EopMetaDataProperty.EarthObservationMetaData.identifier);


            } catch (NullReferenceException e) {

                throw new ArgumentNullException("sarEo", e.Source);

            }

            return item;

        }

        public static AtomItem CreateAtomItemFromSarEarthObservationType(Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType sarEo) {

            AtomItem item = null;

            try {

                item = new AtomItem(String.Format("{0} {1} {2} {3} {4} {7} {5}/{6}",
                                                  sarEo.procedure.Eop21EarthObservationEquipment.platform.Platform.shortName,
                                                  sarEo.EopMetaDataProperty.EarthObservationMetaData.productType,
                                                  string.Join("/", sarEo.procedure.Eop21EarthObservationEquipment.sensor.Sensor.operationalMode.Text),
                                                  sarEo.EopMetaDataProperty.EarthObservationMetaData.processing.First().ProcessingInformation.processingLevel,
                                                  sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.polarisationChannels,
                                                  DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value).ToUniversalTime().ToString("O"),
                                                  DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value).ToUniversalTime().ToString("O"),
                                                  sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.wrsLongitudeGrid.Value
                ),
                                    "",
                                    null,
                                    sarEo.EopMetaDataProperty.EarthObservationMetaData.identifier,
                                    DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value)
                );

            

                item.Summary = new Terradue.ServiceModel.Syndication.TextSyndicationContent(
                    GetHtmlSummaryFromSarEarthObservationType(sarEo),
                    Terradue.ServiceModel.Syndication.TextSyndicationContentKind.Html);

                MemoryStream stream = new MemoryStream();
                OgcHelpers.Sar21Serializer.Serialize(stream, sarEo);
                stream.Seek(0, SeekOrigin.Begin);

                item.ElementExtensions.Add(XmlReader.Create(stream));

                item.ElementExtensions.Add("date", OgcHelpers.DC, string.Format("{0}/{1}", sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value, sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value));

                item.ElementExtensions.Add("identifier", OgcHelpers.DC, sarEo.EopMetaDataProperty.EarthObservationMetaData.identifier);

                                                                                 
            } catch (NullReferenceException e) {

                throw new ArgumentNullException("sarEo", e.Source);

            }

            return item;

        }

        public static string GetHtmlSummaryFromOptEarthObservationType(Terradue.ServiceModel.Ogc.Opt21.OptEarthObservationType optEo) {
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
                writer.Write("Cloud Cover");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(optEo.result.Opt21EarthObservationResult.cloudCoverPercentage.Value);
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

        public static string GetHtmlSummaryFromSarEarthObservationType(Terradue.ServiceModel.Ogc.Sar21.SarEarthObservationType sarEo) {
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
                writer.Write(sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.orbitNumber);
                writer.Write(" ");
                writer.Write(sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.orbitDirection);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Track");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(sarEo.procedure.Eop21EarthObservationEquipment.acquisitionParameters.SarAcquisition.wrsLongitudeGrid.Value);
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

        private static void ApplyLinks(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo, AtomItem item){
            if (eo.result != null && eo.result.Eop21EarthObservationResult.product != null) {
                foreach (var pr in eo.result.Eop21EarthObservationResult.product) {
                    var link = SyndicationLink.CreateMediaEnclosureLink(new Uri(pr.ProductInformation.fileName.ServiceReference.href), "application/octet-stream", long.Parse(pr.ProductInformation.size.Text[0]));
                    link.Title = pr.ProductInformation.fileName.ServiceReference.title;
                    item.Links.Add(link);
                }
            }
        }

        private static void AddWMSOffering(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo, AtomItem item){
            Terradue.ServiceModel.Ogc.Eop21.BrowseInformationPropertyType[] bi = null;
            if (eo.result != null && eo.result.Eop21EarthObservationResult.browse != null) {
                bi = eo.result.Eop21EarthObservationResult.browse;
            }
            if (bi != null) {
                foreach (var browse in bi) {

                    if (browse.BrowseInformation.type != "WMS")
                        continue;

                    OwcOffering offering = new OwcOffering();
                    offering.Code = "http://www.opengis.net/spec/owc-atom/1.0/req/wms";
                    offering.Operations = new OwcOperation[1];
                    offering.Operations[0] = new OwcOperation();
                    offering.Operations[0].Code = "GetMap";
                    offering.Operations[0].Method = "GET";
                    offering.Operations[0].RequestUrl = new Uri(browse.BrowseInformation.fileName.ServiceReference.href);

                    item.ElementExtensions.Add(offering, new XmlSerializer(typeof(OwcOffering)));

                }
            }
        }

        private static void AddGeoRss(Terradue.ServiceModel.Ogc.Eop21.EarthObservationType eo, AtomItem item){

            var geom = MetadataHelpers.FindGeometryFromEarthObservation(eo);
            if (geom != null) {
                item.ElementExtensions.Add(geom.ToGeoRss().CreateReader());
            }

        }

    }
}

