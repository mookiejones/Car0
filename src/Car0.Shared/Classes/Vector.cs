#if WPF
using System.Windows;
    #else
using System.Windows.Forms;
#endif
namespace CarZero
{
    using System;
    using System.Collections.Generic; 

    public class Vector
    {
        public List<double> Vec;

        public Vector(Transformation a)
        {
            Vec = new List<double>(3);
            for (var i = 0; i < 3; i++)
            {
                Vec.Add(a.mat[(4 * i) + 3]);
            }
        }

        public Vector(Vector b)
        {
            Vec = new List<double>(b.Vec.Count);
            for (var i = 0; i < b.Vec.Count; i++)
            {
                Vec.Add(b.Vec[i]);
            }
        }

        public Vector(Vector3 b)
        {
            Vec = new List<double>(3);
            Vec.Add(b.x);
            Vec.Add(b.y);
            Vec.Add(b.z);
        }

        public Vector(int Dim)
        {
            Vec = new List<double>(Dim);
            for (var i = 0; i < Dim; i++)
            {
                Vec.Add(0.0);
            }
        }

        public Vector Add(Vector b)
        {
            var vector = new Vector(Vec.Count);
            for (var i = 0; i < Vec.Count; i++)
            {
                vector.Vec[i] = Vec[i] + b.Vec[i];
            }
            return vector;
        }

        public Vector CrossProduct(Vector b)
        {
            var vector = new Vector(Vec.Count);
            vector.Vec[0] = (Y() * b.Z()) - (Z() * b.Y());
            vector.Vec[1] = (Z() * b.X()) - (X() * b.Z());
            vector.Vec[2] = (X() * b.Y()) - (Y() * b.X());
            return vector;
        }

        public static void CrossProduct(double[] a, double[] b, ref double[] c)
        {
            var num = 0.0;
            var num2 = 0.0;
            var num3 = 0.0;
            var num4 = 0.0;
            var num5 = 0.0;
            var num6 = 0.0;
            num = a[0];
            num2 = a[1];
            num3 = a[2];
            num4 = b[0];
            num5 = b[1];
            num6 = b[2];
            c[0] = (num2 * num6) - (num3 * num5);
            c[1] = (num3 * num4) - (num * num6);
            c[2] = (num * num5) - (num2 * num4);
        }

        public Vector Equate(Vector b)
        {
            if (!Vec.Count.Equals(b.Vec.Count))
            {
                return new Vector(b);
            }
            for (var i = 0; i < Vec.Count; i++)
            {
                Vec[i] = b.Vec[i];
            }
            return this;
        }

        public static void Equate(double[] a, ref double[] b, int dim)
        {
            for (var i = 0; i < dim; i++)
            {
                b[i] = a[i];
            }
        }

        public Vector ListRowEquate(List<Vector> a, int row)
        {
            if (!a[row].Vec.Count.Equals(Vec.Count))
            {
                MessageBox.Show("Dimension error", "ListRowEquate");
            }
            return new Vector(a[row]);
        }

        public double magof()
        {
            var d = 0.0;
            for (var i = 0; i < Vec.Count; i++)
            {
                d += Math.Pow(Vec[i], 2.0);
            }
            return Math.Sqrt(d);
        }

        public static double magof(double[] n)
        {
            var d = 0.0;
            for (var i = 0; i < 3; i++)
            {
                d += n[i] * n[i];
            }
            return Math.Sqrt(d);
        }

        public Vector Max(Vector b)
        {
            var vector = new Vector(Vec.Count);
            if (Vec.Count.Equals(b.Vec.Count))
            {
                for (var i = 0; i < Vec.Count; i++)
                {
                    vector.Vec[i] = Math.Max(Vec[i], b.Vec[i]);
                }
                return vector;
            }
            MessageBox.Show("Vector dimension mismatch", "In Vector.Max");
            return vector;
        }

        public Vector Min(Vector b)
        {
            var vector = new Vector(Vec.Count);
            if (Vec.Count.Equals(b.Vec.Count))
            {
                for (var i = 0; i < Vec.Count; i++)
                {
                    vector.Vec[i] = Math.Min(Vec[i], b.Vec[i]);
                }
                return vector;
            }
            MessageBox.Show("Vector dimension mismatch", "In Vector.Min");
            return vector;
        }

        public static void mscale(ref double[] a, double factor)
        {
            for (var i = 0; i < 3; i++)
            {
                a[i] *= factor;
            }
        }

        public Vector normalize()
        {
            var num = magof();
            if (num < 1E-05)
            {
                MessageBox.Show("Matrix to small", "normalize");
                return new Vector(Vec.Count);
            }
            return Scale(1.0 / num);
        }

        public static void normalize(ref double[] n)
        {
            var num = magof(n);
            mscale(ref n, 1.0 / num);
        }

        public Vector rdmult(Rotation3x3 a)
        {
            var vector = new Vector(3);
            if (!Vec.Count.Equals(3))
            {
                MessageBox.Show("Illegal dimension", "rdmult");
                return null;
            }
            for (var i = 0; i < Vec.Count; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    int num3;
                    List<double> list;
                    (list = vector.Vec)[num3 = i] = list[num3] + (a.rot[(3 * i) + j] * Vec[j]);
                }
            }
            return vector;
        }

        public Vector Scale(double factor)
        {
            var vector = new Vector(Vec.Count);
            for (var i = 0; i < Vec.Count; i++)
            {
                vector.Vec[i] = Vec[i] * factor;
            }
            return vector;
        }

        public Vector Sub(Vector b)
        {
            var vector = new Vector(Vec.Count);
            for (var i = 0; i < Vec.Count; i++)
            {
                vector.Vec[i] = Vec[i] - b.Vec[i];
            }
            return vector;
        }

        public static void Vadd(double[] a, double[] b, ref double[] c, int Dim)
        {
            for (var i = 0; i < Dim; i++)
            {
                c[i] = a[i] + b[i];
            }
        }

        public static void VrEquate(double[,] a, ref double[] b, int row, int nc)
        {
            for (var i = 0; i < nc; i++)
            {
                b[i] = a[row, i];
            }
        }

        public static void Vsub(double[] a, double[] b, ref double[] c, int Dim)
        {
            for (var i = 0; i < Dim; i++)
            {
                c[i] = a[i] - b[i];
            }
        }

        public double X()
        {
            if (!Vec.Count.Equals(3))
            {
                MessageBox.Show("Illegal vector dimension", "In Vector.X");
                return 0.0;
            }
            return Vec[0];
        }

        public double Y()
        {
            if (!Vec.Count.Equals(3))
            {
                MessageBox.Show("Illegal vector dimension", "In Vector.Y");
                return 0.0;
            }
            return Vec[1];
        }

        public double Z()
        {
            if (!Vec.Count.Equals(3))
            {
                MessageBox.Show("Illegal vector dimension", "In Vector.Z");
                return 0.0;
            }
            return Vec[2];
        }
    }
}

