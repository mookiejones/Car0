using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;
using System.Windows.Forms;

namespace Car0
{
    class GetPlane
    {

        public event NotifyMessageEventHandler NotifyMessage;
        private void raiseNotify(string message, string title)
        {
            if (NotifyMessage!=null)
                NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
        }

        #region Public Variables
        public Vector3 Normal;
        public double Distance, MaxError, AveError;
        #endregion
        #region Private Variables
        private Matrix X, B, C, A_T, A_T_A, A_T_y, inv_Bn, last_p, work, N, NN, temp;
        private List<Matrix> y = new List<Matrix>(3);
        private List<Matrix> p = new List<Matrix>(3);
        private List<Matrix> A = new List<Matrix>(3);

        private int I, J, K;
        private Boolean Negative;
        #endregion
        #region Public Methods
        public GetPlane()
        {
            //This is kept, but is pretty meaningless without the needed data
            Normal = new Vector3();
            Distance = MaxError = AveError = 0.0;
        }
        public GetPlane(List<Vector3> PlanePoints)
        {
            if (Init(PlanePoints))
            {
                int count = 0, md_ptr = 0;
                Boolean use_this_one = false;

                NN.equate(N);

                last_p = new Matrix(3, 1);
                work = new Matrix(3, 1);

                for (md_ptr = 3; md_ptr < PlanePoints.Count; ++md_ptr)
                {
                    if (count.Equals(0))
                    {
                        use_this_one = true;
                        last_p.equate(PlanePoints[md_ptr]);
                    }
                    else
                    {
                        // Calculate the distance between the two points
                        work.equate(PlanePoints[md_ptr]);
                        work = work.msub(last_p);

                        //Has it moved at least 2mm
                        if (work.magof() > 2.0)
                        {
                            use_this_one = true;
                            last_p.equate(PlanePoints[md_ptr]);
                        }
                        else
                            use_this_one = false;
                    }

                    if (use_this_one)
                    {
                        Matrix localA = new Matrix(A[0]);
                        Matrix localY = new Matrix(y[0]);

                        //Assign values to A and y matrices
                        asgn_Ay(last_p,ref localA, ref localY);

                        //Update B & C matrices.
                        ls_update(localY, localA);
                    }

                    //Increment the count.
                    ++count;
                }

                //Solve for the leaste squares solution
                find_Nd();

                //Check the results
                Check(PlanePoints);

                Normal = new Vector3(N);
            }
        }



        #endregion
        #region Private Methods
        private Boolean Init(List<Vector3> PlanePoints)
        {
            int i;

            //Allocate matrices
            inv_Bn = new Matrix(3, 3);
            A_T_A = new Matrix(3, 3);
            A_T_y = new Matrix(3, 1);
            NN = new Matrix(3, 1);
            N = new Matrix(3, 1);
            X = new Matrix(3, 1);
            temp = new Matrix(3, 1);

            //Allocate matrix arrays
            for (i = 0; i < 3; ++i)
            {
                p.Add(new Matrix(3, 1));
                A.Add(new Matrix(1, 3));
                y.Add(new Matrix(1, 1));
            }

            //Stuff first 3 records into p
            for (i = 0; i < 3; ++i)
                p[i].equate(PlanePoints[i]);

            /* Calculate initial plane.  NOTE: If more than 3 markers are used, this method requres.          *
             * that 1st 3 picked are well suited for plane calculation.                   */

            if (!calc_Nd())
                return false;
            
            //Assign initial points to the A and Y vectors
            for (i = 0; i < 3; ++i)
            {
                Matrix MyA = new Matrix(A[i]);
                Matrix MyY = new Matrix(y[i]);

                asgn_Ay(p[i], ref MyA, ref MyY);

                A[i] = new Matrix(MyA);
                y[i] = new Matrix(MyY);
            }

            /* Calculate initial values for: B = A[0]^T*A[0] + A[1]^T*A[1] + A[2]^T*A[2]   and    *
             *                   C = A[0]^T*y[0] + A[1]^T*y[1] + A[2]^T*y[2]      */
            A_T = A[0].trans();
            B = A_T.mmult(A[0]);
            C = A_T.mmult(y[0]);

            A_T = A[1].trans();
            A_T_A = A_T.mmult(A[1]);
            B = A_T_A.madd(B);
            A_T_y = A_T.mmult(y[1]);
            C = A_T_y.madd(C);

            A_T = A[2].trans();
            A_T_A = A_T.mmult(A[2]);
            B = A_T_A.madd(B);
            A_T_y = A_T.mmult(y[2]);
            C = A_T_y.madd(C);

            return true;
        }


        private Boolean calc_Nd()
        {
            Matrix t1 = p[1].msub(p[0]);
            Matrix t2 = p[2].msub(p[1]);

            N = t1.CrossProduct(t2);
            N.Normalize();

            Distance = N.DotProduct(p[0]);

            return des_order(N);
        }

        Boolean des_order(Matrix v)
        {
            double x = 0.0, y = 0.0, z = 0.0;

            /* Set up absolute values.                       */

            x = Math.Abs(v.getvalue(0, 0));
            y = Math.Abs(v.getvalue(1, 0));
            z = Math.Abs(v.getvalue(2, 0));

            if ((x >= y) && (x >= z))
            {
                I = 0;

                if (y >= z)
                {
                    J = 1;
                    K = 2;
                }
                else
                {
                    J = 2;
                    K = 1;
                }
            }
            else if ((y >= x) && (y >= z))
            {
                I = 1;

                if (x >= z)  /* DEBUG had y >= z */
                {
                    J = 0;
                    K = 2;
                }
                else
                {
                    J = 2;
                    K = 0;
                }
            }
            else if ((z >= x) && (z >= y))
            {
                I = 2;

                if (x >= y) /* DEBUG had y >= z   */
                {
                    J = 0;
                    K = 1;
                }
                else
                {
                    J = 1;
                    K = 0;
                }
            }
            else
            {
                raiseNotify("Algorithm failed", "des_order");

                return false;
            }

            /* Set up the negative flag.                 */

            Negative = (v.getvalue(I, 0) <= 0.0);

            return true;
        }

        private void asgn_Ay(Matrix point, ref Matrix A_mat, ref Matrix y_mat)
        {
            y_mat.assign(0, 0, point.getvalue(I, 0));
            A_mat.assign(0, 0, 1.0);
            A_mat.assign(0, 1, -point.getvalue(J, 0));
            A_mat.assign(0, 2, -point.getvalue(K, 0));
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

        private void find_Nd()
        {
            //Calculate the X matrix.
            calc_x();
            
            //Perform assignments.
            N.assign(I, 0, 1.0);
            N.assign(J, 0, X.getvalue(1, 0));
            N.assign(K, 0, X.getvalue(2, 0));

            //Calculate Distance while handling negations
            if (Negative)
            {
                Distance = -(X.getvalue(0, 0) / N.magof());
                N.Normalize(-1.0);
            }
            else
            {
                Distance = X.getvalue(0, 0) / N.magof();
                N.Normalize();
            }
        }

        private void calc_x()
        {
            inv_Bn = B.inverse();  

            X = inv_Bn.mmult(C);
        }

        private void Check(List<Vector3> PlanePoints)
        {
            double error = 0.0, sum_error = 0.0, ddd = 0.0;
            int i;

            MaxError = 0.0;
            for (i = 0; i < PlanePoints.Count; ++i)
            {
                temp.equate(PlanePoints[i]);
                ddd = temp.DotProduct(N);

                error = Math.Abs(ddd - Distance);

                sum_error += error;

                MaxError = Math.Max(MaxError, error);
            }

            AveError = sum_error / Convert.ToDouble(PlanePoints.Count);
        }
        #endregion
    }
}
