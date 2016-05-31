using System;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems.Transformations;

namespace Terradue.Metadata.EarthObservation {

    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

    public struct OrbitStateVector {
        
        private TimeSpan timeFromAnx;
        private double x, y, z, velocityX, velocityY, velocityZ, accelerationX, accelerationY, accelerationZ;

        public TimeSpan TimeFromAnx {
            get {
                return timeFromAnx;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------

        public double X { 
            get { return x; }
        }

        //---------------------------------------------------------------------------------------------------------------------

        public double Y { 
            get { return y; }
        }

        //---------------------------------------------------------------------------------------------------------------------

        public double Z { 
            get { return z; }
        }

        //---------------------------------------------------------------------------------------------------------------------

        public double VelocityX {
            get { return velocityX; }
        }

        //---------------------------------------------------------------------------------------------------------------------

        public double VelocityY {
            get { return velocityY; }
        }

        //---------------------------------------------------------------------------------------------------------------------

        public double VelocityZ {
            get { return velocityZ; }
        }

        //---------------------------------------------------------------------------------------------------------------------

        public double AccelerationX {
            get { return accelerationX; }
        }

        //---------------------------------------------------------------------------------------------------------------------

        public double AccelerationY {
            get { return accelerationY; }
        }

        //---------------------------------------------------------------------------------------------------------------------

        public double AccelerationZ {
            get { return accelerationZ; }
        }

        public double this[int i] {
            get {
                switch (i) {
                    case 0 : return X;
                    case 1 : return Y;
                    case 2 : return Z;
                    case 3 : return VelocityX;
                    case 4 : return VelocityY;
                    case 5 : return VelocityZ;
                    case 6 : return AccelerationX;
                    case 7 : return AccelerationY;
                    case 8 : return AccelerationZ;
                    default : throw new IndexOutOfRangeException();
                }
            }
        }

        //---------------------------------------------------------------------------------------------------------------------

        public OrbitStateVector(TimeSpan timeFromAnx, double x, double y, double z, double vx, double vy, double vz, double ax, double ay, double az) {
            this.timeFromAnx = timeFromAnx;
            this.x = x;
            this.y = y;
            this.z = z;
            this.velocityX = vx;
            this.velocityY = vy;
            this.velocityZ = vz;
            this.accelerationX = ax;
            this.accelerationY = ay;
            this.accelerationZ = az;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------

}