using System;

namespace Terradue.Metadata.EarthObservation {

    /// <summary>Linear algebra functions.</summary>
    public class LinearAlgebra {

        /// <summary>Calculates the cross product of two vectors in the euclidian three-dimensional space.</summary>
        /// <param name="vectorA">First vector.</param>
        /// <param name="vectorB">Second vector.</param>
        /// <returns>The vector representing the product.</returns>
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

        /// <summary>Calculates the cross product of two matrices in the three die.</summary>
        /// <param name="matrixA">Matrix a.</param>
        /// <param name="vectorB">Vector b.</param>
        /// <returns>The matrix product.</returns>
        public static double[,] MatrixProduct(double[,] matrixA, double[] vectorB) {
            double[,] matrixB = new double[vectorB.Length, 1];
            for (int i = 0; i < vectorB.Length; i++)
                matrixB[i, 0] = vectorB[i];

            return MatrixProduct(matrixA, matrixB);
        }

        //---------------------------------------------------------------------------------------------------------------------

        public static double[,] MatrixProduct(double[,] matrixA, double[,] matrixB) {

            for (int ii = 0; ii < matrixA.GetUpperBound(0) + 1; ii++) {
                Console.Write("MA ");
                for (int jj = 0; jj < matrixA.GetUpperBound(1) + 1; jj++) {
                    Console.Write("  {0,8:0.000}", (matrixA[ii, jj]));
                }
                Console.WriteLine();
            }
            for (int ii = 0; ii < matrixB.GetUpperBound(0) + 1; ii++) {
                Console.Write("MB ");
                for (int jj = 0; jj < matrixB.GetUpperBound(1) + 1; jj++) {
                    Console.Write("  {0,8:0.000}", (matrixB[ii, jj]));
                }
                Console.WriteLine();
            }

            // Number of columns of matrix A must be equal to number of rows of matrix B
            int summandCount = matrixA.GetUpperBound(1) + 1;
            if (matrixB.GetUpperBound(0) + 1 != summandCount)
                throw new ArgumentException("Column number of matrix A is different from row number of matrix B");

            int resultRowCount = matrixA.GetUpperBound(0) + 1;
            int resultColCount = matrixB.GetUpperBound(1) + 1;
            double[,] result = new double[resultRowCount, resultColCount];

            for (int i = 0; i < resultRowCount; i++) {
                for (int j = 0; i < resultColCount; i++) {
                    for (int k = 0; k < summandCount; k++)
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                }
            }

            for (int ii = 0; ii < resultRowCount; ii++) {
                Console.Write("MR ");
                for (int jj = 0; jj < resultColCount; jj++) {
                    Console.Write("  {0,8:0.000}", (result[ii, jj]));
                }
                Console.WriteLine();
            }

            return result;
        }

        //---------------------------------------------------------------------------------------------------------------------

        /// <summary>Calculates the inverse of the specified matrix using the Gaussion elimination algorithm.</summary>
        /// <param name="matrix">Matrix.</param>
        /// <param name="inverseMatrix">Contains the inverse matrix if it exists.</param>
        /// <returns><c>true</c> if the inverse matrix was calculated, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException">Is thrown if the input matrix is not square.</exception>
        public static bool TryInvertMatrix(double[,] matrix, out double[,] inverseMatrix) {
            double minPrecision = 0.000000001;

            // Check if matrix is square matrix
            int size = matrix.GetUpperBound(0) + 1;
            if (matrix.GetUpperBound(1) + 1 != size)
                throw new ArgumentException("Matrix cannot be inverted because is not square");

            // Attach identity matrix to the right
            double[,] tempMatrix = new double[size, 2 * size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++)
                    tempMatrix[i, j] = matrix[i, j];
                for (int j = 0; j < size; j++)
                    tempMatrix[i, j + size] = (j == i ? 1 : 0);
            }

            for (int ii = 0; ii < size; ii++) {
                for (int jj = 0; jj < size; jj++) {
                    Console.Write("  {0,8:0.000}", (tempMatrix[ii, jj]));
                }
                Console.Write("  |");
                for (int jj = 0; jj < size; jj++) {
                    Console.Write("  {0,8:0.000}", (tempMatrix[ii, jj + size]));
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            // Gaussian elimination
            // Loop over diagonal elements of left half matrix (row and column at the same time)
            for (int i = 0; i < size; i++) {
                int best = i;
                // Loop over rows below current element of outer loop and find value highest element in that column
                for (int j = i + 1; j < size; j++)
                    if (tempMatrix[j, i] > tempMatrix[best, i])
                        best = j;

                if (Math.Abs(tempMatrix[best, i]) < minPrecision) {
                    //printf("\n Elements are too small to deal with !!!");
                    //throw new Exception("Too small");
                }
                // If better row was found, swap it with current row
                if (best != i) {
                    for (int j = 0; j < 2 * size; j++) {
                        double tempValue = tempMatrix[i, j];
                        tempMatrix[i, j] = tempMatrix[best, j];
                        tempMatrix[best, j] = tempValue;
                    }
                }

                // Perform row operations to transform left half matrix into identity matrix
                for (int j = 0; j < size; j++) {
                    double r = tempMatrix[j, i];
                    if (i == j) {
                        for (int k = 0; k < 2 * size; k++)
                            tempMatrix[j, k] /= r;
                    } else {
                        for (int k = 0; k < 2 * size; k++)
                            tempMatrix[j, k] -= tempMatrix[i, k] * r / tempMatrix[i, i];
                    }
                }
                for (int ii = 0; ii < size; ii++) {
                    for (int jj = 0; jj < size; jj++) {
                        Console.Write("  {0,8:0.000}", (tempMatrix[ii, jj]));
                    }
                    Console.Write("  |");
                    for (int jj = 0; jj < size; jj++) {
                        Console.Write("  {0,8:0.000}", (tempMatrix[ii, jj + size]));
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();

            }

            inverseMatrix = new double[size, size];

            // Check whether left matrix is the identity

            bool result = true;
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    if (Math.Abs(tempMatrix[i, j] - (i == j ? 1 : 0)) > minPrecision)
                        return false;
                }
            }

            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    inverseMatrix[i, j] = tempMatrix[i, j + size];
                }
            }

            Console.WriteLine("SUCCESS");

            return true;
        }

        /*
        * Given an nXn matrix A, solve n linear equations to find the inverse of A.
        * */
        public static double[,] InvertMatrix(double[,] A) {
            int n = A.GetLength(0);
            //e will represent each column in the identity matrix
            double[] e;
            //x will hold the inverse matrix to be returned
            double[,] x = new double[n,n];
            /*
    * solve will contain the vector solution for the LUP decomposition as we solve
    * for each vector of x.  We will combine the solutions into the double[][] array x.
    * */
            double[] solve;

            //Get the LU matrix and P matrix (as an array)
            Tuple<double[,], int[]> results = LUPDecomposition(A);

            double[,] LU = results.Item1;
            int[] P = results.Item2;

            /*
    * Solve AX = e for each column ei of the identity matrix using LUP decomposition
    * */
            for (int i = 0; i < n; i++) {
                e = new double[n];
                e[i] = 1;
                solve = LUPSolve(LU, P, e);
                for (int j = 0; j < solve.Length; j++) {
                    x[j,i] = solve[j];
                }
            }
            return x;
        }

        /*
* Perform LUP decomposition on a matrix A.
* Return L and U as a single matrix(double[][]) and P as an array of ints.
* We implement the code to compute LU "in place" in the matrix A.
* In order to make some of the calculations more straight forward and to 
* match Cormen's et al. pseudocode the matrix A should have its first row and first columns
* to be all 0.
* */
        public static Tuple<double[,], int[]> LUPDecomposition(double[,] A) {
            int n = A.GetLength(0) - 1;
            /*
    * pi represents the permutation matrix.  We implement it as an array
    * whose value indicates which column the 1 would appear.  We use it to avoid 
    * dividing by zero or small numbers.
    * */
            int[] pi = new int[n + 1];
            double p = 0;
            int kp = 0;
            int pik = 0;
            int pikp = 0;
            double aki = 0;
            double akpi = 0;

            //Initialize the permutation matrix, will be the identity matrix
            for (int j = 0; j <= n; j++) {
                pi[j] = j;
            }

            for (int k = 0; k <= n; k++) {
                /*
        * In finding the permutation matrix p that avoids dividing by zero
        * we take a slightly different approach.  For numerical stability
        * We find the element with the largest 
        * absolute value of those in the current first column (column k).  If all elements in
        * the current first column are zero then the matrix is singluar and throw an
        * error.
        * */
                p = 0;
                for (int i = k; i <= n; i++) {
                    if (Math.Abs(A[i,k]) > p) {
                        p = Math.Abs(A[i,k]);
                        kp = i;
                    }
                }
                if (p == 0) {
                    throw new Exception("singular matrix");
                }
                /*
        * These lines update the pivot array (which represents the pivot matrix)
        * by exchanging pi[k] and pi[kp].
        * */
                pik = pi[k];
                pikp = pi[kp];
                pi[k] = pikp;
                pi[kp] = pik;

                /*
        * Exchange rows k and kpi as determined by the pivot
        * */
                for (int i = 0; i <= n; i++) {
                    aki = A[k,i];
                    akpi = A[kp,i];
                    A[k,i] = akpi;
                    A[kp,i] = aki;
                }

                /*
            * Compute the Schur complement
            * */
                for (int i = k + 1; i <= n; i++) {
                    A[i,k] = A[i,k] / A[k,k];
                    for (int j = k + 1; j <= n; j++) {
                        A[i,j] = A[i,j] - (A[i,k] * A[k,j]); 
                    }
                }
            }
            return Tuple.Create(A, pi);
        }

        /*
* Given L,U,P and b solve for x.
* Input the L and U matrices as a single matrix LU.
* Return the solution as a double[].
* LU will be a n+1xm+1 matrix where the first row and columns are zero.
* This is for ease of computation and consistency with Cormen et al.
* pseudocode.
* The pi array represents the permutation matrix.
* */
        public static double[] LUPSolve(double[,] LU, int[] pi, double[] b) {
            int n = LU.GetLength(0) - 1;
            double[] x = new double[n + 1];
            double[] y = new double[n + 1];
            double suml = 0;
            double sumu = 0;
            double lij = 0;

            /*
    * Solve for y using formward substitution
    * */
            for (int i = 0; i <= n; i++) {
                suml = 0;
                for (int j = 0; j <= i - 1; j++) {
                    /*
            * Since we've taken L and U as a singular matrix as an input
            * the value for L at index i and j will be 1 when i equals j, not LU[i][j], since
            * the diagonal values are all 1 for L.
            * */
                    if (i == j) {
                        lij = 1;
                    } else {
                        lij = LU[i,j];
                    }
                    suml = suml + (lij * y[j]);
                }
                y[i] = b[pi[i]] - suml;
            }
            //Solve for x by using back substitution
            for (int i = n; i >= 0; i--) {
                sumu = 0;
                for (int j = i + 1; j <= n; j++) {
                    sumu = sumu + (LU[i,j] * x[j]);
                }
                x[i] = (y[i] - sumu) / LU[i,i];
            }
            return x;
        }

    }



}

