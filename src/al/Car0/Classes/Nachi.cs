using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;
using System.IO;
using System.Windows.Forms;


namespace Car0
{
     class Nachi
    {

        #region Private Variables
        private const int Car0FileNumber = 911, J4FileNumber = 914, J5FileNumber = 915, J6FileNumber = 916;

        #endregion
        //Contains Nachi specific methods
        #region Private Methods
        private  string Car0FileName(string RobotRootPath, ref ArrayList Problems, int ThisFileNumber)
        {
            string[] Files = Directory.GetFiles(RobotRootPath, "*." + ThisFileNumber.ToString(), SearchOption.TopDirectoryOnly);

            if (Files.Length.Equals(1))
                return Files[0];

            Problems.Add("There are " + Files.Length.ToString() + " files with a dot extension of '" + ThisFileNumber.ToString() + "' in the robot folder.");

            return "";
        }
        //Designed to get only J4, J5 and J6
        private  Vector VectorFromString(string str)
        {
            Vector v = new Vector(0);
            string[] items = str.Split(',');

            foreach (string s in items)
            {
                if (s.Length > 0)
                    v.Vec.Add(Convert.ToDouble(s));
            }

            return v;
        }

        private  Boolean UpdateJ123ForSingleJointTest(string Fpath, Vector J123Values, ref ArrayList Problems)
        {
            try
            {
                int i;

                //Process Data
                ArrayList ProgList = Utils.FileToArrayList(Fpath);
                string mesbuf = null, d = null;

                for (i = 0; i < ProgList.Count; ++i)
                {
                    d = ProgList[i].ToString();

                    if (d.Contains("(") && d.Contains(")"))
                    {
                        mesbuf = d.Substring(ProgList[i].ToString().IndexOf("(") + 1);
                        mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(")"));
                        mesbuf = mesbuf.Replace(" ", "");

                        Vector oldVals = VectorFromString(mesbuf);

                        string start = d.Substring(0, ProgList[i].ToString().IndexOf("(") + 1);
                        string end = d.Substring(d.IndexOf(")"));

                        mesbuf = J123Values.Vec[0].ToString("F3") + "," + J123Values.Vec[1].ToString("F3") + "," + J123Values.Vec[2].ToString("F3") + "," +
                                    oldVals.Vec[3].ToString("F3") + "," + oldVals.Vec[4].ToString("F3") + "," + oldVals.Vec[5].ToString("F3");

                        ProgList[i] = start + mesbuf + end;
                    }
                }

                Utils.ArrayListToFile(Fpath, ProgList, true);
            }
            catch (Exception e)
            {
                Problems.Add(e.ToString());
                return false;
            }

            return true;
        }
        #endregion
        #region Public Methods
        public  void WriteDefiner(string RobotRootPath, string FrameName, double[] FrameDef, ref int LinesWritten, Boolean WriteFrame, ref ArrayList Problems)
        {
            string Fpath = Path.Combine(RobotRootPath, "Car0Definer.txt");
            string mesbuf = null;

            try
            {
                using (StreamWriter fs = new System.IO.StreamWriter(@Fpath, !LinesWritten.Equals(0)))
                {
                    if (WriteFrame)
                    {
                        mesbuf = FrameName + "={X " + FrameDef[0].ToString("F4") + ",Y " + FrameDef[1].ToString("F4") + ",Z " + FrameDef[2].ToString("F4") +
                            ",Rx " + FrameDef[3].ToString("F4") + ",Ry " + FrameDef[4].ToString("F4") + ",Rz " + FrameDef[5].ToString("F4") + "}\n";

                        fs.WriteLine(mesbuf);

                        ++LinesWritten;
                    }

                    fs.Flush();
                }
            }
            catch (Exception e)
            {
                Problems.Add(e.ToString());
            }
        }

        public  void ReadCar0J456Targets(string RobotRootPath, out List<Vector> J4Targets, out List<Vector> J5Targets, out List<Vector> J6Targets, ref ArrayList Problems)
        {
            J4Targets = new List<Vector>();
            J5Targets = new List<Vector>();
            J6Targets = new List<Vector>();

            string J4Fpath = Car0FileName(RobotRootPath, ref Problems, J4FileNumber);
            string J5Fpath = Car0FileName(RobotRootPath, ref Problems, J5FileNumber);
            string J6Fpath = Car0FileName(RobotRootPath, ref Problems, J6FileNumber);

            if (!(File.Exists(J4Fpath) && File.Exists(J5Fpath) && File.Exists(J6Fpath)))
            {
                if (!File.Exists(J4Fpath))
                    Problems.Add("Required file missing: " + J4Fpath);

                if (!File.Exists(J5Fpath))
                    Problems.Add("Required file missing: " + J5Fpath);

                if (!File.Exists(J6Fpath))
                    Problems.Add("Required file missing: " + J6Fpath);


                return;
            }

            try
            {
                int i;

                //Process J4 Data
                ArrayList ProgList = Utils.FileToArrayList(J4Fpath);
                string mesbuf = null;

                for (i = 0; i < ProgList.Count; ++i)
                {
                    if (ProgList[i].ToString().Contains("(") && ProgList[i].ToString().Contains(")"))
                    {
                        mesbuf = ProgList[i].ToString().Substring( ProgList[i].ToString().IndexOf("(") + 1);
                        mesbuf = mesbuf.Substring(0,mesbuf.IndexOf(")"));
                        mesbuf = mesbuf.Replace(" ", "");
                        J4Targets.Add(new Vector(VectorFromString(mesbuf)));
                    }
                }
                
                //Process J5 Data
                ProgList = Utils.FileToArrayList(J5Fpath);

                for (i = 0; i < ProgList.Count; ++i)
                {
                    if (ProgList[i].ToString().Contains("(") && ProgList[i].ToString().Contains(")"))
                    {
                        mesbuf = ProgList[i].ToString().Substring( ProgList[i].ToString().IndexOf("(") + 1);
                        mesbuf = mesbuf.Substring(0,mesbuf.IndexOf(")"));
                        J5Targets.Add(new Vector(VectorFromString(mesbuf)));
                    }
                }

                //Process J6 Data
                ProgList = Utils.FileToArrayList(J6Fpath);

                for (i = 0; i < ProgList.Count; ++i)
                {
                    if (ProgList[i].ToString().Contains("(") && ProgList[i].ToString().Contains(")"))
                    {
                        mesbuf = ProgList[i].ToString().Substring(ProgList[i].ToString().IndexOf("(") + 1);
                        mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(")"));
                        J6Targets.Add(new Vector(VectorFromString(mesbuf)));
                    }
                }

                if (J4Targets.Count.Equals(0))
                    Problems.Add("No Joint 4 targets were found in :" + Car0FileName(RobotRootPath, ref Problems, J4FileNumber));

                if (J5Targets.Count.Equals(0))
                    Problems.Add("No Joint 5 targets were found in :" + Car0FileName(RobotRootPath, ref Problems, J5FileNumber));

                if (J6Targets.Count.Equals(0))
                    Problems.Add("No Joint 6 targets were found in :" + Car0FileName(RobotRootPath, ref Problems, J6FileNumber));

            }
            catch (Exception ee)
            {
                Problems.Add(ee.Message);
            }
        }

        public  List<Vector3> ReadCar0BasePtsProgram(string RobotRootPath, ref ArrayList Problems)
        {
            string mesbuf;
            List<Vector3> BaseRecords = new List<Vector3>();
            string Fpath = Car0FileName(RobotRootPath, ref Problems, Car0FileNumber);

            if (File.Exists(@Fpath))
            {
                try
                {
                    ArrayList Raw = Utils.FileToArrayList(Fpath);
                    int i;

                    for (i = 0; i < Raw.Count; ++i)
                    {
                        if (Raw[i].ToString().Contains("("))
                        {
                            mesbuf = Raw[i].ToString().Substring(Raw[i].ToString().IndexOf("(") + 1);
                            if (mesbuf.Contains(")"))
                            {
                                int j;
                                mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(")"));
                                string[] items = mesbuf.Split(',');
                                List<double> Vals = new List<double>();

                                for (j = 0; j < items.Length; ++j)
                                {
                                    if (items[j].Length > 0)
                                        Vals.Add(Convert.ToDouble(items[j]));
                                }

                                if (items.Length >= 6)
                                    BaseRecords.Add(new Vector3(Vals[0], Vals[1], Vals[2]));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Problems.Add("Nachi.ReadCar0BasePtsProgram experienced exception below: \n" + e.Message);
                    return new List<Vector3>();
                }

            }
            else
                Problems.Add("Reference points program not found (" + Fpath + ")");

            return BaseRecords;
        }

        public  void SetJ123Values(string RobotRootPath, Vector J123Values, ref ArrayList Problems)
        {
            string J4Fpath = Car0FileName(RobotRootPath, ref Problems, J4FileNumber);
            string J5Fpath = Car0FileName(RobotRootPath, ref Problems, J5FileNumber);
            string J6Fpath = Car0FileName(RobotRootPath, ref Problems, J6FileNumber);

            if (!(File.Exists(J4Fpath) && File.Exists(J5Fpath) && File.Exists(J6Fpath)))
            {
                if (!File.Exists(J4Fpath))
                    Problems.Add("Required file missing: " + J4Fpath);

                if (!File.Exists(J5Fpath))
                    Problems.Add("Required file missing: " + J5Fpath);

                if (!File.Exists(J6Fpath))
                    Problems.Add("Required file missing: " + J6Fpath);


                return;
            }

            //Process J4 data
            if (!UpdateJ123ForSingleJointTest(J4Fpath, J123Values, ref Problems))
                return;

            //Process J5 data
            if (!UpdateJ123ForSingleJointTest(J5Fpath, J123Values, ref Problems))
                return;

            //Process J6 data
            if (!UpdateJ123ForSingleJointTest(J6Fpath, J123Values, ref Problems))
                return;
        }

        public  void SetSingleJointAxisValue(string RobotRootPath, double value, int FileIndx, int AxisIndx, ref ArrayList Problems)
        {
            string Fpath = null;

            switch (FileIndx)
            {
                case 3:

                    Fpath = Car0FileName(RobotRootPath, ref Problems, J4FileNumber);
                    break;

                case 4:

                    Fpath = Car0FileName(RobotRootPath, ref Problems, J5FileNumber);
                    break;

                case 5:

                    Fpath = Car0FileName(RobotRootPath, ref Problems, J6FileNumber);
                    break;

                default:

                    Problems.Add("SetSingleJointAxisValue bug: unsupported axis number = " + AxisIndx.ToString());
                    return;
            }

            if (!File.Exists(Fpath))
            {
                Problems.Add("Required file missing: " + Fpath);
                return;
            }

            try
            {
                ArrayList ProgList = Utils.FileToArrayList(Fpath);
                string mesbuf = null, d = null;
                int i;

                for (i = 0; i < ProgList.Count; ++i)
                {
                    d = ProgList[i].ToString();

                    if (d.Contains("(") && d.Contains(")"))
                    {
                        mesbuf = d.Substring(ProgList[i].ToString().IndexOf("(") + 1);
                        mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(")"));
                        mesbuf = mesbuf.Replace(" ", "");

                        Vector oldVals = VectorFromString(mesbuf);

                        string start = d.Substring(0, ProgList[i].ToString().IndexOf("(") + 1);
                        string end = d.Substring(d.IndexOf(")"));

                        oldVals.Vec[AxisIndx] = value;

                        mesbuf = oldVals.Vec[0].ToString("F3") + "," + oldVals.Vec[1].ToString("F3") + "," + oldVals.Vec[2].ToString("F3") + "," +
                                    oldVals.Vec[3].ToString("F3") + "," + oldVals.Vec[4].ToString("F3") + "," + oldVals.Vec[5].ToString("F3");

                        ProgList[i] = start + mesbuf + end;
                    }
                }

                Utils.ArrayListToFile(Fpath, ProgList, true);
            }
            catch (Exception e)
            {
                Problems.Add("Nachi.SetSingleJointAxisValue experienced exception below: \n" + e.Message);
            }
        }
        #endregion
    }
}
