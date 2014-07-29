using System;
using Terradue.OpenSearch.Result;
using System.Xml.XPath;
using Terradue.ServiceModel.Syndication;
using System.Linq;
using System.Xml.Linq;

namespace Terradue.Metadata.EarthObservation {
    public static class EarthObservationOpenSearchResultHelpers {

        public static void RestoreEnclosure(IOpenSearchResultItem item){

            var matchLinks = item.Links.Where(l => l.RelationshipType == "enclosure").ToArray();
            if (matchLinks.Count() == 0) {
                foreach (var eo in item.ElementExtensions) {
                    XElement eoElement = (XElement)XElement.ReadFrom(eo.GetReader());
                    if (eoElement.Name.LocalName == "EarthObservation") {
                        var result = eoElement.XPathSelectElement(string.Format("om:result/eop20:EarthObservationResult/eop20:product/eop20:ProductInformation", EONamespaces.TypeNamespaces[eo.OuterNamespace]), EONamespaces.GetXmlNamespaceManager(eoElement));
                        if (result != null) {
                            var link = result.XPathSelectElement("eop20:fileName/ows:ServiceReference", EONamespaces.GetXmlNamespaceManager(result));
                            if (link != null && link.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink")) != null) {
                                long size = 0;
                                var sizeElement = result.XPathSelectElement("eop20:size", EONamespaces.GetXmlNamespaceManager(result));
                                if ( sizeElement != null )
                                    long.TryParse(sizeElement.Value, out size);
                                item.Links.Add(new SyndicationLink(new Uri(link.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink")).Value), "enclosure", "Product file", "application/x-binary", size));
                            }
                        }
                    }
                }
            }

        }

        public static void RestoreIdentifier(IOpenSearchResultItem item) {
            var elements = item.ElementExtensions.ReadElementExtensions<string>("identifier", "http://purl.org/dc/elements/1.1/");
            if (elements.Count == 0) {
                foreach (var eo in item.ElementExtensions.ToList()) {
                    XElement eoElement = (XElement)XElement.ReadFrom(eo.GetReader());
                    if (eoElement.Name.LocalName == "EarthObservation") {
                        var result = eoElement.XPathSelectElement(string.Format("eop20:metaDataProperty/eop20:EarthObservationMetaData/eop20:identifier", EONamespaces.TypeNamespaces[eo.OuterNamespace]), EONamespaces.GetXmlNamespaceManager(eoElement));
                        if (result != null) {
                            XElement identifier = new XElement(XName.Get("identifier", "http://purl.org/dc/elements/1.1/"), result.Value);
                            item.ElementExtensions.Add(identifier.CreateReader());
                        }
                    }
                }
            }

        }

    }
}

