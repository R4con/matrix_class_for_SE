using TestProject;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        const double dV = 0.001;

        [TestMethod]
        public void basicMathTest()
        {
            myMatrix2D M1 = new myMatrix2D(new double[,] { { 1, 0, 3 }, { -4, 5, 6 }, { 7, -8, 9 } });
            myMatrix2D Solution = new myMatrix2D(new double[,] { { 22, -24, 30 }, { 18, -23, 72 }, { 102, -112, 54 } });
            myMatrix2D Result = M1 * M1;

            Assert.AreEqual(M1, M1, "Matrix Equals Failed!");

            Console.WriteLine(Result.String());
            Assert.AreEqual(Result, Solution, "Matrix multiplication failed!");

            Result = M1 + M1;
            Solution = new myMatrix2D(new double[,] { { 2, 0, 6 }, { -8, 10, 12 }, { 14, -16, 18 } });
            Console.WriteLine(Result.String());
            Assert.AreEqual(Result, Solution, "Matrix addition failed!");

            Result = M1.T();
            Solution = new myMatrix2D(new double[,] { { 1, -4, 7 }, { 0, 5, -8 }, { 3, 6, 9 } });
            Console.WriteLine(Result.String());
            Assert.AreEqual(Result, Solution, "Matrix transposition failed!");

            Result = M1;
            Result.swapRows(0, 1);
            Solution = new myMatrix2D(new double[,] { { -4, 5, 6 }, { 1, 0, 3 }, { 7, -8, 9 } });
            Assert.AreEqual(Result, Solution, "Matrix swapRows failed!");
        }

        [TestMethod]
        public void matrixVectorMultiply()
        {
            myMatrix2D M1 = new myMatrix2D(new double[,] { { 1, 0, 3 }, { -4, 5, 6 }, { 7, -8, 9 } });
            double[] V1 = new double[] { 5, 0, -1 };

            double[] Result = M1 * V1;
            double[] Solution = new double[] { 2, -26, 26 };
            Console.WriteLine(string.Join(", ", Result));

            for (int i = 0; i < V1.Length; i++)
                Assert.AreEqual(Result[i], Solution[i], 0.0001, "Matrix - vector multiplication failed!");

        }

        [TestMethod]
        public void luDecomposition()
        {
            myMatrix2D M1 = new myMatrix2D(new double[,] { { 1, 0, 3 }, { -4, 5, 6 }, { 7, -8, 9 } });
            double[] V1 = new double[3] { 0, 0, 0 };

            myMatrix2D Result = myMatrix2D.LUFactorization(M1, ref V1);
            myMatrix2D Solution = new myMatrix2D(new double[,] { { 7, -8, 9 }, { 1.0 / 7, 8.0 / 7, 12.0 / 7 }, { -4.0 / 7, 3.0 / 8, 21.0 / 2 } });
            Console.WriteLine(Result.String());
            Assert.AreEqual(Result, Solution, "LU-Decomposition failed!");
        }

        public void luDecompositionNew(myMatrix2D M1)
        {
            double[] V1 = [1, 2, 3];
            myMatrix2D Result = myMatrix2D.LUFactorization(M1, ref V1);

            // Extract the L matrix from the LU decomposition
            myMatrix2D L = new myMatrix2D(Result.getRows(), Result.getColumns());
            for (int i = 0; i < L.getRows(); i++)
            {
                for (int j = 0; j < L.getColumns(); j++)
                {
                    if (i > j)
                    {
                        L.set(j, i, Result.get(j, i));
                    }
                    else if (i == j)
                    {
                        L.set(j, i, 1);
                    }
                    else
                    {
                        L.set(j, i, 0);
                    }
                }
            }

            // Extract the U matrix from the LU decomposition
            myMatrix2D U = new myMatrix2D(Result.getRows(), Result.getColumns());
            for (int i = 0; i < U.getRows(); i++)
            {
                for (int j = 0; j < U.getColumns(); j++)
                {
                    if (i <= j)
                    {
                        U.set(j, i, Result.get(j, i));
                    }
                    else
                    {
                        U.set(j, i, 0);
                    }
                }
            }

            // swap Rows, so it matches the original
            myMatrix2D Product = L * U;

            int index1 = Array.IndexOf(V1, 1);
            Product.swapRows(0, index1);
            V1[index1] = V1[0];
            V1[0] = 1;
            Product.swapRows(1, Array.IndexOf(V1, 2));

            Console.WriteLine("Original Matrix:");
            Console.WriteLine(M1.String());
            Console.WriteLine("L Matrix:");
            Console.WriteLine(L.String());
            Console.WriteLine("U Matrix:");
            Console.WriteLine(U.String());
            Console.WriteLine("Product of L and U:");
            Console.WriteLine(Product.String());

            Assert.AreEqual(M1, Product, "LU decomposition is incorrect");
        }

        [TestMethod]
        public void randomLUDecomp()
        {
            Random rnd = new Random();
            myMatrix2D M1 = new myMatrix2D(3, 3);
            for (int count = 0; count < 20; count++)
            {
                for (int i = 0; i < M1.getColumns(); i++)
                {
                    for (int j = 0; j < M1.getRows(); j++)
                    {
                        M1.set(i, j, rnd.Next(-100, 100));
                    }
                }
                luDecompositionNew(M1);
            }
        }

        [TestMethod]
        public void forwardSubs()
        {
            myMatrix2D LUMatrix = new myMatrix2D(new double[,] { { 25, 5, 1 }, { 2.56, -4.8, -1.56 }, { 5.76, 3.5, 0.7 } });
            double[] C = new double[3] { 106.8, 177.2, 279.2 };

            double[] Result = myMatrix2D.forwardSubstitution(LUMatrix, C);
            double[] Solution = [106.8, -96.208, 0.76];
            Console.Write(string.Join(", ", Result));

            for (int i = 0; i < Result.Length; i++)
                Assert.AreEqual(Result[i], Solution[i], 1e-6, "forward Substituion failed!");
        }

        [TestMethod]
        public void backSubs()
        {
            myMatrix2D LUMatrix = new myMatrix2D(new double[,] { { 25, 5, 1 }, { 2.56, -4.8, -1.56 }, { 5.76, 3.5, 0.7 } });
            double[] Z = new double[3] { 106.8, -96.208, 0.76 };

            double[] Result = myMatrix2D.backSubstitution(LUMatrix, Z);
            double[] Solution = [0.29048, 19.691, 1.0857];
            Console.Write(string.Join(", ", Result));

            for (int i = 0; i < Result.Length; i++)
                Assert.AreEqual(Result[i], Solution[i], 1e-3, "back Substituion failed!");
        }

        [TestMethod]
        public void solveEquation()
        {
            myMatrix2D M1 = new myMatrix2D(new double[,] { { 3.0, -0.1, -0.2 }, { 0.1, 7.0, -0.3 }, { 0.3, -0.2, 10.0 } });
            double[] V1 = new double[3] { 7.85, -19.3, 71.4 };

            myMatrix2D LU = myMatrix2D.LUFactorization(M1, ref V1);
            double[] Result = new double[3];
            Result = myMatrix2D.solve(LU, V1);

            double[] Solution = new double[3] { 3, -2.5, 7 };
            Console.WriteLine(string.Join(", ", Result));

            for (int i = 0; i < Result.Length; i++)
                Assert.AreEqual(Result[i], Solution[i], 1e-6, "Equation solving failed!");
        }

        [TestMethod]
        public void matrixInversion()
        {
            myMatrix2D M1 = new myMatrix2D(new double[,] { { 1.0, 0.0, 3.0 }, { -4.0, 5.0, 6.0 }, { 7.0, -8.0, 9.0 } });

            myMatrix2D Result = M1.inverse();
            myMatrix2D Solution = new myMatrix2D(new double[,] { { 31.0 / 28, -2.0 / 7, -5.0 / 28 }, { 13.0 / 14, -1.0 / 7, -3.0 / 14 }, { -1.0 / 28, 2.0 / 21, 5.0 / 84 } });
            Console.WriteLine(Result.String());
            Console.WriteLine((M1 * Result).round(6).String());
            Assert.AreEqual(Result, Solution, "LU-Decomposition failed!");
        }

        [TestMethod]
        public void solveLstSq()
        {
            myMatrix2D M1 = new myMatrix2D(new double[,] { { 3, -1, -10, 1, -2 }, { 3, 3, -4, -1, -4 }, { 20, 20, 20, 20, 20 } });
            double[] V1 = new double[3] { 0, 0, 50 };

            Console.WriteLine("Rows: " + M1.getRows() + " Columns: " + M1.getColumns());

            double[] Result = myMatrix2D.solveLstsq(M1, V1);

            double[] Solution = [0.686, 0.455, 0.113, 0.672, 0.575];
            double[] printArr = new double[Solution.Length];
            for (int i = 0; i < Result.Length; i++)
            {
                printArr[i] = Math.Round(Result[i], 3);
            }
            Console.WriteLine("[ " + string.Join(", ", printArr) + " ]");

            for (int i = 0; i < Solution.Length; i++)
                Assert.AreEqual(printArr[i], Solution[i], 1e-6, "LstSq solving failed!");
        }

        [TestMethod]
        public void TMPsolveLstSq()
        {
            //myMatrix2D M1 = new myMatrix2D(new double[,] { { 5, 0, 0, 0, -5, 0 }, { 0, -5, 1, 1, 0, 5 }, { -1, -1, 3, -3, -1, -1 }, { 20, 20, 20, 20, 20, 20 } });
            myMatrix2D M1 = new myMatrix2D(new double[,] {{ 1, -1, 0, 0, 0},{ 0, 0, 2, -4, 0},{ 0,0,0,0,1},{ 20, 20, 20, 20, 20} });
            //double[] V1 = new double[4] { -0.0062, -3.018, 13.374, 50 };
            double[] V1 = new double[4] { 0, 0, 0, 50000 };

            Console.WriteLine("Rows: " + M1.getRows() + " Columns: " + M1.getColumns());

            double[] Result = myMatrix2D.solveLstsq(M1, V1);

            double[] Solution = [0.686, 0.455, 0.113, 0.672, 0.575, 0.123];
            double[] printArr = new double[Solution.Length];
            for (int i = 0; i < Result.Length; i++)
            {
                printArr[i] = Math.Round(Result[i], 3);
            }
            Console.WriteLine("[ " + string.Join(", ", printArr) + " ]");

            for (int i = 0; i < Solution.Length; i++)
                Assert.AreEqual(printArr[i], Solution[i], 1e-6, "LstSq solving failed!");
        }
    }
}