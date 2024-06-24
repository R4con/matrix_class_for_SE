public class myMatrix2D : IEquatable<myMatrix2D>
{
    // if this does not work, maybe use this? https://stackoverflow.com/questions/46836908/how-to-invert-double-in-c-sharp
    private double[][] matrix;
    private int columns; // x
    private int rows;   // y
                        // [y,x] y=row, x=column

    public myMatrix2D Copy()
    {
        myMatrix2D other = new myMatrix2D(this.columns, this.rows);
        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < this.columns; j++)
            {
                other.set(j, i, this.matrix[i][j]);
            }
        }
        return other;
    }

    public myMatrix2D(int columns, int rows)
    {
        this.columns = columns;
        this.rows = rows;
        this.matrix = new double[rows][];
        for (int i = 0; i < rows; i++)
        {
            this.matrix[i] = new double[columns];
        }
    }

    public myMatrix2D(double[,] start_matrix)
    {
        this.columns = start_matrix.GetLength(1);
        this.rows = start_matrix.GetLength(0);

        double[][] jagged = new double[this.rows][];

        for (int i = 0; i < this.rows; i++)
        {
            double[] row = new double[this.columns];
            for (int j = 0; j < this.columns; j++)
            {
                row[j] = start_matrix[i, j];
            }
            jagged[i] = row;
        }
        this.matrix = jagged;
    }

    public myMatrix2D(double[][] start_matrix)
    {
        this.columns = start_matrix[0].Length;
        this.rows = start_matrix.Length;
        this.matrix = start_matrix;
    }

    public void set(int x, int y, double value) // ! be careful to not use these the other way around !
    {
        if (x < 0 || x >= columns || y < 0 || y >= rows)
        {
            throw new ArgumentException("wrong dimension in x or y");
        }

        this.matrix[y][x] = value;
    }

    public double get(int x, int y) // ! be careful to not use these the other way around !
    {
        if (x < 0 || x >= this.columns || y < 0 || y >= this.rows)
        {
            throw new ArgumentException("wrong dimension in x or y");
        }

        return this.matrix[y][x];
    }

    public int getRows()
    {
        return this.rows;
    }

    public int getColumns()
    {
        return this.columns;
    }

    public string String()
    {
        string matrix_string = "";

        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < this.columns; j++)
            {
                if (j != 0)
                    matrix_string += " : ";

                matrix_string += this.matrix[i][j];
            }
            matrix_string += "\n";
        }

        return matrix_string;
    }

    public myMatrix2D T()
    {
        myMatrix2D matrixT = new myMatrix2D(this.rows, this.columns);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                matrixT.set(i, j, this.get(j, i));
            }
        }

        return matrixT;
    }

    public myMatrix2D append(myMatrix2D matrix_B)
    {
        if (matrix_B.columns != this.columns) // columns is egal, da ja beide Matrizen zusammen gefühgt werden sollen.
        {
            throw new ArgumentException("column number is not equal");
        }

        int new_cols = this.columns + matrix_B.columns;
        int new_rows = this.rows;

        myMatrix2D new_matrix = new myMatrix2D(new_cols, new_rows);


        for (int i = 0; i < new_rows; i++)
        {
            for (int j = 0; j < new_cols; j++)
            {
                if (j < this.columns)
                {
                    new_matrix.set(j, i, this.get(j, i));
                }
                else
                {
                    new_matrix.set(j, i, matrix_B.get(j - this.columns, i));
                }
            }
        }
        return new_matrix;
    }

    public void swapRows(int index1, int index2)
    {
        // this will not be pretty. And probably completly useless.
        // TODO rewrite everything to use "Jagged arrays"

        if (index1 == index2) return;

        if (index1 < 0 || index1 >= this.rows || index2 < 0 || index2 >= this.rows)
        {
            throw new ArgumentException("index out of range");
        }

        for (int i = 0; i < this.columns; i++)
        {
            double tmp;
            tmp = this.matrix[index1][i];
            this.matrix[index1][i] = this.matrix[index2][i];
            this.matrix[index2][i] = tmp;
        }
    }

    public static double[] solve(myMatrix2D LUMatrix, double[] bVector)
    {
        double[] yVector = myMatrix2D.forwardSubstitution(LUMatrix, bVector);   //Ly=b
        double[] xVector = myMatrix2D.backSubstitution(LUMatrix, yVector);      //Ux=y

        return xVector;
    }

    public static double[] solveLstsq(myMatrix2D aMatrix, double[] bVector)
    {
        double[] xVector;

        if (aMatrix.getRows() < aMatrix.getColumns())   // System is underdetermined => infinitly many solution
        {
            // find Inverse with LU: https://www.gamedev.net/tutorials/programming/math-and-physics/matrix-inversion-using-lu-decomposition-r3637/
            //b = A.T() * (A * A.T()) ^ -1 * yVec

            xVector = (aMatrix.T() * (aMatrix * aMatrix.T()).inverse()) * bVector;
        }
        else
        {
            // I have an underdetermined System, this should handly both other cases.
            // this might not work

            myMatrix2D ATA = aMatrix.T() * aMatrix;
            double[] ATb = aMatrix.T() * bVector;

            //Console.Write(ATA.String());
            myMatrix2D LUMatrix = LUFactorization(ATA, ref ATb);
            double[] yVector = myMatrix2D.forwardSubstitution(LUMatrix, ATb);
            xVector = myMatrix2D.backSubstitution(LUMatrix, yVector);
        }

        return xVector;
    }

    public static myMatrix2D LUFactorization(myMatrix2D InputMatrix, ref double[] bVector)
    {
        // THIS WORKS !

        // look at wikipedia: https://en.wikipedia.org/wiki/LU_decomposition
        // also this is helpful? https://www.baeldung.com/cs/solving-system-linear-equations
        // LU decomposition is from https://stackoverflow.com/questions/28441509/how-to-implement-lu-decomposition-with-partial-pivoting-in-python
        if (InputMatrix.rows != InputMatrix.columns)
        {
            throw new ArgumentException("provided matrix is not of rank NxN");
        }

        myMatrix2D aMatrix = InputMatrix.Copy();
        int n = aMatrix.rows;   // matrix should be NxN

        for (int i = 0; i < n; i++)
        {
            //Console.WriteLine(i + ": --");
            //Console.WriteLine(aMatrix.String());
            //Console.WriteLine(UMatrix.String());
            //Console.WriteLine(LMatrix.String());
            //Console.WriteLine(string.Join(", ", bVector));

            //partial pivoting: swap rows, so that the largest element is at (i,i)
            //TODO Instead of searching for the maximum value in the entire column for each iteration, you should search only from the current row to the end.
            double[] tmpRow;
            int indexOfMax;
            if (i == 0)
            {
                tmpRow = aMatrix.T().matrix[i];
                indexOfMax = Array.IndexOf(tmpRow, tmpRow.Select(Math.Abs).Max());
                if (indexOfMax == -1)
                {
                    indexOfMax = Array.IndexOf(tmpRow, -tmpRow.Select(Math.Abs).Max());
                }
            }
            else
            {
                tmpRow = aMatrix.T().matrix[i].Skip(i).ToArray();
                indexOfMax = Array.IndexOf(tmpRow, tmpRow.Select(Math.Abs).Max());
                if (indexOfMax == -1)
                {
                    indexOfMax = Array.IndexOf(tmpRow, -tmpRow.Select(Math.Abs).Max());
                }
                indexOfMax += i;
            }

            aMatrix.swapRows(i, indexOfMax);

            // swap bVector
            double tmp = bVector[i];
            bVector[i] = bVector[indexOfMax];
            bVector[indexOfMax] = tmp;
            // partial pivoting end
            // */

            for (int j = i + 1; j < n; j++)
            {
                aMatrix.set(i, j, aMatrix.get(i, j) / aMatrix.get(i, i));
                for (int k = i + 1; k < n; k++)
                {
                    aMatrix.set(k, j, aMatrix.get(k, j) - aMatrix.get(i, j) * aMatrix.get(k, i));
                }
            }
        }

        return aMatrix;    // LUMatrix = L+U-I
    }

    public static double[] backSubstitution(myMatrix2D InputUMatrix, double[] cVector)
    {
        // from Wikipedia:
        double sum = 0;
        int n = InputUMatrix.getColumns();
        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            sum = 0;
            for (int k = i + 1; k < n; k++)
                sum += InputUMatrix.get(k, i) * x[k];
            x[i] = (1 / InputUMatrix.get(i, i)) * (cVector[i] - sum);
        }
        return x;
    }

    public static double[] forwardSubstitution(myMatrix2D InputLMatrix, double[] bVector)
    {
        //from Wikipedia
        double sum = 0;
        int n = InputLMatrix.getColumns();
        double[] y = new double[n];
        for (int i = 0; i < n; i++)
        {
            sum = 0;
            for (int k = 0; k < i; k++)
                sum += InputLMatrix.get(k, i) * y[k];
            y[i] = bVector[i] - sum;
        }

        return y;
    }

    public myMatrix2D inverse()
    {
        //https://www.gamedev.net/tutorials/programming/math-and-physics/matrix-inversion-using-lu-decomposition-r3637/
        myMatrix2D inverseMatrix = new myMatrix2D(this.columns, this.rows);

        for (int column = 0; column < inverseMatrix.columns; column++)
        {
            myMatrix2D thisCopy = this.Copy();
            double[] iVec = new double[inverseMatrix.rows];
            iVec[column] = 1;

            myMatrix2D LUMatrix = myMatrix2D.LUFactorization(thisCopy, ref iVec);
            double[] tmpCol = myMatrix2D.solve(LUMatrix, iVec);


            for (int row = 0; row < inverseMatrix.rows; row++)
            {
                inverseMatrix.set(column, row, tmpCol[row]);
            }
        }

        return inverseMatrix;
    }

    public myMatrix2D round(int dec)
    {
        myMatrix2D retMatrix = this.Copy();

        for (int i = 0; i < retMatrix.columns; i++)
        {
            for (int j = 0; j < retMatrix.rows; j++)
            {
                retMatrix.set(i, j, Math.Round(this.get(i, j), dec));
            }
        }
        return retMatrix;
    }

    public static myMatrix2D operator *(myMatrix2D A, myMatrix2D B)
    {
        myMatrix2D result = new myMatrix2D(B.columns, A.rows);
        for (int a = 0; a < result.rows; a++)
        {
            for (int b = 0; b < result.columns; b++)
            {
                double sum = 0;

                for (int i = 0; i < A.columns; i++)
                {
                    sum += A.get(i, a) * B.get(b, i);
                }
                result.set(b, a, sum);
            }
        }

        return result;
    }

    public static double[] operator *(myMatrix2D A, double[] b)
    {
        if (A.columns != b.Length)
        {
            throw new ArgumentException("wrong dimension. A.columns has to be == b.Length");
        }

        double[] result = new double[A.rows];
        for (int a = 0; a < A.rows; a++)
        {
            double sum = 0;

            for (int i = 0; i < A.columns; i++)
            {
                sum += A.get(i, a) * b[i];
            }
            result[a] = sum;
        }

        return result;
    }

    public static myMatrix2D operator +(myMatrix2D A, myMatrix2D B)
    {
        if (A.rows != B.rows || A.columns != B.columns)
        {
            throw new ArgumentException("A and B do not have the same dimension");
        }

        myMatrix2D result = new myMatrix2D(A.columns, A.rows);
        for (int a = 0; a < result.rows; a++)
        {
            for (int b = 0; b < result.columns; b++)
            {
                result.set(b, a, A.get(b, a) + B.get(b, a));
            }
        }

        return result;
    }

    // implement Equals Stuff
    public bool Equals(myMatrix2D otherMatrix)
    {
        const double dV = 0.001;
        if (this.columns != otherMatrix.columns || this.rows != otherMatrix.rows) { return false; } //Console.WriteLine("unequal ranks");

            for (int x = 0; x < this.columns; x++)
        {
            for (int y = 0; y < this.rows; y++)
            {
                double tmp = Math.Abs(this.get(x, y) - otherMatrix.get(x, y));
                if (tmp > dV)
                {
                    //Console.WriteLine("dV: " + tmp + "  x: " + x + " y: " + y);
                    //Console.WriteLine("this " + this.get(x, y) + " other: " + otherMatrix.get(x, y));
                    return false;
                }
            }
        }

        return true;
    }

    public override bool Equals(object other)
    {
        if (other is myMatrix2D)
            return Equals((myMatrix2D)other);
        else
            return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(myMatrix2D matrix1, myMatrix2D matrix2)
    {
        return matrix1.Equals(matrix2);
    }

    public static bool operator !=(myMatrix2D matrix1, myMatrix2D matrix2)
    {
        return !matrix1.Equals(matrix2);
    }
}
