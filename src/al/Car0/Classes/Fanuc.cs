using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Car0
{
     class Fanuc
    {
        public event NotifyMessageEventHandler NotifyMessage;
        private void raiseNotify(string message, string title)
        {
            if (NotifyMessage!=null)
                NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
        }

        //Contains Fanuc robot specific methods
        #region Private Methods
        private  string make_two_digit_string(int mint)
        {
            string mybuf;

            if (mint >= 10)
                mybuf = "" + mint;
            else
                mybuf = "0" + mint;

            return mybuf;
        }

        private  Boolean WriteFanucMnLine(string LineData, ref int LineNum, ref StreamWriter fs)
        {
            try
            {
                string mesbuf = LineNum.ToString();
                ++LineNum;
                mesbuf = mesbuf.PadLeft(4) + ":  " + LineData + ";";
                fs.WriteLine(mesbuf);
            }
            catch (Exception ee)
            {
                raiseNotify(ee.Message, "WriteFanucMnLine");
                return false;
            }

            return true;
        }

        /* FormatDefinerNumber(val,fstring)
         * 
         *      Formats a numeric string for use in a definer program handling the fact that 
         *      negative numbers must be between ()'s.
         */
        private  string FormatDefinerNumber(double val, string fstring)
        {
            string mybuf;

            if (val < 0.0)
                mybuf = "(" + val.ToString(fstring) + ")";
            else
                mybuf = val.ToString(fstring);

            return mybuf;
        }


        /* WriteFanucDefinerFrameRecord(TypeMessage,FrameNum,fromAto,ref LineNum,ref fs)
         * 
         *      Writes a frame definition record to a Fanuc Frame Definer program.
         * 
         *  Inputs:     TypeMessage     Defines the type of the frame (UTOOL or UFRAME).
         *              FrameNum        Number of the frame being written.
         *              fromAto         Transformation coordinates being written.
         * 
         *  I/Os:       LineNum         NM section line number.
         *              fs              Filestream
         */
        private  void WriteFanucDefinerFrameRecord(string TypeMessage, int FrameNum, double[] fromAto, ref StreamWriter fs, ref int LineNum, Boolean Rj3Controller)
        {
            string mybuf = null;
            int i;

            WriteFanucMnLine("!Defining " + TypeMessage + " " + FrameNum, ref LineNum, ref fs);
            WriteFanucMnLine("PR[9] = LPOS", ref LineNum, ref fs);

            for (i=0; i<6; ++i)
               WriteFanucMnLine("PR[GP1:9," + (i+1).ToString() + "] = " + FormatDefinerNumber(fromAto[i], "F3"), ref LineNum, ref fs);

            mybuf = "" + LineNum++;

            mybuf = (Rj3Controller) ? ("$MN" + TypeMessage + "[1," + FrameNum + "] = PR[9]") : (TypeMessage + "[" + FrameNum + "] = PR[9]");
            WriteFanucMnLine(mybuf, ref LineNum, ref fs);

            WriteFanucMnLine("", ref LineNum, ref fs);
        }

        private  Vector VectorFromString(string str, Vector v, ref ArrayList Problems)
        {
            if ((str.Contains("J1") && str.Contains("J2") && str.Contains("J3")))
            {
                try
                {
                    int i;

                    for (i = 1; i <= 3; ++i)
                    {
                        string mesbuf = str.Substring(str.IndexOf("J" + i.ToString()));
                        mesbuf = mesbuf.Substring(mesbuf.IndexOf("=") + 1);

                        while (mesbuf.StartsWith(" "))
                            mesbuf = mesbuf.Substring(1);

                        mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(" "));
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
            else if ((str.Contains("J4") && str.Contains("J5") && str.Contains("J6")))
            {
                try
                {
                    int i;

                    for (i = 4; i <= 6; ++i)
                    {
                        string mesbuf = str.Substring(str.IndexOf("J" + i.ToString()));
                        mesbuf = mesbuf.Substring(mesbuf.IndexOf("=") + 1);

                        while (mesbuf.StartsWith(" "))
                            mesbuf = mesbuf.Substring(1);

                        mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(" "));
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
            {
                if ((str.Contains("W") && str.Contains("P") && str.Contains("R")))
                    Problems.Add("CAR0TOOL.LS contains a WPR format position.  Joint format is required!");
                else
                    Problems.Add("Cannot decode CAR0TOOL.LS position record = '" + str + "'");
            }

            return v;
        }
        //Designed to get only J4, J5 and J6
        private  Vector VectorFromString(string str, ref ArrayList Problems)
        {
            Vector v = new Vector(0);           

            return VectorFromString(str, v, ref Problems);
        }

        private  Vector VectorFromArrayListPair(ArrayList MyList, int FirstIndx, ref ArrayList Problems)
        {
            Vector v = VectorFromString(MyList[FirstIndx].ToString(), ref Problems);

            return VectorFromString(MyList[FirstIndx + 1].ToString(), v, ref Problems);
        }

        private  Boolean ReplaceJointValueInArray(ArrayList MyList, int Indx, string doobie, double Jval,ref ArrayList Problems)
        {
            try
            {
                string d = MyList[Indx].ToString();

                if (!d.Contains(doobie))
                {
                    Problems.Add("ReplaceJointValueInArray: '" + doobie + "' not found in '" + d + "'");
                    return false;
                }

                string buffer1 = d.Substring(0, d.IndexOf(doobie) + 3);

                string buffer2 = d.Substring(d.IndexOf(doobie) + 3);

                if (!buffer2.Contains(" deg"))
                {
                    Problems.Add("ReplaceJointValueInArray: ' deg' not found in '" + buffer2 + "'");
                    return false;
                }

                buffer2 = buffer2.Substring(buffer2.IndexOf(" deg"));

                MyList[Indx] = buffer1 + Jval.ToString("F3").PadLeft(10) + buffer2;
            }
            catch (Exception ee)
            {
                Problems.Add("ReplaceJointValueInArray Exception: " + ee.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Public Methods

        public  Boolean ReadDefinerToolData(string RobotRootPath, int ToolNum, ref double[] FrameDef)
        {
            string Fpath = Path.Combine(RobotRootPath, "definer.ls");
            string line = null, mesbuf = null;
            string DefiningRec = "!Defining UTOOL " + ToolNum.ToString() + ";";
            string UtoolRec = "UTOOL[" + ToolNum.ToString() + "] = PR[9] ;";
            Boolean foundAll = false;

            try
            {
                using (StreamReader myStream = new System.IO.StreamReader(@Fpath))
                {
                    Boolean FoundDefining = false, FoundPrEqualsLPOS = false, FoundVal = false, foundUtoolRec = false;

                    while (!foundAll && ((line = myStream.ReadLine()) != null))
                    {
                        if (!FoundDefining)
                        {
                            if (line.Contains(DefiningRec))
                            {
                                while (!FoundPrEqualsLPOS && ((line = myStream.ReadLine()) != null))
                                {
                                    if (line.Contains("PR[9] = LPOS"))
                                    {
                                        int i;

                                        for (i = 0; i < 6; ++i)
                                        {
                                            FoundVal = false;
                                            mesbuf = "PR[GP1:9," + (i + 1).ToString() + "] = ";

                                            while (!FoundVal && ((line = myStream.ReadLine()) != null))
                                            {
                                                if (line.Contains(mesbuf))
                                                {
                                                    string buffer1 = line.Substring(line.IndexOf(mesbuf) + mesbuf.Length);
                                                    buffer1 = buffer1.Replace(" ", "");
                                                    buffer1 = buffer1.Replace("(", "");
                                                    buffer1 = buffer1.Replace(")", "");
                                                    buffer1 = buffer1.Replace(";", "");

                                                    FrameDef[i] = Convert.ToDouble(buffer1);
                                                }
                                                FoundVal = true;
                                            }

                                            if (!FoundVal)
                                                break;
                                        }

                                        FoundPrEqualsLPOS = FoundVal;

                                        if (FoundVal)
                                        {
                                            //Look for confirming record

                                            while (!foundUtoolRec && ((line = myStream.ReadLine()) != null))
                                            {
                                                if (line.Contains(UtoolRec))
                                                    foundUtoolRec = foundAll = true;
                                            }

                                        }
                                    }
                                }

                                FoundDefining = true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return foundAll;
        }

        public  List<Vector3> ReadCar0BasePtsProgram(string RobotRootPath, ref ArrayList Problems)
        {
            List<Vector3> BaseRecords = new List<Vector3>();
            string Fpath = Path.Combine(RobotRootPath, "CAR0PTS.LS");

            if (File.Exists(@Fpath))
            {
                try
                {
                    ArrayList Raw = Utils.FileToArrayList(Fpath);
                    int i;
                    Boolean PosFound = false;
                    Vector3 xyz = new Vector3();

                    for (i = 0; i < Raw.Count; ++i)
                    {
                        if (PosFound)
                        {
                            if (Raw[i].ToString().Contains("X =") && Raw[i].ToString().Contains("Y =") && Raw[i].ToString().Contains("Z ="))
                            {
                                xyz.x = Utils.DoubleFromString(Raw[i].ToString(), "X =", " ");
                                xyz.y = Utils.DoubleFromString(Raw[i].ToString(), "Y =", " ");
                                xyz.z = Utils.DoubleFromString(Raw[i].ToString(), "Z =", " ");

                                BaseRecords.Add(new Vector3(xyz));

                            }
                        }
                        else
                            PosFound = Raw[i].ToString().StartsWith("/POS");
                    }

                    if (!PosFound)
                        Problems.Add("No /POS section found in CAR0PTS.LS");
                }
                catch (Exception e)
                {
                    Problems.Add("Fanuc.ReadCar0BasePtsProgram experienced exception below: \n" + e.Message);
                    return new List<Vector3>();
                }

            }
            else
                Problems.Add("Reference points program not found (" + Fpath + ")");

            return BaseRecords;
        }

        public  void ReadCar0J456Targets(string RobotRootPath, out List<Vector> J4Targets, out List<Vector> J5Targets, out List<Vector> J6Targets, ref ArrayList Problems)
        {
            J4Targets = new List<Vector>();
            J5Targets = new List<Vector>();
            J6Targets = new List<Vector>();
            Boolean InPos = false;

            string Fpath = Path.Combine(RobotRootPath, "CAR0TOOL.LS");

            if (!File.Exists(@Fpath))
            {
                Problems.Add("CAR0TOOL.LS" + " not found in robot folder: " + RobotRootPath);
                return;
            }

            try
            {
                int i;
                ArrayList ProgList = Utils.FileToArrayList(Fpath);

                for (i=0; i<ProgList.Count; ++i)
                {
                    if (InPos)
                    {
                        if (ProgList[i].ToString().Contains("P["))
                        {
                            if (ProgList[i].ToString().Contains("J4_"))
                            {
                                J4Targets.Add(new Vector(VectorFromArrayListPair(ProgList, i + 3, ref Problems)));

                                if (Problems.Count.Equals(0))
                                    i += 4;
                                else
                                    i = ProgList.Count;
                            }
                            else if (ProgList[i].ToString().Contains("J5_"))
                            {
                                J5Targets.Add(new Vector(VectorFromArrayListPair(ProgList, i + 3, ref Problems)));

                                if (Problems.Count.Equals(0))
                                    i += 4;
                                else
                                    i = ProgList.Count;
                            }
                            else if (ProgList[i].ToString().Contains("J6_"))
                            {
                                J6Targets.Add(new Vector(VectorFromArrayListPair(ProgList, i + 3, ref Problems)));

                                if (Problems.Count.Equals(0))
                                    i += 4;
                                else
                                    i = ProgList.Count;
                            }
                        }
                    }
                    else if (!InPos)
                    {
                        if (ProgList[i].ToString().StartsWith("/POS"))
                            InPos = true;
                    }
                }

                if (J4Targets.Count.Equals(0))
                    Problems.Add("No Joint 4 targets were found in :" + "CAR0TOOL.LS");

                if (J5Targets.Count.Equals(0))
                    Problems.Add("No Joint 5 targets were found in :" + "CAR0TOOL.LS");

                if (J6Targets.Count.Equals(0))
                    Problems.Add("No Joint 6 targets were found in :" + "CAR0TOOL.LS");

            }
            catch (Exception ee)
            {
                Problems.Add(ee.Message);
            }
        }

        public  void WriteDefiner(string RobotRootPath, string FrameName, double[] FrameDef, ref int MnLineNumber, Boolean WriteFrame, Boolean Rj3Controller, ref ArrayList Problems)
        {
            string Fpath = Path.Combine(RobotRootPath, "C0Define.ls");
            string mesbuf = null, FrameType = null;
            int FrameNum = 0;
            StreamWriter fs = null;

            try
            {
                if (WriteFrame)
                {
                    //Decode the frame name into the things we need to write the record
                    FrameType = FrameName.Substring(0, FrameName.IndexOf(" "));
                    mesbuf = FrameName.Substring(FrameName.IndexOf(" "));

                    while (mesbuf.StartsWith(" "))
                        mesbuf = mesbuf.Substring(1);

                    mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(" "));

                    FrameNum = Convert.ToInt32(mesbuf);
                }

                using (fs = new System.IO.StreamWriter(@Fpath, !MnLineNumber.Equals(0)))
                {
                    if (MnLineNumber.Equals(0))
                    {
                        //SWAG in a header
                        fs.WriteLine("/PROG    C0Define");
                        fs.WriteLine("/ATTR");
                        fs.WriteLine("OWNER            = MNEDITOR;");
                        fs.WriteLine("COMMENT          = \"Car 0 Definer\";");
                        fs.WriteLine("PROG_SIZE        = 0;");

                        mesbuf = "CREATE           = DATE " + make_two_digit_string(DateTime.Now.Year - 2000) + "-" +
                            make_two_digit_string(DateTime.Now.Month) + "-" + make_two_digit_string(DateTime.Now.Day) +
                            "    TIME " + make_two_digit_string(DateTime.Now.Hour) + ":" +
                            make_two_digit_string(DateTime.Now.Minute) + ":" +
                            make_two_digit_string(DateTime.Now.Second) + " ;";

                        fs.WriteLine(mesbuf);

                        mesbuf = "MODIFIED         = DATE " + make_two_digit_string(DateTime.Now.Year - 2000) + "-" +
                            make_two_digit_string(DateTime.Now.Month) + "-" + make_two_digit_string(DateTime.Now.Day) +
                            "    TIME " + make_two_digit_string(DateTime.Now.Hour) + ":" +
                            make_two_digit_string(DateTime.Now.Minute) + ":" +
                            make_two_digit_string(DateTime.Now.Second) + " ;";

                        fs.WriteLine(mesbuf);

                        fs.WriteLine("DEFAULT_GROUP   = 1,*,*,*,*;");
                        fs.WriteLine("CONTROL_CODE    = 00000000 00000000;");
                        fs.WriteLine("/APPL\r\nSPOT : TRUE ;\r\nSPOT Welding Equipment Number : 1 ;\r\nCYCLE_REFERENCE = 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0;\r\nCYCLE_TARGET = 0.00 ;");
                        fs.WriteLine("/MN");
                        ++MnLineNumber;
                        WriteFanucMnLine("!************************************", ref MnLineNumber, ref fs);
                        WriteFanucMnLine("!Car 0 Definer File", ref MnLineNumber, ref fs);
                        WriteFanucMnLine("", ref MnLineNumber, ref fs);
                        WriteFanucMnLine("!Use once to update measured frames", ref MnLineNumber, ref fs);
                        WriteFanucMnLine("!THEN DISCARD", ref MnLineNumber, ref fs);
                        WriteFanucMnLine("!------------------------------------", ref MnLineNumber, ref fs);
                    }


                    if (WriteFrame)
                        WriteFanucDefinerFrameRecord(FrameType, FrameNum, FrameDef, ref fs, ref MnLineNumber, Rj3Controller);
                    else
                    {
                        fs.WriteLine("/POS");
                        fs.WriteLine("/END");
                    }

                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                Problems.Add(e.ToString());
            }
        }

        public  void SetJ123Values(string RobotRootPath, Vector J123Values, ref ArrayList Problems)
        {
            string Fpath = Path.Combine(RobotRootPath, "CAR0TOOL.LS");

            if (!File.Exists(Fpath))
            {
                Problems.Add("Required file missing: " + Fpath);
                return;
            }

            try
            {
                int i;
                ArrayList ProgList = Utils.FileToArrayList(Fpath);
                Boolean InPos = false;

                for (i = 0; i < ProgList.Count; ++i)
                {
                    if (InPos)
                    {
                        if (ProgList[i].ToString().Contains("P["))
                        {
                            if (ProgList[i].ToString().Contains("J4_") || ProgList[i].ToString().Contains("J5_") || ProgList[i].ToString().Contains("J6_"))
                            {
                                if (!(ReplaceJointValueInArray(ProgList, i + 3, "J1=", J123Values.Vec[0], ref Problems) &&
                                    ReplaceJointValueInArray(ProgList, i + 3, "J2=", J123Values.Vec[1], ref Problems) &&
                                    ReplaceJointValueInArray(ProgList, i + 3, "J3=", J123Values.Vec[2], ref Problems)))
                                    return;

                                if (Problems.Count.Equals(0))
                                    i += 4;
                                else
                                    i = ProgList.Count;
                            }
                        }
                    }
                    else if (!InPos)
                    {
                        if (ProgList[i].ToString().StartsWith("/POS"))
                            InPos = true;
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
            string Fpath = Path.Combine(RobotRootPath, "CAR0TOOL.LS");

            if (!File.Exists(Fpath))
            {
                Problems.Add("Required file missing: " + Fpath);
                return;
            }

            try
            {
                ArrayList ProgList = Utils.FileToArrayList(Fpath);
                int i;
                Boolean InPos = false;
                string MovingJoint = "J" + MovingJointNumber.ToString() + "_";
                string JointSearchString = "J" + (AxisIndx + 1).ToString() + "=";

                for (i = 0; i < ProgList.Count; ++i)
                {
                    if (InPos)
                    {
                        if (ProgList[i].ToString().Contains("P["))
                        {
                            if (ProgList[i].ToString().Contains(MovingJoint))
                            {
                                if (!ReplaceJointValueInArray(ProgList, i + 4, JointSearchString, value, ref Problems))
                                    return;

                                if (Problems.Count.Equals(0))
                                    i += 4;
                                else
                                    i = ProgList.Count;
                            }
                        }
                    }
                    else if (!InPos)
                    {
                        if (ProgList[i].ToString().StartsWith("/POS"))
                            InPos = true;
                    }
                }

                Utils.ArrayListToFile(Fpath, ProgList, true);
            }
            catch (Exception ee)
            {
                Problems.Add("Fanuc.SetSingleJointAxisValue experienced exception below: \n" + ee.Message);;
            }
        }
        #endregion
    }
}
