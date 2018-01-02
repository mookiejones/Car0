using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Car0
{
     class Kuka
    {
        //Contains KUKA robot specific methods
        #region Private Methods
        //Designed to get only J4, J5 and J6
        private  Vector VectorFromString(string str, ref ArrayList Problems)
        {
            Vector v = new Vector(0);
            
            // Insure that string is uppercase for evaluation
            str = str.ToUpper();
            if ((str.Contains("A1") && str.Contains("A2") && str.Contains("A3") && str.Contains("A4") && str.Contains("A5") && str.Contains("A6")))
            {
                try
                {
                    int i;

                    for (i = 1; i <= 6; ++i)
                    {
                        string mesbuf = str.Substring(str.IndexOf("A" + i.ToString()));
                        mesbuf = mesbuf.Substring(mesbuf.IndexOf(" ") + 1);

                        while (mesbuf.StartsWith(" "))
                            mesbuf = mesbuf.Substring(1);

                        mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(","));
                        mesbuf = mesbuf.Replace(" ", "");

                        double x = Convert.ToDouble(mesbuf);

                        v.Vec.Add(x);
                    }
                }
                catch (Exception e)
                {
                    Problems.Add("VectorFromString: " + e.Message);
                }
            }
            else
                Problems.Add("Cannot decode position record = '" + str + "'");

            return v;
        }
        private  Boolean ReplaceJointValueInString(ref string d, string doobie, double Jval, ref ArrayList Problems)
        {
            try
            {
                if (!d.Contains(doobie))
                {
                    Problems.Add("ReplaceJointValueInArray: '" + doobie + "' not found in '" + d + "'");
                    return false;
                }

                string buffer1 = d.Substring(0, d.IndexOf(doobie) + 3);

                string buffer2 = d.Substring(d.IndexOf(doobie) + 3);

                if (!buffer2.Contains(","))
                {
                    Problems.Add("ReplaceJointValueInArray: ',' not found in '" + buffer2 + "'");
                    return false;
                }

                buffer2 = buffer2.Substring(buffer2.IndexOf(","));

                d = buffer1 + Jval.ToString("F3") + buffer2;
            }
            catch (Exception ee)
            {
                Problems.Add("ReplaceJointValueInArray Exception: " + ee.Message);
                return false;
            }
            return true;
        }
        private  Double GetDefinerComponent(string line, string component)
        {
            
            line = line.ToUpper().Substring(line.IndexOf(component) + 1);
            if (!(component == "C"))
            {
                line = line.Substring(0, line.IndexOf(","));
            }
            else
            {
                line = line.Substring(0, line.IndexOf("}"));
            }

            return Convert.ToDouble(line.Trim());
        }
        #endregion
        #region Public Methods
        public  Boolean ReadDefinerData(string RobotRootPath, int ToolNum, double[] FrameDef, string TypeData)
        {
            string f = Path.Combine(RobotRootPath, "Temporary");
            string Fpath = Path.Combine(f, "definer.src");
            string line = string.Empty, Look4Me = TypeData + ToolNum.ToString() + "]";
            Boolean found = false;

            try
            {
                using (StreamReader myStream = new System.IO.StreamReader(@Fpath))
                {

                    while (!found && ((line = myStream.ReadLine()) != null))
                    {
                        if (line.ToUpper().Contains(Look4Me))
                        {
                            FrameDef[0] = GetDefinerComponent(line, "X");
                            FrameDef[1] = GetDefinerComponent(line, "Y");
                            FrameDef[2] = GetDefinerComponent(line, "Z");
                            FrameDef[3] = GetDefinerComponent(line, "A");
                            FrameDef[4] = GetDefinerComponent(line, "B");
                            FrameDef[5] = GetDefinerComponent(line, "C");

                            found = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return found;
        }
        public  Boolean ReadDefinerBaseData(string RobotRootPath, int ToolNum, ref double[] FrameDef)
        {
            return ReadDefinerData(RobotRootPath, ToolNum, FrameDef, "BASE_DATA[");
        }
        public  Boolean ReadDefinerToolData(string RobotRootPath, int ToolNum, ref double[] FrameDef)
        {
            return ReadDefinerData(RobotRootPath, ToolNum, FrameDef, "TOOL_DATA[");

        }

        public  void ReadCar0J456Targets(string RobotRootPath, out List<Vector> J4Targets, out List<Vector> J5Targets, out List<Vector> J6Targets, ref ArrayList Problems)
        {
            J4Targets = new List<Vector>();
            J5Targets = new List<Vector>();
            J6Targets = new List<Vector>();

            string Fpath = Path.Combine(RobotRootPath, Path.Combine("Temporary", "Car0J456.dat"));

            if (!File.Exists(@Fpath))
            {
                Problems.Add("Car0J456.dat" + " not found in robot folder: " + Path.Combine(RobotRootPath, "Temporary"));
                return;
            }

            try
            {
                int i;
                ArrayList ProgList = Utils.FileToArrayList(Fpath);

                for (i = 0; i < ProgList.Count; ++i)
                {
                    if (ProgList[i].ToString().ToUpper().Contains("DECL E6AXIS"))
                    {
                        if (ProgList[i].ToString().ToUpper().Contains("J4_"))
                        {
                            J4Targets.Add(new Vector(VectorFromString(ProgList[i].ToString().ToUpper(), ref Problems)));

                            if (!Problems.Count.Equals(0))
                                i = ProgList.Count;
                        }
                        else if (ProgList[i].ToString().ToUpper().Contains("J5_"))
                        {
                            J5Targets.Add(new Vector(VectorFromString(ProgList[i].ToString().ToUpper(), ref Problems)));

                            if (!Problems.Count.Equals(0))
                                i = ProgList.Count;
                        }
                        else if (ProgList[i].ToString().ToUpper().Contains("J6_"))
                        {
                            J6Targets.Add(new Vector(VectorFromString(ProgList[i].ToString().ToUpper(), ref Problems)));

                            if (!Problems.Count.Equals(0))
                                i = ProgList.Count;
                        }
                    }
                }

                if (J4Targets.Count.Equals(0))
                    Problems.Add("No Joint 4 targets were found");

                if (J5Targets.Count.Equals(0))
                    Problems.Add("No Joint 5 targets were found");

                if (J6Targets.Count.Equals(0))
                    Problems.Add("No Joint 6 targets were found");

            }
            catch (Exception ee)
            {
                Problems.Add(ee.Message);
            }
        }

        public  void WriteDefiner(string RobotRootPath, string FrameName, double[] FrameDef, ref int LinesWritten, Boolean WriteFrame, Boolean WriteEndStatement, ref ArrayList Problems)
        {
            string Fpath = Path.Combine(RobotRootPath, Path.Combine("Temporary", "Car0Definer.src"));
            string mesbuf = null;

            try
            {
                using (StreamWriter fs = new System.IO.StreamWriter(@Fpath, !LinesWritten.Equals(0)))
                {
                    if (LinesWritten.Equals(0))
                        fs.WriteLine("&ACCESS RV1\n&REL 1\n&COMMENT Car 0 Frame defining\n&PARAM TEMPLATE = C:\\KRC\\Roboter\\Template\\vorgabe\n&PARAM EDITMASK = *\nDEF Car0Definer()\n\n;\n;Run after normal definer\n;Then delete immediately\n\n");

                    if (WriteFrame)
                    {
                        mesbuf = FrameName + "={X " + FrameDef[0].ToString("F4") + ",Y " + FrameDef[1].ToString("F4") + ",Z " + FrameDef[2].ToString("F4") +
                            ",A " + FrameDef[3].ToString("F4") + ",B " + FrameDef[4].ToString("F4") + ",C " + FrameDef[5].ToString("F4") + "}\n";

                        fs.WriteLine(mesbuf);
                        ++LinesWritten;
                    }

                    if (WriteEndStatement)
                        fs.WriteLine("END\n");

                    fs.Flush();

                }
            }
            catch (Exception e)
            {
                Problems.Add(e.ToString());
            }
        }

        public  List<Vector3> ReadCar0BasePtsProgram(string RobotRootPath, ref ArrayList Problems)
        {
            List<Vector3> BaseRecords = new List<Vector3>();
            string Fpath = Path.Combine(Path.Combine(RobotRootPath, "Temporary"), "Car0Pts.dat");

            if (File.Exists(@Fpath))
            {
                try
                {
                    ArrayList Raw = Utils.FileToArrayList(Fpath);
                    int i;
                    Vector3 xyz = new Vector3();

                    for (i = 0; i < Raw.Count; ++i)
                    {
                        Raw[i] = Raw[i].ToString().ToUpper();
                        if (Raw[i].ToString().Contains("X ") && Raw[i].ToString().Contains("Y ") && Raw[i].ToString().Contains("Z "))
                        {
                            xyz.x = Utils.DoubleFromString(Raw[i].ToString().ToUpper(), "X ", ",");
                            xyz.y = Utils.DoubleFromString(Raw[i].ToString().ToUpper(), "Y ", ",");
                            xyz.z = Utils.DoubleFromString(Raw[i].ToString().ToUpper(), "Z ", ",");

                            BaseRecords.Add(new Vector3(xyz));
                        }
                    }
                }
                catch (Exception e)
                {
                    Problems.Add("Kuka.ReadCar0BasePtsProgram experienced exception below: \n" + e.Message);
                    return new List<Vector3>();
                }
            }
            else
                Problems.Add("Reference points program not found (" + Fpath + ")");

            return BaseRecords;
        }


        public  void SetJ123Values(string RobotRootPath, Vector J123Values, ref ArrayList Problems)
        {
            string Fpath = Path.Combine(RobotRootPath, Path.Combine("Temporary", "Car0J456.dat"));

            if (!File.Exists(@Fpath))
            {
                Problems.Add("Car0J456.dat" + " not found in robot folder: " + Path.Combine(RobotRootPath, "Temporary"));
                return;
            }

            try
            {
                int i;
                string d = null;
                ArrayList ProgList = Utils.FileToArrayList(Fpath);

                for (i = 0; i < ProgList.Count; ++i)
                {
                    d = ProgList[i].ToString().ToUpper();

                    if (d.Contains("DECL E6AXIS"))
                    {
                        if (d.Contains("J4_") || d.Contains("J5_") || d.Contains("J6_"))
                        {
                            int j;

                            for (j = 0; j < 3; ++j)
                            {
                                if (!ReplaceJointValueInString(ref d, "A" + (j + 1).ToString(), J123Values.Vec[j], ref Problems))
                                    return;
                            }

                            ProgList[i] = d;
                        }
                    }

                }
                Utils.ArrayListToFile(Fpath, ProgList, true);
            }
            catch (Exception ee)
            {
                Problems.Add(ee.Message);
            }
        }

        public  void SetSingleJointAxisValue(string RobotRootPath, double value, int MovingJointNumber, int AxisIndx, ref ArrayList Problems)
        {
            string Fpath = Path.Combine(RobotRootPath, Path.Combine("Temporary", "Car0J456.dat"));

            if (!File.Exists(@Fpath))
            {
                Problems.Add("Car0J456.dat" + " not found in robot folder: " + Path.Combine(RobotRootPath, "Temporary"));
                return;
            }

            try
            {
                int i;
                string d = null;
                ArrayList ProgList = Utils.FileToArrayList(Fpath);
                string MovingJoint = "J" + MovingJointNumber.ToString() + "_";
                string JointSearchString = "A" + (AxisIndx + 1).ToString();

                for (i = 0; i < ProgList.Count; ++i)
                {
                    d = ProgList[i].ToString().ToUpper();

                    if (d.Contains("DECL E6AXIS"))
                    {
                        if (d.Contains(MovingJoint))
                        {
                            if (!ReplaceJointValueInString(ref d, JointSearchString, value, ref Problems))
                                return;

                            ProgList[i] = d;
                        }
                    }

                }

                Utils.ArrayListToFile(Fpath, ProgList, true);
            }
            catch (Exception ee)
            {
                Problems.Add(ee.Message);
            }
        }
        #endregion
    }
}
