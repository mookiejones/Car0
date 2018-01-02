using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;
using System.Windows.Forms;

namespace Car0
{
    class AxisOfRotation
    {
        public event NotifyMessageEventHandler NotifyMessage;
        private void raiseNotify(string message,string title)
        {
            if (NotifyMessage!=null)
                NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
        }

        #region Public Variables
        public Vector3 Origin;              //rot_ax
        public Vector3 Normal;              //unt_nor
        public double Radius, MaxError, AveError;
        #endregion
        #region Private Variables
        private const double AX_OF_ROT_MIN_DIST = 10.0;
        private Matrix inv_Bn, A, A_T, A_T_A, A_T_y, B, C, tempP, T, L, Xo, Z;
        private List<Matrix> p = new List<Matrix>(2);
        private List<Matrix> N = new List<Matrix>(2);
        private List<Matrix> d = new List<Matrix>(2);
        #endregion
        #region Public Methods
        public AxisOfRotation()
        {
            //This is kept, but is pretty meaningless without the needed data
            Origin = new Vector3();
            Normal = new Vector3();

            Radius = MaxError = AveError = 0.0;
        }
        public AxisOfRotation(List<Vector3> CircPoints, Vector3 PlaneNormal, double PlaneDistance)
        {
            int NextPoint = 0;
            double di = 0.0;

            if (Init(CircPoints, PlaneNormal, PlaneDistance, ref NextPoint))
            {
                //If there are any more points
                if (NextPoint < CircPoints.Count)
                {
                    do
                    {
                        // Calculate the perpendicular bisector for the next two points.
                        N[0] = perp_bisec(ref di);
                        d[0].assign(0, 0, di);

                        //Set A = (N[0])^T 
                        A = N[0].trans();

                        //Update the estimate of the axis of rotation
                        ls_update(d[0], A);


                    } while (next_p(CircPoints, ref NextPoint));
                }

                //Solve for axis of rotation and the average radius.
                ar_calc_ave_radius(CircPoints);

                //Check the results.
                ar_res_check(CircPoints);

                //Define output vectors
                Origin = new Vector3(Xo);
                Normal = new Vector3(T);
            }
        }
        #endregion
        #region Private Methods
        private Boolean Init(List<Vector3> CircPoints, Vector3 PlaneNormal, double PlaneDistance, ref int j)
        {
            int i;
            double diff = 0.0, di = 0.0;

            j = 0;

            //Allocate matrices
            inv_Bn = new Matrix(3, 3);
            A_T = new Matrix(3, 1);
            A_T_A = new Matrix(3, 3);
            A_T_y = new Matrix(3, 1);
            Xo = new Matrix(3, 1);
            Z = new Matrix(1, 1);
            T = new Matrix(PlaneNormal);

            //Allocate matrix arrays
            for (i = 0; i < 2; ++i)
            {
                p.Add(new Matrix(3, 1));
                N.Add(new Matrix(3, 1));
                d.Add(new Matrix(1, 1));
            }

            //Process first 2 measured points
            for (i = 0; i < 2; ++i)
            {
                p[i].equate(CircPoints[i]);

                //Insure significant distance bewtween points (in case points recorded before start motion)
                if (!i.Equals(0))
                {
                    j++;

                    do
                    {
                        N[0] = p[0].msub(p[1]);

                        diff = N[0].magof();

                        if (diff < AX_OF_ROT_MIN_DIST)
                        {
                            p[i].equate(CircPoints[j]);
                            ++j;
                        }

                    } while ((diff < AX_OF_ROT_MIN_DIST) && (j < CircPoints.Count));
                }
            }

            ++j;

            if (j >= CircPoints.Count)
            {
                raiseNotify("Circle points too close together", "AxisOfRotation Init");
                return false;
            }

            //Calculate the first two perpendicular bisecting planes
            for (i = 0; i < 2; ++i)
            {
                N[i] = perp_bisec(ref di);
                d[i].assign(0, 0, di);

                //Get next point and be sure we have enough for the 2nd time around loop
                if (i.Equals(0) && !next_p(CircPoints, ref j))
                {
                    
                    raiseNotify("Not enough points for circle, need at least 3", "AxisOfRotation Init");
                    return false;
                }
            }

            //Calculate A = (T)^T, B = T * (T)^T & C = T.
            A = T.trans();
            B = T.mmult(A);
            C = new Matrix(T);

            //Scale C = PlaneDistance * T
            C.scale(PlaneDistance);

            //Loop
            for (i = 0; i < 2; ++i)
            {
                //Calculate A = N[i] * (T)^T,  A_T_A = N[i] * (N[i])^T and B += A_T_A.
                A = N[i].trans();
                A_T_A = N[i].mmult(A);
                B = B.madd(A_T_A);

                //Calculate tempP = N[i]*d[i] and C += tempP.
                tempP = N[i].mmult(d[i]);
                C = C.madd(tempP);
            }

            //Initialize L and Z constraint matrices
            L = T.trans();
            Z.assign(0, 0, PlaneDistance);

            return true;
        }

        private Matrix perp_bisec(ref double di)
        {
            Matrix n = p[1].msub(p[0]);

            n.Normalize();
            tempP = p[1].madd(p[0]);

            di = 0.5 * n.DotProduct(tempP);

            return n;
        }

        private Boolean next_p(List<Vector3> CircPoints, ref int j)
        {
            if (j < CircPoints.Count)
            {
                p[0].equate(p[1]);
                p[1].equate(CircPoints[j]);
                ++j;

                return true;
            }

            return false;
        }

        private void ls_update(Matrix Ymat, Matrix Amat)
        {
            /* A_T <==  A^T.                                */
            /* A_T_A <== A_T * A.                           */
            /* B <== B + A_T_A.                             */
            /* A_T_y <== A_T * y.                           */
            /* C <== C + A_T_y.                             */

            A_T = Amat.trans();
            A_T_A = A_T.mmult(Amat);
            B = B.madd(A_T_A);
            A_T_y = A_T.mmult(Ymat);
            C = C.madd(A_T_y);
        }

        private void ar_calc_ave_radius(List<Vector3> CircPoints)
        {
            int i;

            Radius = 0.0;       //Initialize

            //Calculate best fit axis of rotation.
            ar_calc_x();

            //Calculate the average radius.
            for (i = 0; i < CircPoints.Count; ++i)
            {
                p[0].equate(CircPoints[i]);

                //Calculate the radius to the current point.
                Radius += ar_calc_radius();
            }

            //Calculate the average.
            Radius /= Convert.ToDouble(CircPoints.Count);
        }

        private void ar_calc_x()
        {
            //Calculate r = {L * inv_Bn * (L)^T}^(-1) * {Z - L * inv_Bn * Cn} 
            Matrix L_T = L.trans();
            inv_Bn = B.inverse();
            Matrix t1 = L.mmult(inv_Bn);
            Matrix t2 = t1.mmult(L_T).inverse();
            Matrix r = t1.mmult(C);
            r = Z.msub(r);
            r = t2.mmult(r);

            //Calculate Xo = inv_Bn * {Cn + (L)^T * r}
            Matrix t3 = L_T.mmult(r);
            t3 = C.madd(t3);
            Xo = inv_Bn.mmult(t3);
        }

        private double ar_calc_radius()
        {
            //Set tempP = T, then scale by the distance between plane and p[0]
            tempP.equate(T);

            /* Subtract tempP from p[0] to get vector from origin to point on plane lying on p[0].  Then *
             * subtact this vector from Xo that runs from origin to axis of rotation.                    */
            tempP = p[0].msub(tempP);
            tempP = Xo.msub(tempP);

            //Calculate the radius
            return tempP.magof();
        }

        private void ar_res_check(List<Vector3> CircPoints)
        {
            int i;
            double error;

            AveError = MaxError = 0.0;

            //Calculate error statistics.
            for (i = 0; i < CircPoints.Count; ++i)
            {
                p[0].equate(CircPoints[i]);

                error = Math.Abs(ar_calc_radius() - Radius);

                MaxError = Math.Max(MaxError, error);

                AveError += error;
            }

            AveError /= Convert.ToDouble(CircPoints.Count);
        }
        #endregion
    }
}
