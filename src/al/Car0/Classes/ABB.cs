using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Car0
{
     class ABB
    {
         public event NotifyMessageEventHandler NotifyMessage;
         private void raiseNotify(string message, string title)
         {
             if (NotifyMessage!=null)
                 NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
         }

        #region Public Variables
        #endregion
        #region Private Variables
        private  Boolean CheckedDecoderRing = false;
        private  string MyCar0BasePtsProgram = "mycar0pts.mod";
        private  string MyCar0Definer = "C0Define.mod";
        private  string MyDefinerCarriedGunTcpfFormat = "GunData<ToolNumber>";
        private  string MyDefinerGripperPinTcpfFormat = "GripData<ToolNumber>";
        private  string MyDefinerPedGunTipFormat = "PedTip<ToolNumber>";
        private  string MyDefinerWobjFormat = "Wobj_<RobotName>_<WobjNumber>";
        private  string MyCar0JointPosesProgram = "Czp.mod";
        #endregion
        #region Private Methods
        private  void CheckDecoderRing()
        {
            if (!CheckedDecoderRing)
            {
                try
                {
                    string Fname = Path.Combine(UserPreferences.WorkFolderName, "DecoderRing.txt");

                    if (File.Exists(@Fname))
                    {
                        ArrayList Data = Utils.FileToArrayList(Fname);
                        int i;

                        for (i = 0; i < Data.Count; ++i)
                        {
                            if (Data[i].ToString().StartsWith("Car0BasePtsProgram") && Data[i].ToString().Contains(","))
                                MyCar0BasePtsProgram = Data[i].ToString().Substring(Data[i].ToString().IndexOf(",") + 1);
                            else if (Data[i].ToString().StartsWith("Car0Definer") && Data[i].ToString().Contains(","))
                                MyCar0Definer = Data[i].ToString().Substring(Data[i].ToString().IndexOf(",") + 1);
                            else if (Data[i].ToString().StartsWith("DefinerCarriedGunTcpfFormat") && Data[i].ToString().Contains(","))
                                MyDefinerCarriedGunTcpfFormat = Data[i].ToString().Substring(Data[i].ToString().IndexOf(",") + 1);
                            else if (Data[i].ToString().StartsWith("DefinerGripperPinTcpfFormat") && Data[i].ToString().Contains(","))
                                MyDefinerGripperPinTcpfFormat = Data[i].ToString().Substring(Data[i].ToString().IndexOf(",") + 1);
                            else if (Data[i].ToString().StartsWith("DefinerPedGunTipFormat") && Data[i].ToString().Contains(","))
                                MyDefinerPedGunTipFormat = Data[i].ToString().Substring(Data[i].ToString().IndexOf(",") + 1);
                            else if (Data[i].ToString().StartsWith("DefinerWobjFormat") && Data[i].ToString().Contains(","))
                                MyDefinerWobjFormat = Data[i].ToString().Substring(Data[i].ToString().IndexOf(",") + 1);
                            else if (Data[i].ToString().StartsWith("Car0JointPosesProgram") && Data[i].ToString().Contains(","))
                                MyCar0JointPosesProgram = Data[i].ToString().Substring(Data[i].ToString().IndexOf(",") + 1);
                        }
                    }
                }
                catch (Exception)
                {
                }

                CheckedDecoderRing = true;
            }
        }
        private  string Car0BasePtsProgramName()
        {
            CheckDecoderRing();
            return MyCar0BasePtsProgram;
        }
        private  string Car0DefinerName()
        {
            CheckDecoderRing();
            return MyCar0Definer;
        }
        private  string DefinerCarriedGunTcpfFormat()
        {
            CheckDecoderRing();
            return MyDefinerCarriedGunTcpfFormat;
        }
        private  string DefinerGripperPinTcpfFormat()
        {
            CheckDecoderRing();
            return MyDefinerGripperPinTcpfFormat;
        }
        private  string DefinerPedGunTipFormat()
        {
            CheckDecoderRing();
            return MyDefinerPedGunTipFormat;
        }
        private  string DefinerWobjFormat()
        {
            CheckDecoderRing();
            return MyDefinerWobjFormat;
        }
        private  string Car0JointPosesProgram()
        {
            CheckDecoderRing();
            return MyCar0JointPosesProgram;
        }
        private  Boolean FoundExactMatchRecord(ArrayList Items, string TypeString, string MatchString, out int i)
        {
            for (i = 0; i < Items.Count; ++i)
            {
                if (Items[i].ToString().Contains(TypeString) && Items[i].ToString().Contains(MatchString))
                {
                    //Check that charactor before MatchString is a space.
                    int j = Items[i].ToString().IndexOf(MatchString);
                    string buf1 = Items[i].ToString().Substring(j - 1);

                    if (buf1.StartsWith(" "))
                    {
                        //Check that the character after MatchString is a space or a :
                        string buf2 = buf1.Substring(MatchString.Length + 1);

                        if (buf2.StartsWith(" ") || buf2.StartsWith(":="))
                            return true;
                    }
                }
            }

            return false;
        }
        //Returns with LastOfType set to the LAST matching record
        private  Boolean FoundMatchingType(ArrayList Items, string TypeString, out int LastOfType)
        {
            int i;
            Boolean FoundAny = false;

            LastOfType = -1;

            for (i = 0; i < Items.Count; ++i)
            {
                if (Items[i].ToString().Contains(TypeString))
                {
                    FoundAny = true;
                    LastOfType = i;
                }
            }

            return FoundAny;
        }
        private  string BuildFrameCoordsString(double[] FrameDef, Boolean WobjDataRecDetected, string EndRec)
        {
            double q1 = 0.0, q2 = 0.0, q3 = 0.0, q4 = 0.0;
            string mesbuf;

            if (WobjDataRecDetected)
            {
                Transformation rfAf = new Transformation(FrameDef[0], FrameDef[1], FrameDef[2], Utils.DegreesToRad(FrameDef[3]), Utils.DegreesToRad(FrameDef[4]), Utils.DegreesToRad(FrameDef[5]));

                rfAf.trans_Quaterneon(ref q1, ref q2, ref q3, ref q4);
                mesbuf = FrameDef[0].ToString("F3") + "," + FrameDef[1].ToString("F3") + "," + FrameDef[2].ToString("F3") + EndRec +
                    q1.ToString("F7") + "," + q2.ToString("F7") + "," + q3.ToString("F7") + "," + q4.ToString("F7");
            }
            else
                mesbuf = FrameDef[0].ToString("F3") + "," + FrameDef[1].ToString("F3") + "," + FrameDef[2].ToString("F3");

            return mesbuf;
        }

        private  Boolean EndModuleRecordFound(ArrayList Items, out int i)
        {
            for (i = 0; i < Items.Count; ++i)
            {
                if (Items[i].ToString().Contains("ENDMODULE"))
                    return true;
            }

            return false;
        }
        private  Boolean EndModuleRecordFound(ArrayList Items)
        {
            int i;

            return EndModuleRecordFound(Items, out i);
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
        #endregion
        #region Public Methods
        public  List<Vector3> ReadCar0BasePtsProgram(string RobotRootPath, ref ArrayList Problems)
        {
            List<Vector3> BaseRecords = new List<Vector3>();
            string Fpath = Path.Combine(RobotRootPath, Car0BasePtsProgramName());

            if (File.Exists(@Fpath))
            {
                try
                {
                    ArrayList Raw = Utils.FileToArrayList(Fpath);
                    int i;
//                    Vector3 xyz = new Vector3();
                    string mesbuf=null;

                    for (i = 0; i < Raw.Count; ++i)
                    {
                        if (Raw[i].ToString().Contains("LOCAL") && Raw[i].ToString().Contains("CONST") && Raw[i].ToString().Contains("robtarget") && Raw[i].ToString().Contains(":=[["))
                        {
                            mesbuf = Raw[i].ToString().Substring(Raw[i].ToString().IndexOf(":=[[") + 4);
                            if (mesbuf.Contains("]"))
                            {
                                int j;
                                mesbuf = mesbuf.Substring(0, mesbuf.IndexOf("]"));
                                string[] items = mesbuf.Split(',');
                                List<double> Vals = new List<double>();

                                for (j = 0; j < items.Length; ++j)
                                {
                                    if (items[j].Length > 0)
                                        Vals.Add(Convert.ToDouble(items[j]));
                                }

                                if (items.Length >= 3)
                                    BaseRecords.Add(new Vector3(Vals[0], Vals[1], Vals[2]));
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Problems.Add("ABB.ReadCar0BasePtsProgram experienced exception below: \n" + e.Message);
                    return new List<Vector3>();
                }

            }
            else
                Problems.Add("Reference points program not found (" + Fpath + ")");

            return BaseRecords;
        }

        public  void ReadCar0J456Targets(string RobotRootPath, out List<Vector> J4Targets, out List<Vector> J5Targets, out List<Vector> J6Targets, ref ArrayList Problems)
        {
            string mesbuf = null, JointTargetName = null;
            J4Targets = new List<Vector>();
            J5Targets = new List<Vector>();
            J6Targets = new List<Vector>();

            string Fpath = Path.Combine(RobotRootPath, Car0JointPosesProgram());

            if (!File.Exists(@Fpath))
            {
                Problems.Add(Car0JointPosesProgram() + " not found in robot folder: " + RobotRootPath);
                return;
            }

            try
            {
                foreach (string d in Utils.FileToArrayList(Fpath))
                {
                    if (d.Contains("PERS jointtarget"))
                    {
                        JointTargetName = d.Substring(d.IndexOf("PERS jointtarget") + 17);

                        if (!(JointTargetName.Contains(":=") && JointTargetName.Contains("[[")))
                        {
                            Problems.Add("Cannot decode tool definition line: '" + d + "'");
                            return;
                        }

                        JointTargetName = JointTargetName.Substring(0, JointTargetName.IndexOf(":="));
                        JointTargetName = JointTargetName.Replace(" ", "");

                        mesbuf = d.Substring(d.IndexOf("[[") + 2);

                        if (!mesbuf.Contains("]"))
                        {
                            Problems.Add("Cannot decode tool definition line: '" + d + "'");
                            return;
                        }

                        mesbuf = mesbuf.Substring(0, mesbuf.IndexOf("]"));

                        if (JointTargetName.StartsWith("J4_"))
                           J4Targets.Add(new Vector(VectorFromString(mesbuf)));
                        else if (JointTargetName.StartsWith("J5_"))
                            J5Targets.Add(new Vector(VectorFromString(mesbuf)));
                        else if (JointTargetName.StartsWith("J6_"))
                            J6Targets.Add(new Vector(VectorFromString(mesbuf)));
                    }
                }

                if (J4Targets.Count.Equals(0))
                    Problems.Add("No Joint 4 targets were found in :" + Car0JointPosesProgram());

                if (J5Targets.Count.Equals(0))
                    Problems.Add("No Joint 5 targets were found in :" + Car0JointPosesProgram());

                if (J6Targets.Count.Equals(0))
                    Problems.Add("No Joint 6 targets were found in :" + Car0JointPosesProgram());

            }
            catch (Exception ee)
            {
                Problems.Add(ee.Message);
            }
        }


        public  void WriteDefiner(string RobotRootPath, double[] FrameDef, ref int MnLineNumber, Boolean WriteFrame, Brand RobBrand, int ind, ref ArrayList Problems)
        {
            string Fpath = Path.Combine(RobotRootPath, Car0DefinerName());
            string mesbuf = null, buffer1 = null, buffer2 = null, EndRec = null;
            ArrayList ExistingRecords = new ArrayList();
            int MyIndex = 0;
            Boolean WobjDataRecDetected = false;

            try
            {
                if (File.Exists(@Fpath))
                    ExistingRecords = Utils.FileToArrayList(Fpath);
                else
                {
                    //Add basic header to the ExistingRecords
                    mesbuf = Car0DefinerName();                    
                    mesbuf = mesbuf.Substring(0, mesbuf.IndexOf("."));

                    ExistingRecords.Add("%%%");
                    ExistingRecords.Add("  VERSION:1");
                    ExistingRecords.Add("  LANGUAGE:ENGLISH");
                    ExistingRecords.Add("%%%");
                    ExistingRecords.Add("");
                    ExistingRecords.Add("MODULE " + mesbuf);
                    ExistingRecords.Add("");
                }

                if (WriteFrame)
                {
                    string ExactMatch = "", SimilarRecord = "";

                    ExactMatch = Utils.RobotEquivalentStringName(ind, RobBrand);

                    switch (RobotData.FrameTypes[ind])
                    {
                        case RobotData.FrameType.DropTool:
                        case RobotData.FrameType.FixtureCar0:
                        case RobotData.FrameType.GripperCar0:
                        case RobotData.FrameType.PickTool:

                            SimilarRecord = "wobjdata";
                            WobjDataRecDetected = true;
                            break;

                        case RobotData.FrameType.GripPin:
                        case RobotData.FrameType.LaserTip:
                        case RobotData.FrameType.MigGunTip:
                        case RobotData.FrameType.NutGunTip:
                        case RobotData.FrameType.PedRivetGunTip:
                        case RobotData.FrameType.PedScribeGunTip:
                        case RobotData.FrameType.PedSealGunTip:
                        case RobotData.FrameType.PedSpotGunTip:
                        case RobotData.FrameType.PedStudGunTip:
                        case RobotData.FrameType.PierceTip:
                        case RobotData.FrameType.RivetGunTip:
                        case RobotData.FrameType.ScribeGun:
                        case RobotData.FrameType.SealGunTip:
                        case RobotData.FrameType.SpotGunTip:
                        case RobotData.FrameType.StudGunTip:
                        case RobotData.FrameType.VisionTcp:

                            SimilarRecord = "tooldata";
                            break;
                    }

                    EndRec = (WobjDataRecDetected) ? "]],[" : "],[";

                    //Do we have an exact match to replace coordinates in
                    if (FoundExactMatchRecord(ExistingRecords, SimilarRecord, ExactMatch, out MyIndex))
                    {
                        //Replacing record at MyIndex

                        if (!(ExistingRecords[MyIndex].ToString().Contains("[[") && ExistingRecords[MyIndex].ToString().Contains(EndRec)))
                        {
                            raiseNotify("Error decoding record #" + MyIndex.ToString() + " = \n'" + ExistingRecords[MyIndex].ToString() + "'", "ABB.WriteDefiner");
                            throw new DefinerWriteException(String.Format("Error decoding record #{0} = \n'{1}'", MyIndex.ToString(), ExistingRecords[MyIndex].ToString()));
                        }

                        buffer1 = ExistingRecords[MyIndex].ToString().Substring(0, ExistingRecords[MyIndex].ToString().IndexOf("[[") + 2);
                        buffer2 = ExistingRecords[MyIndex].ToString().Substring(ExistingRecords[MyIndex].ToString().IndexOf(EndRec));

                        ExistingRecords[MyIndex] = buffer1 + BuildFrameCoordsString(FrameDef, WobjDataRecDetected, "],[") + buffer2;
                    }
                    else if (FoundMatchingType(ExistingRecords, SimilarRecord, out MyIndex))
                    {
                        //Inserting record after MyIndex
                        if (WobjDataRecDetected)
                        {
                            mesbuf = (Utils.IsPed(ind)) ? "TRUE," : "FALSE,";
                            buffer2 = (true) ? "TRUE" : "FALSE";                                //TODO: The FALSE must come up for frames associated with RTU rails

                            //TODO: There will be cases where the last section of this must contain the "Object coordinates" field
                            buffer1 = "  TASK PERS wobjdata " + ExactMatch + ":=[" + mesbuf + buffer2 +
                                ",\"\",[[" + BuildFrameCoordsString(FrameDef, WobjDataRecDetected, "],[") + EndRec + "[0, 0, 0],[ 1, 0, 0, 0]]];"; 
                        }
                        else
                        {
                            mesbuf = (Utils.IsPed(ind)) ? "FALSE" : "TRUE";

                            //TODO: The load record portion of this has been hard coded for now.
                            buffer1 = "  TASK PERS tooldata " + ExactMatch + ":=[" + mesbuf + ",[[" +
                                BuildFrameCoordsString(FrameDef, WobjDataRecDetected, EndRec) + "],[1, 0, 0, 0]],[" + "5,[20,0,0],[1,0,0,0],0,0,0]];";

                            Problems.Add("Warning: matching record not found for '" + ExactMatch + "' in \n" + Fpath + "\n\tOrientation faked to [1, 0, 0, 0]!!!\n");
                        }

                        if (MyIndex.Equals(ExistingRecords.Count - 1))
                            ExistingRecords.Add(buffer1);
                        else
                            ExistingRecords.Insert(MyIndex + 1, buffer1);
                    }
                    else
                    {
                        //Adding record at end
                        //Inserting record after MyIndex
                        if (WobjDataRecDetected)
                        {
                            mesbuf = (Utils.IsPed(ind)) ? "TRUE," : "FALSE,";
                            buffer2 = (true) ? "TRUE" : "FALSE";                                //TODO: The FALSE must come up for frames associated with RTU rails

                            //TODO: There will be cases where the last section of this must contain the "Object coordinates" field
                            buffer1 = "\tTASK PERS wobjdata " + ExactMatch + ":=[" + mesbuf + buffer2 +
                                ",\"\",[[" + BuildFrameCoordsString(FrameDef, WobjDataRecDetected, "],[") + EndRec + "[0, 0, 0],[ 1, 0, 0, 0]]];";
                            Problems.Add("Warning: No records of type '" + SimilarRecord + "' in \n" + Fpath + "\n");
                        }
                        else
                        {
                            mesbuf = (Utils.IsPed(ind)) ? "FALSE" : "TRUE";

                            //TODO: The load record portion of this has been hard coded for now.
                            buffer1 = "  TASK PERS tooldata " + ExactMatch + ":=[" + mesbuf + ",[[" +
                                BuildFrameCoordsString(FrameDef, WobjDataRecDetected, EndRec) + "],[1, 0, 0, 0]],[" + "5,[20,0,0],[1,0,0,0],0,0,0]];";
                            Problems.Add("Warning: No records of type '" + SimilarRecord + "' in \n" + Fpath + "\n\tOrientation faked to [1, 0, 0, 0]!!!\n");

                        }
                        if (EndModuleRecordFound(ExistingRecords,out MyIndex))
                            ExistingRecords.Insert(MyIndex - 1, buffer1);
                        else
                            ExistingRecords.Add(buffer1);
                    }
                }

                if (!EndModuleRecordFound(ExistingRecords))
                    ExistingRecords.Add("ENDMODULE");

                Utils.ArrayListToFile(Fpath, ExistingRecords, true);
            }
            catch (Exception e)
            {
                Problems.Add(e.ToString());
            }
        }

        public  void SetJ123Values(string RobotRootPath, Vector J123Values, ref ArrayList Problems)
        {
            string Fpath = Path.Combine(RobotRootPath, Car0JointPosesProgram());

            if (File.Exists(@Fpath))
            {
                try
                {
                    ArrayList Raw = Utils.FileToArrayList(Fpath);
                    int i;
//                    Vector3 xyz = new Vector3();
                    string d = null, JointTargetName = null, mesbuf = null;

                    for (i = 0; i < Raw.Count; ++i)
                    {
                        d = Raw[i].ToString();

                        if (d.Contains("PERS jointtarget"))
                        {
                            JointTargetName = d.Substring(d.IndexOf("PERS jointtarget") + 17);

                            if (!(JointTargetName.Contains(":=") && JointTargetName.Contains("[[")))
                            {
                                Problems.Add("Cannot decode tool definition line: '" + d + "'");
                                return;
                            }

                            JointTargetName = JointTargetName.Substring(0, JointTargetName.IndexOf(":="));
                            JointTargetName = JointTargetName.Replace(" ", "");

                            mesbuf = d.Substring(d.IndexOf("[[") + 2);

                            if (!mesbuf.Contains("]"))
                            {
                                Problems.Add("Cannot decode tool definition line: '" + d + "'");
                                return;
                            }

                            mesbuf = mesbuf.Substring(0, mesbuf.IndexOf("]"));

                            if (JointTargetName.StartsWith("J4_") || JointTargetName.StartsWith("J5_") || JointTargetName.StartsWith("J6_"))
                            {
                                Vector oldVals = VectorFromString(mesbuf);
                                string start = d.Substring(0, d.IndexOf("[[") + 2);
                                string end = d.Substring(d.IndexOf("]"));
                                mesbuf = J123Values.Vec[0].ToString("F3") + "," + J123Values.Vec[1].ToString("F3") + "," + J123Values.Vec[2].ToString("F3") + "," +
                                    oldVals.Vec[3].ToString("F3") + "," + oldVals.Vec[4].ToString("F3") + "," + oldVals.Vec[5].ToString("F3");

                                Raw[i] = start + mesbuf + end;
                            }
                        }
                    }

                    Utils.ArrayListToFile(Fpath,Raw,true);
                }
                catch (Exception e)
                {
                    Problems.Add("ABB.SetJ123Values experienced exception below: \n" + e.Message);
                }
            }
            else
                Problems.Add(Car0JointPosesProgram() + " not found in robot folder: " + RobotRootPath);
        }

        public  void SetSingleJointAxisValue(string RobotRootPath, double value, int MovingJointNumber, int AxisIndx, ref ArrayList Problems)
        {
            string Fpath = Path.Combine(RobotRootPath, Car0JointPosesProgram());

            if (File.Exists(@Fpath))
            {
                try
                {
                    ArrayList Raw = Utils.FileToArrayList(Fpath);
                    int i;
//                    Vector3 xyz = new Vector3();
                    string d = null, JointTargetName = null, mesbuf = null;
                    string MovingJoint = "J" + MovingJointNumber.ToString() + "_";

                    for (i = 0; i < Raw.Count; ++i)
                    {
                        d = Raw[i].ToString();

                        if (d.Contains("PERS jointtarget"))
                        {
                            JointTargetName = d.Substring(d.IndexOf("PERS jointtarget") + 17);

                            if (!(JointTargetName.Contains(":=") && JointTargetName.Contains("[[")))
                            {
                                Problems.Add("Cannot decode tool definition line: '" + d + "'");
                                return;
                            }

                            JointTargetName = JointTargetName.Substring(0, JointTargetName.IndexOf(":="));
                            JointTargetName = JointTargetName.Replace(" ", "");

                            mesbuf = d.Substring(d.IndexOf("[[") + 2);

                            if (!mesbuf.Contains("]"))
                            {
                                Problems.Add("Cannot decode tool definition line: '" + d + "'");
                                return;
                            }

                            mesbuf = mesbuf.Substring(0, mesbuf.IndexOf("]"));

                            if (JointTargetName.StartsWith(MovingJoint))
                            {
                                Vector oldVals = VectorFromString(mesbuf);
                                string start = d.Substring(0, d.IndexOf("[[") + 2);
                                string end = d.Substring(d.IndexOf("]"));

                                oldVals.Vec[AxisIndx] = value;

                                mesbuf = oldVals.Vec[0].ToString("F3") + "," + oldVals.Vec[1].ToString("F3") + "," + oldVals.Vec[2].ToString("F3") + "," +
                                    oldVals.Vec[3].ToString("F3") + "," + oldVals.Vec[4].ToString("F3") + "," + oldVals.Vec[5].ToString("F3");

                                Raw[i] = start + mesbuf + end;
                            }
                        }
                    }

                    Utils.ArrayListToFile(Fpath, Raw, true);
                }
                catch (Exception e)
                {
                    Problems.Add("ABB.SetSingleJointAxisValue experienced exception below: \n" + e.Message);
                }
            }
            else
                Problems.Add(Car0JointPosesProgram() + " not found in robot folder: " + RobotRootPath);
        }
        #endregion
    }
}
