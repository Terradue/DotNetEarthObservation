# DotNetEarthObservation

.Net C# library implementing the Earth Observation Metadata profile of Observations &amp;  Measurements

Classes are directly generated from the XSD files from the OGC Earth Observation Metadata profile of Observations & Measurements (1.0)	 10-157r3 

see more at http://www.opengeospatial.org/standards/om

## Curent implementation

The current implementation includes:
- EarthObservation classes as the base types. XML namespace: http://www.opengis.net/eop/2.0 [eop]
- Sar classes as the classes to Sar dataset. XML namespace: http://www.opengis.net/sar/2.0 [sar]
- Sentinel-1 classes as the classes specific to Sentinel-1 mission. XML namespace: http://www.opengis.net/sen1/2.0 [sen1]
