using System.Reflection;
using System.Runtime.CompilerServices;
using NuGet4Mono.Extensions;

/*!

\namespace Terradue.Metadata.EarthObservation
@{
    Terradue .Net C# library implementing the Earth Observation Metadata profile of Observations & Measurements

    \xrefitem sw_version "Versions" "Software Package Version" 1.5.6

    \xrefitem sw_link "Link" "Software Package Link" [DotNetEarthObservation](https://github.com/Terradue/DotNetEarthObservation)

    \xrefitem sw_license "License" "Software License" [AGPL](https://github.com/Terradue/DotNetEarthObservation/blob/master/LICENSE)

    \xrefitem sw_req "Require" "Software Dependencies" \ref Terradue.OpenSearch

    \xrefitem sw_req "Require" "Software Dependencies" \ref Terradue.GeoJson

    \xrefitem sw_req "Require" "Software Dependencies" \ref NetTopologySuite

    \ingroup Data
@}

*/

// Information about this assembly is defined by the following attributes.
// Change them to the values specific to your project.
[assembly: AssemblyTitle("Terradue.Metadata.EarthObservation")]
[assembly: AssemblyDescription(".Net C# library implementing the Earth Observation Metadata profile of Observations and Measurements")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Terradue")]
[assembly: AssemblyProduct("Terradue.Metadata.EarthObservation")]
[assembly: AssemblyCopyright("Terradue")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.
[assembly: AssemblyVersion("1.5.6.*")]
[assembly: AssemblyInformationalVersion("1.5.6")]
[assembly: AssemblyAuthors("Emmanuel Mathot")]
[assembly: AssemblyProjectUrl("https://github.com/Terradue/DotNetEarthObservation")]
[assembly: AssemblyLicenseUrl("https://github.com/Terradue/DotNetEarthObservation/blob/master/LICENSE")]
// The following attributes are used to specify the signing key for the assembly,
// if desired. See the Mono documentation for more information about signing.
//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]