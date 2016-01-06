using System;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Linq;


/*! 
\defgroup EOProfile Earth Observation Metadata profile
@{

The Earth Observation (EO) Metadata profile of Observations and Measurements is intended to provide a standard schema 
for encoding Earth Observation product metadata to support the description and cataloguing of products acquired by sensors aboard EO satellites.

EO products are differentiated by parameters such as the date of acquisition and the image footprint as well as characteristics pertaining to the type of sensor,
the type of platform, the applied processing chain, and more. This candidate standard identifies the metadata elements that enable the robust description 
of general EO products and defines specialisations for specific thematic classes of EO products, such as optical, radar, atmospheric, altimetry, 
limb-looking and systematic/synthesized EO products. In addition, this document describes the mechanism used to extend these thematic schemas for specific EO missions.

\xrefitem norm "Normative References" "Normative References" [OGC Earth Observation Metadata profile of Observations & Measurements (10-157r4)](https://portal.opengeospatial.org/files/61098)

\xrefitem cptype_sm "ServiceModel" "Service Models"

\ingroup Model

@}

*/

namespace Terradue.Metadata.EarthObservation {
    public class EONamespaces {

        public static NameValueCollection TypeNamespaces {
            get {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Set("http://www.opengis.net/sar/2.0", "sar");
                nvc.Set("http://www.opengis.net/eop/2.0", "eop");
                nvc.Set("http://www.opengis.net/opt/2.0", "opt");
                nvc.Set("http://www.opengis.net/atm/2.0", "atm");
                nvc.Set("http://www.opengis.net/lmb/2.0", "lmb");
                nvc.Set("http://www.opengis.net/alt/2.0", "alt");
                nvc.Set("http://www.opengis.net/ssp/2.0", "ssp");
                nvc.Set("http://www.opengis.net/gml", "gml");
                nvc.Set("http://www.w3.org/2001/SMIL20/", "smil20");
                nvc.Set("http://www.w3.org/2001/SMIL20/Language", "lang");
                nvc.Set("http://www.isotc211.org/2005/gmd", "gmd");
                nvc.Set("http://www.opengis.net/gml/3.2", "gml32");
                nvc.Set("http://www.opengis.net/swe/1.0", "swe");
                nvc.Set("http://www.w3.org/1999/xlink", "xlink");
                nvc.Set("http://www.isotc211.org/2005/gco", "gco");
                nvc.Set("http://www.opengis.net/om/2.0", "om");
                nvc.Set("http://www.w3.org/2002/12/cal/ical#", "ical");
                nvc.Set("http://www.opengis.net/ows/2.0", "ows");
                nvc.Set("http://purl.org/dc/elements/1.1/", "dc");
                nvc.Set("http://www.georss.org/georss", "georss");
                nvc.Set("http://a9.com/-/opensearch/extensions/eo/1.0/", "eo");
                nvc.Set("http://a9.com/-/opensearch/extensions/geo/1.0/", "geo");
                nvc.Set("http://a9.com/-/opensearch/extensions/time/1.0/", "time");
                nvc.Set("http://a9.com/-/opensearch/extensions/param/1.0/", "param");
                nvc.Set("http://a9.com/-/spec/opensearch/1.1/", "os");

                return nvc;
            }
        }

        public static NameValueCollection TypeNamespaces21 {
            get {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Set("http://www.opengis.net/sar/2.1", "sar");
                nvc.Set("http://www.opengis.net/eop/2.1", "eop");
                nvc.Set("http://www.opengis.net/opt/2.1", "opt");
                nvc.Set("http://www.opengis.net/atm/2.1", "atm");
                nvc.Set("http://www.opengis.net/lmb/2.1", "lmb");
                nvc.Set("http://www.opengis.net/alt/2.1", "alt");
                nvc.Set("http://www.opengis.net/ssp/2.1", "ssp");
                nvc.Set("http://www.opengis.net/gml", "gml");
                nvc.Set("http://www.w3.org/2001/SMIL20/", "smil20");
                nvc.Set("http://www.w3.org/2001/SMIL20/Language", "lang");
                nvc.Set("http://www.isotc211.org/2005/gmd", "gmd");
                nvc.Set("http://www.opengis.net/gml/3.2", "gml32");
                nvc.Set("http://www.opengis.net/swe/1.0", "swe");
                nvc.Set("http://www.w3.org/1999/xlink", "xlink");
                nvc.Set("http://www.isotc211.org/2005/gco", "gco");
                nvc.Set("http://www.opengis.net/om/2.0", "om");
                nvc.Set("http://www.w3.org/2002/12/cal/ical#", "ical");
                nvc.Set("http://www.opengis.net/ows/2.0", "ows");
                nvc.Set("http://purl.org/dc/elements/1.1/", "dc");
                nvc.Set("http://www.georss.org/georss", "georss");
                nvc.Set("http://a9.com/-/opensearch/extensions/eo/1.0/", "eo");
                nvc.Set("http://a9.com/-/opensearch/extensions/geo/1.0/", "geo");
                nvc.Set("http://a9.com/-/opensearch/extensions/time/1.0/", "time");
                nvc.Set("http://a9.com/-/opensearch/extensions/param/1.0/", "param");
                nvc.Set("http://a9.com/-/spec/opensearch/1.1/", "os");

                return nvc;
            }
        }

        public static XmlNamespaceManager GetXmlNamespaceManager(XElement elem) {
            XmlNamespaceManager xnsm = new XmlNamespaceManager(elem.CreateReader().NameTable);
            foreach (var key in TypeNamespaces.AllKeys) {
                xnsm.AddNamespace(TypeNamespaces[key], key);
            }

            return xnsm;
        }
    }
}

