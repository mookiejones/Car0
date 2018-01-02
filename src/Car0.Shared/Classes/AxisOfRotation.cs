#if WPF
using System.Windows;
    #else
using System.Windows.Forms;
#endif
namespace CarZero
{
    using System;
    using System.Collections.Generic;
     

    internal class AxisOfRotation
    {
        private Matrix A;
        private Matrix A_T;
        private Matrix A_T_A;
        private Matrix A_T_y;
        public double AveError;
        private const double AX_OF_ROT_MIN_DIST = 10.0;
        private Matrix B;
        private Matrix C;
        private List<Matrix> d;
        private Matrix inv_Bn;
        private Matrix L;
        public double MaxError;
        private List<Matrix> N;
        public Vector3 Normal;
        public Vector3 Origin;
        private List<Matrix> p;
        public double Radius;
        private Matrix T;
        private Matrix tempP;
        private Matrix Xo;
        private Matrix Z;

        public AxisOfRotation()
        {
            p = new List<Matrix>(2);
            N = new List<Matrix>(2);
            d = new List<Matrix>(2);
            Origin = new Vector3();
            Normal = new Vector3();
            Radius = MaxError = AveError = 0.0;
        }

        public AxisOfRotation(List<Vector3> CircPoints, Vector3 PlaneNormal, double PlaneDistance)
        {
            p = new List<Matrix>(2);
            N = new List<Matrix>(2);
            d = new List<Matrix>(2);
            var j = 0;
            var di = 0.0;
            if (Init(CircPoints, PlaneNormal, PlaneDistance, ref j))
            {
                if (j < CircPoints.Count)
                {
                    do
                    {
                        N[0] = perp_bisec(ref di);
                        d[0].assign(0, 0, di);
                        A = N[0].trans();
                        ls_update(d[0], A);
                    }
                    while (next_p(CircPoints, ref j));
                }
                ar_calc_ave_radius(CircPoints);
                ar_res_check(CircPoints);
                Origin = new Vector3(Xo);
                Normal = new Vector3(T);
            }
        }

        private void ar_calc_ave_radius(List<Vector3> CircPoints)
        {
            Radius = 0.0;
            ar_calc_x();
            for (var i = 0; i < CircPoints.Count; i++)
            {
                p[0].equate(CircPoints[i]);
                Radius += ar_calc_radius();
            }
            Radius /= Convert.ToDouble(CircPoints.Count);
        }

        private double ar_calc_radius()
        {
            tempP.equate(T);
            tempP = p[0].msub(tempP);
            tempP = Xo.msub(tempP);
            return tempP.magof();
        }

        private void ar_calc_x()
        {
            var b = L.trans();
            inv_Bn = B.inverse();
            var matrix2 = L.mmult(inv_Bn);
            var matrix3 = matrix2.mmult(b).inverse();
            var matrix4 = matrix2.mmult(C);
            matrix4 = Z.msub(matrix4);
            matrix4 = matrix3.mmult(matrix4);
            var matrix5 = b.mmult(matrix4);
            matrix5 = C.madd(matrix5);
            Xo = inv_Bn.mmult(matrix5);
        }

        private void ar_res_check(List<Vector3> CircPoints)
        {
            AveError = MaxError = 0.0;
            for (var i = 0; i < CircPoints.Count; i++)
            {
                p[0].equate(CircPoints[i]);
                var num2 = Math.Abs((double) (ar_calc_radius() - Radius));
                MaxError = Math.Max(MaxError, num2);
                AveError += num2;
            }
            AveError /= Convert.ToDouble(CircPoints.Count);
        }

        private bool Init(List<Vector3> CircPoints, Vector3 PlaneNormal, double PlaneDistance, ref int j)
        {
            int num;
            var num2 = 0.0;
            var di = 0.0;
            j = 0;
            inv_Bn = new Matrix(3, 3);
            A_T = new Matrix(3, 1);
            A_T_A = new Matrix(3, 3);
            A_T_y = new Matrix(3, 1);
            Xo = new Matrix(3, 1);
            Z = new Matrix(1, 1);
            T = new Matrix(PlaneNormal);
            for (num = 0; num < 2; num++)
            {
                p.Add(new Matrix(3, 1));
                N.Add(new Matrix(3, 1));
                d.Add(new Matrix(1, 1));
            }
            for (num = 0; num < 2; num++)
            {
                p[num].equate(CircPoints[num]);
                if (!num.Equals(0))
                {
                    j++;
                    do
                    {
                        N[0] = p[0].msub(p[1]);
                        num2 = N[0].magof();
                        if (num2 < 10.0)
                        {
                            p[num].equate(CircPoints[j]);
                            j++;
                        }
                    }
                    while ((num2 < 10.0) && (j < CircPoints.Count));
                }
            }
            j++;
            if (j >= CircPoints.Count)
            {
                MessageBox.Show("Circle points too close together", "AxisOfRotation Init");
                return false;
            }
            for (num = 0; num < 2; num++)
            {
                N[num] = perp_bisec(ref di);
                d[num].assign(0, 0, di);
                if (!(!num.Equals(0) || next_p(CircPoints, ref j)))
                {
                    MessageBox.Show("Not enough points for circle, need at least 3", "AxisOfRotation Init");
                    return false;
                }
            }
            A = T.trans();
            B = T.mmult(A);
            C = new Matrix(T);
            C.scale(PlaneDistance);
            for (num = 0; num < 2; num++)
            {
                A = N[num].trans();
                A_T_A = N[num].mmult(A);
                B = B.madd(A_T_A);
                tempP = N[num].mmult(d[num]);
                C = C.madd(tempP);
            }
            L = T.trans();
            Z.assign(0, 0, PlaneDistance);
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

        private bool next_p(List<Vector3> CircPoints, ref int j)
        {
            if (j < CircPoints.Count)
            {
                p[0].equate(p[1]);
                p[1].equate(CircPoints[j]);
                j++;
                return true;
            }
            return false;
        }

        private Matrix perp_bisec(ref double di)
        {
            var matrix = p[1].msub(p[0]);
            matrix.Normalize();
            tempP = p[1].madd(p[0]);
            di = 0.5 * matrix.DotProduct(tempP);
            return matrix;
        }
    }
}

