using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Forms;

namespace Car0
{
     class MatrixSolver
    {
         public event NotifyMessageEventHandler NotifyMessage;
         private void raiseNotify(string message, string title)
         {
             if (NotifyMessage!=null)
                 NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
         }

         #region Public Methods

        public Matrix msolve(Matrix mx, Matrix my)
        {
            //Give message for misuse
            if (!mx.Rows.Equals(mx.Cols))
            {
                raiseNotify("Matrix A is not square", "msolve");
                return null;
            }

            if (!my.Rows.Equals(my.Cols))
            {
                raiseNotify("Matrix Y is not square", "msolve");
                return null;
            }

            if (!mx.Rows.Equals(my.Cols))
            {
                raiseNotify("Matrix A and Y not of same dimension", "msolve");
                return null;
            }

            Matrix lu = new Matrix(mx.Cols, mx.Cols);
            Matrix x = new Matrix(mx.Cols, mx.Cols);

            MatrixMap map = new MatrixMap(x.Cols);

            //LU decompose here
            if (lu_decomp(ref x,ref lu, ref map))
            {
                int i;

                for (i = 0; i < x.Cols; ++i)
                {
                    forward_sub(ref mx, x, lu, my, map, i);
                    reverse_sub(ref mx, x, lu, map, i);
                }
            }

            return x;
        }

        //Begin code originally in lu_decomp

        public  Boolean lu_decomp(ref Matrix a, ref Matrix lu, ref MatrixMap map)
        {
            int j;
            for (int i = 0; i < a.Cols; ++i)
            {
                for ( j = i; j < a.Cols; ++j)
                    lu.Value[Index(map, j, i, a.Cols)] = pivot(lu, a, map, j, i, i);

                int  p = find_pivot_row(lu, map, i, a.Cols);
                swap_row(ref map, i, p);

                double u_ii = lu.Value[Index(map, i, i, a.Cols)];

                //Check for underflow --> singular matrix.
                if (Math.Abs(u_ii) < 1.0e-10)
                {
                    raiseNotify("Singular matrix encountered", "lu_decomp");
                    return false;
                }

                scale_col(ref lu, map, i, a.Cols, u_ii);

                for (j = i + 1; j < a.Cols; ++j)
                    lu.Value[Index(map, i, j, a.Cols)] = pivot(lu, a, map, i, j, i);

            }
            return true;
        }

        //End code originally in lu_decomp
        #endregion
        #region Private Methods
        private int m_index(MatrixMap map, int i, int j, int n)
        {
            return map.mvalue[i] * n + j;
        }


        private  void forward_sub(ref Matrix pt_x, Matrix pt_a, Matrix zu, Matrix y, MatrixMap m, int col)
        {
            double temp = 0.0;

            for (int i = 0; i < pt_a.Cols; ++i)
            {
                temp = y.Value[m_index(m, i, col, pt_x.Cols)];

                for (int j = 0; j < i; ++j)
                    temp -= (zu.Value[m_index(m, i, j, pt_a.Cols)] * pt_x.Value[j * pt_x.Cols + col]);

                pt_x.Value[i * pt_x.Cols + col] = temp;
            }
        }

        private  void reverse_sub(ref Matrix pt_x, Matrix pt_a, Matrix zu, MatrixMap m, int col)
        {
            double temp = 0.0;

            for (int i = pt_a.Cols - 1; i >= 0; --i)
            {
                temp = pt_x.Value[i * pt_x.Cols + col];

                for (int j = i + 1; j < pt_a.Cols; ++j)
                    temp -= (zu.Value[m_index(m, i, j, pt_a.Cols)] * pt_x.Value[j * pt_x.Cols + col]);

                pt_x.Value[i * pt_x.Cols + col] = temp / zu.Value[m_index(m, i, i, pt_a.Cols)];
            }
        }

        //Start private methods originally in lu_decomp.c

        private  int Index(MatrixMap map, int i, int j, int n)          //Note, added the n eliminating the lidmat global
        {
            return map.mvalue[i] * n + j;
        }

        private  double pivot(Matrix lu, Matrix pt_a, MatrixMap m, int i, int j, int limit)        
        {
            double p = pt_a.Value[Index(m, i, j, pt_a.Cols)];

            for (int k = 0; k < limit; ++k)
                p -= (lu.Value[Index(m, i, k, pt_a.Cols)] * lu.Value[Index(m, k, j, pt_a.Cols)]);

            return p;
        }

        private  int find_pivot_row(Matrix lu, MatrixMap m, int col, int n)          //Note, added the n eliminating the lidmat global
        {
            int p = col, j;           /* Index counters.      */
            double test = 0.0, temp = 0.0;

            test = Math.Abs(lu.Value[Index(m, col, col, n)]);

            for (j = col + 1; j < n; ++j)
            {
                temp = Math.Abs(lu.Value[Index(m, j, col, n)]);

                if (temp > test)
                {
                    test = temp;
                    p = j;
                }
            }

            return p;
        }

        private  void swap_row(ref MatrixMap map, int j, int k)
        {
            int temp = map.mvalue[j];
            map.mvalue[j] = map.mvalue[k];
            map.mvalue[k] = temp;
        }

        private  void scale_col(ref Matrix lu, MatrixMap map, int col, int n, double factor)        //Note, added the n eliminating the lidmat global
        {
            for (int k = col + 1; k < n; ++k)
                lu.Value[Index(map, k, col, n)] /= factor;
        }


        //End private methods originally in lu_decomp.c

        #endregion
    }
}
