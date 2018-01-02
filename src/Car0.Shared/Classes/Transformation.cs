namespace CarZero
{
    using System;

    internal class Transformation
    {
        public double[] mat;

        public Transformation()
        {
            mat = new double[0x10];
        }

        public Transformation(Transformation t)
        {
            mat = new double[0x10];
            for (var i = 0; i < 0x10; i++)
            {
                mat[i] = t.mat[i];
            }
        }

        public Transformation(SpecialTransformType Type)
        {
            mat = new double[0x10];
            if (Type == SpecialTransformType.Identity)
            {
                for (var i = 0; i < 4; i++)
                {
                    mat[5 * i] = 1.0;
                }
            }
        }

        public Transformation(double[] axis, double theta)
        {
            var num = Math.Sin(theta);
            var num2 = Math.Cos(theta);
            var num3 = 1.0 - num2;
            var num4 = axis[0];
            var num5 = axis[1];
            var num6 = axis[2];
            mat = new double[0x10];
            mat[0] = ((num4 * num4) * num3) + num2;
            mat[1] = ((num4 * num5) * num3) - (num6 * num);
            mat[2] = ((num4 * num6) * num3) + (num5 * num);
            mat[4] = ((num4 * num5) * num3) + (num6 * num);
            mat[5] = ((num5 * num5) * num3) + num2;
            mat[6] = ((num5 * num6) * num3) - (num4 * num);
            mat[8] = ((num4 * num6) * num3) - (num6 * num);
            mat[9] = ((num5 * num6) * num3) + (num4 * num);
            mat[10] = ((num6 * num6) * num3) + num2;
        }

        public Transformation(SpecialTransformType Type, double ang)
        {
            mat = new double[0x10];
            var num = Math.Sin(ang);
            var num2 = Math.Cos(ang);
            switch (Type)
            {
                case SpecialTransformType.RotX:
                    SetNx(1.0);
                    SetOy(num2);
                    SetOz(num);
                    SetAy(-num);
                    SetAz(num2);
                    break;

                case SpecialTransformType.RotY:
                    SetNx(num2);
                    SetNz(-num);
                    SetOy(1.0);
                    SetAx(num);
                    SetAz(num2);
                    break;

                case SpecialTransformType.RotZ:
                    SetNx(num2);
                    SetNy(num);
                    SetOx(-num);
                    SetOy(num2);
                    SetAz(1.0);
                    break;
            }
        }

        public Transformation(Vector axis, double theta)
        {
            var num = Math.Sin(theta);
            var num2 = Math.Cos(theta);
            var num3 = 1.0 - num2;
            mat = new double[0x10];
            mat[0] = ((axis.X() * axis.X()) * num3) + num2;
            mat[1] = ((axis.X() * axis.Y()) * num3) - (axis.Z() * num);
            mat[2] = ((axis.X() * axis.Z()) * num3) + (axis.Y() * num);
            mat[4] = ((axis.X() * axis.Y()) * num3) + (axis.Z() * num);
            mat[5] = ((axis.Y() * axis.Y()) * num3) + num2;
            mat[6] = ((axis.Y() * axis.Z()) * num3) - (axis.X() * num);
            mat[8] = ((axis.X() * axis.Z()) * num3) - (axis.Z() * num);
            mat[9] = ((axis.Y() * axis.Z()) * num3) + (axis.X() * num);
            mat[10] = ((axis.Z() * axis.Z()) * num3) + num2;
        }

        public Transformation(double Offset, double Length, double Twist, double InitAngle, double Angle)
        {
            mat = new double[0x10];
            var d = InitAngle + Angle;
            SetNx(Math.Cos(d));
            SetNy(Math.Sin(d));
            SetNz(0.0);
            SetOx(-(Math.Sin(d) * Math.Cos(Twist)));
            SetOy(Math.Cos(d) * Math.Cos(Twist));
            SetOz(Math.Sin(Twist));
            SetAx(Math.Sin(d) * Math.Sin(Twist));
            SetAy(-(Math.Cos(d) * Math.Sin(Twist)));
            SetAz(Math.Cos(Twist));
            SetPx(Length * Math.Cos(d));
            SetPy(Length * Math.Sin(d));
            SetPz(Offset);
        }

        public Transformation(double x, double y, double z, double w, double p, double r)
        {
            mat = new double[0x10];
            SetNx(Math.Cos(p) * Math.Cos(r));
            SetNy(Math.Cos(p) * Math.Sin(r));
            SetNz(-Math.Sin(p));
            SetOx(((Math.Sin(w) * Math.Sin(p)) * Math.Cos(r)) - (Math.Cos(w) * Math.Sin(r)));
            SetOy((Math.Cos(w) * Math.Cos(r)) + ((Math.Sin(w) * Math.Sin(p)) * Math.Sin(r)));
            SetOz(Math.Sin(w) * Math.Cos(p));
            SetAx(((Math.Cos(w) * Math.Sin(p)) * Math.Cos(r)) + (Math.Sin(w) * Math.Sin(r)));
            SetAy(((Math.Cos(w) * Math.Sin(p)) * Math.Sin(r)) - (Math.Sin(w) * Math.Cos(r)));
            SetAz(Math.Cos(w) * Math.Cos(p));
            SetPx(x);
            SetPy(y);
            SetPz(z);
        }

        public void attack_trans(Vector a)
        {
            whatever_trans(a, 2);
        }

        public double Ax()
        {
            return mat[2];
        }

        public double Ay()
        {
            return mat[6];
        }

        public double Az()
        {
            return mat[10];
        }

        public void disp_trans(Vector d)
        {
            for (var i = 0; i < 3; i++)
            {
                mat[(4 * i) + 3] = d.Vec[i];
            }
        }

        public Transformation inv_trans()
        {
            double num;
            var transformation = new Transformation();
            var vector = new Vector(this);
            var r = new Rotation3x3(this).transp();
            transformation.rot_trans(r);
            r = r.Scale(-1.0);
            var d = vector.rdmult(r);
            transformation.disp_trans(d);
            transformation.mat[14] = num = 0.0;
            transformation.mat[12] = transformation.mat[13] = num;
            transformation.mat[15] = 1.0;
            return transformation;
        }

        public Transformation mult_trans(Transformation b)
        {
            double num;
            var transformation = new Transformation();
            var a = new Rotation3x3(this);
            var rotationx2 = new Rotation3x3(b);
            var vector = new Vector(this);
            var vector2 = new Vector(b);
            var r = a.rmult(rotationx2);
            transformation.rot_trans(r);
            var d = vector2.rdmult(a).Add(vector);
            transformation.disp_trans(d);
            transformation.mat[14] = num = 0.0;
            transformation.mat[12] = transformation.mat[13] = num;
            transformation.mat[15] = 1.0;
            return transformation;
        }

        public void norm_trans(Vector n)
        {
            whatever_trans(n, 0);
        }

        public double Nx()
        {
            return mat[0];
        }

        public double Ny()
        {
            return mat[4];
        }

        public double Nz()
        {
            return mat[8];
        }

        public void orient_trans(Vector o)
        {
            whatever_trans(o, 1);
        }

        public double Ox()
        {
            return mat[1];
        }

        public double Oy()
        {
            return mat[5];
        }

        public double Oz()
        {
            return mat[9];
        }

        public void point_trans(Vector p)
        {
            whatever_trans(p, 3);
        }

        public double Px()
        {
            return mat[3];
        }

        public double Py()
        {
            return mat[7];
        }

        public double Pz()
        {
            return mat[11];
        }

        public void rot_trans(Rotation3x3 r)
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    mat[(4 * i) + j] = r.rot[(3 * i) + j];
                }
            }
        }

        public Transformation Scale(double factor)
        {
            var transformation = new Transformation();
            for (var i = 0; i < 12; i++)
            {
                transformation.mat[i] = mat[i] * factor;
            }
            return transformation;
        }

        public void SetAx(double value)
        {
            mat[2] = value;
        }

        public void SetAy(double value)
        {
            mat[6] = value;
        }

        public void SetAz(double value)
        {
            mat[10] = value;
        }

        public void SetNx(double value)
        {
            mat[0] = value;
        }

        public void SetNy(double value)
        {
            mat[4] = value;
        }

        public void SetNz(double value)
        {
            mat[8] = value;
        }

        public void SetOx(double value)
        {
            mat[1] = value;
        }

        public void SetOy(double value)
        {
            mat[5] = value;
        }

        public void SetOz(double value)
        {
            mat[9] = value;
        }

        public void SetPx(double value)
        {
            mat[3] = value;
        }

        public void SetPy(double value)
        {
            mat[7] = value;
        }

        public void SetPz(double value)
        {
            mat[11] = value;
        }

        public void tequate(Transformation a)
        {
            for (var i = 0; i < 0x10; i++)
            {
                mat[i] = a.mat[i];
            }
        }

        public Vector trans_attack()
        {
            return trans_whatever(2);
        }

        public Vector trans_norm()
        {
            return trans_whatever(0);
        }

        public Vector trans_orient()
        {
            return trans_whatever(1);
        }

        public Vector trans_point()
        {
            return trans_whatever(3);
        }

        public void trans_Quaterneon(ref double q1, ref double q2, ref double q3, ref double q4)
        {
            if ((((mat[0] + mat[5]) + mat[10]) + 1.0) < 0.0)
            {
                q1 = 0.0;
            }
            else
            {
                q1 = 0.5 * Math.Sqrt(((mat[0] + mat[5]) + mat[10]) + 1.0);
            }
            if ((((mat[0] - mat[5]) - mat[10]) + 1.0) < 0.0)
            {
                q2 = 0.0;
            }
            else
            {
                q2 = 0.5 * Math.Sqrt(((mat[0] - mat[5]) - mat[10]) + 1.0);
            }
            if ((((-mat[0] + mat[5]) - mat[10]) + 1.0) < 0.0)
            {
                q3 = 0.0;
            }
            else
            {
                q3 = 0.5 * Math.Sqrt(((-mat[0] + mat[5]) - mat[10]) + 1.0);
            }
            if ((((-mat[0] - mat[5]) + mat[10]) + 1.0) < 0.0)
            {
                q4 = 0.0;
            }
            else
            {
                q4 = 0.5 * Math.Sqrt(((-mat[0] - mat[5]) + mat[10]) + 1.0);
            }
            var num = ((mat[9] - mat[6]) < 0.0) ? -1.0 : 1.0;
            var num2 = ((mat[2] - mat[8]) < 0.0) ? -1.0 : 1.0;
            var num3 = ((mat[4] - mat[1]) < 0.0) ? -1.0 : 1.0;
            q2 *= num;
            q3 *= num2;
            q4 *= num3;
        }

        public void trans_RPY(ref double x, ref double y, ref double z, ref double w, ref double p, ref double r)
        {
            x = Px();
            y = Py();
            z = Pz();
            p = Math.Atan2(-Nz(), Math.Sqrt(Math.Pow(Nx(), 2.0) + Math.Pow(Ny(), 2.0)));
            var num = Math.Cos(p);
            if (Math.Abs(num) < 0.001)
            {
                r = 0.0;
                w = Math.Atan2(Ox(), Oy());
                if (Nz() == 1.0)
                {
                    w = -w;
                }
            }
            else
            {
                r = Math.Atan2(Ny() / num, Nx() / num);
                w = Math.Atan2(Oz() / num, Az() / num);
            }
        }

        public void trans_RPY(ref double x, ref double y, ref double z, ref double w, ref double p, ref double r, bool Dummy)
        {
            trans_RPY(ref x, ref y, ref z, ref w, ref p, ref r);
            w = Utils.RadToDegrees(w);
            p = Utils.RadToDegrees(p);
            r = Utils.RadToDegrees(r);
        }

        private Vector trans_whatever(int cl)
        {
            var vector = new Vector(3);
            for (var i = 0; i < 3; i++)
            {
                vector.Vec[i] = mat[(4 * i) + cl];
            }
            return vector;
        }

        private void whatever_trans(Vector n, int min)
        {
            for (var i = 0; i < 3; i++)
            {
                mat[(4 * i) + min] = n.Vec[i];
            }
        }

        public enum SpecialTransformType
        {
            Identity,
            RotX,
            RotY,
            RotZ
        }
    }
}

