using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Car0
{
    public class Vector3
    {

        public event NotifyMessageEventHandler NotifyMessage;
        private void raiseNotify(string message, string title)
        {
            if (NotifyMessage!=null)
                NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
        }

        #region Variables
        private double x = 0.0;
        private double y = 0.0;
        public double z = 0.0;

        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }
        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
        public double Z
        {
            get { return this.z; }
            set { this.z = value; }
        }

        #endregion
        #region Public Methods
        public Vector3()
        {
        }
        public Vector3(double tx, double ty, double tz)
        {
            x = tx;
            y = ty;
            z = tz;
        }
        public Vector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        public Vector3(Matrix m)
        {
            if (m.Rows.Equals(3) && m.Cols.Equals(1))
            {
                x = m.getvalue(0, 0);
                y = m.getvalue(1, 0);
                z = m.getvalue(2, 0);
            }
            else
            {
                raiseNotify("Matrix dimension programming error", "Vector3");
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
                raiseNotify("Vector dimension programming error", "Vector3");
                x = y = z = 0.0;
            }
        }
        public Vector3(List<Vector3> vecs, ref double Max, ref double Ave, out int WorstPointNum)
        {
            int i;
            Vector3 err;
            double e = 0.0;
            x = y = z = Max = Ave = 0.0;

            WorstPointNum = 0;

            for (i = 0; i < vecs.Count; ++i)
            {
                x += vecs[i].x;
                y += vecs[i].y;
                z += vecs[i].z;
            }

            x /= Convert.ToDouble(vecs.Count);
            y /= Convert.ToDouble(vecs.Count);
            z /= Convert.ToDouble(vecs.Count);

            for (i = 0; i < vecs.Count; ++i)
            {
                err = Subtract(vecs[i]);
                e = err.Magof();

                if (e > Max)
                {
                    Max = e;
                    WorstPointNum = i;
                }
                Ave += e;
            }

            Ave /= Convert.ToDouble(vecs.Count);
        }

        public Vector3 Add(Vector3 v)
        {
            return new Vector3(x + v.x, y + v.y, z + v.z);
        }

        public Vector3 Subtract(Vector3 v)
        {
            return new Vector3(x - v.x, y - v.y, z - v.z);
        }

        public void Scale(double sf)
        {
            x *= sf;
            y *= sf;
            z *= sf;
        }

        public double DistFrom(Vector3 other)
        {
            return Math.Sqrt(Math.Pow((x - other.x), 2) + Math.Pow((y - other.y), 2) + Math.Pow((z - other.z), 2));
        }

        public double Magof()
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        public void Normalize()
        {
            try
            {
                Scale(1 / Magof());
            }
            catch (Exception)
            {
                raiseNotify("Matrix too small to normalize", "Normalize");
            }
        }

        /* CrossProduct(a,b,c)
         * 
         * Sets c = a X b.
         * 
         * Assumes all 3 are of dim 3.
         */
        public  Vector3 CrossProduct(Vector3 a, Vector3 b)
        {
            return new Vector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

        //This version crosses This X B, returning the result
        public Vector3 CrossProduct(Vector3 b)
        {
            return new Vector3(y * b.z - z * b.y, z * b.x - x * b.z, x * b.y - y * b.x);
        }

        public Vector3 MidPoint(Vector3 b)
        {
            return new Vector3((x+b.x)/2.0,(y+b.y)/2.0,(z+b.z)/2.0);
        }

        public double DotProduct(Vector3 b)
        {
            return x * b.x + y * b.y + z * b.z;
        }

        public void Equate(Vector3 a)
        {
            x = a.x;
            y = a.y;
            z = a.z;
        }

        public  Boolean LineLineIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, ref Vector3 pa, ref Vector3 pb, ref ArrayList Problems, string IDmessage)
        {
            Vector3 p13 = p1.Subtract(p3);
            Vector3 p43 = p4.Subtract(p3);

            //Check for P4 and P3 coincident
            if (p43.Magof() < 0.001)
            {
                Problems.Add(IDmessage + " End points are too close");
                return false;
            }

            Vector3 p21 = p2.Subtract(p1);

            if (p21.Magof() < 0.001)
            {
                Problems.Add(IDmessage + " First line is way too short");
                return false;
            }

            double d1343 = p13.DotProduct(p43);
            double d4321 = p43.DotProduct(p21);
            double d1321 = p13.DotProduct(p21);
            double d4343 = p43.DotProduct(p43);
            double d2121 = p21.DotProduct(p21);

            double denom = d2121 * d4343 - d4321 * d4321;

            if (Math.Abs(denom) < 0.001)
            {
                Problems.Add(IDmessage + " No solution due to denominator");
                return false;
            }

            double numer = d1343 * d4321 - d1321 * d4343;

            double mua = numer / denom;
            double mub = (d1343 + d4321 * mua) / d4343;

            pa.x = p1.x + mua * p21.x;
            pa.y = p1.y + mua * p21.y;
            pa.z = p1.z + mua * p21.z;

            pb.x = p3.x + mub * p43.x;
            pb.y = p3.y + mub * p43.y;
            pb.z = p3.z + mub * p43.z;

            return true;
        }

        // This vector is taken as the Point
        public void PointSlopeToEndpoints(Vector3 Slope, double Magnitude, ref Vector3 sp, ref Vector3 ep)
        {
            double Offset = Magnitude / 2.0;

            sp.x = x - Offset * Slope.x;
            sp.y = y - Offset * Slope.y;
            sp.z = z - Offset * Slope.z;

            ep.x = x + Offset * Slope.x;
            ep.y = y + Offset * Slope.y;
            ep.z = z + Offset * Slope.z;
        }

        public  void AddToList(List<Vector3> NewStuff, ref List<Vector3> BuildList)
        {
            for (int i = 0; i < NewStuff.Count; ++i)
                BuildList.Add(NewStuff[i]);
        }
        #endregion
    }
}
