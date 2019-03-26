using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using Terradue.Metadata.EarthObservation.Ogc.Extensions;


namespace Terradue.Metadata.EarthObservation.Helpers {

    public static class Formatter {

        /// <summary>
        /// Generates Html Summary from an Observation Type object
        /// </summary>
        /// <param name="om"></param>
        /// <returns></returns>
        public static string GetHtmlSummaryForOgcObservationsAndMeasurements(ServiceModel.Ogc.Om20.OM_ObservationType om) {
            // Initialize StringWriter instance.
            StringWriter stringWriter = new StringWriter();

            // Put HtmlTextWriter in using block because it needs to call Dispose.
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter)) {
                //table
                writer.RenderBeginTag(HtmlTextWriterTag.Table);

                var platformShortName = om.FindPlatformShortName();
                AddTableRow(writer, "Platform", platformShortName);

                var instrumentShortName = om.FindInstrumentShortName();
                AddTableRow(writer, "Sensor", instrumentShortName);

                var productType = om.FindProductType();
                AddTableRow(writer, "Product Type", productType);

                var startTime = om.FindBeginPosition();
                var endTime = om.FindEndPosition();
                if (startTime.Ticks != 0 && endTime.Ticks != 0) {
                    AddTableRow(writer, "Start Time", startTime);
                    AddTableRow(writer, "End Time", endTime);
                } else {
                    var relevantDate = om.FindRelevantDate();
                    AddTableRow(writer, "Date", relevantDate);
                }

                var orbitNumber = om.FindOrbitNumber();
                var orbitDirection = om.FindOrbitDirection();
                var orbitValues = new List<string>();
                if (!string.IsNullOrEmpty(orbitNumber)) orbitValues.Add(orbitNumber);
                if (!string.IsNullOrEmpty(orbitDirection)) orbitValues.Add(orbitDirection);
                var orbitString = string.Join(" ", orbitValues);
                AddTableRow(writer, "Orbit", orbitString);

                var track = om.FindTrack();
                AddTableRow(writer, "Track", track);

                var mode = om.FindOperationalMode();
                AddTableRow(writer, "Mode", mode);

                var swath = om.FindSwathIdentifier();
                AddTableRow(writer, "Swath", swath);

                var polarisationChannels = om.FindPolarisationChannels();
                if (!String.IsNullOrEmpty(polarisationChannels)) AddTableRow(writer, "Polarisation channels", polarisationChannels);

                var identifier = om.FindIdentifier();
                if (!String.IsNullOrEmpty(identifier)) AddTableRow(writer, "Identifier", identifier);

                if (om.IsOpticalDataset()) {
                    var cc = om.FindCloudCoverPercentage();
                    if (cc > 1) {
                        AddTableRow(writer, "Cloud coverage", cc.ToString());
                    }
                }
                // end table
                writer.RenderEndTag();
            }

            return stringWriter.ToString();
        }


        /// <summary>
        ///  Generates an atom tile from an Observation Type object
        /// </summary>
        /// <param name="om"></param>
        /// <returns></returns>
        public static string GetAtomTitleForOgcObservationsAndMeasurements(ServiceModel.Ogc.Om20.OM_ObservationType om) {
            var infoList = new Collection<string>();
            infoList.Add(om.FindPlatformShortName());
            infoList.Add(om.FindProductType());
            infoList.Add(om.FindOperationalMode());
            infoList.Add(om.FindProcessingLevel());
            infoList.Add(om.FindTrack());
            infoList.Add(om.FindRelevantDate().ToString("R"));
            return String.Join(" ", infoList.Where(s => !string.IsNullOrEmpty(s)));
        }


        private static void AddTableRow(HtmlTextWriter writer, string propertyName, string stringValue) {
            if (string.IsNullOrEmpty(stringValue)) return;
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(propertyName);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.RenderBeginTag(HtmlTextWriterTag.Strong);
            writer.Write(stringValue);
            writer.RenderEndTag();
            writer.RenderEndTag();
            // end row
            writer.RenderEndTag();
        }


        private static void AddTableRow(HtmlTextWriter writer, string propertyName, DateTime dateTimeValue) {
            if (dateTimeValue.Ticks == 0) return;
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(propertyName);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.RenderBeginTag(HtmlTextWriterTag.Strong);
            writer.Write(dateTimeValue.ToUniversalTime().ToString("O"));
            writer.RenderEndTag();
            writer.RenderEndTag();
            // end row
            writer.RenderEndTag();
        }

    }

}