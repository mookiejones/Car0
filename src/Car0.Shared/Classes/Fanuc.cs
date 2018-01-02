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
    using System.IO; 

    internal class Fanuc
    {
        private static string FormatDefinerNumber(double val, string fstring)
        {
            if (val < 0.0)
            {
                return ("(" + val.ToString(fstring) + ")");
            }
            return val.ToString(fstring);
        }

        private static string make_two_digit_string(int number)
        {
            if (number >= 10)
            {
                return (number.ToString());
            }
            return "0" + number.ToString();
        }

        public static List<Vector3> ReadCar0BasePtsProgram(string RobotRootPath, ref ArrayList Problems)
        {
            var list = new List<Vector3>();
            var path = Path.Combine(RobotRootPath, "CAR0PTS.LS");
            if (File.Exists(path))
            {
                try
                {
                    var list2 = Utils.FileToArrayList(path);
                    var flag = false;
                    var v = new Vector3();
                    for (var i = 0; i < list2.Count; i++)
                    {
                        if (flag)
                        {
                            if ((list2[i].ToString().Contains("X =") && list2[i].ToString().Contains("Y =")) && list2[i].ToString().Contains("Z ="))
                            {
                                v.x = Utils.DoubleFromString(list2[i].ToString(), "X =", " ");
                                v.y = Utils.DoubleFromString(list2[i].ToString(), "Y =", " ");
                                v.z = Utils.DoubleFromString(list2[i].ToString(), "Z =", " ");
                                list.Add(new Vector3(v));
                            }
                        }
                        else
                        {
                            flag = list2[i].ToString().StartsWith("/POS");
                        }
                    }
                    if (!flag)
                    {
                        Problems.Add("No /POS section found in CAR0PTS.LS");
                    }
                }
                catch (Exception exception)
                {
                    Problems.Add("Fanuc.ReadCar0BasePtsProgram experienced exception below: \n" + exception.Message);
                    return new List<Vector3>();
                }
                return list;
            }
            Problems.Add("Reference points program not found (" + path + ")");
            return list;
        }

        public static void ReadCar0J456Targets(string RobotRootPath, out List<Vector> J4Targets, out List<Vector> J5Targets, out List<Vector> J6Targets, ref ArrayList Problems)
        {
            J4Targets = new List<Vector>();
            J5Targets = new List<Vector>();
            J6Targets = new List<Vector>();
            var flag = false;
            var path = Path.Combine(RobotRootPath, "CAR0TOOL.LS");
            if (!File.Exists(path))
            {
                Problems.Add("CAR0TOOL.LS not found in robot folder: " + RobotRootPath);
            }
            else
            {
                try
                {
                    var myList = Utils.FileToArrayList(path);
                    for (var i = 0; i < myList.Count; i++)
                    {
                        if (flag)
                        {
                            if (myList[i].ToString().Contains("P["))
                            {
                                if (myList[i].ToString().Contains("J4_"))
                                {
                                    J4Targets.Add(new Vector(VectorFromArrayListPair(myList, i + 3, ref Problems)));
                                    if (Problems.Count.Equals(0))
                                    {
                                        i += 4;
                                    }
                                    else
                                    {
                                        i = myList.Count;
                                    }
                                }
                                else if (myList[i].ToString().Contains("J5_"))
                                {
                                    J5Targets.Add(new Vector(VectorFromArrayListPair(myList, i + 3, ref Problems)));
                                    if (Problems.Count.Equals(0))
                                    {
                                        i += 4;
                                    }
                                    else
                                    {
                                        i = myList.Count;
                                    }
                                }
                                else if (myList[i].ToString().Contains("J6_"))
                                {
                                    J6Targets.Add(new Vector(VectorFromArrayListPair(myList, i + 3, ref Problems)));
                                    if (Problems.Count.Equals(0))
                                    {
                                        i += 4;
                                    }
                                    else
                                    {
                                        i = myList.Count;
                                    }
                                }
                            }
                        }
                        else if (!flag && myList[i].ToString().StartsWith("/POS"))
                        {
                            flag = true;
                        }
                    }
                    if (J4Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 4 targets were found in :CAR0TOOL.LS");
                    }
                    if (J5Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 5 targets were found in :CAR0TOOL.LS");
                    }
                    if (J6Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 6 targets were found in :CAR0TOOL.LS");
                    }
                }
                catch (Exception exception)
                {
                    Problems.Add(exception.Message);
                }
            }
        }

        public static bool ReadDefinerToolData(string RobotRootPath, int ToolNum, ref double[] FrameDef)
        {
            var path = Path.Combine(RobotRootPath, "definer.ls");
            string str2 = null;
            string str3 = null;
            var str4 = "!Defining UTOOL " + ToolNum.ToString() + ";";
            var str5 = "UTOOL[" + ToolNum.ToString() + "] = PR[9] ;";
            var flag = false;
            try
            {
                var reader = new StreamReader(path);
                var flag2 = false;
                var flag3 = false;
                var flag4 = false;
                var flag5 = false;
                while (!flag && ((str2 = reader.ReadLine()) != null))
                {
                    if (!flag2 && str2.Contains(str4))
                    {
                        while (!flag3 && ((str2 = reader.ReadLine()) != null))
                        {
                            if (str2.Contains("PR[9] = LPOS"))
                            {
                                for (var i = 0; i < 6; i++)
                                {
                                    flag4 = false;
                                    str3 = "PR[GP1:9," + ((i + 1)).ToString() + "] = ";
                                    while (!flag4 && ((str2 = reader.ReadLine()) != null))
                                    {
                                        if (str2.Contains(str3))
                                        {
                                            var str6 = str2.Substring(str2.IndexOf(str3) + str3.Length).Replace(" ", "").Replace("(", "").Replace(")", "").Replace(";", "");
                                            FrameDef[i] = Convert.ToDouble(str6);
                                        }
                                        flag4 = true;
                                    }
                                    if (!flag4)
                                    {
                                        break;
                                    }
                                }
                                flag3 = flag4;
                                if (flag4)
                                {
                                    while (!flag5 && ((str2 = reader.ReadLine()) != null))
                                    {
                                        if (str2.Contains(str5))
                                        {
                                            flag5 = flag = true;
                                        }
                                    }
                                }
                            }
                        }
                        flag2 = true;
                    }
                }
                reader.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return flag;
        }

        private static bool ReplaceJointValueInArray(ArrayList MyList, int Indx, string doobie, double Jval, ref ArrayList Problems)
        {
            try
            {
                var str = MyList[Indx].ToString();
                if (!str.Contains(doobie))
                {
                    Problems.Add("ReplaceJointValueInArray: '" + doobie + "' not found in '" + str + "'");
                    return false;
                }
                var str2 = str.Substring(0, str.IndexOf(doobie) + 3);
                var str3 = str.Substring(str.IndexOf(doobie) + 3);
                if (!str3.Contains(" deg"))
                {
                    Problems.Add("ReplaceJointValueInArray: ' deg' not found in '" + str3 + "'");
                    return false;
                }
                str3 = str3.Substring(str3.IndexOf(" deg"));
                MyList[Indx] = str2 + Jval.ToString("F3").PadLeft(10) + str3;
            }
            catch (Exception exception)
            {
                Problems.Add("ReplaceJointValueInArray Exception: " + exception.Message);
                return false;
            }
            return true;
        }

        public static void SetJ123Values(string RobotRootPath, Vector J123Values, ref ArrayList Problems)
        {
            var path = Path.Combine(RobotRootPath, "CAR0TOOL.LS");
            if (!File.Exists(path))
            {
                Problems.Add("Required file missing: " + path);
            }
            else
            {
                try
                {
                    var myList = Utils.FileToArrayList(path);
                    var flag = false;
                    for (var i = 0; i < myList.Count; i++)
                    {
                        if (flag)
                        {
                            if (myList[i].ToString().Contains("P[") && ((myList[i].ToString().Contains("J4_") || myList[i].ToString().Contains("J5_")) || myList[i].ToString().Contains("J6_")))
                            {
                                if (!((ReplaceJointValueInArray(myList, i + 3, "J1=", J123Values.Vec[0], ref Problems) && ReplaceJointValueInArray(myList, i + 3, "J2=", J123Values.Vec[1], ref Problems)) && ReplaceJointValueInArray(myList, i + 3, "J3=", J123Values.Vec[2], ref Problems)))
                                {
                                    return;
                                }
                                if (Problems.Count.Equals(0))
                                {
                                    i += 4;
                                }
                                else
                                {
                                    i = myList.Count;
                                }
                            }
                        }
                        else if (!flag && myList[i].ToString().StartsWith("/POS"))
                        {
                            flag = true;
                        }
                    }
                    Utils.ArrayListToFile(path, myList, true);
                }
                catch (Exception exception)
                {
                    Problems.Add(exception.Message);
                }
            }
        }

        public static void SetSingleJointAxisValue(string RobotRootPath, double value, int MovingJointNumber, int AxisIndx, ref ArrayList Problems)
        {
            var path = Path.Combine(RobotRootPath, "CAR0TOOL.LS");
            if (!File.Exists(path))
            {
                Problems.Add("Required file missing: " + path);
            }
            else
            {
                try
                {
                    var myList = Utils.FileToArrayList(path);
                    var flag = false;
                    var str2 = "J" + MovingJointNumber.ToString() + "_";
                    var doobie = "J" + ((AxisIndx + 1)).ToString() + "=";
                    for (var i = 0; i < myList.Count; i++)
                    {
                        if (flag)
                        {
                            if (myList[i].ToString().Contains("P[") && myList[i].ToString().Contains(str2))
                            {
                                if (!ReplaceJointValueInArray(myList, i + 4, doobie, value, ref Problems))
                                {
                                    return;
                                }
                                if (Problems.Count.Equals(0))
                                {
                                    i += 4;
                                }
                                else
                                {
                                    i = myList.Count;
                                }
                            }
                        }
                        else if (!flag && myList[i].ToString().StartsWith("/POS"))
                        {
                            flag = true;
                        }
                    }
                    Utils.ArrayListToFile(path, myList, true);
                }
                catch (Exception exception)
                {
                    Problems.Add("Fanuc.SetSingleJointAxisValue experienced exception below: \n" + exception.Message);
                }
            }
        }

        private static Vector VectorFromArrayListPair(ArrayList MyList, int FirstIndx, ref ArrayList Problems)
        {
            var v = VectorFromString(MyList[FirstIndx].ToString(), ref Problems);
            return VectorFromString(MyList[FirstIndx + 1].ToString(), v, ref Problems);
        }

        private static Vector VectorFromString(string str, ref ArrayList Problems)
        {
            var v = new Vector(0);
            return VectorFromString(str, v, ref Problems);
        }

        private static Vector VectorFromString(string str, Vector v, ref ArrayList Problems)
        {
            int num;
            string str2;
            double num2;
            Exception exception;
            if ((str.Contains("J1") && str.Contains("J2")) && str.Contains("J3"))
            {
                try
                {
                    for (num = 1; num <= 3; num++)
                    {
                        str2 = str.Substring(str.IndexOf("J" + num.ToString()));
                        str2 = str2.Substring(str2.IndexOf("=") + 1);
                        while (str2.StartsWith(" "))
                        {
                            str2 = str2.Substring(1);
                        }
                        num2 = Convert.ToDouble(str2.Substring(0, str2.IndexOf(" ")).Replace(" ", ""));
                        v.Vec.Add(num2);
                    }
                }
                catch (Exception exception1)
                {
                    exception = exception1;
                    Problems.Add("VectorFromString: " + exception.Message);
                }
                return v;
            }
            if ((str.Contains("J4") && str.Contains("J5")) && str.Contains("J6"))
            {
                try
                {
                    for (num = 4; num <= 6; num++)
                    {
                        str2 = str.Substring(str.IndexOf("J" + num.ToString()));
                        str2 = str2.Substring(str2.IndexOf("=") + 1);
                        while (str2.StartsWith(" "))
                        {
                            str2 = str2.Substring(1);
                        }
                        num2 = Convert.ToDouble(str2.Substring(0, str2.IndexOf(" ")).Replace(" ", ""));
                        v.Vec.Add(num2);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    Problems.Add("VectorFromString: " + exception.Message);
                }
                return v;
            }
            if ((str.Contains("W") && str.Contains("P")) && str.Contains("R"))
            {
                Problems.Add("CAR0TOOL.LS contains a WPR format position.  Joint format is required!");
                return v;
            }
            Problems.Add("Cannot decode CAR0TOOL.LS position record = '" + str + "'");
            return v;
        }

        public static void WriteDefiner(string RobotRootPath, string FrameName, double[] FrameDef, ref int MnLineNumber, bool WriteFrame, bool Rj3Controller, ref ArrayList Problems)
        {
            var path = Path.Combine(RobotRootPath, "C0Define.ls");
            string str2 = null;
            string typeMessage = null;
            var frameNum = 0;
            StreamWriter fs = null;
            try
            {
                if (WriteFrame)
                {
                    typeMessage = FrameName.Substring(0, FrameName.IndexOf(" "));
                    str2 = FrameName.Substring(FrameName.IndexOf(" "));
                    while (str2.StartsWith(" "))
                    {
                        str2 = str2.Substring(1);
                    }
                    frameNum = Convert.ToInt32(str2.Substring(0, str2.IndexOf(" ")));
                }
                using (fs = new StreamWriter(path, !((int) MnLineNumber).Equals(0)))
                {
                    if (((int) MnLineNumber).Equals(0))
                    {
                        fs.WriteLine("/PROG    C0Define");
                        fs.WriteLine("/ATTR");
                        fs.WriteLine("OWNER            = MNEDITOR;");
                        fs.WriteLine("COMMENT          = \"Car 0 Definer\";");
                        fs.WriteLine("PROG_SIZE        = 0;");
                        str2 = "CREATE           = DATE " + make_two_digit_string(DateTime.Now.Year - 0x7d0) + "-" + make_two_digit_string(DateTime.Now.Month) + "-" + make_two_digit_string(DateTime.Now.Day) + "    TIME " + make_two_digit_string(DateTime.Now.Hour) + ":" + make_two_digit_string(DateTime.Now.Minute) + ":" + make_two_digit_string(DateTime.Now.Second) + " ;";
                        fs.WriteLine(str2);
                        str2 = "MODIFIED         = DATE " + make_two_digit_string(DateTime.Now.Year - 0x7d0) + "-" + make_two_digit_string(DateTime.Now.Month) + "-" + make_two_digit_string(DateTime.Now.Day) + "    TIME " + make_two_digit_string(DateTime.Now.Hour) + ":" + make_two_digit_string(DateTime.Now.Minute) + ":" + make_two_digit_string(DateTime.Now.Second) + " ;";
                        fs.WriteLine(str2);
                        fs.WriteLine("DEFAULT_GROUP   = 1,*,*,*,*;");
                        fs.WriteLine("CONTROL_CODE    = 00000000 00000000;");
                        fs.WriteLine("/APPL\r\nSPOT : TRUE ;\r\nSPOT Welding Equipment Number : 1 ;\r\nCYCLE_REFERENCE = 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0;\r\nCYCLE_TARGET = 0.00 ;");
                        fs.WriteLine("/MN");
                        MnLineNumber++;
                        WriteFanucMnLine("!************************************", ref MnLineNumber, ref fs);
                        WriteFanucMnLine("!Car 0 Definer File", ref MnLineNumber, ref fs);
                        WriteFanucMnLine("", ref MnLineNumber, ref fs);
                        WriteFanucMnLine("!Use once to update measured frames", ref MnLineNumber, ref fs);
                        WriteFanucMnLine("!THEN DISCARD", ref MnLineNumber, ref fs);
                        WriteFanucMnLine("!------------------------------------", ref MnLineNumber, ref fs);
                    }
                    if (WriteFrame)
                    {
                        WriteFanucDefinerFrameRecord(typeMessage, frameNum, FrameDef, ref fs, ref MnLineNumber, Rj3Controller);
                    }
                    else
                    {
                        fs.WriteLine("/POS");
                        fs.WriteLine("/END");
                    }
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception exception)
            {
                Problems.Add(exception.ToString());
            }
        }

        private static void WriteFanucDefinerFrameRecord(string TypeMessage, int FrameNum, double[] fromAto, ref StreamWriter fs, ref int LineNum, bool Rj3Controller)
        {
            string lineData = null;
            WriteFanucMnLine(string.Concat(new object[] { "!Defining ", TypeMessage, " ", FrameNum }), ref LineNum, ref fs);
            WriteFanucMnLine("PR[9] = LPOS", ref LineNum, ref fs);
            for (var i = 0; i < 6; i++)
            {
                WriteFanucMnLine("PR[GP1:9," + ((i + 1)).ToString() + "] = " + FormatDefinerNumber(fromAto[i], "F3"), ref LineNum, ref fs);
            }
            lineData = ((int) LineNum++).ToString();
            lineData = Rj3Controller ? string.Concat(new object[] { "$MN", TypeMessage, "[1,", FrameNum, "] = PR[9]" }) : string.Concat(new object[] { TypeMessage, "[", FrameNum, "] = PR[9]" });
            WriteFanucMnLine(lineData, ref LineNum, ref fs);
            WriteFanucMnLine("", ref LineNum, ref fs);
        }

        private static bool WriteFanucMnLine(string LineData, ref int LineNum, ref StreamWriter fs)
        {
            try
            {
                var str = ((int) LineNum).ToString();
                LineNum++;
                str = str.PadLeft(4) + ":  " + LineData + ";";
                fs.WriteLine(str);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "WriteFanucMnLine");
                return false;
            }
            return true;
        }
    }

    public interface Robot
    {
         string FormatDefinerNumber(double val, string fstring);
         string make_two_digit_string(int number);
    }
}

