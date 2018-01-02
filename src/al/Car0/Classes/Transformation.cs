using System;
using System.Collections.Generic;

using System.Text;

namespace Car0
{
    public class Transformation
    {
        #region Public Variables
        public enum SpecialTransformType { Identity, RotX, RotY, RotZ };
        public double[] mat;
        #endregion
        #region Private Variables
        #endregion
        #region Public Methods
        public Transformation()
        {
            mat = new double[16];
        }
        public Transformation(double[] axis, double theta)              //First grot overload
        {
            double st = Math.Sin(theta), ct = Math.Cos(theta), vt = 1.0 - ct;
            double ax = axis[0], ay = axis[1], az = axis[2];
            mat = new double[16];

            mat[0] = ax * ax * vt + ct;
            mat[1] = ax * ay * vt - az * st;
            mat[2] = ax * az * vt + ay * st;
            mat[4] = ax * ay * vt + az * st;
            mat[5] = ay * ay * vt + ct;
            mat[6] = ay * az * vt - ax * st;
            mat[8] = ax * az * vt - az * st;
            mat[9] = ay * az * vt + ax * st;
            mat[10] = az * az * vt + ct;
        }
        public Transformation(Vector axis, double theta)              //Second grot overload
        {
            double st = Math.Sin(theta), ct = Math.Cos(theta), vt = 1.0 - ct;
            mat = new double[16];

            mat[0] = axis.X() * axis.X() * vt + ct;
            mat[1] = axis.X() * axis.Y() * vt - axis.Z() * st;
            mat[2] = axis.X() * axis.Z() * vt + axis.Y() * st;
            mat[4] = axis.X() * axis.Y() * vt + axis.Z() * st;
            mat[5] = axis.Y() * axis.Y() * vt + ct;
            mat[6] = axis.Y() * axis.Z() * vt - axis.X() * st;
            mat[8] = axis.X() * axis.Z() * vt - axis.Z() * st;
            mat[9] = axis.Y() * axis.Z() * vt + axis.X() * st;
            mat[10] = axis.Z() * axis.Z() * vt + ct;
        }
        public Transformation(SpecialTransformType Type)
        {
            mat = new double[16];

            switch (Type)
            {
                case SpecialTransformType.Identity:

                    int i;

                    for (i = 0; i < 4; ++i)
                        mat[5 * i] = 1.0;

                    break;
            }
        }
        public Transformation(SpecialTransformType Type, double ang)
        {
            mat = new double[16];
            double st = Math.Sin(ang), ct = Math.Cos(ang);

            switch (Type)
            {
                case SpecialTransformType.RotX:

                    SetNx(1.0);
                    SetOy(ct);
                    SetOz(st);
                    SetAy(-st);
                    SetAz(ct);
                    break;

                case SpecialTransformType.RotY:

                    SetNx(ct);
                    SetNz(-st);
                    SetOy(1.0);
                    SetAx(st);
                    SetAz(ct);
                    break;

                case SpecialTransformType.RotZ:

                    SetNx(ct);
                    SetNy(st);
                    SetOx(-st);
                    SetOy(ct);
                    SetAz(1.0);
                    break;
            }
        }
        public Transformation(double x, double y, double z, double w, double p, double r)
        {
            mat = new double[16];

            SetNx(Math.Cos(p) * Math.Cos(r));
            SetNy(Math.Cos(p) * Math.Sin(r));
            SetNz(-Math.Sin(p));

            SetOx(Math.Sin(w) * Math.Sin(p) * Math.Cos(r) - Math.Cos(w) * Math.Sin(r));
            SetOy(Math.Cos(w) * Math.Cos(r) + Math.Sin(w) * Math.Sin(p) * Math.Sin(r));
            SetOz(Math.Sin(w) * Math.Cos(p));

            SetAx(Math.Cos(w) * Math.Sin(p) * Math.Cos(r) + Math.Sin(w) * Math.Sin(r));
            SetAy(Math.Cos(w) * Math.Sin(p) * Math.Sin(r) - Math.Sin(w) * Math.Cos(r));
            SetAz(Math.Cos(w) * Math.Cos(p));

            SetPx(x);
            SetPy(y);
            SetPz(z);
        }
        public Transformation(double Offset, double Length, double Twist, double InitAngle, double Angle)       //link1_trans
        {
            mat = new double[16];
            double angle =  InitAngle + Angle;

            SetNx(Math.Cos(angle));
            SetNy(Math.Sin(angle));
            SetNz(0.0);

            SetOx(-(Math.Sin(angle) * Math.Cos(Twist)));
            SetOy(Math.Cos(angle) * Math.Cos(Twist));
            SetOz(Math.Sin(Twist));

            SetAx(Math.Sin(angle) * Math.Sin(Twist));
            SetAy(-(Math.Cos(angle) * Math.Sin(Twist)));
            SetAz(Math.Cos(Twist));

            SetPx(Length * Math.Cos(angle));
            SetPy(Length * Math.Sin(angle));
            SetPz(Offset);
        }
        public Transformation(Transformation t)
        {
            mat = new double[16];
            int i;

            for (i = 0; i < 16; ++i)
                mat[i] = t.mat[i];
        }



        /* Scale(a, factor) -  Performs scaler matrix mult. a = factor * a.
         *
         *      Inputs:         a       Pointer to (3x4) transformation  a.
         *                      factor  Double precision floating point scaling
         *                              factor that each element of a is multiplied
         *                              by.
         *
         *      Outputs:        a       Elements of a are scaled by factor upon return.
         *
         *      Description:
         *              1)      Scales the elements of a by factor.
         *
         */
        public Transformation Scale(double factor)
        {
            int i;
            Transformation a = new Transformation();

            for (i = 0; i < 12; ++i)
                a.mat[i] = mat[i] * factor;

            return a;
        }




        /* inv_trans() -     Performs inverse of a homogeneous transformation:
         *                      b = this^(-1) where a and b are (3x4) matrices being
         *                      treated as (4x4) homogeneous transformations.
         *
         *                      b       Pointer to (3x4) homogeneous transformation b.
         *
         *      Outputs:        b       Elements of this homogeneous transformation
         *                              set equal to the  result of the matrix
         *                              inverse operation: b = this^(-1).
         *
         *      Description:
         *              1)      Performs matrix transpose operation: b = this^(-1).
         *
         */
        public Transformation inv_trans()
        {
            Transformation b = new Transformation();
            Vector tda = new Vector(this);                 /* Set tda to translational part of a.       */

            Rotation3x3 tra = new Rotation3x3(this);        /* Set tra to rotational part of a.          */

            Rotation3x3 trb = tra.transp();                 /* Set trb = tra^T.                          */

            b.rot_trans(trb);                               /* Set rotational part of b to trb.          */

            trb = trb.Scale(-1.0);

            Vector tdb = tda.rdmult(trb);                   /* Set tdb = -tra^T * tda.                   */

            b.disp_trans(tdb);                              /* Set displacement part of b to tdb.        */

            b.mat[12] = b.mat[13] = b.mat[14] = 0.0;
            b.mat[15] = 1.0;

            return b;
        }


        /* mult_trans(b) - Performs multiplication of transformations c = this X b.
         *
         *      Inputs:         b       Pointer to (3x4) homogeneous transformation.
         *
         *      returns:        c       Elements of this (3x4) homogeneous
         *                              transformation are set equal to the
         *                              result of matrix multiplication: c = this X b.
         *
         *      Description:
         *              1)      Performs the matrix multiplication:  c = this X b; where,
         *                      are , and b & c are (3x4) homogeneous transformations.
         *
         */
        public Transformation mult_trans(Transformation b)
        {
            Transformation c = new Transformation();
            Rotation3x3 tra = new Rotation3x3(this);       /* Set tra to rotational part of a.    */
            Rotation3x3 trb = new Rotation3x3(b);       /* Set trb to rotational part of b.    */
            Vector tda = new Vector(this);                 /* Set tda to translational part of a. */
            Vector tdb = new Vector(b);                 /* Set tda to translational part of b. */

            Rotation3x3 trc = tra.rmult(trb);       /* Set trc = tra * trb.                */

            c.rot_trans(trc);                 /* Set rotational part of c to trc.    */

            Vector tdc = tdb.rdmult(tra);                  /* Set tdc = tra * tdb.                */

            /* Set tdc = tra * tdb + tda                                             */
            tdc = tdc.Add(tda);

            c.disp_trans(tdc);                /* Set displacement part of c to tdc.  */

            c.mat[12] = c.mat[13] = c.mat[14] = 0.0;
            c.mat[15] = 1.0;

            return c;
        }

        /* trans_norm(a,n) -    Assigns the first column (Normal vector part) of this
         *                      transformation matrix a to (3x1) Matrix n.
         *
         *      Inputs:         this    Pointer to (3x4) homogeneous transformation a.
         *
         *      Outputs:        n       Elements of n set equal to the first column
         *                              in a.
         *
         *      Description:
         *              1)      Calls trans_whatever with the index set to 0 so that
         *                      the first column (normal vector) will be
         *                      selected.
         *
         */
        public Vector trans_norm()
        {
            return trans_whatever(0);
        }

        /* trans_orient() -  Assigns the second column (Orientation vector part) of
         *                      transformation matrix a to (3x1) Matrix o.
         *
         *      Inputs:         this    Current transformation.
         *
         *      Returns:        o       Elements of o set equal to the second column
         *                              in a.
         *
         *      Description:
         *              1)      Calls trans_whatever with the index set to 1 so that
         *                      the second column (orientation vector) will be
         *                      selected.
         *
         */
        public Vector trans_orient()
        {
            return trans_whatever(1);
        }

        /* trans_attack() -  Assigns the third column (angle of attack vector part)
         *                      of  homogeneous transformation t to (3x1) Matrix a.
         *
         *      Inputs:         this    Current transformation.
         *
         *      Description:
         *              1)      Calls trans_whatever with the index set to 2 so that
         *                      the third column (angle of attack vector) will be
         *                      selected.
         *              2)      Assigns the elements of the third column of t into
         *                      the elements of a.
         *
         */
        public Vector trans_attack()
        {
            return trans_whatever(2);
        }


        /* trans_point() -   Assigns the fourth column (Displacement vector part) of
         *                      a homogeneous transformation a to (3x1) Matrix p.
         *
         *      Inputs:         this    Current transformation.
         *
         *      Description:
         *              1)      Calls trans_whatever with the index set to 3 so that
         *                      the fourth column (point vector) will be selected.
         *
         */
        public Vector trans_point()
        {
            return trans_whatever(3);
        }


        /* rot_trans(r) -     Assigns elements of (3x3) rotation matrix r to the
         *                      rotational part of (3x4) homogeneous transformation a.
         *
         *      Inputs:         a       Pointer to (3x4) homogeneous transformation a.
         *                      r       Pointer to (3x3) rotation matrix r whose
         *                              elements are to be assigned to the rotational
         *                              part of a.
         *
         *      Outputs:        this       Rotational part of homogeneous transformation this
         *                                  set equal to the elements of r.
         *
         *      Description:
         *              1)      Copys elements of r into rotation portion of a.
         *
         */
        public void rot_trans(Rotation3x3 r)
        {
            int i, j;

            for (i = 0; i < 3; ++i)                      /*  For all rows of r.       */
            {
                for (j = 0; j < 3; ++j)                    /*  For all columns of r.    */
                    mat[4 * i + j] = r.rot[3 * i + j];
            }
        }

        /* disp_trans(d) -    Assigns the elements of the (3x1) displacement vector d
         *                      to the displacement part of the (3x4) homogeneous
         *                      transformation matrix a.
         *
         *      Inputs:         a       Pointer to (3x4) homogeneous transformation a.
         *                      d       Pointer to (3x1) displacement vector d whose
         *                              elements are to be assigned to the displacement
         *                              part of a.
         *
         *      Outputs:        a       Displacement part of homogeneous
         *                              transformation a set equal to the elements of
         *                              d.
         *
         *      Description:
         *              1)      Copys elements of d into displacement portion of a.
         *
         */
        public void disp_trans(Vector d)
        {
            int j;

            for (j = 0; j < 3; ++j)                        /*  For all columns of d.    */
                mat[4 * j + 3] = d.Vec[j];
        }

        /* norm_trans(n,a) -    Assigns the elements of a (3x1) matrix n to the first
         *                      column (Normal vector part) of a homogeneous
         *                      transformation a.
         *
         *      Inputs:         a       Pointer to (3x4) homogeneous transformation a
         *                              whose first column elements are to be assigned
         *                              the values in n.
         *                      n       Pointer to (3x1) matrix whose elements are to
         *                              be assigned to the the first column of a.
         *
         *      Outputs:        a       Elements of the first column of a are set
         *                              equal to the elements of n.
         *
         *      Description:
         *              1)      Calls whatever_trans with index set to zero so that
         *                      the first column (normal vector) will be selected.
         *
         */
        public void norm_trans(Vector n)
        {
            whatever_trans(n, 0);
        }


        /* orient_trans(o,a) -  Assigns the elements of a (3x1) Matrix o to the second
         *                      column (Orientation vector part) of a homogeneous
         *                      transformation a.
         *
         *      Inputs:         a       Pointer to (3x4) homogeneous transformation a
         *                              whose second column elements are to be assigned
         *                              the values in n.
         *                      o       Pointer to (3x1) matrix whose elements are to
         *                              be assigned to the the second column of a.
         *
         *      Outputs:        a       Elements of the second column of a are set
         *                              equal to the elements of o.
         *
         *      Description:
         *              1)      Calls whatever_trans with the index set to 1 so that
         *                      the second column (orientation vector) will be
         *                      selected.
         *
         */
        public void orient_trans(Vector o)
        {
            whatever_trans(o, 1);
        }


        /* attack_trans(a,t) -  Assigns the elements of a (3x1) Matrix a to the third
         *                      column (Angle of attack vector part) of a homogeneous
         *                      transformation t.
         *
         *      Inputs:         t       Pointer to (3x4) homogeneous transformation a
         *                              whose third column elements are to be assigned
         *                              the values in n.
         *                      a       Pointer to (3x1) matrix whose elements are to
         *                              be assigned to the the third column of a.
         *
         *      Outputs:        t       Elements of the third column of t are set
         *                              equal to the elements of a.
         *
         *      Description:
         *              1)      Calls whatever_trans with the index set to 2 so that
         *                      the third column (angle of attack vector) will be
         *                      selected.
         *
         */
        public void attack_trans(Vector a)
        {
            whatever_trans(a, 2);
        }


        /* point_trans(p,a) -   Assigns the elements of a (3x1) Matrix p to the fourth
         *                      column (Angle of attack vector part) of a homogeneous
         *                      transformation a.
         *
         *      Inputs:         a       Pointer to (3x4) homogeneous transformation a
         *                              whose fourth column elements are to be assigned
         *                              the values in n.
         *                      p       Pointer to (3x1) matrix whose elements are to
         *                              be assigned to the the fourth column of a.
         *
         *      Outputs:        a       Elements of the fourth column of a are set
         *                              equal to the elements of p.
         *
         *      Description:
         *              1)      Calls whatever_trans with the index set to 3 so that
         *                      the fourth column (point vector) will be selected.
         *
         */
        public void point_trans(Vector p)
        {
            whatever_trans(p, 3);
        }

        /* tequate(a) -      Sets the elements (3x4) homogeneous transformation this
         *                      equal to the elements of (3x4) homogeneous
         *                      transformation a.
         *
         *      Inputs:         a       Pointer to (3x4) homogeneous transformation a.
         *
         *      Outputs:        this       Elements of this homogeneous transformation
         *                              equal to the elements of homogeneous
         *                              transformation a.
         *
         *      Description:
         *              1)      Performs matrix equation operation: this = a.
         *
         */
        public void tequate(Transformation a)
        {
            int i;

            for (i = 0; i < 16; ++i)
                mat[i] = a.mat[i];
        }

        /* Nx(a)
         * 
         *  Returns a[0]
         */
        public double Nx()
        {
            return mat[0];
        }
        /* Ny(a)
         * 
         *  Returns a[4]
         */
        public double Ny()
        {
            return mat[4];
        }
        /* Nz(a)
         * 
         *  Returns a[8]
         */
        public double Nz()
        {
            return mat[8];
        }
        /* Ox(a)
         * 
         *  Returns a[1]
         */
        public double Ox()
        {
            return mat[1];
        }
        /* Oy(a)
         * 
         *  Returns a[5]
         */
        public double Oy()
        {
            return mat[5];
        }
        /* Oz(a)
         * 
         *  Returns a[9]
         */
        public double Oz()
        {
            return mat[9];
        }
        /* Ax(a)
         * 
         *  Returns a[2]
         */
        public double Ax()
        {
            return mat[2];
        }
        /* Ay(a)
         * 
         *  Returns a[6]
         */
        public double Ay()
        {
            return mat[6];
        }
        /* Az(a)
         * 
         *  Returns a[10]
         */
        public double Az()
        {
            return mat[10];
        }
        /* Px(a)
         * 
         *  Returns a[3]
         */
        public double Px()
        {
            return mat[3];
        }
        /* Py(a)
         * 
         *  Returns a[7]
         */
        public double Py()
        {
            return mat[7];
        }
        /* Pz(a)
         * 
         *  Returns a[11]
         */
        public double Pz()
        {
            return mat[11];
        }

        /* Nx(a)
         * 
         *  Sets a[0]
         */
        public void SetNx(double value)
        {
            mat[0] = value;
        }
        /* Ny(a)
         * 
         *  Sets a[4]
         */
        public void SetNy(double value)
        {
            mat[4] = value;
        }
        /* Nz(a)
         * 
         *  Sets a[8]
         */
        public void SetNz(double value)
        {
            mat[8] = value;
        }
        /* Ox(a)
         * 
         *  Sets a[1]
         */
        public void SetOx(double value)
        {
            mat[1] = value;
        }
        /* Oy(a)
         * 
         *  Sets a[5]
         */
        public void SetOy(double value)
        {
            mat[5] = value;
        }
        /* Oz(a)
         * 
         *  Sets a[9]
         */
        public void SetOz(double value)
        {
            mat[9] = value;
        }
        /* Ax(a)
         * 
         *  Sets a[2]
         */
        public void SetAx(double value)
        {
            mat[2] = value;
        }
        /* Ay(a)
         * 
         *  Sets a[6]
         */
        public void SetAy(double value)
        {
            mat[6] = value;
        }
        /* Az(a)
         * 
         *  Sets a[10]
         */
        public void SetAz(double value)
        {
            mat[10] = value;
        }
        /* Px(a)
         * 
         *  Sets a[3]
         */
        public void SetPx(double value)
        {
            mat[3] = value;
        }
        /* Py(a)
         * 
         *  Sets a[7]
         */
        public void SetPy(double value)
        {
            mat[7] = value;
        }
        /* Pz(a)
         * 
         *  Sets a[11]
         */
        public void SetPz(double value)
        {
            mat[11] = value;
        }

        /* trans_RPY(x, y, z, w, p, r)
         * 
         *      Converts the transformation to roll, pitch, yaw format.
         */
        public void trans_RPY(ref double x, ref double y, ref double z, ref double w, ref double p, ref double r)
        {
            double Cp;

            x = Px();
            y = Py();
            z = Pz();

            p = Math.Atan2(-Nz(), Math.Sqrt(Math.Pow(Nx(), 2) + Math.Pow(Ny(), 2)));

            Cp = Math.Cos(p);

            if (Math.Abs(Cp) < 0.001)
            {
                //Force r = 0
                r = 0;

                w = Math.Atan2(Ox(), Oy());

                if (Nz() == 1.0)
                    w = -w;
            }
            else
            {
                r = Math.Atan2((Ny() / Cp), (Nx() / Cp));
                w = Math.Atan2((Oz() / Cp), (Az() / Cp));
            }
        }
        public void trans_RPY(ref double x, ref double y, ref double z, ref double w, ref double p, ref double r, Boolean Dummy)
        {
            trans_RPY(ref x, ref y, ref z, ref w, ref p, ref r);

            w = Utils.RadToDegrees(w);
            p = Utils.RadToDegrees(p);
            r = Utils.RadToDegrees(r);
        }

        public void trans_Quaterneon(ref double q1, ref double q2, ref double q3, ref double q4)
        {
            if ((mat[0] + mat[5] + mat[10] + 1.0) < 0.0)
                q1 = 0.0;
            else
                q1 = 0.5 * Math.Sqrt(mat[0] + mat[5] + mat[10] + 1.0);

            if ((mat[0] - mat[5] - mat[10] + 1.0) < 0.0)
                q2 = 0.0;
            else
                q2 = 0.5 * Math.Sqrt(mat[0] - mat[5] - mat[10] + 1.0);

            if ((-mat[0] + mat[5] - mat[10] + 1.0) < 0.0)
                q3 = 0.0;
            else
                q3 = 0.5 * Math.Sqrt(-mat[0] + mat[5] - mat[10] + 1.0);

            if ((-mat[0] - mat[5] + mat[10] + 1.0) < 0.0)
                q4 = 0.0;
            else
                q4 = 0.5 * Math.Sqrt(-mat[0] - mat[5] + mat[10] + 1.0);

            double sign_q2 = ((mat[9] - mat[6]) < 0.0) ? -1.0 : 1.0;
            double sign_q3 = ((mat[2] - mat[8]) < 0.0) ? -1.0 : 1.0;
            double sign_q4 = ((mat[4] - mat[1]) < 0.0) ? -1.0 : 1.0;

            q2 *= sign_q2;
            q3 *= sign_q3;
            q4 *= sign_q4;
        }
        #endregion
        #region Private Methods

        /* trans_whatever(a,n,cl) -     Assigns the cl.th column of a transformation
         *                              matrix a to (3x1) Matrix n.
         *
         *      Inputs:         a       Pointer to (3x4) homogeneous transformation a.
         *                      n       Pointer to (3x1) matrix whose elements are to
         *                              be assigned the values of the first column
         *                              in a.
         *                      cl      Column number to be assigned to the matrix n.
         *
         *      Outputs:        n       Elements of n set equal to the cl.th column
         *                              in a.
         *
         *      Description:
         *              1)      Calls check_3x1 to insure that n has dimension (3x1).
         *              2)      Assigns the elements of the cl.th column of a into
         *                      the elements of n.
         *
         */
        private Vector trans_whatever(int cl)
        {
            int j;
            Vector n = new Vector(3);

            for (j = 0; j < 3; ++j)                /*  For all rows of n.               */
                n.Vec[j] = mat[4 * j + cl];

            return n;
        }

        /* whatever_trans(n,a,in) -     Assigns the elements of a (3x1) matrix n to
         *                              the in.th column of the homogeneous
         *                              transformation a.
         *
         *      Inputs:         a       Pointer to (3x4) homogeneous transformation a
         *                              whose first column elements are to be assigned
         *                              the values in n.
         *                      n       Pointer to (3x1) matrix whose elements are to
         *                              be assigned to the the first column of a.
         *                      min      The column number to be assigned.
         *
         *
         *      Outputs:        a       Elements of the first column of a are set
         *                              equal to the elements of n.
         *
         *      Description:
         *              1)      Calls check_3x1 to insure that n has dimension (3x1).
         *              2)      Assigns the elements of n to the elements of the in.th
         *                      column of a.
         *
         */
        private void whatever_trans(Vector n, int min)
        {
            int j;

            for (j = 0; j < 3; ++j)                /*  For all rows of n.               */
                mat[4 * j + min] = n.Vec[j];
        }
        #endregion
    }
}
