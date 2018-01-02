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
   
    internal class FrameRoutines
    {
        public const int A_Coord = 3;
        public const int B_Coord = 4;
        public const int C_Coord = 5;
        public const double CoincidenceLimit = 100.0;
        public const double MinATAN2Arg = 0.001;
        public const int X_Coord = 0;
        public const int Y_Coord = 1;
        public const int Z_Coord = 2;

        public static int ComputeABCFromNSA(double[] NVector, double[] SVector, double[] AVector, ref double[] ResultFrame)
        {
            if ((Math.Abs(AVector[0]) < 0.001) && (Math.Abs(AVector[2]) < 0.001))
            {
                ResultFrame[3] = 0.0;
                if (NVector[2] < 0.0)
                {
                    ResultFrame[4] = 90.0;
                    ResultFrame[5] = Utils.RadToDegrees(Math.Atan2(SVector[0], AVector[0]));
                }
                else
                {
                    ResultFrame[4] = -90.0;
                    ResultFrame[5] = Utils.RadToDegrees(Math.Atan2(-SVector[0], -AVector[0]));
                }
                return 1;
            }
            ResultFrame[3] = Utils.RadToDegrees(Math.Atan2(NVector[1], NVector[0]));
            if ((Math.Abs(SVector[2]) < 0.001) && (Math.Abs(AVector[2]) < 0.001))
            {
                ResultFrame[3] = 0.0;
                if (NVector[2] < 0.0)
                {
                    ResultFrame[4] = 90.0;
                    ResultFrame[5] = Utils.RadToDegrees(Math.Atan2(SVector[0], AVector[0]));
                }
                else
                {
                    ResultFrame[4] = -90.0;
                    ResultFrame[5] = Utils.RadToDegrees(Math.Atan2(-SVector[0], -AVector[0]));
                }
                return 1;
            }
            ResultFrame[5] = Utils.RadToDegrees(Math.Atan2(SVector[2], AVector[2]));
            var num = Math.Cos(ResultFrame[5]);
            var num2 = Math.Sin(ResultFrame[5]);
            ResultFrame[4] = Utils.RadToDegrees(Math.Atan2(-NVector[2], (num2 * SVector[2]) + (num * AVector[2])));
            return 0;
        }

        public static int ComputeFrameFrom321Pts(double[] Origin, double[] Xplus, double[] Yplus, ref double[] wAcar0)
        {
            int num;
            var d = 0.0;
            var numArray = new double[3];
            var nVector = new double[3];
            var sVector = new double[3];
            var aVector = new double[3];
            wAcar0[0] = Origin[0];
            wAcar0[1] = Origin[1];
            wAcar0[2] = Origin[2];
            numArray[0] = Xplus[0] - Origin[0];
            numArray[1] = Xplus[1] - Origin[1];
            numArray[2] = Xplus[2] - Origin[2];
            for (num = 0; num < 3; num++)
            {
                d += numArray[num] * numArray[num];
            }
            d = Math.Sqrt(d);
            if (d < 100.0)
            {
                return 1;
            }
            for (num = 0; num < 3; num++)
            {
                nVector[num] = numArray[num] / d;
            }
            numArray[0] = Yplus[0] - Origin[0];
            numArray[1] = Yplus[1] - Origin[1];
            numArray[2] = Yplus[2] - Origin[2];
            d = 0.0;
            for (num = 0; num < 3; num++)
            {
                d += numArray[num] * numArray[num];
            }
            if (Math.Sqrt(d) < 100.0)
            {
                return 1;
            }
            d = ((nVector[0] * numArray[0]) + (nVector[1] * numArray[1])) + (nVector[2] * numArray[2]);
            for (num = 0; num < 3; num++)
            {
                numArray[num] -= nVector[num] * d;
            }
            d = 0.0;
            for (num = 0; num < 3; num++)
            {
                d += numArray[num] * numArray[num];
            }
            d = Math.Sqrt(d);
            if (d < 100.0)
            {
                return 2;
            }
            for (num = 0; num < 3; num++)
            {
                sVector[num] = numArray[num] / d;
            }
            aVector[0] = (nVector[1] * sVector[2]) - (nVector[2] * sVector[1]);
            aVector[1] = (nVector[2] * sVector[0]) - (nVector[0] * sVector[2]);
            aVector[2] = (nVector[0] * sVector[0]) - (nVector[1] * sVector[0]);
            return ComputeABCFromNSA(nVector, sVector, aVector, ref wAcar0);
        }

        public static void cs_change(ref Vector point, Transformation xform)
        {
            var b = new Transformation(Transformation.SpecialTransformType.Identity);
            b.point_trans(point);
            point = xform.mult_trans(b).trans_point();
        }

        public static void cs_change(ref Vector3 point, Transformation xform)
        {
            var vector = new Vector(point);
            cs_change(ref vector, xform);
            point = new Vector3(vector);
        }

        public static void get_bac(List<Vector> tdata, ref Vector t3x1, Transformation Cap, ref Transformation Bac, ref Transformation bap)
        {
            var n = new Vector(3);
            var p = new Vector(3);
            var vector3 = new Vector(3);
            for (var i = 0; i < 3; i++)
            {
                p.Vec[i] = tdata[0].Vec[i];
                vector3.Vec[i] = tdata[1].Vec[i];
            }
            rfgp(tdata, ref n);
            bap.point_trans(p);
            bap.attack_trans(n);
            t3x1 = vector3.Sub(p).normalize();
            bap.norm_trans(t3x1);
            t3x1 = n.CrossProduct(t3x1).normalize();
            bap.orient_trans(t3x1);
            Bac = Cap.inv_trans();
            Bac = bap.mult_trans(Bac);
        }

        public static Vector get_dk(List<Vector> rx, List<Vector> tx, ref Vector t3x1, Transformation A, ref double sm)
        {
            var vector = new Vector(3);
            var b = new Vector(3);
            sm = 0.0;
            for (var i = 0; i < rx.Count; i++)
            {
                t3x1 = t3x1.ListRowEquate(tx, i);
                cs_change(ref t3x1, A);
                vector = vector.ListRowEquate(rx, i);
                sm += vector.magof() * t3x1.magof();
                t3x1 = vector.CrossProduct(t3x1);
                b = t3x1.Add(b);
            }
            t3x1 = b.Scale(1.0 / sm);
            sm = Math.Asin(t3x1.magof());
            if (sm > 1E-06)
            {
                b = b.normalize();
            }
            return b;
        }

        private static Vector get_dp(List<Vector> rx, List<Vector> tx, Transformation A, ref Vector t3x1)
        {
            var vector = new Vector(3);
            var b = new Vector(3);
            for (var i = 0; i < rx.Count; i++)
            {
                t3x1 = t3x1.ListRowEquate(tx, i);
                cs_change(ref t3x1, A);
                t3x1 = vector.ListRowEquate(rx, i).Sub(t3x1);
                b = t3x1.Add(b);
            }
            return b.Scale(1.0 / Convert.ToDouble(rx.Count));
        }

        public static Transformation get_model(List<Vector> tdpts, int NumPts, ref Vector t3x1)
        {
            var n = new Vector(3);
            var p = new Vector(3);
            var vector3 = new Vector(3);
            var transformation = new Transformation(Transformation.SpecialTransformType.Identity);
            rfgp(tdpts, ref n);
            for (var i = 0; i < 3; i++)
            {
                p.Vec[i] = tdpts[0].Vec[i];
                vector3.Vec[i] = tdpts[1].Vec[i];
            }
            transformation.point_trans(p);
            transformation.attack_trans(n);
            t3x1 = vector3.Sub(p).normalize();
            transformation.norm_trans(t3x1);
            t3x1 = n.CrossProduct(t3x1).normalize();
            transformation.orient_trans(t3x1);
            return transformation;
        }

        public static Transformation j4Atf(double[] WristPars, Vector3 vjas)
        {
            var jas = new double[] { vjas.x, vjas.y, vjas.z };
            return j4Atf(WristPars, jas);
        }

        public static Transformation j4Atf(double[] WristPars, double[] jas)
        {
            var transformation = new Transformation(WristPars[0], WristPars[1], WristPars[2], WristPars[3], jas[0]);
            var b = new Transformation(WristPars[4], WristPars[5], WristPars[6], WristPars[7], jas[1]);
            var transformation3 = new Transformation(WristPars[8], WristPars[9], WristPars[10], WristPars[11], jas[2]);
            return transformation.mult_trans(b).mult_trans(transformation3);
        }

        public static Transformation j4Atf(double[] WristPars, Vector3 vjas, bool dummy)
        {
            var jas = new double[] { Utils.DegreesToRad(vjas.x), Utils.DegreesToRad(vjas.y), Utils.DegreesToRad(vjas.z) };
            return j4Atf(WristPars, jas);
        }

        public static void okyet(List<Vector> rx, List<Vector> tx, ref Vector t3x1, Transformation A, ref int worst, ref double Ew, ref double se)
        {
            var num2 = 0.0;
            var vector = new Vector(3);
            Ew = se = 0.0;
            for (var i = 0; i < rx.Count; i++)
            {
                t3x1 = t3x1.ListRowEquate(tx, i);
                cs_change(ref t3x1, A);
                t3x1 = vector.ListRowEquate(rx, i).Sub(t3x1);
                num2 = t3x1.magof();
                se += num2;
                if (num2 >= Ew)
                {
                    Ew = num2;
                    worst = i + 1;
                }
            }
            se /= Convert.ToDouble(rx.Count);
        }

        public static List<Vector3> PointSetCsChange(Transformation toAfrom, List<Vector3> fromVecSet)
        {
            return PointSetCsChange(toAfrom, fromVecSet, false);
        }

        public static List<Vector3> PointSetCsChange(Transformation toAfrom, List<Vector3> fromVecSet, bool SkipZeroVectors)
        {
            var list = new List<Vector3>();
            for (var i = 0; i < fromVecSet.Count; i++)
            {
                Vector3 vector;
                if (((SkipZeroVectors && fromVecSet[i].x.Equals((double) 0.0)) && fromVecSet[i].y.Equals((double) 0.0)) && fromVecSet[i].z.Equals((double) 0.0))
                {
                    vector = new Vector3();
                }
                else
                {
                    vector = new Vector3(fromVecSet[i]);
                    cs_change(ref vector, toAfrom);
                }
                list.Add(vector);
            }
            return list;
        }

        public static List<Vector3> PointSetJointTransform(List<Vector3> j4Measurements, List<Vector3> jaVecs, double[] WristPars)
        {
            return PointSetJointTransform(j4Measurements, jaVecs, WristPars, false);
        }

        public static List<Vector3> PointSetJointTransform(List<Vector3> j4Measurements, List<Vector3> jaVecs, double[] WristPars, bool SkipZeroVectors)
        {
            var list = new List<Vector3>();
            for (var i = 0; i < jaVecs.Count; i++)
            {
                Vector3 vector;
                if (((SkipZeroVectors && j4Measurements[i].x.Equals((double) 0.0)) && j4Measurements[i].y.Equals((double) 0.0)) && j4Measurements[i].z.Equals((double) 0.0))
                {
                    vector = new Vector3();
                }
                else
                {
                    var xform = j4Atf(WristPars, jaVecs[i], true).inv_trans();
                    vector = new Vector3(j4Measurements[i]);
                    cs_change(ref vector, xform);
                }
                list.Add(vector);
            }
            return list;
        }

        public static bool ReadDefinerBaseData(string RobotRootPath, int ToolNum, ref double[] FrameDef, Brand.RobotBrands Kind)
        {
            if (Kind == Brand.RobotBrands.KUKA)
            {
                return Kuka.ReadDefinerBaseData(RobotRootPath, ToolNum, ref FrameDef);
            }
            MessageBox.Show("Robot brand not supported.", "ReadDefinerBaseData");
            return false;
        }

        public static bool ReadDefinerPedData(string RobotRootPath, int ToolNum, ref double[] FrameDef, Brand.RobotBrands Kind)
        {
            switch (Kind)
            {
                case Brand.RobotBrands.ABB:
                case Brand.RobotBrands.Nachi:
                    return true;

                case Brand.RobotBrands.KUKA:
                    return Kuka.ReadDefinerBaseData(RobotRootPath, ToolNum, ref FrameDef);
            }
            MessageBox.Show("Robot brand not supported.", "ReadDefinerPedData");
            return false;
        }

        public static bool ReadDefinerToolData(string RobotRootPath, int ToolNum, ref double[] FrameDef, Brand.RobotBrands Kind)
        {
            switch (Kind)
            {
                case Brand.RobotBrands.ABB:
                case Brand.RobotBrands.Nachi:
                    return true;

                case Brand.RobotBrands.Fanuc:
                case Brand.RobotBrands.FanucRJ:
                    return Fanuc.ReadDefinerToolData(RobotRootPath, ToolNum, ref FrameDef);

                case Brand.RobotBrands.KUKA:
                    return Kuka.ReadDefinerToolData(RobotRootPath, ToolNum, ref FrameDef);
            }
            MessageBox.Show("Robot brand not supported.", "ReadDefinerToolData");
            return false;
        }

        public static void rfgp(List<Vector> tdpts, ref Vector N)
        {
            var vector = new Vector(3);
            var b = new Vector(3);
            var vector3 = new Vector(3);
            for (var i = 0; i < 3; i++)
            {
                vector.Vec[i] = tdpts[2].Vec[i];
                b.Vec[i] = tdpts[1].Vec[i];
                vector3.Vec[i] = tdpts[0].Vec[i];
            }
            var vector4 = b.Sub(vector3);
            var vector5 = vector.Sub(b);
            N = vector4.CrossProduct(vector5).normalize();
        }

        public static bool where(List<Vector> rfpts, List<Vector> tdpts, ref Transformation rA0, ref double AveError, ref double Ew, ref int worst, ref ArrayList Problems)
        {
            var num = 0;
            var num2 = 0;
            var flag = false;
            var sm = 0.0;
            var num4 = 999999.0;
            var vector = new Vector(3);
            var p = new Vector(3);
            var bap = new Transformation();
            var numArray = new double[6];
            Ew = AveError = 0.0;
            worst = 0;
            var cap = get_model(rfpts, 3, ref vector);
            get_bac(tdpts, ref vector, cap, ref rA0, ref bap);
            rA0 = rA0.inv_trans();
            do
            {
                bap.tequate(rA0);
                num++;
                p = get_dp(rfpts, tdpts, rA0, ref vector);
                cap = new Transformation(Transformation.SpecialTransformType.Identity);
                cap.point_trans(p);
                rA0 = cap.mult_trans(rA0);
                cap = new Transformation(get_dk(rfpts, tdpts, ref vector, rA0, ref sm), -sm);
                vector = cap.trans_norm();
                p = cap.trans_orient();
                vector = vector.normalize();
                p = p.normalize();
                cap.norm_trans(vector);
                cap.orient_trans(p);
                p = vector.CrossProduct(p).normalize();
                cap.attack_trans(p);
                rA0 = cap.mult_trans(rA0);
                Ew = 0.0;
                okyet(rfpts, tdpts, ref vector, rA0, ref worst, ref Ew, ref AveError);
                flag = Math.Abs((double) (num4 - AveError)) < 1E-06;
                if (flag || (Math.Abs(AveError) > Math.Abs(num4)))
                {
                    if (!flag)
                    {
                        rA0.tequate(bap);
                        AveError = num4;
                    }
                    flag = true;
                }
                num4 = AveError;
            }
            while (!flag || (++num2 > 100));
            if (num2 > 100)
            {
                Problems.Add("Transformation fit failed to converge");
                return false;
            }
            return true;
        }

        public static void WriteDefiner(string RobotRootPath, string FrameName, double[] FrameDef, ref int LinesWritten, bool WriteFrame, bool WriteEndStatement, Brand Kind, int ind, ref ArrayList Problems)
        {
            switch (Kind.Company)
            {
                case Brand.RobotBrands.ABB:
                    ABB.WriteDefiner(RobotRootPath, FrameDef, ref LinesWritten, WriteFrame, Kind, ind, ref Problems);
                    break;

                case Brand.RobotBrands.Fanuc:
                    Fanuc.WriteDefiner(RobotRootPath, FrameName, FrameDef, ref LinesWritten, WriteFrame, false, ref Problems);
                    break;

                case Brand.RobotBrands.FanucRJ:
                    Fanuc.WriteDefiner(RobotRootPath, FrameName, FrameDef, ref LinesWritten, WriteFrame, true, ref Problems);
                    break;

                case Brand.RobotBrands.KUKA:
                    Kuka.WriteDefiner(RobotRootPath, FrameName, FrameDef, ref LinesWritten, WriteFrame, WriteEndStatement, ref Problems);
                    break;

                case Brand.RobotBrands.Nachi:
                    Nachi.WriteDefiner(RobotRootPath, FrameName, FrameDef, ref LinesWritten, WriteFrame, ref Problems);
                    break;

                default:
                    Problems.Add("Robot brand not supported detected in: WriteDefiner");
                    break;
            }
        }
    }
}

