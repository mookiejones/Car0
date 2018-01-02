#if WPF
using System.Windows;
    #else
using System.Windows.Forms;
#endif
namespace CarZero
{
    using System;
    using System.Collections.Generic;
   
    internal class Matrix
    {
        public int cols;
        public int rows;
        public List<double> value;

        public Matrix()
        {
            rows = cols = 0;
            value = new List<double>();
        }

        public Matrix(Matrix CloneMe)
        {
            rows = CloneMe.rows;
            cols = CloneMe.cols;
            value = new List<double>(rows * cols);
            for (var i = 0; i < (rows * cols); i++)
            {
                value.Add(CloneMe.value[i]);
            }
        }

        public Matrix(Vector3 v)
        {
            rows = 3;
            cols = 1;
            value = new List<double>(3);
            value.Add(v.x);
            value.Add(v.y);
            value.Add(v.z);
        }

        public Matrix(int NumRows, int NumCols)
        {
            rows = NumRows;
            cols = NumCols;
            value = new List<double>(rows * cols);
            for (var i = 0; i < (rows * cols); i++)
            {
                value.Add(0.0);
            }
        }

        public void assign(int AtRow, int AtCol, double x)
        {
            value[(AtRow * cols) + AtCol] = x;
        }

        public Matrix CrossProduct(Matrix b)
        {
            if (!(((rows.Equals(3) && cols.Equals(1)) && b.rows.Equals(3)) && b.cols.Equals(1)))
            {
                MessageBox.Show("Matrix dimensions wrong", "CrossProduct");
                return null;
            }
            var num = getvalue(0, 0);
            var num2 = getvalue(1, 0);
            var num3 = getvalue(2, 0);
            var num4 = b.getvalue(0, 0);
            var num5 = b.getvalue(1, 0);
            var num6 = b.getvalue(2, 0);
            var matrix = new Matrix(rows, cols);
            matrix.assign(0, 0, (num2 * num6) - (num3 * num5));
            matrix.assign(1, 0, (num3 * num4) - (num * num6));
            matrix.assign(2, 0, (num * num5) - (num2 * num4));
            return matrix;
        }

        public double DotProduct(Matrix b)
        {
            if (!(((rows.Equals(3) && cols.Equals(1)) && b.rows.Equals(3)) && b.cols.Equals(1)))
            {
                MessageBox.Show("Matrix dimensions wrong", "DotProduct");
                return 0.0;
            }
            var num = 0.0;
            for (var i = 0; i < 3; i++)
            {
                num += getvalue(i, 0) * b.getvalue(i, 0);
            }
            return num;
        }

        public void equate(Matrix CloneMe)
        {
            rows = CloneMe.rows;
            cols = CloneMe.cols;
            value = new List<double>(rows * cols);
            for (var i = 0; i < (rows * cols); i++)
            {
                value.Add(CloneMe.value[i]);
            }
        }

        public void equate(Vector3 v)
        {
            if (!(rows.Equals(3) && cols.Equals(1)))
            {
                MessageBox.Show("Vector3 equate matrix not 3x1", "equate");
            }
            else
            {
                assign(0, 0, v.x);
                assign(1, 0, v.y);
                assign(2, 0, v.z);
            }
        }

        public double getvalue(int FromRow, int FromCol)
        {
            return value[(FromRow * cols) + FromCol];
        }

        public Matrix inverse()
        {
            if (!rows.Equals(cols))
            {
                MessageBox.Show("Matrix not square", "inverse");
                return null;
            }
            var y = mident(rows);
            return MatrixSolver.msolve(this, y);
        }

        public Matrix madd(Matrix b)
        {
            var matrix = new Matrix(rows, cols);
            for (var i = 0; i < (rows * cols); i++)
            {
                matrix.value[i] = value[i] + b.value[i];
            }
            return matrix;
        }

        public double magof()
        {
            var d = 0.0;
            for (var i = 0; i < (rows * cols); i++)
            {
                d += Math.Pow(value[i], 2.0);
            }
            return Math.Sqrt(d);
        }

        public Matrix mident(int dimension)
        {
            var matrix = new Matrix(dimension, dimension);
            for (var i = 0; i < dimension; i++)
            {
                for (var j = 0; j < dimension; j++)
                {
                    if (i.Equals(j))
                    {
                        matrix.assign(i, j, 1.0);
                    }
                }
            }
            return matrix;
        }

        public Matrix mmult(Matrix b)
        {
            var matrix = new Matrix(rows, b.cols);
            if (cols.Equals(b.rows))
            {
                for (var i = 0; i < matrix.rows; i++)
                {
                    for (var j = 0; j < matrix.cols; j++)
                    {
                        var num4 = (i * matrix.cols) + j;
                        matrix.value[num4] = 0.0;
                        for (var k = 0; k < cols; k++)
                        {
                            List<double> list;
                            int num5;
                            (list = matrix.value)[num5 = num4] = list[num5] + (value[(i * cols) + k] * b.value[(k * b.cols) + j]);
                        }
                    }
                }
                return matrix;
            }
            MessageBox.Show("Matrix dimension error", "mmult");
            return matrix;
        }

        public Matrix msub(Matrix b)
        {
            var matrix = new Matrix(rows, cols);
            for (var i = 0; i < (rows * cols); i++)
            {
                matrix.value[i] = value[i] - b.value[i];
            }
            return matrix;
        }

        public void Normalize()
        {
            var num = magof();
            if (num < 1E-10)
            {
                MessageBox.Show("Matrix too small", "Normalize");
            }
            else
            {
                scale(1.0 / num);
            }
        }

        public void Normalize(double Scaler)
        {
            var num = magof();
            if (num < 1E-10)
            {
                MessageBox.Show("Matrix too small", "Normalize");
            }
            else
            {
                scale(Scaler / num);
            }
        }

        public void scale(double factor)
        {
            for (var i = 0; i < (rows * cols); i++)
            {
                List<double> list;
                int num2;
                (list = value)[num2 = i] = list[num2] * factor;
            }
        }

        public Matrix trans()
        {
            var matrix = new Matrix(cols, rows);
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    matrix.value[(j * matrix.cols) + i] = value[(i * cols) + j];
                }
            }
            return matrix;
        }
    }
}

