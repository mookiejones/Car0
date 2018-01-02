#if WPF
using System.Windows;
    #else
using System.Windows.Forms;
#endif
using CarZero.Classes;

namespace CarZero
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
   
    internal class Utils
    {
        public static void AddTransToArrayList(ref ArrayList BuildArrList, Transformation fAt, string Header)
        {
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;
            var w = 0.0;
            var p = 0.0;
            var r = 0.0;
            fAt.trans_RPY(ref x, ref y, ref z, ref w, ref p, ref r, true);
            BuildArrList.Add(Header + "," + x.ToString() + "," + y.ToString() + "," + z.ToString() + "," + w.ToString() + "," + p.ToString() + "," + r.ToString());
        }

        public static void AddVec3ListToArrayList(ref ArrayList BuildArrList, List<Vector3> VecLst, string Header)
        {
            BuildArrList.Add(Header);
            for (var i = 0; i < VecLst.Count; i++)
            {
                BuildArrList.Add(VecLst[i].x.ToString() + "," + VecLst[i].y.ToString() + "," + VecLst[i].z.ToString());
            }
        }

        public static void AddVec3ToArrayList(ref ArrayList BuildArrList, Vector3 Vec, string Header)
        {
            BuildArrList.Add(Header + "," + Vec.x.ToString() + "," + Vec.y.ToString() + "," + Vec.z.ToString());
        }

        public static void ArrayListToFile(string FilePath, ArrayList Al, bool Overwrite)
        {
            if (Al.Count > 0)
            {
                using (var writer = new StreamWriter(FilePath, !Overwrite))
                {
                    for (var i = 0; i < Al.Count; i++)
                    {
                        writer.WriteLine(Al[i].ToString());
                    }
                    writer.Flush();
                }
            }
        }

        public static List<bool> CheckJ1toJ3Range(List<Vector> J4data, List<Vector> J5data, List<Vector> J6data, out Vector Range, out Vector Average, double MaxOk)
        {
            int num;
            var list = new List<bool>();
            var list2 = new List<Vector>();
            var b = new Vector(6);
            var vector2 = new Vector(6);
            Average = new Vector(6);
            Range = new Vector(6);
            for (num = 0; num < 6; num++)
            {
                b.Vec[num] = -99999.9;
                vector2.Vec[num] = 99999.9;
            }
            for (num = 0; num < J4data.Count; num++)
            {
                list2.Add(J4data[num]);
            }
            for (num = 0; num < J5data.Count; num++)
            {
                list2.Add(J5data[num]);
            }
            for (num = 0; num < J6data.Count; num++)
            {
                list2.Add(J6data[num]);
            }
            for (num = 0; num < list2.Count; num++)
            {
                Average = Average.Add(list2[num]);
                b = list2[num].Max(b);
                vector2 = list2[num].Min(vector2);
            }
            Average = Average.Scale(1.0 / Convert.ToDouble(list2.Count));
            Range = b.Sub(vector2);
            for (num = 0; num < 3; num++)
            {
                list.Add(Range.Vec[num] <= MaxOk);
            }
            return list;
        }

        public static bool CheckValidTextDoubleConvert(string MyText, out double val)
        {
            val = 0.0;
            try
            {
                val = Convert.ToDouble(MyText);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static List<bool> CheckWristData(List<Vector> Jdata, out Vector Range, out Vector Average, double MaxOk)
        {
            int num;
            var list = new List<bool>();
            var b = new Vector(6);
            var vector2 = new Vector(6);
            Average = new Vector(6);
            Range = new Vector(6);
            for (num = 0; num < 6; num++)
            {
                b.Vec[num] = -99999.9;
                vector2.Vec[num] = 99999.9;
            }
            for (num = 0; num < Jdata.Count; num++)
            {
                Average = Average.Add(Jdata[num]);
                b = Jdata[num].Max(b);
                vector2 = Jdata[num].Min(vector2);
            }
            Average = Average.Scale(1.0 / Convert.ToDouble(Jdata.Count));
            Range = b.Sub(vector2);
            for (num = 0; num < 3; num++)
            {
                list.Add(Range.Vec[num + 3] <= MaxOk);
            }
            return list;
        }

        public static double DegreesToRad(double MyNum)
        {
            return ((MyNum * 3.1415926535897931) / 180.0);
        }

        public static double DoubleFromString(string Instr, string Look4, string EndStr)
        {
            var num = 0.0;
            if (Instr.Contains(Look4))
            {
                var str = Instr.Substring(Instr.IndexOf(Look4) + Look4.Length);
                while (str.StartsWith(" "))
                {
                    str = str.Substring(1);
                }
                if (str.Contains(EndStr))
                {
                    return Convert.ToDouble(str.Substring(0, str.IndexOf(EndStr)));
                }
                MessageBox.Show("Programming error:  Cannot decode '" + Instr + "'", "DoubleFromString");
                return num;
            }
            MessageBox.Show("Programming error:  Cannot find '" + Look4 + "' in '" + Instr + "'", "DoubleFromString");
            return num;
        }

        public static ArrayList FileToArrayList(string FilePath)
        {
            var list = new ArrayList();
            string str = null;
            try
            {
                var reader = new StreamReader(FilePath);
                while ((str = reader.ReadLine()) != null)
                {
                    if (str.IndexOf('#') != 0)
                    {
                        list.Add(str);
                    }
                }
                reader.Close();
            }
            catch (Exception)
            {
            }
            return list;
        }

        public static int IntFromString(string Instr, string Look4)
        {
            var num = 0;
            try
            {
                num = Convert.ToInt32(Instr.Substring(Instr.IndexOf(Look4) + Look4.Length).Replace(" ", ""));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "IntFromString");
            }
            return num;
        }

        public static bool IsPed(int ind)
        {
            switch (RobotData.FrameTypes[ind])
            {
                case FrameType.PedRivetGunTip:
                case FrameType.PedSealGunTip:
                case FrameType.PedSpotGunTip:
                case FrameType.PedScribeGunTip:
                case FrameType.PedStudGunTip:
                    return true;
            }
            return false;
        }

        public static double RadToDegrees(double MyNum)
        {
            return ((MyNum * 180.0) / 3.1415926535897931);
        }

        public static float RadToDegrees(float MyNum)
        {
            return ((MyNum * 180f) / 3.141593f);
        }

        public static string RobotEquivalentStringName(int ind, Brand Rbrand)
        {
            string str = null;
            try
            {
                var name = new AppToolName(RobotData.FrameTypes[ind], Rbrand);
                str = name.Name.Contains("<Robot>") ? name.Name.Replace("<Robot>", "") : name.Name;
                while (str.StartsWith("_"))
                {
                    str = str.Substring(1);
                }
                if (str.Contains("<Num>"))
                {
                    str = str.Replace("<Num>", RobotData.FrameNumbers[ind].ToString());
                }
                if (str.Contains("<Station>"))
                {
                    str = str.Replace("<Station>", RobotData.StationCode + RobotData.RobotStationName);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error building frame name", "RobotEquivalentStringName");
            }
            return str;
        }

        public static string StrFromString(string Instr, string Look4)
        {
            string str = null;
            try
            {
                str = Instr.Substring(Instr.IndexOf(Look4) + Look4.Length).Replace(" ", "");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "StrFromString");
            }
            return str;
        }
    }
}

