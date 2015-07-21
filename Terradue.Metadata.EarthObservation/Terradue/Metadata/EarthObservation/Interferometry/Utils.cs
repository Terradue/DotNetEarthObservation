using System;
using log4net.Repository.Hierarchy;
using log4net;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;

namespace Terradue.Metadata.EarthObservation {

    public class Utils {

        static readonly ILog Logger = LogManager.GetLogger(typeof(Utils));


        public static IInterpolation[] PolyInterpol(OrbitStateVector[] orbitStateVectors){

            var time = Array.ConvertAll<OrbitStateVector, double>(orbitStateVectors, o => o.TimeFromAnx.TotalSeconds);

            Vector<double> x1 = DenseVector.OfArray(Array.ConvertAll<OrbitStateVector, double>(orbitStateVectors, o => o.X));
            Vector<double> y1 = DenseVector.OfArray(Array.ConvertAll<OrbitStateVector, double>(orbitStateVectors, o => o.Y));
            Vector<double> z1 = DenseVector.OfArray(Array.ConvertAll<OrbitStateVector, double>(orbitStateVectors, o => o.Z));

            Vector<double> vx1 = DenseVector.OfArray(Array.ConvertAll<OrbitStateVector, double>(orbitStateVectors, o => o.VelocityX));
            Vector<double> vy1 = DenseVector.OfArray(Array.ConvertAll<OrbitStateVector, double>(orbitStateVectors, o => o.VelocityY));
            Vector<double> vz1 = DenseVector.OfArray(Array.ConvertAll<OrbitStateVector, double>(orbitStateVectors, o => o.VelocityZ));

            Vector<double> ax1 = DenseVector.Create(vx1.Count, 0.0);
            Vector<double> ay1 = DenseVector.Create(vy1.Count, 0.0);
            Vector<double> az1 = DenseVector.Create(vz1.Count, 0.0);
            for (int i = 0; i < vx1.Count; i++) {
                ax1[i] = vx1[i] * i+1;
                ay1[i] = vy1[i] * i+1;
                az1[i] = vz1[i] * i+1;
            }

            IInterpolation[] coeff = new IInterpolation[orbitStateVectors.Length];

            coeff[0] = Interpolate.CubicSplineRobust(time, x1.ToArray());
            coeff[1] = Interpolate.CubicSplineRobust(time, y1.ToArray());
            coeff[2] = Interpolate.CubicSplineRobust(time, z1.ToArray());
            coeff[3] = Interpolate.CubicSplineRobust(time, vx1.ToArray());
            coeff[4] = Interpolate.CubicSplineRobust(time, vy1.ToArray());
            coeff[5] = Interpolate.CubicSplineRobust(time, vz1.ToArray());
            coeff[6] = Interpolate.CubicSplineRobust(time, ax1.ToArray());
            coeff[7] = Interpolate.CubicSplineRobust(time, ay1.ToArray());
            coeff[8] = Interpolate.CubicSplineRobust(time, az1.ToArray());



            return coeff;

        }
    }
}

