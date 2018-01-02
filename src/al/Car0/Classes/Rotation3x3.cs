using System;
using System.Collections.Generic;

using System.Text;

namespace Car0
{
    public class Rotation3x3
    {
        #region Public Variables
        public enum RotAxis { RotX, RotY, RotZ } ;
        public double[] rot;
        #endregion
        #region Private Variables
        #endregion
        #region Public Methods
        public Rotation3x3()
        {
            rot = new double[9];
        }
        public Rotation3x3(Transformation a)            //This was trans_rot
        {
            rot = new double[9];
            int i, j;

            for (i = 0; i < 3; ++i)                      /*  For all rows of r.       */
            {
                for (j = 0; j < 3; ++j)                    /*  For all columns of r.    */
                    rot[3 * i + j] = a.mat[4 * i + j];
            }
        }
        public Rotation3x3(RotAxis axis, double theta)
        {
            double st = Math.Sin(theta), ct = Math.Cos(theta);
            rot = new double[9];

            switch (axis)
            {
                case RotAxis.RotX:

                    rot[0] = 1.0;
                    rot[4] = ct;
                    rot[5] = -st;
                    rot[7] = st;
                    rot[8] = ct;
                    break;

                case RotAxis.RotY:

                    rot[0] = ct;
                    rot[2] = st;
                    rot[4] = 1.0;
                    rot[6] = -st;
                    rot[8] = ct;
                    break;

                case RotAxis.RotZ:

                    rot[0] = ct;
                    rot[1] = -st;
                    rot[3] = st;
                    rot[4] = ct;
                    rot[8] = 1.0;

                    break;
            }
        }
        
        /* rmult(b) -     Performs  multiplication c = ab where a,b,& c are
         *                      (3,3) rotational matrices.
         *
         *      Inputs:         b       Pointer to (3x3) rotation matrix b.
         *
         *      Returns:        c       this X b;
         *
         *      Description:
         *              1)      Performs the matrix multiplication:  c = ab,
         *                      where a,b,& c are (3,3) rotational matrices.
         *              2)      Capable of handling instances where some or
         *                      all of a, b, and c point to the same addresses.
         *
         */
        public Rotation3x3 rmult(Rotation3x3 b)
        {
            int i, j, k, index;          /* Indexes.                          */
            Rotation3x3 c = new Rotation3x3();

            for (i = 0; i < 3; ++i)
            {
                for (j = 0; j < 3; ++j)
                {
                    index = 3 * i + j;
                    c.rot[index] = 0.0;     /* Zero the element.                 */

                    /* Set element equal to the sum of the products of the          *
                     * kth  column of a times the kth row in b.                     */

                    for (k = 0; k < 3; ++k)
                        c.rot[index] += rot[3 * i + k] * b.rot[3 * k + j];
                }
            }

            return c;
        }
        /* transp() -       Performs matrix transpose: b = this ^T where a and b are
         *                      (3x3) rotation matrices.
         *
         *      Inputs:         b       Pointer to (3x3) rotation matrix b.
         *
         *      Returns:        b       Elements of this rotation matrix set by
         *                              result of matrix transpose operation: b = this^T.
         *
         *      Description:
         *              1)      Performs matrix transpose operation: b = this^T.
         *
         */
        public Rotation3x3 transp()
        {
            int i, j;
            Rotation3x3 b = new Rotation3x3();

            for (i = 0; i < 3; ++i)
            {
                for (j = 0; j < 3; ++j)
                    b.rot[3 * j + i] = rot[3 * i + j];
            }

            return b;
        }

        /* Scale(factor) -   
         *
         *      Outputs:        this       Elements of a are scaled by factor upon return.
         *
         *      Description:
         *              1)      Scales the elements of a by factor.
         *
         */
        public Rotation3x3 Scale(double factor)
        {
            int i;
            Rotation3x3 r = new Rotation3x3();

            for (i = 0; i < 9; ++i)
                r.rot[i] = rot[i] * factor;

            return r;
        }
        #endregion
        #region Private Methods
        #endregion
    }
}
