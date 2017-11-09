using System;
using System.IO;
using System.Web.UI;
using Terradue.Metadata.EarthObservation.Ogc.Extensions;

namespace Terradue.Metadata.EarthObservation.Helpers
{
    public static class Formatter
    {

        public static string GetHtmlSummaryForOgcObservationsAndMeasurements(ServiceModel.Ogc.Om20.OM_ObservationType om)
        {
            // Initialize StringWriter instance.
            StringWriter stringWriter = new StringWriter();

            // Put HtmlTextWriter in using block because it needs to call Dispose.
            using (System.Web.UI.HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {

                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                writer.RenderBeginTag(HtmlTextWriterTag.Tbody);

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");

                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                writer.RenderBeginTag(HtmlTextWriterTag.Tbody);

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Identifier");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(om.FindIdentifier());
                writer.RenderEndTag();
                writer.RenderEndTag();

                var date = om.FindRelevantDate();
                if (date.Ticks != 0)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write("Time");
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(date.ToUniversalTime().ToString("O"));
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                var pt = om.FindProductType();
                if (!string.IsNullOrEmpty(pt))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write("Product Type");
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(pt);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                var orbit = om.FindOrbitNumber();
                if (!string.IsNullOrEmpty(orbit))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write("Orbit");
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(orbit);
                    var orbitdir = om.FindOrbitDirection();
                    if (!string.IsNullOrEmpty(orbitdir))
                    {
                        writer.Write(" ");
                        writer.Write(orbitdir);
                    }
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                var track = om.FindTrack();
                if (!string.IsNullOrEmpty(track))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write("Track");
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(track);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                var swath = om.FindSwathIdentifier();
                if (!string.IsNullOrEmpty(swath))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write("Swath");
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(swath);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                if (om.IsOpticalDataset())
                {
                    var cc = om.FindCloudCoverPercentage();
                    if (cc > 1)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                        writer.Write("Cloud Cover");
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.Write(cc);
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                    }
                }

                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");

                string imageSrc = "";
                var browse = om.FindBrowseUrl();
                if (browse != null)
                    imageSrc = browse.ToString();

                if (!string.IsNullOrEmpty(imageSrc))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, imageSrc);
                    writer.AddAttribute(HtmlTextWriterAttribute.Height, "300px");
                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderEndTag();
                writer.RenderEndTag();


            }

            return stringWriter.ToString();

        }


    }
}
