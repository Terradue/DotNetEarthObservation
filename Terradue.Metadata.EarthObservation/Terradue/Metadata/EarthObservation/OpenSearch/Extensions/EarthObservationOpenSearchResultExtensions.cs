using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Terradue.GeoJson.Geometry;
using Terradue.GeoJson.GeoRss;
using Terradue.GeoJson.GeoRss10;
using Terradue.Metadata.EarthObservation.Helpers;
using Terradue.Metadata.EarthObservation.Ogc.Extensions;
using Terradue.OpenSearch;
using Terradue.OpenSearch.Result;
using Terradue.OpenSearch.Schema;

namespace Terradue.Metadata.EarthObservation.OpenSearch.Extensions
{
	public static class EarthObservationOpenSearchResultExtensions
	{



		public static ServiceModel.Ogc.Om20.OM_ObservationType GetEarthObservationProfile(this IOpenSearchResultItem item)
		{
			return item.ElementExtensions.GetEarthObservationProfile();
		}

		public static GeometryObject FindGeometry(this IOpenSearchResultItem item)
		{

			var geom = FindGeoTimeGeometry(item);
			if (geom != null)
				return geom;
			return item.FindEarthObservationProfileGeometry();

		}

		public static GeometryObject FindGeoTimeGeometry(this IOpenSearchResultItem item)
		{

			GeometryObject savegeom = null;

			if (item.ElementExtensions != null && item.ElementExtensions.Count > 0)
			{

				foreach (var ext in item.ElementExtensions)
				{

					XmlReader xr = ext.GetReader();

					switch (xr.NamespaceURI)
					{
						// 1) search for georss
						case "http://www.georss.org/georss":
                            var georss = GeoJson.GeoRss.GeoRssHelper.Deserialize(xr);
                            savegeom = georss.ToGeometry();
                            if (!(georss is GeoRssBox || georss is GeoRssPoint))
                            {
								return savegeom;
							}
							break;
						// 2) search for georss10
						case "http://www.georss.org/georss/10":
                            var georss10 = GeoJson.GeoRss10.GeoRss10Helper.Deserialize(xr);
                            savegeom = georss10.ToGeometry();
                            if (!(georss10 is GeoRssBox || georss10 is GeoRssPoint))
                            {
								return savegeom;
							}
							break;
						// 3) search for dct:spatial
						case "http://purl.org/dc/terms/":
							if (xr.LocalName == "spatial")
							{
								savegeom = WktExtensions.WktToGeometry(xr.ReadElementContentAsString());
							}
							if (!(savegeom is Point))
							{
								return savegeom;
							}
							break;
						default:
							continue;
					}

				}

			}

			return savegeom;

		}

		public static GeometryObject FindEarthObservationProfileGeometry(this IOpenSearchResultItem item)
		{

			var eop = item.GetEarthObservationProfile();

			if (eop != null)
				return eop.FindGeometry();

			return null;
		}

		public static string FindIdentifier(this IOpenSearchResultItem item)
		{

			var elements = item.ElementExtensions.ReadElementExtensions<string>("identifier", "http://purl.org/dc/elements/1.1/");
			if (elements.Count > 0)
				return elements[0];

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
				return eo.FindIdentifier();

			return null;

		}

		public static DateTime FindStartDate(this IOpenSearchResultItem item)
		{

			var elements = item.ElementExtensions.ReadElementExtensions<string>("date", "http://purl.org/dc/elements/1.1/");
			if (elements.Count > 0)
			{
				if (elements[0].Contains('/'))
				{
					return DateTime.Parse(elements[0].Split('/').First()).ToUniversalTime();
				}
				else
					return DateTime.Parse(elements[0]).ToUniversalTime();
			}

			elements = item.ElementExtensions.ReadElementExtensions<string>("date", "");
			if (elements.Count > 0)
			{
				if (elements[0].Contains('/'))
				{
					return DateTime.Parse(elements[0].Split('/').First()).ToUniversalTime();
				}
				else
					return DateTime.Parse(elements[0]).ToUniversalTime();
			}

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{
				return eo.FindBeginPosition();
			}

			return DateTime.MinValue;

		}

		public static DateTime FindEndDate(this IOpenSearchResultItem item)
		{

			var elements = item.ElementExtensions.ReadElementExtensions<string>("date", "http://purl.org/dc/elements/1.1/");
			if (elements.Count > 0)
			{
				if (elements[0].Contains('/'))
					return DateTime.Parse(elements[0].Split('/').Last()).ToUniversalTime();
			}

			elements = item.ElementExtensions.ReadElementExtensions<string>("dtend", "http://www.w3.org/2002/12/cal/ical#");
			if (elements.Count > 0)
				return DateTime.Parse(elements[0]).ToUniversalTime();

			elements = item.ElementExtensions.ReadElementExtensions<string>("date", "");
			if (elements.Count > 0)
			{
				if (elements[0].Contains('/'))
					return DateTime.Parse(elements[0].Split('/').Last()).ToUniversalTime();
			}

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{
				return eo.FindEndPosition();
			}

			return DateTime.MaxValue;

		}



		public static string FindProductType(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{
				return eo.FindProductType();
			}

			return null;

		}

		public static string FindParentIdentifier(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{
				return eo.FindParentIdentifier();
			}

			return null;

		}

		public static string FindOrbitNumber(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindOrbitNumber();
			}

			return null;

		}



		public static string FindOrbitDirection(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindOrbitDirection();
			}

			return null;

		}

		public static string FindTrack(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindTrack();
			}

			return null;

		}


		public static string FindFrame(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindFrame();
			}

			return null;

		}

		public static double FindCloudCoverPercentage(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{
				return eo.FindCloudCoverPercentage();
			}

			return 0;

		}

		public static string FindSwathIdentifier(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindSwathIdentifier();
			}

			return null;

		}

		public static string FindPlatformShortName(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindPlatformShortName();
			}

			return null;

		}

		public static string FindInstrumentShortName(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindInstrumentShortName();
			}

			return null;

		}

		public static string FindOperationalMode(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindOperationalMode();
			}

			return null;

		}

		public static string FindPolarisationChannels(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindPolarisationChannels();
			}

			return null;

		}

		public static string FindWrsLongitudeGrid(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindWrsLongitudeGrid();
			}

			return null;

		}

		public static string FindWrsLatitudeGrid(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindWrsLatitudeGrid();
			}

			return null;

		}

		public static string FindProcessingLevel(this IOpenSearchResultItem item)
		{

			var eo = item.GetEarthObservationProfile();

			if (eo != null)
			{

				return eo.FindProcessingLevel();
			}

			return null;

		}


	}

}

