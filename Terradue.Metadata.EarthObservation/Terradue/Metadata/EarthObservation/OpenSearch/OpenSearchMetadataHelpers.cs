using System;
using Terradue.GeoJson.Geometry;
using Terradue.OpenSearch.Result;
using System.Xml;
using Terradue.ServiceModel.Syndication;
using System.Linq;
using Terradue.GeoJson.Feature;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Terradue.Metadata.EarthObservation.OpenSearch {
    public class OpenSearchMetadataHelpers {

        public static Feature FindFeatureFromOpenSearchResultItem(IOpenSearchResultItem item ){

            var georss = item.ElementExtensions.ReadElementExtensions<XmlElement>("where", "http://www.georss.org/georss");
            if (georss.Count > 0) {
                return GeometryFactory.GeoRSSToFeature(georss[0]);
            }
            georss = item.ElementExtensions.ReadElementExtensions<XmlElement>("point", "http://www.georss.org/georss");
            if (georss.Count > 0) {
                return GeometryFactory.GeoRSSToFeature(georss[0]);
            }
            georss = item.ElementExtensions.ReadElementExtensions<XmlElement>("line", "http://www.georss.org/georss");
            if (georss.Count > 0) {
                return GeometryFactory.GeoRSSToFeature(georss[0]);
            }
            georss = item.ElementExtensions.ReadElementExtensions<XmlElement>("box", "http://www.georss.org/georss");
            if (georss.Count > 0) {
                return GeometryFactory.GeoRSSToFeature(georss[0]);
            }
            georss = item.ElementExtensions.ReadElementExtensions<XmlElement>("polygon", "http://www.georss.org/georss");
            if (georss.Count > 0) {
                return GeometryFactory.GeoRSSToFeature(georss[0]);
            }
            georss = item.ElementExtensions.ReadElementExtensions<XmlElement>("where", "http://www.georss.org/georss/10");
            if (georss.Count > 0) {
                return GeometryFactory.GeoRSSToFeature(georss[0]);
            }
            georss = item.ElementExtensions.ReadElementExtensions<XmlElement>("point", "http://www.georss.org/georss/10");
            if (georss.Count > 0) {
                return GeometryFactory.GeoRSSToFeature(georss[0]);
            }
            georss = item.ElementExtensions.ReadElementExtensions<XmlElement>("line", "http://www.georss.org/georss/10");
            if (georss.Count > 0) {
                return GeometryFactory.GeoRSSToFeature(georss[0]);
            }
            georss = item.ElementExtensions.ReadElementExtensions<XmlElement>("box", "http://www.georss.org/georss/10");
            if (georss.Count > 0) {
                return GeometryFactory.GeoRSSToFeature(georss[0]);
            }
            georss = item.ElementExtensions.ReadElementExtensions<XmlElement>("polygon", "http://www.georss.org/georss/10");
            if (georss.Count > 0) {
                return GeometryFactory.GeoRSSToFeature(georss[0]);
            }

            var dctspatial = item.ElementExtensions.ReadElementExtensions<string>("spatial", "http://purl.org/dc/terms/");
            if (dctspatial.Count > 0) {
                return GeometryFactory.WktToFeature(dctspatial[0]);
            }

            foreach (SyndicationElementExtension ext in item.ElementExtensions.ToArray()) {
                XmlReader reader;
                try {
                    reader = ext.GetReader();
                } catch {
                    return null;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                foreach (XmlNode node in doc.ChildNodes) {
                    if (node.LocalName == "EarthObservation") {
                        XmlNode footprint = EarthObservationOpenSearchResultHelpers.FindNodeByAttributeId((XmlElement)node, "footprint");
                        if (footprint != null && footprint.ChildNodes[0] is XmlElement) {
                            return GeometryFactory.GmlToFeature((XmlElement)footprint.ChildNodes[0]);
                        }
                        footprint = EarthObservationOpenSearchResultHelpers.FindNodeByAttributeId((XmlElement)node, "nominalTrack");
                        if (footprint != null && footprint.ChildNodes[0] is XmlElement) {
                            return GeometryFactory.GmlToFeature((XmlElement)footprint.ChildNodes[0]);
                        }
                    }
                }
            }
            return null;

        }

        public static string FindIdentifierFromOpenSearchResultItem(IOpenSearchResultItem item ){

            var elements = item.ElementExtensions.ReadElementExtensions<string>("identifier", "http://purl.org/dc/elements/1.1/");
            if (elements.Count > 0)
                return elements[0];

            foreach (var eo in item.ElementExtensions.ToList()) {
                XElement eoElement = (XElement)XElement.ReadFrom(eo.GetReader());
                if (eoElement.Name.LocalName == "EarthObservation") {
                    var result = eoElement.XPathSelectElement(string.Format("eop:metaDataProperty/eop:EarthObservationMetaData/eop:identifier", EONamespaces.TypeNamespaces[eo.OuterNamespace]), EONamespaces.GetXmlNamespaceManager(eoElement));
                    if (result != null) {
                        return result.Value;
                    }
                }
            }

            return null;

        }

    }
}

