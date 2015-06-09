using System;

namespace Terradue.Metadata.EarthObservation.Extra {
 
    public partial class orbitListType {

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

    public partial class orbitType {

        private System.DateTime timeField;

        private referenceFrameType frameField;

        private double[] positionField;

        private double[] velocityField;

        private double slantRangeTime;

        private double sr0;

        public System.DateTime time {
            get {
                return this.timeField;
            }
            set {
                this.timeField = value;
            }
        }

        public referenceFrameType frame {
            get {
                return this.frameField;
            }
            set {
                this.frameField = value;
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
    }
        
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

