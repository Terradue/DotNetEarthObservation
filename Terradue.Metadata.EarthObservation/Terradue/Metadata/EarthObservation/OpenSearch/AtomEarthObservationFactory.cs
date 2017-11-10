using System;
using Terradue.OpenSearch.Result;
using Terradue.ServiceModel.Syndication;
using System.Xml.Serialization;
using Terradue.ServiceModel.Ogc;
using Terradue.GeoJson.GeoRss;
using Terradue.ServiceModel.Ogc.Owc.AtomEncoding;
using Terradue.Metadata.EarthObservation.Helpers;
using Terradue.Metadata.EarthObservation.Ogc.Extensions;
using System.Linq;

namespace Terradue.Metadata.EarthObservation.OpenSearch
{
    public static class AtomEarthObservationFactory
    {

        public static AtomItem CreateEarthObservationAtomItem(this ServiceModel.Ogc.Eop20.EarthObservationType eop)
        {

            AtomItem item = new AtomItem();

            try
            {

                item = new AtomItem(String.Format("{0} {1} {2} {3} {4} {5}",
                                                  eop.FindPlatformShortName(),
                                                  eop.FindProductType(),
                                                  eop.FindOperationalMode(),
                                                  eop.FindProcessingLevel(),
                                                  eop.FindTrack(),
                                                  eop.FindRelevantDate().ToString("R")
                                                 ),
                                    "",
                                    null,
                                    eop.FindIdentifier(),
                                    eop.FindRelevantDate()
                                   );



                item.Summary = new Terradue.ServiceModel.Syndication.TextSyndicationContent(
                    Formatter.GetHtmlSummaryForOgcObservationsAndMeasurements(eop),
                    Terradue.ServiceModel.Syndication.TextSyndicationContentKind.Html);

                item.ElementExtensions.Add(eop.CreateDcDateXmlReader());

                item.ElementExtensions.Add("identifier", OgcHelpers.DC, eop.FindIdentifier());

                item.ElementExtensions.Add(eop.CreateReader());

                AddEnclosure(eop, ref item);

                AddWMSOffering(eop, ref item);

                AddGeoRss(eop, ref item);

            }
            catch (NullReferenceException e)
            {
                throw new ArgumentNullException("eop", e.Source);
            }

            return item;

        }

        public static AtomItem CreateEarthObservationAtomItem(this ServiceModel.Ogc.Eop21.EarthObservationType eop)
        {

            AtomItem item = new AtomItem();

            try
            {

                item = new AtomItem(String.Format("{0} {1} {2} {3} {4} {5}",
                                                  eop.FindPlatformShortName(),
                                                  eop.FindProductType(),
                                                  eop.FindOperationalMode(),
                                                  eop.FindProcessingLevel(),
                                                  eop.FindTrack(),
                                                  eop.FindRelevantDate().ToString("R")
                                                 ),
                                    "",
                                    null,
                                    eop.FindIdentifier(),
                                    eop.FindRelevantDate()
                                   );



                item.Summary = new Terradue.ServiceModel.Syndication.TextSyndicationContent(
                    Formatter.GetHtmlSummaryForOgcObservationsAndMeasurements(eop),
                    Terradue.ServiceModel.Syndication.TextSyndicationContentKind.Html);

                item.ElementExtensions.Add(eop.CreateDcDateXmlReader());

                item.ElementExtensions.Add("identifier", OgcHelpers.DC, eop.FindIdentifier());

                item.ElementExtensions.Add(eop.CreateReader());

                AddEnclosure(eop, ref item);

                AddWMSOffering(eop, ref item);

                AddGeoRss(eop, ref item);

            }
            catch (NullReferenceException e)
            {
                throw new ArgumentNullException("eop", e.Source);
            }

            return item;

        }

        private static void AddEnclosure(ServiceModel.Ogc.Eop21.EarthObservationType eo, ref AtomItem item)
        {
            if (eo.result != null && eo.result.Eop21EarthObservationResult.product != null && eo.result.Eop21EarthObservationResult.product.Count() > 0)
            {
                var pr = eo.result.Eop21EarthObservationResult.product[0];
                var link = SyndicationLink.CreateMediaEnclosureLink(new Uri(pr.ProductInformation.fileName.ServiceReference.href), "application/octet-stream", long.Parse(pr.ProductInformation.size.Text[0]));
                link.Title = pr.ProductInformation.fileName.ServiceReference.title;
                item.Links.Add(link);
            }
        }

        private static void AddWMSOffering(ServiceModel.Ogc.Eop20.EarthObservationType eo, ref AtomItem item)
        {
            ServiceModel.Ogc.Eop20.BrowseInformationPropertyType[] bi = null;
            if (eo.result != null && eo.result.Eop20EarthObservationResult.browse != null)
            {
                bi = eo.result.Eop20EarthObservationResult.browse;
            }
            if (bi != null)
            {
                foreach (var browse in bi)
                {

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

        private static void AddEnclosure(ServiceModel.Ogc.Eop20.EarthObservationType eo, ref AtomItem item)
        {
            if (eo.result != null && eo.result.Eop20EarthObservationResult.product != null && eo.result.Eop20EarthObservationResult.product.Count() > 0)
            {
                var pr = eo.result.Eop20EarthObservationResult.product[0];
                var link = SyndicationLink.CreateMediaEnclosureLink(new Uri(pr.ProductInformation.fileName.ServiceReference.href), "application/octet-stream", long.Parse(pr.ProductInformation.size.Text));
                link.Title = pr.ProductInformation.fileName.ServiceReference.title;
                item.Links.Add(link);
            }
        }

        private static void AddWMSOffering(ServiceModel.Ogc.Eop21.EarthObservationType eo, ref AtomItem item)
        {
            ServiceModel.Ogc.Eop21.BrowseInformationPropertyType[] bi = null;
            if (eo.result != null && eo.result.Eop21EarthObservationResult.browse != null)
            {
                bi = eo.result.Eop21EarthObservationResult.browse;
            }
            if (bi != null)
            {
                foreach (var browse in bi)
                {

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

        private static void AddGeoRss(ServiceModel.Ogc.Eop21.EarthObservationType eo, ref AtomItem item)
        {

            var geom = eo.FindGeometry();
            if (geom != null)
            {
                item.ElementExtensions.Add(geom.ToGeoRss().CreateReader());
            }

        }

        private static void AddGeoRss(ServiceModel.Ogc.Eop20.EarthObservationType eo, ref AtomItem item)
        {

            var geom = eo.FindGeometry();
            if (geom != null)
            {
                item.ElementExtensions.Add(geom.ToGeoRss().CreateReader());
            }

        }

    }
}

