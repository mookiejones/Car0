using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Car0
{
     class Utils
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
        #endregion
        #region Public Methods

        public  float RadToDegrees(float rad)
        {
            return rad * 180.0f / (float)Math.PI;
        }
        public  double RadToDegrees(double rad)
        {
            return rad * 180.0D / Math.PI;
        }
        public static double DegreesToRad(double deg)
        {
            return deg * Math.PI / 180.0D;
        }

        /* FileToArrayList(FilePath)
         * 
         *      Returns an arraylist containing all the text in a file.
         */
        public  ArrayList FileToArrayList(string FilePath)
        {
            ArrayList MyData = new ArrayList();
            string line = null;

            try
            {
                using (System.IO.StreamReader InStream = new System.IO.StreamReader(FilePath))
                {
                    while ((line = InStream.ReadLine()) != null)
                    {
                        if (line.IndexOf('#') != 0)
                            MyData.Add(line);
                    }
                }

            }
            catch (Exception)
            {
                //Do nothing for exceptions, just get no header
            }

            return MyData;
        }

        public  void ArrayListToFile(string FilePath, ArrayList Al, Boolean Overwrite)
        {
            StreamWriter fs;
            int i;

            if (Al.Count > 0)
            {
                using (fs = new StreamWriter(FilePath, !Overwrite))
                {
                    for (i = 0; i < Al.Count; ++i)
                        fs.WriteLine(Al[i].ToString());

                    fs.Flush();
                }
            }
        }

        public  double DoubleFromString(string Instr, string Look4, string EndStr)
        {
            double val = 0.0;

            if (Instr.Contains(Look4))
            {
                string mesbuf = Instr.Substring(Instr.IndexOf(Look4) + Look4.Length);

                while (mesbuf.StartsWith(" "))
                    mesbuf = mesbuf.Substring(1);

                if (mesbuf.Contains(EndStr))
                {
                    mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(EndStr));
                    val = Convert.ToDouble(mesbuf);
                }
                else
                    raiseNotify("Programming error:  Cannot decode '" + Instr + "'", "DoubleFromString");
            }
            else
                raiseNotify("Programming error:  Cannot find '" + Look4 + "' in '" + Instr + "'", "DoubleFromString");

            return val;
        }

        public  int IntFromString(string Instr, string Look4)
        {
            int rval = 0;

            try
            {
                string mesbuf = Instr.Substring(Instr.IndexOf(Look4) + Look4.Length);
                mesbuf = mesbuf.Replace(" ", "");
                rval = Convert.ToInt32(mesbuf);
            }
            catch (Exception ee)
            {
                raiseNotify(ee.Message, "IntFromString");
            }

            return rval;
        }

        public  string StrFromString(string Instr, string Look4)
        {
            string rval = null;

            try
            {
                string mesbuf = Instr.Substring(Instr.IndexOf(Look4) + Look4.Length);
                mesbuf = mesbuf.Replace(" ", "");
                rval = mesbuf;
            }
            catch (Exception ee)
            {
                raiseNotify(ee.Message, "StrFromString");
            }

            return rval;
        }

        public  string RobotEquivalentStringName(int ind, Brand Rbrand)
        {
            string Result = null;

            try
            {
                AppToolName MyName = new AppToolName(RobotData.FrameTypes[ind], Rbrand);

                Result = (MyName.Name.Contains("<Robot>")) ? MyName.Name.Replace("<Robot>","") : MyName.Name;

                while (Result.StartsWith("_"))
                    Result = Result.Substring(1);

                if (Result.Contains("<Num>"))
                    Result = Result.Replace("<Num>", RobotData.FrameNumbers[ind].ToString());

                if (Result.Contains("<Station>"))
                    Result = Result.Replace("<Station>", RobotData.StationCode + RobotData.RobotStationName);
            }
            catch (Exception)
            {
                raiseNotify("Error building frame name", "RobotEquivalentStringName");
            }

            return Result;
        }

        public  Boolean IsPed(int ind)
        {
            Boolean rc = false;

            switch (RobotData.FrameTypes[ind])
            {
                case RobotData.FrameType.PedRivetGunTip:
                case RobotData.FrameType.PedScribeGunTip:
                case RobotData.FrameType.PedSealGunTip:
                case RobotData.FrameType.PedSpotGunTip:
                case RobotData.FrameType.PedStudGunTip:

                    rc = true;
                    break;
            }

            return rc;
        }

        public  List<Boolean> CheckJ1toJ3Range(List<Vector> J4data, List<Vector> J5data, List<Vector> J6data, out Vector Range, out Vector Average, double MaxOk)
        {
            List<Boolean> rvals = new List<Boolean>();
            List<Vector> All = new List<Vector>();
            Vector MyMax = new Vector(6), MyMin = new Vector(6);

            Average = new Vector(6);
            Range = new Vector(6);

            int i;

            //Initialize
            for (i = 0; i < 6; ++i)
            {
                MyMax.Vec[i] = -99999.9;
                MyMin.Vec[i] = 99999.9;
            }

            //Combine the 3 lists
            for (i = 0; i < J4data.Count; ++i)
                All.Add(J4data[i]);

            for (i = 0; i < J5data.Count; ++i)
                All.Add(J5data[i]);

            for (i = 0; i < J6data.Count; ++i)
                All.Add(J6data[i]);

            //Loop through all data
            for (i = 0; i < All.Count; ++i)
            {
                Average = Average.Add(All[i]);

                MyMax = All[i].Max(MyMax);
                MyMin = All[i].Min(MyMin);
            }

            //Calculate the actual average (include all axes although not needed)
            Average = Average.Scale(1 / Convert.ToDouble(All.Count));

            //Calculate the range
            Range = MyMax.Sub(MyMin);

            //Set return codes
            for (i=0; i<3; ++i)
                rvals.Add(Range.Vec[i] <= MaxOk);

            return rvals;
        }
        public  List<Boolean> CheckWristData(List<Vector> Jdata, out Vector Range, out Vector Average, double MaxOk)
        {
            List<Boolean> rvals = new List<Boolean>();
            Vector MyMax = new Vector(6), MyMin = new Vector(6);

            Average = new Vector(6);
            Range = new Vector(6);

            int i;

            //Initialize
            for (i = 0; i < 6; ++i)
            {
                MyMax.Vec[i] = -99999.9;
                MyMin.Vec[i] = 99999.9;
            }

            //Loop through all data
            for (i = 0; i < Jdata.Count; ++i)
            {
                Average = Average.Add(Jdata[i]);

                MyMax = Jdata[i].Max(MyMax);
                MyMin = Jdata[i].Min(MyMin);
            }

            //Calculate the actual average (include all axes although not needed)
            Average = Average.Scale(1 / Convert.ToDouble(Jdata.Count));

            //Calculate the range
            Range = MyMax.Sub(MyMin);

            //Set return codes
            for (i = 0; i < 3; ++i)
                rvals.Add(Range.Vec[i+3] <= MaxOk);

            return rvals;
        }

        //Returns TRUE if MyText converts to a valid double.
        public  Boolean CheckValidTextDoubleConvert(string MyText, out double val)
        {
            val = 0;

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

        public  void AddVec3ListToArrayList(ref ArrayList BuildArrList, List<Vector3> VecLst, string Header)
        {
            int i;

            BuildArrList.Add(Header);

            for (i = 0; i < VecLst.Count; ++i)
                BuildArrList.Add(VecLst[i].x.ToString() + "," + VecLst[i].y.ToString() + "," + VecLst[i].z.ToString());
        }
        public  void AddVec3ToArrayList(ref ArrayList BuildArrList, Vector3 Vec, string Header)
        {
            BuildArrList.Add(Header + "," + Vec.x.ToString() + "," + Vec.y.ToString() + "," + Vec.z.ToString());
        }

        public  void AddTransToArrayList(ref ArrayList BuildArrList, Transformation fAt, string Header)
        {
            double x = 0, y = 0, z = 0, w = 0, p = 0, r = 0;

            fAt.trans_RPY(ref x, ref y, ref z, ref w, ref p, ref r, true);

            BuildArrList.Add(Header + "," + x.ToString() + "," + y.ToString() + "," + z.ToString() + "," + w.ToString() + "," + p.ToString() + "," + r.ToString());
        }
        #endregion
        #region Private Methods
        #endregion
    }
}
