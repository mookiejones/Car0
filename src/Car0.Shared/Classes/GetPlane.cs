#if WPF
using System.Windows;
    #else
using System.Windows.Forms;
#endif
namespace CarZero
{
    using System;
    using System.Collections.Generic;
    

    internal class GetPlane
    {
        private List<Matrix> A;
        private Matrix A_T;
        private Matrix A_T_A;
        private Matrix A_T_y;
        public double AveError;
        private Matrix B;
        private Matrix C;
        public double Distance;
        private int I;
        private Matrix inv_Bn;
        private int J;
        private int K;
        private Matrix last_p;
        public double MaxError;
        private Matrix N;
        private bool Negative;
        private Matrix NN;
        public Vector3 Normal;
        private List<Matrix> p;
        private Matrix temp;
        private Matrix work;
        private Matrix X;
        private List<Matrix> y;

        public GetPlane()
        {
            y = new List<Matrix>(3);
            p = new List<Matrix>(3);
            A = new List<Matrix>(3);
            Normal = new Vector3();
            Distance = MaxError = AveError = 0.0;
        }

        public GetPlane(List<Vector3> PlanePoints)
        {
            y = new List<Matrix>(3);
            p = new List<Matrix>(3);
            A = new List<Matrix>(3);
            if (Init(PlanePoints))
            {
                var num = 0;
                var num2 = 0;
                var flag = false;
                NN.equate(N);
                last_p = new Matrix(3, 1);
                work = new Matrix(3, 1);
                for (num2 = 3; num2 < PlanePoints.Count; num2++)
                {
                    if (num.Equals(0))
                    {
                        flag = true;
                        last_p.equate(PlanePoints[num2]);
                    }
                    else
                    {
                        work.equate(PlanePoints[num2]);
                        work = work.msub(last_p);
                        if (work.magof() > 2.0)
                        {
                            flag = true;
                            last_p.equate(PlanePoints[num2]);
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        var matrix = new Matrix(A[0]);
                        var matrix2 = new Matrix(y[0]);
                        asgn_Ay(last_p, ref matrix, ref matrix2);
                        ls_update(matrix2, matrix);
                    }
                    num++;
                }
                find_Nd();
                Check(PlanePoints);
                Normal = new Vector3(N);
            }
        }

        private void asgn_Ay(Matrix point, ref Matrix A_mat, ref Matrix y_mat)
        {
            y_mat.assign(0, 0, point.getvalue(I, 0));
            A_mat.assign(0, 0, 1.0);
            A_mat.assign(0, 1, -point.getvalue(J, 0));
            A_mat.assign(0, 2, -point.getvalue(K, 0));
        }

        private bool calc_Nd()
        {
            var matrix = p[1].msub(p[0]);
            var b = p[2].msub(p[1]);
            N = matrix.CrossProduct(b);
            N.Normalize();
            Distance = N.DotProduct(p[0]);
            return des_order(N);
        }

        private void calc_x()
        {
            inv_Bn = B.inverse();
            X = inv_Bn.mmult(C);
        }

        private void Check(List<Vector3> PlanePoints)
        {
            var num = 0.0;
            var num2 = 0.0;
            MaxError = 0.0;
            for (var i = 0; i < PlanePoints.Count; i++)
            {
                temp.equate(PlanePoints[i]);
                num = Math.Abs((double) (temp.DotProduct(N) - Distance));
                num2 += num;
                MaxError = Math.Max(MaxError, num);
            }
            AveError = num2 / Convert.ToDouble(PlanePoints.Count);
        }

        private bool des_order(Matrix v)
        {
            var num = 0.0;
            var num2 = 0.0;
            var num3 = 0.0;
            num = Math.Abs(v.getvalue(0, 0));
            num2 = Math.Abs(v.getvalue(1, 0));
            num3 = Math.Abs(v.getvalue(2, 0));
            if ((num >= num2) && (num >= num3))
            {
                I = 0;
                if (num2 >= num3)
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
            else if ((num2 >= num) && (num2 >= num3))
            {
                I = 1;
                if (num >= num3)
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
            else if ((num3 >= num) && (num3 >= num2))
            {
                I = 2;
                if (num >= num2)
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
                MessageBox.Show("Algorithm failed", "des_order");
                return false;
            }
            Negative = v.getvalue(I, 0) <= 0.0;
            return true;
        }

        private void find_Nd()
        {
            calc_x();
            N.assign(I, 0, 1.0);
            N.assign(J, 0, X.getvalue(1, 0));
            N.assign(K, 0, X.getvalue(2, 0));
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

        private bool Init(List<Vector3> PlanePoints)
        {
            int num;
            inv_Bn = new Matrix(3, 3);
            A_T_A = new Matrix(3, 3);
            A_T_y = new Matrix(3, 1);
            NN = new Matrix(3, 1);
            N = new Matrix(3, 1);
            X = new Matrix(3, 1);
            temp = new Matrix(3, 1);
            for (num = 0; num < 3; num++)
            {
                p.Add(new Matrix(3, 1));
                A.Add(new Matrix(1, 3));
                y.Add(new Matrix(1, 1));
            }
            for (num = 0; num < 3; num++)
            {
                p[num].equate(PlanePoints[num]);
            }
            if (!calc_Nd())
            {
                return false;
            }
            for (num = 0; num < 3; num++)
            {
                var matrix = new Matrix(A[num]);
                var matrix2 = new Matrix(y[num]);
                asgn_Ay(p[num], ref matrix, ref matrix2);
                A[num] = new Matrix(matrix);
                y[num] = new Matrix(matrix2);
            }
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

        private void ls_update(Matrix Ymat, Matrix Amat)
        {
            A_T = Amat.trans();
            A_T_A = A_T.mmult(Amat);
            B = B.madd(A_T_A);
            A_T_y = A_T.mmult(Ymat);
            C = C.madd(A_T_y);
        }
    }
}

