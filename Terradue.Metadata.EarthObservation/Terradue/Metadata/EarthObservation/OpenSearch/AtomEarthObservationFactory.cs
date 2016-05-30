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
using Terradue.Metadata.EarthObservation.Ogc.Opt;
using Terradue.ServiceModel.Syndication;
using Terradue.ServiceModel.Ogc.OwsContext;
using System.Xml.Serialization;
using Terradue.GeoJson.Geometry;

namespace Terradue.Metadata.EarthObservation.OpenSearch {
    public class AtomEarthObservationFactory {
        
        public static AtomItem CreateAtomItemFromEarthObservationType(EarthObservationType eo) {

            if (eo is SarEarthObservationType) {

                SarEarthObservationType sarEo = (SarEarthObservationType)eo;
                var item = CreateAtomItemFromSarEarthObservationType(sarEo);
                return item;
            } else if (eo is OptEarthObservationType) {

                OptEarthObservationType optEo = (OptEarthObservationType)eo;
                var item = CreateAtomItemFromOptEarthObservationType(optEo);
                AddWMSOffering(optEo, item);
                AddBox(optEo, item);
                return item;

            } else {
                throw new NotImplementedException(string.Format("EO type {0} not implemented", eo.GetType().ToString()));
            }

        }

        public static AtomItem CreateAtomItemFromOptEarthObservationType(OptEarthObservationType optEo) {

            AtomItem item = null;

            try {

                item = new AtomItem(String.Format("{0} {1} {2} {3} {4} {5}-{6}",
                                                  optEo.EopProcedure.EarthObservationEquipment.platform.Platform.shortName,
                                                  optEo.metaDataProperty1.EarthObservationMetaData.productType,
                                                  string.Join("/", optEo.EopProcedure.EarthObservationEquipment.sensor.Sensor.operationalMode.Text),
                                                  optEo.metaDataProperty1.EarthObservationMetaData.processing.First().ProcessingInformation.processingLevel,
                                                  optEo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value,
                                                  DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.beginPosition.Value).ToUniversalTime().ToString("yyMMddTHHmmss"),
                                                  DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.endPosition.Value).ToUniversalTime().ToString("yyMMddTHHmmss")
                                                 ),
                                    "",
                                    null,
                                    optEo.metaDataProperty1.EarthObservationMetaData.identifier,
                                    DateTime.Parse(optEo.phenomenonTime.GmlTimePeriod.endPosition.Value)
                                   );



                item.Summary = new Terradue.ServiceModel.Syndication.TextSyndicationContent(
                    GetHtmlSummaryFromOptEarthObservationType(optEo),
                    Terradue.ServiceModel.Syndication.TextSyndicationContentKind.Html);

                MemoryStream stream = new MemoryStream();
                MetadataHelpers.OptSerializer.Serialize(stream, optEo);
                stream.Seek(0, SeekOrigin.Begin);

                item.ElementExtensions.Add(XmlReader.Create(stream));

                item.ElementExtensions.Add("date", MetadataHelpers.DC, string.Format("{0}/{1}", optEo.phenomenonTime.GmlTimePeriod.beginPosition.Value, optEo.phenomenonTime.GmlTimePeriod.endPosition.Value));

                item.ElementExtensions.Add("identifier", MetadataHelpers.DC, optEo.metaDataProperty1.EarthObservationMetaData.identifier);


            } catch (NullReferenceException e) {

                throw new ArgumentNullException("sarEo", e.Source);

            }

            return item;

        }

        public static AtomItem CreateAtomItemFromSarEarthObservationType(SarEarthObservationType sarEo) {

            AtomItem item = null;

            try {

                item = new AtomItem(String.Format("{0} {1} {2} {3} {4} {7} {5}/{6}",
                                                  sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.platform.Platform.shortName,
                                                  sarEo.metaDataProperty1.EarthObservationMetaData.productType,
                                                  string.Join("/", sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.sensor.Sensor.operationalMode.Text),
                                                  sarEo.metaDataProperty1.EarthObservationMetaData.processing.First().ProcessingInformation.processingLevel,
                                                  sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition.polarisationChannels,
                                                  DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.beginPosition.Value).ToUniversalTime().ToString("O"),
                                                  DateTime.Parse(sarEo.phenomenonTime.GmlTimePeriod.endPosition.Value).ToUniversalTime().ToString("O"),
                                                  sarEo.SarEarthObservationEquipment.SarEarthObservationEquipment.SarAcquisitionParameters.SarAcquisition.wrsLongitudeGrid.Value
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
                writer.Write(optEo.metaDataProperty1.EarthObservationMetaData.identifier);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Product Type");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(optEo.metaDataProperty1.EarthObservationMetaData.productType);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Orbit");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(optEo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.orbitNumber);
                writer.Write(" ");
                writer.Write(optEo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.orbitDirection.ToString());
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Track");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(optEo.EopProcedure.EarthObservationEquipment.acquisitionParameters.Acquisition.wrsLongitudeGrid.Value);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Cloud Cover");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(optEo.Optresult.OptEarthObservationResult.cloudCoverPercentage.Value);
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
                writer.Write(sarEo.metaDataProperty1.EarthObservationMetaData.identifier);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Product Type");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(sarEo.metaDataProperty1.EarthObservationMetaData.productType);
                writer.RenderEndTag();
                writer.RenderEndTag();

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

        private static void ApplyLinks(EarthObservationType eo, AtomItem item){
            if (eo.EopResult != null && eo.EopResult.EarthObservationResult.product != null) {
                foreach (var pr in eo.EopResult.EarthObservationResult.product) {
                    var link = SyndicationLink.CreateMediaEnclosureLink(new Uri(pr.ProductInformation.fileName.ServiceReference.href), "application/octet-stream", long.Parse(pr.ProductInformation.size.Text[0]));
                    link.Title = pr.ProductInformation.fileName.ServiceReference.title;
                    item.Links.Add(link);
                }
            }
            if (eo is OptEarthObservationType){
                OptEarthObservationType optEO = (OptEarthObservationType)eo;
                if (optEO.Optresult != null && optEO.Optresult.OptEarthObservationResult.product != null) {
                    foreach (var pr in optEO.Optresult.OptEarthObservationResult.product) {
                        var link = SyndicationLink.CreateMediaEnclosureLink(new Uri(pr.ProductInformation.fileName.ServiceReference.href), "application/octet-stream", long.Parse(pr.ProductInformation.size.Text[0]));
                        link.Title = pr.ProductInformation.fileName.ServiceReference.title;
                        item.Links.Add(link);
                    }
                }
            }
        }

        private static void AddWMSOffering(EarthObservationType eo, AtomItem item){
            BrowseInformationPropertyType[] bi = null;
            if (eo.EopResult != null && eo.EopResult.EarthObservationResult.browse != null) {
                bi = eo.EopResult.EarthObservationResult.browse;
            }
            if (eo is OptEarthObservationType){
                OptEarthObservationType optEO = (OptEarthObservationType)eo;
                if ( optEO.Optresult.OptEarthObservationResult.browse != null )
                    bi = optEO.Optresult.OptEarthObservationResult.browse;
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
                    offering.Operations[0].Href = new Uri(browse.BrowseInformation.fileName.ServiceReference.href);

                    item.ElementExtensions.Add(offering, new XmlSerializer(typeof(OwcOffering)));

                }
            }
        }

        private static void AddBox(EarthObservationType eo, AtomItem item){

            var geom = MetadataHelpers.FindGeometryFromEarthObservation(eo);
            if (geom != null) {

                NetTopologySuite.IO.WKTReader wktreader = new NetTopologySuite.IO.WKTReader();
                var igeom = wktreader.Read(geom.ToWkt());
                item.ElementExtensions.Add("box", "http://www.georss.org/georss", string.Format("{0} {1} {2} {3}", igeom.EnvelopeInternal.MinY, igeom.EnvelopeInternal.MinX, igeom.EnvelopeInternal.MaxY, igeom.EnvelopeInternal.MaxX));

            }

        }

    }
}

