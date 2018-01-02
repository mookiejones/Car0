#if WPF
using System.Windows;
#else
using System.Windows.Forms;
#endif
namespace CarZero
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
   

    internal class Vector3
    {
        public double x;
        public double y;
        public double z;

        public Vector3()
        {
            x = y = z = 0.0;
        }

        public Vector3(Matrix m)
        {
            if (m.rows.Equals(3) && m.cols.Equals(1))
            {
                x = m.getvalue(0, 0);
                y = m.getvalue(1, 0);
                z = m.getvalue(2, 0);
            }
            else
            {
                MessageBox.Show("Matrix dimension programming error", "Vector3");
                x = y = z = 0.0;
            }
        }

        public Vector3(Vector v)
        {
            if (v.Vec.Count.Equals(3))
            {
                x = v.Vec[0];
                y = v.Vec[1];
                z = v.Vec[2];
            }
            else
            {
                MessageBox.Show("Vector dimension programming error", "Vector3");
                x = y = z = 0.0;
            }
        }

        public Vector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3(double tx, double ty, double tz)
        {
            x = tx;
            y = ty;
            z = tz;
        }

        public Vector3(List<Vector3> vecs, ref double Max, ref double Ave, out int WorstPointNum)
        {
            int num;
            double num3;
            var num2 = 0.0;
            Ave = num3 = 0.0;
            Max = num3 = num3;
            x = y = z = num3;
            WorstPointNum = 0;
            for (num = 0; num < vecs.Count; num++)
            {
                x += vecs[num].x;
                y += vecs[num].y;
                z += vecs[num].z;
            }
            x /= Convert.ToDouble(vecs.Count);
            y /= Convert.ToDouble(vecs.Count);
            z /= Convert.ToDouble(vecs.Count);
            for (num = 0; num < vecs.Count; num++)
            {
                num2 = Subtract(vecs[num]).Magof();
                if (num2 > Max)
                {
                    Max = num2;
                    WorstPointNum = num;
                }
                Ave += num2;
            }
            Ave /= Convert.ToDouble(vecs.Count);
        }

        public Vector3 Add(Vector3 v)
        {
            return new Vector3(x + v.x, y + v.y, z + v.z);
        }

        public static void AddToList(List<Vector3> NewStuff, ref List<Vector3> BuildList)
        {
            for (var i = 0; i < NewStuff.Count; i++)
            {
                BuildList.Add(NewStuff[i]);
            }
        }

        public Vector3 CrossProduct(Vector3 b)
        {
            return new Vector3((y * b.z) - (z * b.y), (z * b.x) - (x * b.z), (x * b.y) - (y * b.x));
        }

        public static Vector3 CrossProduct(Vector3 a, Vector3 b)
        {
            return new Vector3((a.y * b.z) - (a.z * b.y), (a.z * b.x) - (a.x * b.z), (a.x * b.y) - (a.y * b.x));
        }

        public double DistFrom(Vector3 other)
        {
            return Math.Sqrt((Math.Pow(x - other.x, 2.0) + Math.Pow(y - other.y, 2.0)) + Math.Pow(z - other.z, 2.0));
        }

        public double DotProduct(Vector3 b)
        {
            return (((x * b.x) + (y * b.y)) + (z * b.z));
        }

        public void Equate(Vector3 a)
        {
            x = a.x;
            y = a.y;
            z = a.z;
        }

        public static bool LineLineIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, ref Vector3 pa, ref Vector3 pb, ref ArrayList Problems, string IDmessage)
        {
            var vector = p1.Subtract(p3);
            var b = p4.Subtract(p3);
            if (b.Magof() < 0.001)
            {
                Problems.Add(IDmessage + " End points are too close");
                return false;
            }
            var vector3 = p2.Subtract(p1);
            if (vector3.Magof() < 0.001)
            {
                Problems.Add(IDmessage + " First line is way too short");
                return false;
            }
            var num = vector.DotProduct(b);
            var num2 = b.DotProduct(vector3);
            var num3 = vector.DotProduct(vector3);
            var num4 = b.DotProduct(b);
            var num6 = (vector3.DotProduct(vector3) * num4) - (num2 * num2);
            if (Math.Abs(num6) < 0.001)
            {
                Problems.Add(IDmessage + " No solution due to denominator");
                return false;
            }
            var num7 = (num * num2) - (num3 * num4);
            var num8 = num7 / num6;
            var num9 = (num + (num2 * num8)) / num4;
            pa.x = p1.x + (num8 * vector3.x);
            pa.y = p1.y + (num8 * vector3.y);
            pa.z = p1.z + (num8 * vector3.z);
            pb.x = p3.x + (num9 * b.x);
            pb.y = p3.y + (num9 * b.y);
            pb.z = p3.z + (num9 * b.z);
            return true;
        }

        public double Magof()
        {
            return Math.Sqrt((Math.Pow(x, 2.0) + Math.Pow(y, 2.0)) + Math.Pow(z, 2.0));
        }

        public Vector3 MidPoint(Vector3 b)
        {
            return new Vector3((x + b.x) / 2.0, (y + b.y) / 2.0, (z + b.z) / 2.0);
        }

        public void Normalize()
        {
            try
            {
                Scale(1.0 / Magof());
            }
            catch (Exception)
            {
                MessageBox.Show("Matrix too small to normalize", "Normalize");
            }
        }

        public void PointSlopeToEndpoints(Vector3 Slope, double Magnitude, ref Vector3 sp, ref Vector3 ep)
        {
            var num = Magnitude / 2.0;
            sp.x = x - (num * Slope.x);
            sp.y = y - (num * Slope.y);
            sp.z = z - (num * Slope.z);
            ep.x = x + (num * Slope.x);
            ep.y = y + (num * Slope.y);
            ep.z = z + (num * Slope.z);
        }

        public void Scale(double sf)
        {
            x *= sf;
            y *= sf;
            z *= sf;
        }

        public Vector3 Subtract(Vector3 v)
        {
            return new Vector3(x - v.x, y - v.y, z - v.z);
        }
    }
}

