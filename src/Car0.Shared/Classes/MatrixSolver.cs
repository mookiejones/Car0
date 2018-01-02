#if WPF
using System.Windows;
    #else
using System.Windows.Forms;
#endif

namespace CarZero
{
    using System;
    using System.Collections.Generic;

    internal class MatrixSolver
    {
        private static int find_pivot_row(Matrix lu, MatrixMap m, int col, int n)
        {
            var num = col;
            var num3 = 0.0;
            var num4 = 0.0;
            num3 = Math.Abs(lu.value[Index(m, col, col, n)]);
            for (var i = col + 1; i < n; i++)
            {
                num4 = Math.Abs(lu.value[Index(m, i, col, n)]);
                if (num4 > num3)
                {
                    num3 = num4;
                    num = i;
                }
            }
            return num;
        }

        private static void forward_sub(ref Matrix pt_x, Matrix pt_a, Matrix zu, Matrix y, MatrixMap m, int col)
        {
            var num3 = 0.0;
            for (var i = 0; i < pt_a.cols; i++)
            {
                num3 = y.value[m_index(m, i, col, pt_x.cols)];
                for (var j = 0; j < i; j++)
                {
                    num3 -= zu.value[m_index(m, i, j, pt_a.cols)] * pt_x.value[(j * pt_x.cols) + col];
                }
                pt_x.value[(i * pt_x.cols) + col] = num3;
            }
        }

        private static int Index(MatrixMap m, int i, int j, int n)
        {
            return ((m.mvalue[i] * n) + j);
        }

        public static bool lu_decomp(ref Matrix a, ref Matrix lu, ref MatrixMap map)
        {
            var num4 = 0.0;
            for (var i = 0; i < a.cols; i++)
            {
                var num2 = i;
                while (num2 < a.cols)
                {
                    lu.value[Index(map, num2, i, a.cols)] = pivot(lu, a, map, num2, i, i);
                    num2++;
                }
                var k = find_pivot_row(lu, map, i, a.cols);
                swap_row(ref map, i, k);
                num4 = lu.value[Index(map, i, i, a.cols)];
                if (Math.Abs(num4) < 1E-10)
                {
                    MessageBox.Show("Singular matrix encountered", "lu_decomp");
                    return false;
                }
                scale_col(ref lu, map, i, a.cols, num4);
                for (num2 = i + 1; num2 < a.cols; num2++)
                {
                    lu.value[Index(map, i, num2, a.cols)] = pivot(lu, a, map, i, num2, i);
                }
            }
            return true;
        }

        private static int m_index(MatrixMap m, int i, int j, int n)
        {
            return ((m.mvalue[i] * n) + j);
        }

        public static Matrix msolve(Matrix a, Matrix y)
        {
            if (!a.rows.Equals(a.cols))
            {
                MessageBox.Show("Matrix A is not square", "msolve");
                return null;
            }
            if (!y.rows.Equals(y.cols))
            {
                MessageBox.Show("Matrix Y is not square", "msolve");
                return null;
            }
            if (!a.rows.Equals(y.cols))
            {
                MessageBox.Show("Matrix A and Y not of same dimension", "msolve");
                return null;
            }
            var lu = new Matrix(a.cols, a.cols);
            var matrix2 = new Matrix(a.cols, a.cols);
            var map = new MatrixMap(a.cols);
            if (lu_decomp(ref a, ref lu, ref map))
            {
                for (var i = 0; i < matrix2.cols; i++)
                {
                    forward_sub(ref matrix2, a, lu, y, map, i);
                    reverse_sub(ref matrix2, a, lu, map, i);
                }
            }
            return matrix2;
        }

        private static double pivot(Matrix lu, Matrix pt_a, MatrixMap m, int i, int j, int limit)
        {
            var num2 = pt_a.value[Index(m, i, j, pt_a.cols)];
            for (var k = 0; k < limit; k++)
            {
                num2 -= lu.value[Index(m, i, k, pt_a.cols)] * lu.value[Index(m, k, j, pt_a.cols)];
            }
            return num2;
        }

        private static void reverse_sub(ref Matrix pt_x, Matrix pt_a, Matrix zu, MatrixMap m, int col)
        {
            var num3 = 0.0;
            for (var i = pt_a.cols - 1; i >= 0; i--)
            {
                num3 = pt_x.value[(i * pt_x.cols) + col];
                for (var j = i + 1; j < pt_a.cols; j++)
                {
                    num3 -= zu.value[m_index(m, i, j, pt_a.cols)] * pt_x.value[(j * pt_x.cols) + col];
                }
                pt_x.value[(i * pt_x.cols) + col] = num3 / zu.value[m_index(m, i, i, pt_a.cols)];
            }
        }

        private static void scale_col(ref Matrix lu, MatrixMap m, int col, int n, double factor)
        {
            for (var i = col + 1; i < n; i++)
            {
                List<double> list;
                int num2;
                (list = lu.value)[num2 = Index(m, i, col, n)] = list[num2] / factor;
            }
        }

        private static void swap_row(ref MatrixMap m, int j, int k)
        {
            var num = m.mvalue[j];
            m.mvalue[j] = m.mvalue[k];
            m.mvalue[k] = num;
        }
    }
}

