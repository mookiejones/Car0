namespace CarZero
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    internal class Nachi
    {
        private const int Car0FileNumber = 0x38f;
        private const int J4FileNumber = 0x392;
        private const int J5FileNumber = 0x393;
        private const int J6FileNumber = 0x394;

        private static string Car0FileName(string RobotRootPath, ref ArrayList Problems, int ThisFileNumber)
        {
            var strArray = Directory.GetFiles(RobotRootPath, "*." + ThisFileNumber.ToString(), SearchOption.TopDirectoryOnly);
            var length = strArray.Length;
            if (length.Equals(1))
            {
                return strArray[0];
            }
            var strArray2 = new string[] { "There are ", strArray.Length.ToString(), " files with a dot extension of '", ThisFileNumber.ToString(), "' in the robot folder." };
            Problems.Add(string.Concat(strArray2));
            return "";
        }

        public static List<Vector3> ReadCar0BasePtsProgram(string RobotRootPath, ref ArrayList Problems)
        {
            var list = new List<Vector3>();
            var path = Car0FileName(RobotRootPath, ref Problems, 0x38f);
            if (File.Exists(path))
            {
                try
                {
                    var list2 = Utils.FileToArrayList(path);
                    for (var i = 0; i < list2.Count; i++)
                    {
                        if (list2[i].ToString().Contains("("))
                        {
                            var str = list2[i].ToString().Substring(list2[i].ToString().IndexOf("(") + 1);
                            if (str.Contains(")"))
                            {
                                var strArray = str.Substring(0, str.IndexOf(")")).Split(new char[] { ',' });
                                var list3 = new List<double>();
                                for (var j = 0; j < strArray.Length; j++)
                                {
                                    if (strArray[j].Length > 0)
                                    {
                                        list3.Add(Convert.ToDouble(strArray[j]));
                                    }
                                }
                                if (strArray.Length >= 6)
                                {
                                    list.Add(new Vector3(list3[0], list3[1], list3[2]));
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Problems.Add("Nachi.ReadCar0BasePtsProgram experienced exception below: \n" + exception.Message);
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
            var path = Car0FileName(RobotRootPath, ref Problems, 0x392);
            var str2 = Car0FileName(RobotRootPath, ref Problems, 0x393);
            var str3 = Car0FileName(RobotRootPath, ref Problems, 0x394);
            if ((!File.Exists(path) || !File.Exists(str2)) || !File.Exists(str3))
            {
                if (!File.Exists(path))
                {
                    Problems.Add("Required file missing: " + path);
                }
                if (!File.Exists(str2))
                {
                    Problems.Add("Required file missing: " + str2);
                }
                if (!File.Exists(str3))
                {
                    Problems.Add("Required file missing: " + str3);
                }
            }
            else
            {
                try
                {
                    int num;
                    var list = Utils.FileToArrayList(path);
                    string str = null;
                    for (num = 0; num < list.Count; num++)
                    {
                        if (list[num].ToString().Contains("(") && list[num].ToString().Contains(")"))
                        {
                            str = list[num].ToString().Substring(list[num].ToString().IndexOf("(") + 1);
                            str = str.Substring(0, str.IndexOf(")")).Replace(" ", "");
                            J4Targets.Add(new Vector(VectorFromString(str)));
                        }
                    }
                    list = Utils.FileToArrayList(str2);
                    for (num = 0; num < list.Count; num++)
                    {
                        if (list[num].ToString().Contains("(") && list[num].ToString().Contains(")"))
                        {
                            str = list[num].ToString().Substring(list[num].ToString().IndexOf("(") + 1);
                            str = str.Substring(0, str.IndexOf(")"));
                            J5Targets.Add(new Vector(VectorFromString(str)));
                        }
                    }
                    list = Utils.FileToArrayList(str3);
                    for (num = 0; num < list.Count; num++)
                    {
                        if (list[num].ToString().Contains("(") && list[num].ToString().Contains(")"))
                        {
                            str = list[num].ToString().Substring(list[num].ToString().IndexOf("(") + 1);
                            str = str.Substring(0, str.IndexOf(")"));
                            J6Targets.Add(new Vector(VectorFromString(str)));
                        }
                    }
                    if (J4Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 4 targets were found in :" + Car0FileName(RobotRootPath, ref Problems, 0x392));
                    }
                    if (J5Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 5 targets were found in :" + Car0FileName(RobotRootPath, ref Problems, 0x393));
                    }
                    if (J6Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 6 targets were found in :" + Car0FileName(RobotRootPath, ref Problems, 0x394));
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
            var path = Car0FileName(RobotRootPath, ref Problems, 0x392);
            var str2 = Car0FileName(RobotRootPath, ref Problems, 0x393);
            var str3 = Car0FileName(RobotRootPath, ref Problems, 0x394);
            if ((!File.Exists(path) || !File.Exists(str2)) || !File.Exists(str3))
            {
                if (!File.Exists(path))
                {
                    Problems.Add("Required file missing: " + path);
                }
                if (!File.Exists(str2))
                {
                    Problems.Add("Required file missing: " + str2);
                }
                if (!File.Exists(str3))
                {
                    Problems.Add("Required file missing: " + str3);
                }
            }
            else if ((!UpdateJ123ForSingleJointTest(path, J123Values, ref Problems) || !UpdateJ123ForSingleJointTest(str2, J123Values, ref Problems)) || UpdateJ123ForSingleJointTest(str3, J123Values, ref Problems))
            {
            }
        }

        public static void SetSingleJointAxisValue(string RobotRootPath, double value, int FileIndx, int AxisIndx, ref ArrayList Problems)
        {
            string path = null;
            switch (FileIndx)
            {
                case 3:
                    path = Car0FileName(RobotRootPath, ref Problems, 0x392);
                    break;

                case 4:
                    path = Car0FileName(RobotRootPath, ref Problems, 0x393);
                    break;

                case 5:
                    path = Car0FileName(RobotRootPath, ref Problems, 0x394);
                    break;

                default:
                    Problems.Add("SetSingleJointAxisValue bug: unsupported axis number = " + AxisIndx.ToString());
                    return;
            }
            if (!File.Exists(path))
            {
                Problems.Add("Required file missing: " + path);
            }
            else
            {
                try
                {
                    var al = Utils.FileToArrayList(path);
                    string str2 = null;
                    string str3 = null;
                    for (var i = 0; i < al.Count; i++)
                    {
                        str3 = al[i].ToString();
                        if (str3.Contains("(") && str3.Contains(")"))
                        {
                            str2 = str3.Substring(al[i].ToString().IndexOf("(") + 1);
                            var vector = VectorFromString(str2.Substring(0, str2.IndexOf(")")).Replace(" ", ""));
                            var str4 = str3.Substring(0, al[i].ToString().IndexOf("(") + 1);
                            var str5 = str3.Substring(str3.IndexOf(")"));
                            vector.Vec[AxisIndx] = value;
                            var strArray = new string[11];
                            var num3 = vector.Vec[0];
                            strArray[0] = num3.ToString("F3");
                            strArray[1] = ",";
                            num3 = vector.Vec[1];
                            strArray[2] = num3.ToString("F3");
                            strArray[3] = ",";
                            num3 = vector.Vec[2];
                            strArray[4] = num3.ToString("F3");
                            strArray[5] = ",";
                            num3 = vector.Vec[3];
                            strArray[6] = num3.ToString("F3");
                            strArray[7] = ",";
                            num3 = vector.Vec[4];
                            strArray[8] = num3.ToString("F3");
                            strArray[9] = ",";
                            strArray[10] = vector.Vec[5].ToString("F3");
                            str2 = string.Concat(strArray);
                            al[i] = str4 + str2 + str5;
                        }
                    }
                    Utils.ArrayListToFile(path, al, true);
                }
                catch (Exception exception)
                {
                    Problems.Add("Nachi.SetSingleJointAxisValue experienced exception below: \n" + exception.Message);
                }
            }
        }

        private static bool UpdateJ123ForSingleJointTest(string Fpath, Vector J123Values, ref ArrayList Problems)
        {
            try
            {
                var al = Utils.FileToArrayList(Fpath);
                string str = null;
                string str2 = null;
                for (var i = 0; i < al.Count; i++)
                {
                    str2 = al[i].ToString();
                    if (str2.Contains("(") && str2.Contains(")"))
                    {
                        str = str2.Substring(al[i].ToString().IndexOf("(") + 1);
                        var vector = VectorFromString(str.Substring(0, str.IndexOf(")")).Replace(" ", ""));
                        var str3 = str2.Substring(0, al[i].ToString().IndexOf("(") + 1);
                        var str4 = str2.Substring(str2.IndexOf(")"));
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
                        num2 = vector.Vec[3];
                        strArray[6] = num2.ToString("F3");
                        strArray[7] = ",";
                        num2 = vector.Vec[4];
                        strArray[8] = num2.ToString("F3");
                        strArray[9] = ",";
                        strArray[10] = vector.Vec[5].ToString("F3");
                        str = string.Concat(strArray);
                        al[i] = str3 + str + str4;
                    }
                }
                Utils.ArrayListToFile(Fpath, al, true);
            }
            catch (Exception exception)
            {
                Problems.Add(exception.ToString());
                return false;
            }
            return true;
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

        public static void WriteDefiner(string RobotRootPath, string FrameName, double[] FrameDef, ref int LinesWritten, bool WriteFrame, ref ArrayList Problems)
        {
            var path = Path.Combine(RobotRootPath, "Car0Definer.txt");
            string str2 = null;
            try
            {
                using (var writer = new StreamWriter(path, !((int) LinesWritten).Equals(0)))
                {
                    if (WriteFrame)
                    {
                        str2 = FrameName + "={X " + FrameDef[0].ToString("F4") + ",Y " + FrameDef[1].ToString("F4") + ",Z " + FrameDef[2].ToString("F4") + ",Rx " + FrameDef[3].ToString("F4") + ",Ry " + FrameDef[4].ToString("F4") + ",Rz " + FrameDef[5].ToString("F4") + "}\n";
                        writer.WriteLine(str2);
                        LinesWritten++;
                    }
                    writer.Flush();
                }
            }
            catch (Exception exception)
            {
                Problems.Add(exception.ToString());
            }
        }
    }
}

