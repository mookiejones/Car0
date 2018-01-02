namespace CarZero
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    internal class Kuka
    {
        public static List<Vector3> ReadCar0BasePtsProgram(string RobotRootPath, ref ArrayList Problems)
        {
            var list = new List<Vector3>();
            var path = Path.Combine(Path.Combine(RobotRootPath, "Temporary"), "Car0Pts.dat");
            if (File.Exists(path))
            {
                try
                {
                    var list2 = Utils.FileToArrayList(path);
                    var v = new Vector3();
                    var temp = string.Empty;
                    for (var i = 0; i < list2.Count; i++)
                    {
                        list2[i] = list2[i].ToString().ToLower();
                        if ((list2[i].ToString().Contains("x ") && list2[i].ToString().Contains("y ")) && list2[i].ToString().Contains("z "))
                        {
                            v.x = Utils.DoubleFromString(list2[i].ToString().ToLower(), "x ", ",");
                            v.y = Utils.DoubleFromString(list2[i].ToString().ToLower(), "y ", ",");
                            v.z = Utils.DoubleFromString(list2[i].ToString().ToLower(), "z ", ",");
                            list.Add(new Vector3(v));
                        }
                    }
                }
                catch (Exception exception)
                {
                    Problems.Add("Kuka.ReadCar0BasePtsProgram experienced exception below: \n" + exception.Message);
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
            var path = Path.Combine(RobotRootPath, Path.Combine("Temporary", "Car0J456.dat"));
            if (!File.Exists(path))
            {
                Problems.Add("Car0J456.dat not found in robot folder: " + Path.Combine(RobotRootPath, "Temporary"));
            }
            else
            {
                try
                {
                    var list = Utils.FileToArrayList(path);
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (list[i].ToString().ToUpper().Contains("DECL E6AXIS"))
                        {
                            if (list[i].ToString().ToUpper().Contains("J4_"))
                            {
                                J4Targets.Add(new Vector(VectorFromString(list[i].ToString().ToUpper(), ref Problems)));
                                if (!Problems.Count.Equals(0))
                                {
                                    i = list.Count;
                                }
                            }
                            else if (list[i].ToString().ToUpper().Contains("J5_"))
                            {
                                J5Targets.Add(new Vector(VectorFromString(list[i].ToString().ToUpper(), ref Problems)));
                                if (!Problems.Count.Equals(0))
                                {
                                    i = list.Count;
                                }
                            }
                            else if (list[i].ToString().ToUpper().Contains("J6_"))
                            {
                                J6Targets.Add(new Vector(VectorFromString(list[i].ToString().ToUpper(), ref Problems)));
                                if (!Problems.Count.Equals(0))
                                {
                                    i = list.Count;
                                }
                            }
                        }
                    }
                    if (J4Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 4 targets were found");
                    }
                    if (J5Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 5 targets were found");
                    }
                    if (J6Targets.Count.Equals(0))
                    {
                        Problems.Add("No Joint 6 targets were found");
                    }
                }
                catch (Exception exception)
                {
                    Problems.Add(exception.Message);
                }
            }
        }

        public static bool ReadDefinerBaseData(string RobotRootPath, int ToolNum, ref double[] FrameDef)
        {
            var path = Path.Combine(Path.Combine(RobotRootPath, "Temporary"), "definer.src");
            string str3 = null;
            string str4 = null;
            var str5 = "BASE_DATA[" + ToolNum.ToString() + "]";
            var flag = false;
            try
            {
                using (var reader = new StreamReader(path))
                {
                    while (!flag && ((str3 = reader.ReadLine()) != null))
                    {
                        if (str3.Contains(str5))
                        {
                            str4 = str3.ToLower().Substring(str3.IndexOf("x") + 1);
                            str4 = str4.Substring(0, str4.IndexOf(",")).Replace(",", "").Replace(" ", "");
                            FrameDef[0] = Convert.ToDouble(str4);
                            str4 = str3.ToLower().Substring(str3.IndexOf("y") + 1);
                            str4 = str4.Substring(0, str4.IndexOf(",")).Replace(",", "").Replace(" ", "");
                            FrameDef[1] = Convert.ToDouble(str4);
                            str4 = str3.ToLower().Substring(str3.IndexOf("z") + 1);
                            str4 = str4.Substring(0, str4.IndexOf(",")).Replace(",", "").Replace(" ", "");
                            FrameDef[2] = Convert.ToDouble(str4);
                            str4 = str3.ToLower().Substring(str3.IndexOf("a") + 1);
                            str4 = str4.Substring(0, str4.IndexOf(",")).Replace(",", "").Replace(" ", "");
                            FrameDef[3] = Convert.ToDouble(str4);
                            str4 = str3.ToLower().Substring(str3.IndexOf("b") + 1);
                            str4 = str4.Substring(0, str4.IndexOf(",")).Replace(",", "").Replace(" ", "");
                            FrameDef[4] = Convert.ToDouble(str4);
                            str4 = str3.ToLower().Substring(str3.IndexOf("c") + 1);
                            str4 = str4.Substring(0, str4.IndexOf("}")).Replace(")", "").Replace(" ", "");
                            FrameDef[5] = Convert.ToDouble(str4);
                            flag = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return flag;
        }

        public static bool ReadDefinerToolData(string RobotRootPath, int ToolNum, ref double[] FrameDef)
        {
            var path = Path.Combine(Path.Combine(RobotRootPath, "Temporary"), "definer.src");
            string str3 = null;
            string str4 = null;
            var str5 = "TOOL_DATA[" + ToolNum.ToString() + "]";
            var flag = false;
            try
            {
                using (var reader = new StreamReader(path))
                {
                    while (!flag && ((str3 = reader.ReadLine()) != null))
                    {
                        if (str3.Contains(str5))
                        {
                            str4 = str3.ToLower().Substring(str3.IndexOf("x") + 1);
                            str4 = str4.Substring(0, str4.IndexOf(",")).Replace(",", "").Replace(" ", "");
                            FrameDef[0] = Convert.ToDouble(str4);
                            str4 = str3.ToLower().Substring(str3.IndexOf("y") + 1);
                            str4 = str4.Substring(0, str4.IndexOf(",")).Replace(",", "").Replace(" ", "");
                            FrameDef[1] = Convert.ToDouble(str4);
                            str4 = str3.ToLower().Substring(str3.IndexOf("z") + 1);
                            str4 = str4.Substring(0, str4.IndexOf(",")).Replace(",", "").Replace(" ", "");
                            FrameDef[2] = Convert.ToDouble(str4);
                            str4 = str3.ToLower().Substring(str3.IndexOf("a") + 1);
                            str4 = str4.Substring(0, str4.IndexOf(",")).Replace(",", "").Replace(" ", "");
                            FrameDef[3] = Convert.ToDouble(str4);
                            str4 = str3.ToLower().Substring(str3.IndexOf("b") + 1);
                            str4 = str4.Substring(0, str4.IndexOf(",")).Replace(",", "").Replace(" ", "");
                            FrameDef[4] = Convert.ToDouble(str4);
                            str4 = str3.ToLower().Substring(str3.IndexOf("c") + 1);
                            str4 = str4.Substring(0, str4.IndexOf("}")).Replace(")", "").Replace(" ", "");
                            FrameDef[5] = Convert.ToDouble(str4);
                            flag = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return flag;
        }

        private static bool ReplaceJointValueInString(ref string d, string doobie, double Jval, ref ArrayList Problems)
        {
            try
            {
                if (!d.Contains(doobie))
                {
                    Problems.Add("ReplaceJointValueInArray: '" + doobie + "' not found in '" + d + "'");
                    return false;
                }
                var str = d.Substring(0, d.IndexOf(doobie) + 3);
                var str2 = d.Substring(d.IndexOf(doobie) + 3);
                if (!str2.Contains(","))
                {
                    Problems.Add("ReplaceJointValueInArray: ',' not found in '" + str2 + "'");
                    return false;
                }
                str2 = str2.Substring(str2.IndexOf(","));
                d = str + Jval.ToString("F3") + str2;
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
            var path = Path.Combine(RobotRootPath, Path.Combine("Temporary", "Car0J456.dat"));
            if (!File.Exists(path))
            {
                Problems.Add("Car0J456.dat not found in robot folder: " + Path.Combine(RobotRootPath, "Temporary"));
            }
            else
            {
                try
                {
                    string d = null;
                    var al = Utils.FileToArrayList(path);
                    for (var i = 0; i < al.Count; i++)
                    {
                        d = al[i].ToString().ToUpper();
                        if (d.Contains("DECL E6AXIS") && ((d.Contains("J4_") || d.Contains("J5_")) || d.Contains("J6_")))
                        {
                            for (var j = 0; j < 3; j++)
                            {
                                var num3 = j + 1;
                                if (!ReplaceJointValueInString(ref d, "A" + num3.ToString(), J123Values.Vec[j], ref Problems))
                                {
                                    return;
                                }
                            }
                            al[i] = d;
                        }
                    }
                    Utils.ArrayListToFile(path, al, true);
                }
                catch (Exception exception)
                {
                    Problems.Add(exception.Message);
                }
            }
        }

        public static void SetSingleJointAxisValue(string RobotRootPath, double value, int MovingJointNumber, int AxisIndx, ref ArrayList Problems)
        {
            var path = Path.Combine(RobotRootPath, Path.Combine("Temporary", "Car0J456.dat"));
            if (!File.Exists(path))
            {
                Problems.Add("Car0J456.dat not found in robot folder: " + Path.Combine(RobotRootPath, "Temporary"));
            }
            else
            {
                try
                {
                    string d = null;
                    var al = Utils.FileToArrayList(path);
                    var str3 = "J" + MovingJointNumber.ToString() + "_";
                    var doobie = "A" + ((AxisIndx + 1)).ToString();
                    for (var i = 0; i < al.Count; i++)
                    {
                        d = al[i].ToString().ToUpper();
                        if (d.Contains("DECL E6AXIS") && d.Contains(str3))
                        {
                            if (!ReplaceJointValueInString(ref d, doobie, value, ref Problems))
                            {
                                return;
                            }
                            al[i] = d;
                        }
                    }
                    Utils.ArrayListToFile(path, al, true);
                }
                catch (Exception exception)
                {
                    Problems.Add(exception.Message);
                }
            }
        }

        private static Vector VectorFromString(string str, ref ArrayList Problems)
        {
            var vector = new Vector(0);
            if ((((str.Contains("A1") && str.Contains("A2")) && (str.Contains("A3") && str.Contains("A4"))) && str.Contains("A5")) && str.Contains("A6"))
            {
                try
                {
                    for (var i = 1; i <= 6; i++)
                    {
                        var str2 = str.Substring(str.IndexOf("A" + i.ToString()));
                        str2 = str2.Substring(str2.IndexOf(" ") + 1);
                        while (str2.StartsWith(" "))
                        {
                            str2 = str2.Substring(1);
                        }
                        var item = Convert.ToDouble(str2.Substring(0, str2.IndexOf(",")).Replace(" ", ""));
                        vector.Vec.Add(item);
                    }
                }
                catch (Exception exception)
                {
                    Problems.Add("VectorFromString: " + exception.Message);
                }
                return vector;
            }
            Problems.Add("Cannot decode position record = '" + str + "'");
            return vector;
        }

        public static void WriteDefiner(string RobotRootPath, string FrameName, double[] FrameDef, ref int LinesWritten, bool WriteFrame, bool WriteEndStatement, ref ArrayList Problems)
        {
            var path = Path.Combine(RobotRootPath, Path.Combine("Temporary", "Car0Definer.src"));
            string str2 = null;
            try
            {
                using (var writer = new StreamWriter(path, !((int) LinesWritten).Equals(0)))
                {
                    if (((int) LinesWritten).Equals(0))
                    {
                        writer.WriteLine("&ACCESS RV1\n&REL 1\n&COMMENT Car 0 Frame defining\n&PARAM TEMPLATE = C:\\KRC\\Roboter\\Template\\vorgabe\n&PARAM EDITMASK = *\nDEF Car0Definer()\n\n;\n;Run after normal definer\n;Then delete immediately\n\n");
                    }
                    if (WriteFrame)
                    {
                        str2 = FrameName + "={X " + FrameDef[0].ToString("F4") + ",Y " + FrameDef[1].ToString("F4") + ",Z " + FrameDef[2].ToString("F4") + ",A " + FrameDef[3].ToString("F4") + ",B " + FrameDef[4].ToString("F4") + ",C " + FrameDef[5].ToString("F4") + "}\n";
                        writer.WriteLine(str2);
                        LinesWritten++;
                    }
                    if (WriteEndStatement)
                    {
                        writer.WriteLine("END\n");
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

