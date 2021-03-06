﻿using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Interpolation;

namespace Terradue.Metadata.EarthObservation {

    public class BaselineCalculation {

      
        /// <summary>Synchronizes the vector.</summary>
        /// <returns>The vector.</returns>
        /// <param name="t0">T0.</param>
        /// <param name="groundPoint">Ground point.</param>
        /// <param name="master">Master.</param>
        /// <param name="converged">Converged.</param>
        /// <param name="maxError">Max error.</param>
        /// <param name="maxIterations">Max iterations.</param>
        /// <param name="tolerance">Tolerance.</param>
        public static double SynchronizeVector(
            double t0,
            Point3D groundPoint,
            IInterpolation[] slave,
            out bool converged,
            out double maxError,
            int maxIterations = 100,
            double tolerance = 0.00000001
        ) {
            double t = t0;
            double delta = 0;

            for (int i = 0; i < maxIterations; i++) {

                double x = groundPoint.X;
                double y = groundPoint.Y;
                double z = groundPoint.Z;

                double sxm = Sx(t, slave);
                double sym = Sy(t, slave);
                double szm = Sz(t, slave);

                double vxm = Vx(t, slave);
                double vym = Vy(t, slave);
                double vzm = Vz(t, slave);

                double axm = Ax(t, slave);
                double aym = Ay(t, slave);
                double azm = Az(t, slave);

                delta = - (vxm * (x - sxm) + vym * (y - sym) + vzm * (z - szm)) / (axm * (x - sxm) + aym * (y - sym) + azm * (z - szm) - vxm * vxm - vym * vym - vzm * vzm);
                t += delta;
                if (Math.Abs(delta) < tolerance) break;

            }

            maxError = Math.Abs(delta);
            converged = true;

            Console.WriteLine("RESULT: {0}", t);

            return t;
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double SynchronizeVector2(
            double t0,
            Point3D groundPoint,
            IInterpolation[] interpol,
            out bool converged,
            out double maxError,
            int maxIterations = 100,
            double tolerance = 0.00000001
        ) {
            double t = t0;
            double delta = 0;

            for (int i = 0; i < maxIterations; i++) {

                double x = groundPoint.X;
                double y = groundPoint.Y;
                double z = groundPoint.Z;

                double sxm = Sx(t, interpol);
                double sym = Sy(t, interpol);
                double szm = Sz(t, interpol);

                double vxm = Vx(t, interpol);
                double vym = Vy(t, interpol);
                double vzm = Vz(t, interpol);

                double axm = Ax(t, interpol);
                double aym = Ay(t, interpol);
                double azm = Az(t, interpol);

                delta = - (vxm * (x - sxm) + vym * (y - sym) + vzm * (z - szm)) / (axm * (x - sxm) + aym * (y - sym) + azm * (z - szm) - vxm * vxm - vym * vym - vzm * vzm);
                t += delta;
                if (Math.Abs(delta) < tolerance) break;

            }

            maxError = Math.Abs(delta);
            converged = true;

            Console.WriteLine("RESULT: {0}", t);

            return t;
        }


        public static BaselineVector CalculateBaseline2(OrbitStateVector[] master, OrbitStateVector[] slave, Point3D groundPoint, DateTime anxTime) {

            var interpolSlave = Utils.PolyInterpol(slave);
            var interpolMaster = Utils.PolyInterpol(master);

            double x = groundPoint.X;
            double y = groundPoint.Y;
            double z = groundPoint.Z;

            bool converged;
            double maxError;
            double t = groundPoint.Time.Subtract(anxTime).TotalSeconds;
            //double tm = SynchronizeVector(t, groundPoint, interpolMaster, out converged, out maxError);
            //double ts = SynchronizeVector(t, groundPoint, interpolSlave, out converged, out maxError);


            // Define baseline vector:
            double[] b = new double[3] {
                Sx(t, interpolSlave) - Sx(t, interpolMaster), 
                Sy(t, interpolSlave) - Sy(t, interpolMaster),
                Sz(t, interpolSlave) - Sz(t, interpolMaster)
            };

            double bMod = Math.Sqrt(b[0] * b[0] + b[1] * b[1] + b[2] * b[2]);

            // Baseline unit vector
            double[] bUnit = new double[3]; // 
            for (int k = 0; k < 3; k++) bUnit[k] = b[k] / bMod;

            // Parallel vector
            double[] vpar = new double[3] {
                x - Sx(t, interpolMaster),
                y - Sy(t, interpolMaster),
                z - Sz(t, interpolMaster)
            };
            double vparMod = Math.Sqrt(vpar[0] * vpar[0] + vpar[1] * vpar[1] + vpar[2] * vpar[2]);

            // Parallel unit vector
            double[] vparUnit = new double[3]; 
            for (int k = 0; k < 3; k++) vparUnit[k] = vpar[k] / vparMod;

            // Along-track vector
            double[] va = new double[3] {
                Vx(t, interpolMaster),
                Vy(t, interpolMaster),
                Vz(t, interpolMaster)
            };

            double vaMod = Math.Sqrt(va[0] * va[0] + va[1] * va[1] + va[2] * va[2]);

            // Along-track unit vector
            double[] vaUnit = new double[3];
            for (int k = 0; k < 3; k++) vaUnit[k] = va[k] / vaMod;

            double[] vperpUnit =  VectorProduct3D(vparUnit, vaUnit);

            double bPerp = b[0] * vperpUnit[0] + b[1] * vperpUnit[1] + b[2] * vperpUnit[2];
            double bParallel = b[0] * vparUnit[0] + b[1] * vparUnit[1] + b[2] * vparUnit[2];
            double bAlong = b[0] * vaUnit[0] + b[1] * vaUnit[1] + b[2] * vaUnit[2];

            Console.WriteLine("{0} {1} {2}", bPerp, bParallel, bAlong);

            return new BaselineVector(bPerp, bParallel, bAlong);

        }

        public static double[] VectorProduct3D(double[] vectorA, double[] vectorB) {
            if (vectorA.Length != 3 || vectorB.Length != 3)
                throw new ArgumentException("3D vector product cannot be calculated, because one of the vectors is not a 3D vector");
            return new double[3] {
                vectorA[1] * vectorB[2] - vectorA[2] * vectorB[1],
                vectorA[2] * vectorB[0] - vectorA[0] * vectorB[2],
                vectorA[0] * vectorB[1] - vectorA[1] * vectorB[0]
            };
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double Sx(double time, IInterpolation[] orbitStates) {
            return GetOrbitValue(time, orbitStates, 0);
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double Sy(double time, IInterpolation[] orbitStates) {
            return GetOrbitValue(time, orbitStates, 1);
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double Sz(double time, IInterpolation[] orbitStates) {
            return GetOrbitValue(time, orbitStates, 2);
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double Vx(double time, IInterpolation[] orbitStates) {
            return GetOrbitValue(time, orbitStates, 3);
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double Vy(double time, IInterpolation[] orbitStates) {
            return GetOrbitValue(time, orbitStates, 4);
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double Vz(double time, IInterpolation[] orbitStates) {
            return GetOrbitValue(time, orbitStates, 5);
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double Ax(double time, IInterpolation[] orbitStates) {
            return GetOrbitValue(time, orbitStates, 6);
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double Ay(double time, IInterpolation[] orbitStates) {
            return GetOrbitValue(time, orbitStates, 7);
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double Az(double time, IInterpolation[] orbitStates) {
            return GetOrbitValue(time, orbitStates, 8);
        }

        //---------------------------------------------------------------------------------------------------------------------

        private static double GetOrbitValue(double time, IInterpolation[] interpol, int index) {
            
            return interpol[index].Interpolate(time);

        }

        //---------------------------------------------------------------------------------------------------------------------


    }


}
