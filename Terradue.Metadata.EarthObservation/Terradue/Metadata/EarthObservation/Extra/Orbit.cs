using System;
using System.Xml.Serialization;

namespace Terradue.Metadata.EarthObservation.Extra {
 
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlRootAttribute("orbits", Namespace="http://www.terradue.com/model/eop", IsNullable=false)]
    public partial class orbitListType {

        public static XmlSerializer OrbitsSerializer =new XmlSerializer(typeof(orbitListType));

        private orbitType[] orbitField;

        public orbitType[] orbit {
            get {
                return this.orbitField;
            }
            set {
                this.orbitField = value;
            }
        }
    }
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlRootAttribute("orbit", Namespace="http://www.terradue.com/model/eop", IsNullable=false)]
    public partial class orbitType {

        public orbitType() {
        }

        public orbitType(orbitType orbit){
            this.time = orbit.time;
            this.frame = orbit.frame;
            this.position = orbit.position;
            this.velocity = orbit.velocity;
            this.acceleration = orbit.acceleration;
            this.absoluteOrbit = orbit.absoluteOrbit;
        }

        private System.DateTime timeField;

        private referenceFrameType frameField;

        private double[] positionField;

        private double[] velocityField;

        private double[] accelerationField;

        private int absoluteOrbitField;

        public System.DateTime time {
            get {
                return this.timeField;
            }
            set {
                this.timeField = value;
            }
        }

        public virtual referenceFrameType frame {
            get {
                return this.frameField;
            }
            set {
                this.frameField = value;
            }
        }

        public int absoluteOrbit {
            get {
                return this.absoluteOrbitField;
                }
            set {
                this.absoluteOrbitField = value;
            }
        }
            
        public double[] position {
            get {
                return this.positionField;
            }
            set {
                this.positionField = value;
            }
        }
            
        public double[] velocity {
            get {
                return this.velocityField;
            }
            set {
                this.velocityField = value;
            }
        }

        public double[] acceleration {
            get {
                return this.accelerationField;
            }
            set {
                this.accelerationField = value;
            }
        }

    }
        
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.terradue.com/model/eop")]
    public enum referenceFrameType {

        /// <remarks/>
        Undefined,

        /// <remarks/>
        Galactic,

        /// <remarks/>
        BM1950,

        /// <remarks/>
        BM2000,

        /// <remarks/>
        HM2000,

        /// <remarks/>
        GM2000,

        /// <remarks/>
        MeanOfDate,

        /// <remarks/>
        TrueOfDate,

        /// <remarks/>
        PseudoTrueOfDate,

        /// <remarks/>
        EarthFixed,

        /// <remarks/>
        Topocentric,

        /// <remarks/>
        SatelliteOrbital,

        /// <remarks/>
        SatelliteNominal,

        /// <remarks/>
        SatelliteAttitude,

        /// <remarks/>
        InstrumentAttitude,
    }

}

