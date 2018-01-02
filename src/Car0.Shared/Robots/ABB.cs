 
#if WPF
using System.Windows;
    #else
using System.Windows.Forms;
#endif
using CarZero.Classes;
using CarZero.Robots;

namespace CarZero
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;


    public class ABB:IRobot
    {
        private static bool CheckedDecoderRing = false;
        private static string MyCar0BasePtsProgram = "mycar0pts.mod";
        private static string MyCar0Definer = "C0Define.mod";
        private static string MyCar0JointPosesProgram = "Czp.mod";
        private static string MyDefinerCarriedGunTcpfFormat = "GunData<ToolNumber>";
        private static string MyDefinerGripperPinTcpfFormat = "GripData<ToolNumber>";
        private static string MyDefinerPedGunTipFormat = "PedTip<ToolNumber>";
        private static string MyDefinerWobjFormat = "Wobj_<RobotName>_<WobjNumber>";

        private static string BuildFrameCoordsString(double[] FrameDef, bool WobjDataRecDetected, string EndRec)
        {
            var num = 0.0;
            var num2 = 0.0;
            var num3 = 0.0;
            var num4 = 0.0;
            if (WobjDataRecDetected)
            {
                new Transformation(FrameDef[0], FrameDef[1], FrameDef[2], Utils.DegreesToRad(FrameDef[3]), Utils.DegreesToRad(FrameDef[4]), Utils.DegreesToRad(FrameDef[5])).trans_Quaterneon(ref num, ref num2, ref num3, ref num4);
                return (FrameDef[0].ToString("F3") + "," + FrameDef[1].ToString("F3") + "," + FrameDef[2].ToString("F3") + EndRec + num.ToString("F7") + "," + num2.ToString("F7") + "," + num3.ToString("F7") + "," + num4.ToString("F7"));
            }
            return (FrameDef[0].ToString("F3") + "," + FrameDef[1].ToString("F3") + "," + FrameDef[2].ToString("F3"));
        }

        private static string Car0BasePtsProgramName()
        {
            CheckDecoderRing();
            return MyCar0BasePtsProgram;
        }

        private static string Car0DefinerName()
        {
            CheckDecoderRing();
            return MyCar0Definer;
        }

        private static string Car0JointPosesProgram()
        {
            CheckDecoderRing();
            return MyCar0JointPosesProgram;
        }

        private static void CheckDecoderRing()
        {
            if (!CheckedDecoderRing)
            {
                try
                {
                    var path = Path.Combine(UserPreferences.WorkFolderName, "DecoderRing.txt");
                    if (File.Exists(path))
                    {
                        var list = Utils.FileToArrayList(path);
                        for (var i = 0; i < list.Count; i++)
                        {
                            if (list[i].ToString().StartsWith("Car0BasePtsProgram") && list[i].ToString().Contains(","))
                            {
                                MyCar0BasePtsProgram = list[i].ToString().Substring(list[i].ToString().IndexOf(",") + 1);
                            }
                            else if (list[i].ToString().StartsWith("Car0Definer") && list[i].ToString().Contains(","))
                            {
                                MyCar0Definer = list[i].ToString().Substring(list[i].ToString().IndexOf(",") + 1);
                            }
                            else if (list[i].ToString().StartsWith("DefinerCarriedGunTcpfFormat") && list[i].ToString().Contains(","))
                            {
                                MyDefinerCarriedGunTcpfFormat = list[i].ToString().Substring(list[i].ToString().IndexOf(",") + 1);
                            }
                            else if (list[i].ToString().StartsWith("DefinerGripperPinTcpfFormat") && list[i].ToString().Contains(","))
                            {
                                MyDefinerGripperPinTcpfFormat = list[i].ToString().Substring(list[i].ToString().IndexOf(",") + 1);
                            }
                            else if (list[i].ToString().StartsWith("DefinerPedGunTipFormat") && list[i].ToString().Contains(","))
                            {
                                MyDefinerPedGunTipFormat = list[i].ToString().Substring(list[i].ToString().IndexOf(",") + 1);
                            }
                            else if (list[i].ToString().StartsWith("DefinerWobjFormat") && list[i].ToString().Contains(","))
                            {
                                MyDefinerWobjFormat = list[i].ToString().Substring(list[i].ToString().IndexOf(",") + 1);
                            }
                            else if (list[i].ToString().StartsWith("Car0JointPosesProgram") && list[i].ToString().Contains(","))
                            {
                                MyCar0JointPosesProgram = list[i].ToString().Substring(list[i].ToString().IndexOf(",") + 1);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                CheckedDecoderRing = true;
            }
        }

        private static string DefinerCarriedGunTcpfFormat()
        {
            CheckDecoderRing();
            return MyDefinerCarriedGunTcpfFormat;
        }

        private static string DefinerGripperPinTcpfFormat()
        {
            CheckDecoderRing();
            return MyDefinerGripperPinTcpfFormat;
        }

        private static string DefinerPedGunTipFormat()
        {
            CheckDecoderRing();
            return MyDefinerPedGunTipFormat;
        }

        private static string DefinerWobjFormat()
        {
            CheckDecoderRing();
            return MyDefinerWobjFormat;
        }

        private static bool EndModuleRecordFound(ArrayList Items)
        {
            int num;
            return EndModuleRecordFound(Items, out num);
        }

        private static bool EndModuleRecordFound(ArrayList Items, out int i)
        {
            i = 0;
            while (i < Items.Count)
            {
                if (Items[i].ToString().Contains("ENDMODULE"))
                {
                    return true;
                }
                i++;
            }
            return false;
        }

        private static bool FoundExactMatchRecord(ArrayList Items, string TypeString, string MatchString, out int i)
        {
            i = 0;
            while (i < Items.Count)
            {
                if (Items[i].ToString().Contains(TypeString) && Items[i].ToString().Contains(MatchString))
                {
                    var index = Items[i].ToString().IndexOf(MatchString);
                    var str = Items[i].ToString().Substring(index - 1);
                    if (str.StartsWith(" "))
                    {
                        var str2 = str.Substring(MatchString.Length + 1);
                        if (str2.StartsWith(" ") || str2.StartsWith(":="))
                        {
                            return true;
                        }
                    }
                }
                i++;
            }
            return false;
        }

        private static bool FoundMatchingType(ArrayList Items, string TypeString, out int LastOfType)
        {
            var flag = false;
            LastOfType = -1;
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].ToString().Contains(TypeString))
                {
                    flag = true;
                    LastOfType = i;
                }
            }
            return flag;
        }

        public static List<Vector3> ReadCar0BasePtsProgram(string RobotRootPath, ref ArrayList Problems)
        {
            var list = new List<Vector3>();
            var path = Path.Combine(RobotRootPath, Car0BasePtsProgramName());
            if (File.Exists(path))
            {
                try
                {
                    var list2 = Utils.FileToArrayList(path);
                    var vector = new Vector3();
                    string str2 = null;
                    for (var i = 0; i < list2.Count; i++)
                    {
                        if (((list2[i].ToString().Contains("LOCAL") && list2[i].ToString().Contains("CONST")) && list2[i].ToString().Contains("robtarget")) && list2[i].ToString().Contains(":=[["))
                        {
                            str2 = list2[i].ToString().Substring(list2[i].ToString().IndexOf(":=[[") + 4);
                            if (str2.Contains("]"))
                            {
                                var strArray = str2.Substring(0, str2.IndexOf("]")).Split(new char[] { ',' });
                                var list3 = new List<double>();
                                for (var j = 0; j < strArray.Length; j++)
                                {
                                    if (strArray[j].Length > 0)
                                    {
                                        list3.Add(Convert.ToDouble(strArray[j]));
                                    }
                                }
                                if (strArray.Length >= 3)
                                {
                                    list.Add(new Vector3(list3[0], list3[1], list3[2]));
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Problems.Add("ABB.ReadCar0BasePtsProgram experienced exception below: \n" + exception.Message);
                    return new List<Vector3>();
                }
                return list;
            }
            Problems.Add("Reference points program not found (" + path + ")");
            return list;
        }
        public void  ReadTargets(string path, List<Vector> j4, List<Vector> j5, List<Vector> j6,
            List<string> problems)
        {

        }
        public static void ReadCar0J456Targets(string RobotRootPath, out List<Vector> J4Targets, out List<Vector> J5Targets, out List<Vector> J6Targets, ref ArrayList Problems)
        {
            string str = null;
            string str2 = null;
            J4Targets = new List<Vector>();
            J5Targets = new List<Vector>();
            J6Targets = new List<Vector>();
            var path = Path.Combine(RobotRootPath, Car0JointPosesProgram());
            if (!File.Exists(path))
            {
                Problems.Add(Car0JointPosesProgram() + " not found in robot folder: " + RobotRootPath);
            }
            else
            {
                try
                {
                    foreach (string str4 in Utils.FileToArrayList(path))
                    {
                        if (str4.Contains("PERS jointtarget"))
                        {
                            str2 = str4.Substring(str4.IndexOf("PERS jointtarget") + 0x11);
                            if (!(str2.Contains(":=") && str2.Contains("[[")))
                            {
                                Problems.Add("Cannot decode tool definition line: '" + str4 + "'");
                                return;
                            }
                            str2 = str2.Substring(0, str2.IndexOf(":=")).Replace(" ", "");
                            str = str4.Substring(str4.IndexOf("[[") + 2);
                            if (!str.Contains("]"))
                            {
                                Problems.Add("Cannot decode tool definition line: '" + str4 + "'");
                                return;
                            }
                            str = str.Substring(0, str.IndexOf("]"));
                            if (str2.StartsWith("J4_"))
                            {
                                J4Targets.Add(new Vector(VectorFromString(str)));
                            }
                            else if (str2.StartsWith("J5_"))
                            {
                                J5Targets.Add(new Vector(VectorFromString(str)));
                            }
                            else if (str2.StartsWith("J6_"))
                            {
                                J6Targets.Add(new Vector(VectorFromString(str)));
                            }
                        }
                    }
                    if (J4Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 4 targets were found in :" + Car0JointPosesProgram());
                    }
                    if (J5Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 5 targets were found in :" + Car0JointPosesProgram());
                    }
                    if (J6Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 6 targets were found in :" + Car0JointPosesProgram());
                    }
                }
                catch (Exception exception)
                {
                    Problems.Add(exception.Message);
                }
            }
        }

        public static void SetJ123Values(string RobotRootPath, Vector J123Values, ref ArrayList Problems)
        {
            var path = Path.Combine(RobotRootPath, Car0JointPosesProgram());
            if (File.Exists(path))
            {
                try
                {
                    var al = Utils.FileToArrayList(path);
                    var vector = new Vector3();
                    string str2 = null;
                    string str3 = null;
                    string str = null;
                    for (var i = 0; i < al.Count; i++)
                    {
                        str2 = al[i].ToString();
                        if (str2.Contains("PERS jointtarget"))
                        {
                            str3 = str2.Substring(str2.IndexOf("PERS jointtarget") + 0x11);
                            if (!(str3.Contains(":=") && str3.Contains("[[")))
                            {
                                Problems.Add("Cannot decode tool definition line: '" + str2 + "'");
                                return;
                            }
                            str3 = str3.Substring(0, str3.IndexOf(":=")).Replace(" ", "");
                            str = str2.Substring(str2.IndexOf("[[") + 2);
                            if (!str.Contains("]"))
                            {
                                Problems.Add("Cannot decode tool definition line: '" + str2 + "'");
                                return;
                            }
                            str = str.Substring(0, str.IndexOf("]"));
                            if ((str3.StartsWith("J4_") || str3.StartsWith("J5_")) || str3.StartsWith("J6_"))
                            {
                                var vector2 = VectorFromString(str);
                                var str5 = str2.Substring(0, str2.IndexOf("[[") + 2);
                                var str6 = str2.Substring(str2.IndexOf("]"));
                                var strArray = new string[11];
                                var num2 = J123Values.Vec[0];
                                strArray[0] = num2.ToString("F3");
                                strArray[1] = ",";
                                num2 = J123Values.Vec[1];
                                strArray[2] = num2.ToString("F3");
                                strArray[3] = ",";
                                num2 = J123Values.Vec[2];
                                strArray[4] = num2.ToString("F3");
                                strArray[5] = ",";
                                num2 = vector2.Vec[3];
                                strArray[6] = num2.ToString("F3");
                                strArray[7] = ",";
                                num2 = vector2.Vec[4];
                                strArray[8] = num2.ToString("F3");
                                strArray[9] = ",";
                                strArray[10] = vector2.Vec[5].ToString("F3");
                                str = string.Concat(strArray);
                                al[i] = str5 + str + str6;
                            }
                        }
                    }
                    Utils.ArrayListToFile(path, al, true);
                }
                catch (Exception exception)
                {
                    Problems.Add("ABB.SetJ123Values experienced exception below: \n" + exception.Message);
                }
            }
            else
            {
                Problems.Add(Car0JointPosesProgram() + " not found in robot folder: " + RobotRootPath);
            }
        }

        public static void SetSingleJointAxisValue(string RobotRootPath, double value, int MovingJointNumber, int AxisIndx, ref ArrayList Problems)
        {
            var path = Path.Combine(RobotRootPath, Car0JointPosesProgram());
            if (File.Exists(path))
            {
                try
                {
                    var al = Utils.FileToArrayList(path);
                    var vector = new Vector3();
                    string str2 = null;
                    string str3 = null;
                    string str = null;
                    var str5 = "J" + MovingJointNumber.ToString() + "_";
                    for (var i = 0; i < al.Count; i++)
                    {
                        str2 = al[i].ToString();
                        if (str2.Contains("PERS jointtarget"))
                        {
                            str3 = str2.Substring(str2.IndexOf("PERS jointtarget") + 0x11);
                            if (!(str3.Contains(":=") && str3.Contains("[[")))
                            {
                                Problems.Add("Cannot decode tool definition line: '" + str2 + "'");
                                return;
                            }
                            str3 = str3.Substring(0, str3.IndexOf(":=")).Replace(" ", "");
                            str = str2.Substring(str2.IndexOf("[[") + 2);
                            if (!str.Contains("]"))
                            {
                                Problems.Add("Cannot decode tool definition line: '" + str2 + "'");
                                return;
                            }
                            str = str.Substring(0, str.IndexOf("]"));
                            if (str3.StartsWith(str5))
                            {
                                var vector2 = VectorFromString(str);
                                var str6 = str2.Substring(0, str2.IndexOf("[[") + 2);
                                var str7 = str2.Substring(str2.IndexOf("]"));
                                vector2.Vec[AxisIndx] = value;
                                var strArray = new string[11];
                                var num2 = vector2.Vec[0];
                                strArray[0] = num2.ToString("F3");
                                strArray[1] = ",";
                                num2 = vector2.Vec[1];
                                strArray[2] = num2.ToString("F3");
                                strArray[3] = ",";
                                num2 = vector2.Vec[2];
                                strArray[4] = num2.ToString("F3");
                                strArray[5] = ",";
                                num2 = vector2.Vec[3];
                                strArray[6] = num2.ToString("F3");
                                strArray[7] = ",";
                                num2 = vector2.Vec[4];
                                strArray[8] = num2.ToString("F3");
                                strArray[9] = ",";
                                strArray[10] = vector2.Vec[5].ToString("F3");
                                str = string.Concat(strArray);
                                al[i] = str6 + str + str7;
                            }
                        }
                    }
                    Utils.ArrayListToFile(path, al, true);
                }
                catch (Exception exception)
                {
                    Problems.Add("ABB.SetSingleJointAxisValue experienced exception below: \n" + exception.Message);
                }
            }
            else
            {
                Problems.Add(Car0JointPosesProgram() + " not found in robot folder: " + RobotRootPath);
            }
        }

        private static Vector VectorFromString(string str)
        {
            var vector = new Vector(0);
            var strArray = str.Split(new char[] { ',' });
            foreach (var str2 in strArray)
            {
                if (str2.Length > 0)
                {
                    vector.Vec.Add(Convert.ToDouble(str2));
                }
            }
            return vector;
        }

        public static void WriteDefiner(string RobotRootPath, double[] FrameDef, ref int MnLineNumber, bool WriteFrame, Brand RobBrand, int ind, ref ArrayList Problems)
        {
            var path = Path.Combine(RobotRootPath, Car0DefinerName());
            string str2 = null;
            string str3 = null;
            string str4 = null;
            string str5 = null;
            var items = new ArrayList();
            var i = 0;
            var wobjDataRecDetected = false;
            try
            {
                if (File.Exists(path))
                {
                    items = Utils.FileToArrayList(path);
                }
                else
                {
                    str2 = Car0DefinerName();
                    str2 = str2.Substring(0, str2.IndexOf("."));
                    items.Add("%%%");
                    items.Add("  VERSION:1");
                    items.Add("  LANGUAGE:ENGLISH");
                    items.Add("%%%");
                    items.Add("");
                    items.Add("MODULE " + str2);
                    items.Add("");
                }
                if (WriteFrame)
                {
                    var matchString = "";
                    var typeString = "";
                    matchString = Utils.RobotEquivalentStringName(ind, RobBrand);
                    switch (RobotData.FrameTypes[ind])
                    {
                        case FrameType.MigGunTip:
                        case FrameType.NutGunTip:
                        case FrameType.RivetGunTip:
                        case FrameType.ScribeGun:
                        case FrameType.SealGunTip:
                        case FrameType.SpotGunTip:
                        case FrameType.StudGunTip:
                        case FrameType.GripPin:
                        case FrameType.PedRivetGunTip:
                        case FrameType.PedSealGunTip:
                        case FrameType.PedSpotGunTip:
                        case FrameType.PedScribeGunTip:
                        case FrameType.PedStudGunTip:
                        case FrameType.LaserTip:
                        case FrameType.PierceTip:
                        case FrameType.VisionTcp:
                            typeString = "tooldata";
                            break;

                        case FrameType.FixtureCar0:
                        case FrameType.GripperCar0:
                        case FrameType.PickTool:
                        case FrameType.DropTool:
                            typeString = "wobjdata";
                            wobjDataRecDetected = true;
                            break;
                    }
                    str5 = wobjDataRecDetected ? "]],[" : "],[";
                    if (FoundExactMatchRecord(items, typeString, matchString, out i))
                    {
                        if (!(items[i].ToString().Contains("[[") && items[i].ToString().Contains(str5)))
                        {
                            MessageBox.Show("Error decoding record #" + i.ToString() + " = \n'" + items[i].ToString() + "'", "ABB.WriteDefiner");
                            return;
                        }
                        str3 = items[i].ToString().Substring(0, items[i].ToString().IndexOf("[[") + 2);
                        str4 = items[i].ToString().Substring(items[i].ToString().IndexOf(str5));
                        items[i] = str3 + BuildFrameCoordsString(FrameDef, wobjDataRecDetected, "],[") + str4;
                    }
                    else if (FoundMatchingType(items, typeString, out i))
                    {
                        if (wobjDataRecDetected)
                        {
                            str2 = Utils.IsPed(ind) ? "TRUE," : "FALSE,";
                            str4 = "TRUE";
                            str3 = "  TASK PERS wobjdata " + matchString + ":=[" + str2 + str4 + ",\"\",[[" + BuildFrameCoordsString(FrameDef, wobjDataRecDetected, "],[") + str5 + "[0, 0, 0],[ 1, 0, 0, 0]]];";
                        }
                        else
                        {
                            str2 = Utils.IsPed(ind) ? "FALSE" : "TRUE";
                            str3 = "  TASK PERS tooldata " + matchString + ":=[" + str2 + ",[[" + BuildFrameCoordsString(FrameDef, wobjDataRecDetected, str5) + "],[1, 0, 0, 0]],[5,[20,0,0],[1,0,0,0],0,0,0]];";
                            Problems.Add("Warning: matching record not found for '" + matchString + "' in \n" + path + "\n\tOrientation faked to [1, 0, 0, 0]!!!\n");
                        }
                        if (i.Equals((int) (items.Count - 1)))
                        {
                            items.Add(str3);
                        }
                        else
                        {
                            items.Insert(i + 1, str3);
                        }
                    }
                    else
                    {
                        if (wobjDataRecDetected)
                        {
                            str2 = Utils.IsPed(ind) ? "TRUE," : "FALSE,";
                            str4 = "TRUE";
                            str3 = "\tTASK PERS wobjdata " + matchString + ":=[" + str2 + str4 + ",\"\",[[" + BuildFrameCoordsString(FrameDef, wobjDataRecDetected, "],[") + str5 + "[0, 0, 0],[ 1, 0, 0, 0]]];";
                            Problems.Add("Warning: No records of type '" + typeString + "' in \n" + path + "\n");
                        }
                        else
                        {
                            str2 = Utils.IsPed(ind) ? "FALSE" : "TRUE";
                            str3 = "  TASK PERS tooldata " + matchString + ":=[" + str2 + ",[[" + BuildFrameCoordsString(FrameDef, wobjDataRecDetected, str5) + "],[1, 0, 0, 0]],[5,[20,0,0],[1,0,0,0],0,0,0]];";
                            Problems.Add("Warning: No records of type '" + typeString + "' in \n" + path + "\n\tOrientation faked to [1, 0, 0, 0]!!!\n");
                        }
                        if (EndModuleRecordFound(items, out i))
                        {
                            items.Insert(i - 1, str3);
                        }
                        else
                        {
                            items.Add(str3);
                        }
                    }
                }
                if (!EndModuleRecordFound(items))
                {
                    items.Add("ENDMODULE");
                }
                Utils.ArrayListToFile(path, items, true);
            }
            catch (Exception exception)
            {
                Problems.Add(exception.ToString());
            }
        }
    }
}

