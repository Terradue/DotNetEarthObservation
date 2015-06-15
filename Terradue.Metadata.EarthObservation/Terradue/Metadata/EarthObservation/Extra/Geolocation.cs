using System;
using System.Xml.Serialization;

namespace Terradue.Metadata.EarthObservation.Extra {

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlRootAttribute("geolocationGrid", Namespace="http://www.terradue.com/model/eop", IsNullable=false)]
    public partial class GeolocationGridType {

        public static XmlSerializer GeolocationGridSerializer =new XmlSerializer(typeof(GeolocationGridType));

        private geolocationGridPointListType geolocationGridPointListField;

        /// <remarks>
        ///Geolocation grid. This element is a list of geolocationGridPoint records which contains grid point entries for each line/pixel combination based on a configured resolution. The list contains an entry for each geolocation grid update made along azimuth.
        ///</remarks>
        public geolocationGridPointListType geolocationGridPointList {
            get {
                return this.geolocationGridPointListField;
            }
            set {
                this.geolocationGridPointListField = value;
            }
        }
    }

    public partial class geolocationGridPointListType {

        private string countField24;

        private geolocationGridPointType[] geolocationGridPointField;

        /// <remarks>
        ///Number of geolocation grid point records within the list. 
        ///</remarks>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="nonNegativeInteger")]
        public string count {
            get {
                return this.countField24;
            }
            set {
                this.countField24 = value;
            }
        }

        /// <remarks>
        ///Geolocation grid point. This record describes geolocation information for a single point (line/pixel combination) within the image MDS. For 11 geolotcation grid points across range and a new set of points calculated every 1s in azimuth, for a maximum product length of 25 minutes, the maximum size of this list is 16500 elements.
        ///</remarks>
        [System.Xml.Serialization.XmlElementAttribute("geolocationGridPoint")]
        public geolocationGridPointType[] geolocationGridPoint {
            get {
                return this.geolocationGridPointField;
            }
            set {
                this.geolocationGridPointField = value;
            }
        }
    }

    /// <remarks>
    ///Annotation record for a geolocation grid point.
    ///</remarks>
    [System.Xml.Serialization.XmlRootAttribute("geolocationGridPoint", Namespace="http://www.terradue.com/model/eop", IsNullable=false)]
    public partial class geolocationGridPointType {

        public geolocationGridPointType() {
        }

        public geolocationGridPointType(geolocationGridPointType point){
            this.azimuthTime = point.azimuthTime;
            this.elevationAngle = point.elevationAngle;
            this.height = point.height;
            this.incidenceAngle = point.incidenceAngle;
            this.latitude = point.latitude;
            this.line = point.line;
            this.longitude = point.longitude;
            this.pixel = point.pixel;
            this.slantRangeTime = point.slantRangeTime;
        }

        private System.DateTime azimuthTimeField15;

        private double slantRangeTimeField3;

        private int lineField;

        private int pixelField;

        private double latitudeField;

        private double longitudeField;

        private double heightField;

        private double incidenceAngleField1;

        private double elevationAngleField1;

        /// <remarks>
        ///Zero Doppler azimuth time to which grid point applies [UTC].
        ///</remarks>
        [System.Xml.Serialization.XmlElementAttribute()]
        public System.DateTime azimuthTime {
            get {
                return this.azimuthTimeField15;
            }
            set {
                this.azimuthTimeField15 = value;
            }
        }

        /// <remarks>
        ///Two way slant range time to grid point [s].
        ///</remarks>
        [System.Xml.Serialization.XmlElementAttribute()]
        public double slantRangeTime {
            get {
                return this.slantRangeTimeField3;
            }
            set {
                this.slantRangeTimeField3 = value;
            }
        }

        /// <remarks>
        ///Reference image MDS line to which this geolocation grid point applies.
        ///</remarks>
        [System.Xml.Serialization.XmlElementAttribute()]
        public int line {
            get {
                return this.lineField;
            }
            set {
                this.lineField = value;
            }
        }

        /// <remarks>
        ///Reference image MDS sample to which this geolocation grid point applies.
        ///</remarks>
        [System.Xml.Serialization.XmlElementAttribute()]
        public int pixel {
            get {
                return this.pixelField;
            }
            set {
                this.pixelField = value;
            }
        }

        /// <remarks>
        ///Geodetic latitude of grid point [degrees].
        ///</remarks>
        [System.Xml.Serialization.XmlElementAttribute()]
        public double latitude {
            get {
                return this.latitudeField;
            }
            set {
                this.latitudeField = value;
            }
        }

        /// <remarks>
        ///Geodetic longitude of grid point [degrees].
        ///</remarks>
        [System.Xml.Serialization.XmlElementAttribute()]
        public double longitude {
            get {
                return this.longitudeField;
            }
            set {
                this.longitudeField = value;
            }
        }

        /// <remarks>
        ///Height of the grid point above sea level [m].
        ///</remarks>
        [System.Xml.Serialization.XmlElementAttribute()]
        public double height {
            get {
                return this.heightField;
            }
            set {
                this.heightField = value;
            }
        }

        /// <remarks>
        ///Incidence angle to grid point [degrees].
        ///</remarks>
        [System.Xml.Serialization.XmlElementAttribute()]
        public double incidenceAngle {
            get {
                return this.incidenceAngleField1;
            }
            set {
                this.incidenceAngleField1 = value;
            }
        }

        /// <remarks>
        ///Elevation angle to grid point [degrees].
        ///</remarks>
        [System.Xml.Serialization.XmlElementAttribute()]
        public double elevationAngle {
            get {
                return this.elevationAngleField1;
            }
            set {
                this.elevationAngleField1 = value;
            }
        }
    }
}

