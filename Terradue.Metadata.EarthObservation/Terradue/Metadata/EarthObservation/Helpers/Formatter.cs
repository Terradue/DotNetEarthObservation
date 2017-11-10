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
                // div
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                //table
                writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "table-layout:fixed; width: 100 %;");
                writer.AddAttribute(HtmlTextWriterAttribute.Border, "1");
                writer.AddAttribute("frame", "void");
                writer.AddAttribute(HtmlTextWriterAttribute.Rules, "rows");
                writer.RenderBeginTag(HtmlTextWriterTag.Table);

                //find thhumbnail
                string imageSrc = "";
                var browse = om.FindBrowseUrl();
                if (browse != null)
                    imageSrc = browse.ToString();

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                // if QL add column
                if (!string.IsNullOrEmpty(imageSrc))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Width, "20%");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderEndTag();
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Width, "25%");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderEndTag();
                writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderEndTag();
                writer.RenderEndTag();
               
                // first raw for identifier name
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                if (!string.IsNullOrEmpty(imageSrc))
                    writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "3");
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "font-size:smaller");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write("Id: " + om.FindIdentifier());
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();
                // end identifier raw

                // second raw for platform
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                // if QL add column
                if (!string.IsNullOrEmpty(imageSrc))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, "10");
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, "padding:5px");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, imageSrc);
                    writer.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, imageSrc);
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, "width: 100%; max-height: 100%;");
                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    //end image column
                }

                // platform column
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("Platform");
                writer.RenderEndTag();
                writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write(om.FindPlatformShortName());
                writer.RenderEndTag();
                writer.RenderEndTag();

                // end second raw
                writer.RenderEndTag();

                // third raw for instrument
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                // instrument column
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("Sensor");
                writer.RenderEndTag();
                writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write(om.FindInstrumentShortName());
                writer.RenderEndTag();
                writer.RenderEndTag();

                // end third raw
                writer.RenderEndTag();


                // next rows
                var date = om.FindRelevantDate();
                if (date.Ticks != 0)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write("Time");
                    writer.RenderEndTag();
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write(date.ToUniversalTime().ToString("O"));
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    // end raw
                    writer.RenderEndTag();
                }

                var pt = om.FindProductType();
                if (!string.IsNullOrEmpty(pt))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write("Product");
                    writer.RenderEndTag();
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write(pt);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    // end raw
                    writer.RenderEndTag();
                }

                var mode = om.FindOperationalMode();
                if (!string.IsNullOrEmpty(mode))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write("Mode");
                    writer.RenderEndTag();
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write(mode);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    // end raw
                    writer.RenderEndTag();
                }

                var orbit = om.FindOrbitNumber();
                if (!string.IsNullOrEmpty(orbit))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write("Orbit");
                    writer.RenderEndTag();
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write(orbit);
                    var orbitdir = om.FindOrbitDirection();
                    if (!string.IsNullOrEmpty(orbitdir))
                    {
                        writer.Write(" ");
                        writer.Write(orbitdir);
                    }
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    // end raw
                    writer.RenderEndTag();
                }

                var track = om.FindTrack();
                if (!string.IsNullOrEmpty(track))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write("Track");
                    writer.RenderEndTag();
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write(track);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    // end raw
                    writer.RenderEndTag();
                }

                var swath = om.FindSwathIdentifier();
                if (!string.IsNullOrEmpty(swath))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write("Swath");
                    writer.RenderEndTag();
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.Write(swath);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    // end raw
                    writer.RenderEndTag();
                }

                if (om.IsOpticalDataset())
                {
                    var cc = om.FindCloudCoverPercentage();
                    if (cc > 1)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.Write("Cloud cov");
                        writer.RenderEndTag();
                        writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                        writer.Write(cc);
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                        // end raw
                        writer.RenderEndTag();
                    }
                }


                // end table
                writer.RenderEndTag();

                // end div
                writer.RenderEndTag();



            }

            return stringWriter.ToString();

        }


    }
}
