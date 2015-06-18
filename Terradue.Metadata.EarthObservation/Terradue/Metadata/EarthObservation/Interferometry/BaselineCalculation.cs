﻿using System;

namespace Terradue.Metadata.EarthObservation {
    public static class BaselineCalculation {

        public static double[] GeoEllipsoid(DateTimeOffset t, int ro1,
                                            OrbitStateVector[] master,
                                            double[] ground,
                                            bool conv = 1,
                                            int maxiter = 100,
                                            double tol_s = 0.0001

        ) {



        }

        /// <summary>Computes the solution of a linear equation system.</summary>
        /// <param name="M">
        /// The system of linear equations as an augmented matrix[row, col] where (rows + 1 == cols).
        /// It will contain the solution in "row canonical form" if the function returns "true".
        /// </param>
        /// <returns>Returns whether the matrix has a unique solution or not.</returns>
        public static bool Invert(float[,] M) {
            // input checks
            int rowCount = M.GetUpperBound(0) + 1;
            if (M == null || M.Length != rowCount * (rowCount + 1))
                throw new ArgumentException("The algorithm must be provided with a (n x n+1) matrix.");
            if (rowCount < 1)
                throw new ArgumentException("The matrix must at least have one row.");

            // pivoting
            for (int col = 0; col + 1 < rowCount; col++)
                if (M[col, col] == 0)
 {            // check for zero coefficients
                    // find non-zero coefficient
                    int swapRow = col + 1;
                    for (; swapRow < rowCount; swapRow++)
                        if (M[swapRow, col] != 0)
                            break;

                    if (M[swapRow, col] != 0) { // found a non-zero coefficient?
                        // yes, then swap it with the above
                        float[] tmp = new float[rowCount + 1];
                        for (int i = 0; i < rowCount + 1; i++) {
                            tmp[i] = M[swapRow, i];
                            M[swapRow, i] = M[col, i];
                            M[col, i] = tmp[i];
                        }
                    } else
                        return false; // no, then the matrix has no unique solution
                }

            // elimination
            for (int sourceRow = 0; sourceRow + 1 < rowCount; sourceRow++) {
                for (int destRow = sourceRow + 1; destRow < rowCount; destRow++) {
                    float df = M[sourceRow, sourceRow];
                    float sf = M[destRow, sourceRow];
                    for (int i = 0; i < rowCount + 1; i++)
                        M[destRow, i] = M[destRow, i] * df - M[sourceRow, i] * sf;
                }
            }

            // back-insertion
            for (int row = rowCount - 1; row >= 0; row--) {
                float f = M[row, row];
                if (f == 0)
                    return false;

                for (int i = 0; i < rowCount + 1; i++)
                    M[row, i] /= f;
                for (int destRow = 0; destRow < row; destRow++) {
                    M[destRow, rowCount] -= M[destRow, row] * M[row, rowCount];
                    M[destRow, row] = 0;
                }
            }
            return true;
        }
    }

    struct OrbitStateVector {

        double X;
        double Y;
        double Z;
        double vX;
        double vY;
        double vZ;
        double aX;
        double aY;
        double aZ;

    }
}

