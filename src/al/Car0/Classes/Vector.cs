using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Car0
{
    public class Vector
    {
        #region Public Variables
        public List<double> Vec;
        #endregion
        #region Private Variables
        #endregion
        #region Public Methods
        public Vector(int Dim)
        {
            int i;
            Vec = new List<double>(Dim);

            for (i = 0; i < Dim; ++i)
                Vec.Add(0.0);
        }
        public Vector(Vector vector)
        {
            Vec = new List<double>(vector.Vec.Count);
            int i;

            for (i = 0; i < vector.Vec.Count; ++i)
                Vec.Add(vector.Vec[i]);
        }
        public Vector(Vector3 vector)
        {
            Vec = new List<double>(3);

            Vec.Add(vector.x);
            Vec.Add(vector.y);
            Vec.Add(vector.z);
        }
        public Vector(Transformation transform)                 //Replaces trans_disp
        {
            Vec = new List<double>(3);
            int j;

            for (j = 0; j < 3; ++j)              /*  For all columns of d.              */
                Vec.Add(transform.mat[4 * j + 3]);
        }

        //Returns this + b
        public Vector Add(Vector vector)
        {
            int i;
            Vector c = new Vector(Vec.Count);

            for (i = 0; i < Vec.Count; ++i)
                c.Vec[i] = Vec[i] + vector.Vec[i];

            return c;
        }

        //Returns this - b
        public Vector Sub(Vector vector)
        {
            int i;
            Vector c = new Vector(Vec.Count);

            for (i = 0; i < Vec.Count; ++i)
                c.Vec[i] = Vec[i] - vector.Vec[i];

            return c;
        }

        public Vector Max(Vector vector)
        {
            Vector c = new Vector(Vec.Count);

            if (Vec.Count.Equals(vector.Vec.Count))
            {
                int i;

                for (i = 0; i < Vec.Count; ++i)
                    c.Vec[i] = Math.Max(Vec[i], vector.Vec[i]);
            }
            else
                throw new VectorException("Vector dimension mismatch");

//                raiseNotify("Vector dimension mismatch", "In Vector.Max");

            return c;
        }

        public Vector Min(Vector vector)
        {
            Vector c = new Vector(Vec.Count);

            if (Vec.Count.Equals(vector.Vec.Count))
            {
                int i;

                for (i = 0; i < Vec.Count; ++i)
                    c.Vec[i] = Math.Min(Vec[i], vector.Vec[i]);
            }
            else
                throw new VectorException("Vector dimension mismatch");
             //   raiseNotify("Vector dimension mismatch", "In Vector.Min");

            return c;
        }

        public double X()
        {
            if (!Vec.Count.Equals(3))
            {

                throw new VectorException("Illegal vector dimension");
//                return 0.0;
            }

            return Vec[0];
        }
        public double Y()
        {
            if (!Vec.Count.Equals(3))
            {
                raiseNotify("Illegal vector dimension", "In Vector.Y");

                return 0.0;
            }

            return Vec[1];
        }
        public double Z()
        {
            if (!Vec.Count.Equals(3))
            {
                raiseNotify("Illegal vector dimension", "In Vector.Z");
                return 0.0;
            }

            return Vec[2];
        }

        //Returns this X b
        public Vector CrossProduct(Vector b)
        {
            Vector c = new Vector(Vec.Count);

            c.Vec[0] = Y() * b.Z() - Z() * b.Y();
            c.Vec[1] = Z() * b.X() - X() * b.Z();
            c.Vec[2] = X() * b.Y() - Y() * b.X();

            return c;
        }

        /* magof(n)
         * 
         * Returns The magnetude of the matrix.
         */
        public double magof()
        {
            int i;
            double SumOfSquares = 0.0;

            for (i = 0; i < Vec.Count; ++i)
                SumOfSquares += Math.Pow(Vec[i],2.0);

            return Math.Sqrt(SumOfSquares);
        }

        /* mscale(a, factor)
         * 
         * Returns a = factor * this.
         */
        public Vector Scale(double factor)
        {
            Vector c = new Vector(Vec.Count);
            int i;

            for (i = 0; i < Vec.Count; ++i)
                c.Vec[i] = Vec[i] * factor;

            return c;
        }

        /* normalize(n)
         * 
         * Normalizes dimension 3 vector n.
         */
        public Vector normalize()
        {
            double m = magof();

            if (m < .00001)
            {
                raiseNotify("Matrix to small", "normalize");

                return new Vector(Vec.Count);
            }

            return Scale(1 / m);
        }

        /* Equate(b)
         * 
         * Sets this = b;
         */
        public Vector Equate(Vector b)
        {
            int i;

            if (!Vec.Count.Equals(b.Vec.Count))
                return new Vector(b);

            for (i = 0; i < Vec.Count; ++i)
                Vec[i] = b.Vec[i];

            return this;
        }
        #endregion
        #region Private Methods
        #endregion
        /* Vadd(a,b,ref c,Dim)
         * 
         * Performs: c = a + b.
         */
        public  void Vadd(double[] a, double[] b, ref double[] c, int Dim)
        {
            int i;

            for (i = 0; i < Dim; ++i)
                c[i] = a[i] + b[i];
        }

        /* Vsub(a,b,ref c,Dim)
         * 
         * Performs: c = a - b.
         */
        public  void Vsub(double[] a, double[] b, ref double[] c, int Dim)
        {
            int i;

            for (i = 0; i < Dim; ++i)
                c[i] = a[i] - b[i];
        }

        /* CrossProduct(a,b,c)
         * 
         * Sets c = a X b.
         * 
         * Assumes all 3 are of dim 3.
         */
        public  void CrossProduct(double[] a, double[] b, ref double[] c)
        {
            double ax = 0.0, ay = 0.0, az = 0.0, bx = 0.0, by = 0.0, bz = 0.0;

            ax = a[0];
            ay = a[1];
            az = a[2];

            bx = b[0];
            by = b[1];
            bz = b[2];

            c[0] = ay * bz - az * by;
            c[1] = az * bx - ax * bz;
            c[2] = ax * by - ay * bx;
        }

        /* magof(n)
         * 
         * Returns The magnetude of the matrix.
         */
        public static double magof(double[] n)
        {
            int i;
            double SumOfSquares = 0.0;

            for (i = 0; i < 3; ++i)
                SumOfSquares += n[i] * n[i];

            return Math.Sqrt(SumOfSquares);
        }

        /* mscale(a, factor)
         * 
         * Performs scaler matrix mult. a = factor * a.
         * 
         * Assumes dimension 3.
         */
        public  void mscale(ref double[] a, double factor)
        {
            int i;

            for (i = 0; i < 3; ++i)
                a[i] *= factor;
        }

        /* normalize(n)
         * 
         * Normalizes dimension 3 vector n.
         */
        public  void normalize(ref double[] n)
        {
            double m = Vector.magof(n);

            mscale(ref n, 1.0 / m);
        }


        /* VrEquate(a,b,row,nc)
         * 
         *  Sets b = a[row]
         */
        public  void VrEquate(double[ ,] a, ref double[] b, int row, int nc)
        {
            int i;

            for (i = 0; i < nc; ++i)
                b[i] = a[row, i];
        }

        public Vector ListRowEquate(List<Vector> a, int row)
        {
            if (!a[row].Vec.Count.Equals(Vec.Count))
            {
                raiseNotify("Dimension error", "ListRowEquate");
            }

            return new Vector(a[row]);
        }



        /* rdmult(a, b, c) -    Performs  multiplication c = ab; where, a is a (3x3)
         *                      rotational matrix, and b & c are (1x3) displacement
         *                      vectors.
         *
         *      Inputs:         a       Pointer to (3x3) rotation matrix a.
         *                      b       Pointer to (1x3) displacement vector b.
         *                      c       Pointer to (1x3) displacement vector c.
         *
         *      Outputs:        c       Elements of this displacement vector set by
         *                              result of matrix multiplication: c = a X this.
         *
         *      Description:
         *              1)      Performs the matrix multiplication:  c = ab; where,
         *                      a is a (3x3) rotational matrix, and b & c are (1x3)
         *                      displacement vectors.
         *              2)      Capable of handling instances where  b, and c point
         *                      to the same addresses.
         *
         */
        public Vector rdmult(Rotation3x3 a)
        {
            int i, k;
            Vector c = new Vector(3);

            if (!Vec.Count.Equals(3))
            {
                raiseNotify("Illegal dimension", "rdmult");
                return null;
            }

            /* Perform multiplication. pt = ab.                                     */

            for (i = 0; i < Vec.Count; ++i)
            {
                /* Set element equal to the sum of the products of the               *
                 * kth  column of a times the kth row in b.                          */

                for (k = 0; k < 3; ++k)
                    c.Vec[i] += a.rot[3 * i + k] * Vec[k];
            }

            return c;
        }

        /* Equate(a,b,dim)
         * 
         * Sets b = a;
         */
        public  void Equate(double[] a, ref double[] b, int dim)
        {
            int i;

            for (i = 0; i < dim; ++i)
                b[i] = a[i];
        }


        public event NotifyMessageEventHandler NotifyMessage;
        private void raiseNotify(string message, string title)
        {
            if (NotifyMessage!=null)
                NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
        }
    }
}
