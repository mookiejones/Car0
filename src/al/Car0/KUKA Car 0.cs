using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

[assembly: CLSCompliant(true)]
namespace Car0
{
   
    public partial class KUKAcar0 : Form
    {
        // Sequencing Varibales
        private ExcelIO excel = new ExcelIO();
        private RobotData robotdata = new RobotData();


        public RichTextBox MessageBox
        {
            get { return this.ProblemsTextBox; }
            set { this.ProblemsTextBox = value;}
        }

        #region Class Constants
        #endregion
        #region Class Variables
//        private  OpenFileDialog FileBrowser = new OpenFileDialog();
        private  Boolean Initializing = false;
        internal  RobotData MyRobotData = null;

        private  List<Vector> MyJ4Data = null, MyJ5Data = null, MyJ6Data = null;

        Brand Rbrand = new Brand();
        #endregion

        #region Utility Functions

        /* OperationSelectionControl()
         * 
         * Looks at the state of the Radio buttons (Initial321RadioButton & RobotBasePointsRadioButton) and configures the names and states
         * of the buttons depending on selction.
         */
        private void OperationSelectionControl()
        {
            Boolean SaveInit = Initializing;

            if (Initial321RadioButton.Checked)
            {
                CalculateRobot321Button.Text = "Calculate Car 0 using 3-2-1";
                UserPreferences.OperationRadioButtonSelected = 1;
                WriteDebugDataCheckBox.Visible = false;

                Initializing = true;
    
                //My321PointFileTextBox.Text = UserPreferences.Excel321FileName;

                Initializing = SaveInit;
            }
            else if (RobotBasePointsRadioButton.Checked)
            {
                CalculateRobot321Button.Text = "Calculate Car 0 using reference points";
                UserPreferences.OperationRadioButtonSelected = 2;
                WriteDebugDataCheckBox.Visible = false;

                Initializing = true;

                //My321PointFileTextBox.Text = UserPreferences.MeasuredPointFileName;

                Initializing = SaveInit;
            }
            else
            {
                CalculateRobot321Button.Text = "Calculate radial tool data";
                UserPreferences.OperationRadioButtonSelected = 3;
                WriteDebugDataCheckBox.Visible = true;

                Initializing = true;

                //My321PointFileTextBox.Text = UserPreferences.MeasuredPointFileName;

                Initializing = SaveInit;
            }

            if (!Initializing)
                UserPreferences.Write();
        }

        private void InitializeMyDisplayValues()
        {
            Initializing = true;

            switch (UserPreferences.OperationRadioButtonSelected)
            {
                case 1:

                    Initial321RadioButton.Checked = true;
                    //My321PointFileTextBox.Text = UserPreferences.Excel321FileName;

                    break;

                case 2:

                    RobotBasePointsRadioButton.Checked = true;
                    //My321PointFileTextBox.Text = UserPreferences.MeasuredPointFileName;
                    break;

                case 3:

                    RadialToolDataRadioButton.Checked = true;
                    //My321PointFileTextBox.Text = UserPreferences.MeasuredPointFileName;
                    break;
            }

            FolderDisplayTextBox.Text = UserPreferences.WorkFolderName;
            GetRobotDataButton.Visible = RobotDataDisplayTextBox.Visible = ((UserPreferences.WorkFolderName != null) && !UserPreferences.WorkFolderName.Equals("Not Set"));

            Initializing = false;
        }

        /*  CalculateInitial321Fit()
         * 
         *  Used to calculate the Base_data for the initial attempt.  This uses the 321 method based on 4 measured robot positions.
         */
        private  void CalculateInitial321Fit()
        {
            double[] Origin = new double[3];
            double[] Xref = new double[3];
            double[] Yref = new double[3];
            double[] wAcar0 = new double[6];
            /*
            //TODO: Call check that all initial data is present

            if (ExcelIO.Read123Data(ref Origin, ref Xref, ref Yref))
            {
                rc = FrameRoutines.ComputeFrameFrom321Pts(Origin, Xref, Yref, ref wAcar0);

                if (rc == 0)
                {
                    string mesbuf = null;

                    mesbuf = (MeasuringWhatDomainUpDown.Text.Contains(" ")) ?
                        MeasuringWhatDomainUpDown.Text.Substring(0, MeasuringWhatDomainUpDown.Text.IndexOf(" ")) :
                        MeasuringWhatDomainUpDown.Text;

                    FrameRoutines.WriteDefiner(UserPreferences.WorkFolderName, mesbuf, wAcar0, true, true, true);
                }
                else
                {
                    raiseNotify("Failed to ComputeFrameFrom321Pts, rc = " + rc.ToString());
                } 
            } */
        }


        //This initial version checked and insured that NO Ped data came thru
        private Boolean AnyBaseFramesChecked()
        {
            int i;

            for (i = 0; i < RobotData.RobotFramesChecked.Count; ++i)
            {
                if (RobotData.RobotFramesChecked[i].Equals(true) && RobotData.RobotFrames[i].ToString().Contains(Rbrand.BdStart) && !RobotData.RobotFrames[i].ToString().Contains("Ped"))
                    return true;
            }

            return false;
        }
        private Boolean AnyBaseFramesChecked(Boolean dummy)
        {
            int i;

            for (i = 0; i < RobotData.RobotFramesChecked.Count; ++i)
            {
                if (RobotData.RobotFramesChecked[i].Equals(true) && (RobotData.RobotFrames[i].ToString().Contains(Rbrand.BdStart) ||
                    (RobotData.RobotFrames[i].ToString().Contains(Rbrand.PdStart) && RobotData.RobotFrames[i].ToString().Contains("Ped"))))
                    return true;
            }

            return false;
        }


        /* CalculateFinal3PointMethod()
         * 
         *  Uses two sets of measurement points, on previously established and the other measured wrt the newly established Car0 to perform the calculation.
         */
        private void CalculateFinal3PointMethod(string RobotRootPath, string TrackerFilePath, ref ArrayList Problems)
        {
            List<Vector> InitialPts = null;
            List<Vector> BasePts = null;
            Transformation wAcar0 = new Transformation();
            Transformation wAtracker = null;
            double[] wAcar0_abc = new double[6];
            double[] InitialTool = new double[6];
            double[] InitialBase = new double[6];
            double[] Car0Tool = new double[6];
            double[] Car0Base = new double[6];
            double AveError = 0.0, WorstError = 0.0;
            int i, j, ToolNumber = 0, LinesWritten = 0, WorstPointNumber = -1;
            string mesbuf = null;
            Boolean BaseOneDone = false;

            if (AnyBaseFramesChecked(true))
            {
                if ((InitialPts = excel.GetTrackerFileData(TrackerFilePath, "Initial wrt Robot World", ref Problems, Rbrand)) == null)
                {
                    ReportProblems(Problems);
                    excel.CloseTrackerData();
                    return;
                }
            }
            
            //Loop through all RobotData.RobotFramesChecked
            for (i = 0; i < RobotData.RobotFramesChecked.Count; ++i)
            {
                if (RobotData.RobotFramesChecked[i].Equals(true))
                {
                    //BASE_DATA
                    if (RobotData.RobotFrames[i].ToString().Contains(Rbrand.BdStart) || (RobotData.RobotFrames[i].ToString().Contains(Rbrand.PdStart) && RobotData.RobotFrames[i].ToString().Contains("Ped")))
                    {
                        mesbuf = null;

                        if (RobotData.RobotFrames[i].ToString().Contains("Ped") && !BaseOneDone)
                        {
                                //Force it to process BASE[ 1] data
                          
                                mesbuf = Rbrand.BdStart + " 1" + Rbrand.BdEnd;
                        }
                        else if (RobotData.RobotFrames[i].ToString().Contains(Rbrand.BdStart))
                        {
                            mesbuf = RobotData.RobotFrames[i].ToString().Substring(RobotData.RobotFrames[i].ToString().IndexOf(Rbrand.BdStart));

                            if ((Rbrand.BdEnd.Length > 0) && mesbuf.Contains(Rbrand.BdEnd))
                                mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(Rbrand.BdEnd) + 1);
                        }
                        else if (RobotData.RobotFrames[i].ToString().Contains(Rbrand.PdStart))
                        {
                            mesbuf = RobotData.RobotFrames[i].ToString().Substring(RobotData.RobotFrames[i].ToString().IndexOf(Rbrand.PdStart));

                            if ((Rbrand.PdEnd.Length > 0) && mesbuf.Contains(Rbrand.PdEnd))
                                mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(Rbrand.PdEnd) + 1);
                        }
                        //6/29 Removed  && !RobotData.FrameTypes[i].Equals(RobotData.FrameType.PedSpotGunTip) to make work for combo ped & carried
                        if ((mesbuf != null)) //6/4/12 !RobotData.FrameTypes[i].Equals(RobotData.FrameType.PedSpotGunTip) exp
                        {
                            //Here we get the BASE type data
                            if ((BasePts = excel.GetTrackerFileData(TrackerFilePath, mesbuf, ref Problems, Rbrand)) == null)
                            {
                                ReportProblems(Problems);
                                excel.CloseTrackerData();
                                return;
                            }
                        }
                        else if (RobotData.FrameTypes[i].Equals(RobotData.FrameType.PedSpotGunTip) && (wAcar0 != null))
                            wAtracker = new Transformation(wAcar0); ;

                        if (!RobotData.RobotFrames[i].ToString().Contains("Ped") || !BaseOneDone)
                        {
                            //We get into here for Peds when there has not ben a base found  AL HERE
                            if ((BasePts != null) && InitialPts.Count.Equals(BasePts.Count))
                            {
                                if (!FrameRoutines.where(InitialPts, BasePts, ref wAcar0, ref AveError, ref WorstError, ref WorstPointNumber, ref Problems))
                                {
                                    ReportProblems(Problems);
                                    excel.CloseTrackerData();
                                    return;
                                }

                                //TODO: If PED, just write the PED tip and any others found

                                if (Rbrand.Company.Equals(Brand.RobotBrands.KUKA))
                                    wAcar0.trans_RPY(ref wAcar0_abc[0], ref wAcar0_abc[1], ref wAcar0_abc[2], ref wAcar0_abc[5], ref wAcar0_abc[4], ref wAcar0_abc[3]);
                                else
                                    wAcar0.trans_RPY(ref wAcar0_abc[0], ref wAcar0_abc[1], ref wAcar0_abc[2], ref wAcar0_abc[3], ref wAcar0_abc[4], ref wAcar0_abc[5]);

                                for (j = 3; j < 6; ++j)
                                    wAcar0_abc[j] = Utils.RadToDegrees(wAcar0_abc[j]);

                                if (RobotData.RobotFrames[i].ToString().Contains("Ped"))
                                {
                                    //Save the transformation
                                    wAtracker = new Transformation(wAcar0);
                                }
                                else
                                    FrameRoutines.WriteDefiner(RobotRootPath, mesbuf, wAcar0_abc, ref LinesWritten, true, false, Rbrand, i, ref Problems);

                                Problems.Add("Average / Worst fit error = " + AveError.ToString("F3") + "/" + WorstError.ToString("F3") + ", worst point is #" + WorstPointNumber.ToString());
                            }
                            else if (BasePts != null)
                                Problems.Add("Not the same number of initial points = " + InitialPts.Count.ToString() + " as final points = " + BasePts.Count.ToString());
                            else
                                Problems.Add("Not the same number of initial points = " + InitialPts.Count.ToString() + " as final points = null");
                        }

                        //TODO:  IF this is pedestal, transform the ped tip
                        if (RobotData.RobotFrames[i].ToString().Contains("Ped"))
                        {
                            if (wAtracker == null)
                            {
                                Problems.Add("Program bug: wAtracker is null");
                                ReportProblems(Problems);
                                return;
                            }

                            //Setup to get the tracker data
                            mesbuf = RobotData.RobotFrames[i].ToString().Substring(RobotData.RobotFrames[i].ToString().IndexOf(Rbrand.PdStart));

                            if ((Rbrand.BdEnd.Length > 0) && mesbuf.Contains(Rbrand.BdEnd))
                                mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(Rbrand.BdEnd) + 1);

                            Vector PedTool = new Vector(3);

                            //When there is no base data, we get the TOOL[20] here
                            if (!excel.GetTrackerFileData(TrackerFilePath, i, ref PedTool, ref Problems, Rbrand))
                            {
                                ReportProblems(Problems);
                                excel.CloseTrackerData();
                                return;
                            }

                            Transformation trackerApedtip = new Transformation(Transformation.SpecialTransformType.Identity);
                            trackerApedtip.point_trans(PedTool);
                            Transformation wApedtip = wAtracker.mult_trans(trackerApedtip);

                            PedTool = wApedtip.trans_point();

                            for (j = 0; j < 3; ++j)
                                wAcar0_abc[j] = PedTool.Vec[j];

                            InitialBase = new double[6];
                            ToolNumber = RobotData.FrameNumbers[i];

                            if (FrameRoutines.ReadDefinerPedData(RobotRootPath, ToolNumber, ref InitialBase, Rbrand.Company))
                            {
                                for (j = 3; j < 6; ++j)
                                    wAcar0_abc[j] = InitialBase[j];
                            }
                            else
                            {
                                for (j = 3; j < 6; ++j)
                                    wAcar0_abc[j] = 0.0;
                            }


                            FrameRoutines.WriteDefiner(RobotRootPath, mesbuf, wAcar0_abc, ref LinesWritten, true, false, Rbrand, i, ref Problems);
                        }

                        BaseOneDone = true;

                    }   //TOOL_DATA
                    else if (RobotData.RobotFrames[i].ToString().Contains(Rbrand.TdStart))
                    { 
                        ToolNumber = RobotData.FrameNumbers[i];

                        Car0Tool = new double[6];

                        if (!excel.GetTrackerFileData(TrackerFilePath, i, ref Car0Tool, ref Problems, Rbrand))
                        {
                            ReportProblems(Problems);
                            excel.CloseTrackerData();
                            return;
                        }

                        InitialTool = new double[6];

                        if (FrameRoutines.ReadDefinerToolData(RobotRootPath, ToolNumber, ref InitialTool, Rbrand.Company))
                        {
                            for (j = 3; j < 6; ++j)
                                Car0Tool[j] = InitialTool[j];
                        }
                        else
                            Problems.Add("Unable to find " + mesbuf + " in the definer file.  This tool is downloaded with default orientation");

                        mesbuf = RobotData.RobotFrames[i].ToString().Substring(RobotData.RobotFrames[i].ToString().IndexOf(Rbrand.TdStart));

                        if ((Rbrand.TdEnd.Length > 0) && mesbuf.Contains(Rbrand.TdEnd))
                            mesbuf = mesbuf.Substring(0, mesbuf.IndexOf(Rbrand.TdEnd) + 1);

                        FrameRoutines.WriteDefiner(RobotRootPath, mesbuf, Car0Tool, ref LinesWritten, true, false, Rbrand, i, ref Problems);
                    }   //BASE_DATA
                    else if (RobotData.RobotFrames[i].ToString().Contains("BASE_DATA["))
                    { /*
                        mesbuf = RobotData.RobotFrames[i].ToString().Substring(RobotData.RobotFrames[i].ToString().IndexOf("BASE_DATA["));
                        mesbuf = mesbuf.Substring(0, mesbuf.IndexOf("]") + 1);
                        buffer1 = mesbuf.Substring(mesbuf.IndexOf("[") + 1);
                        buffer1 = buffer1.Replace("]", "");
                        buffer1 = buffer1.Replace(" ", "");
                        ToolNumber = Convert.ToInt32(buffer1);  */

                        ToolNumber = RobotData.FrameNumbers[i];

                        Car0Base = new double[6];

                        if (!excel.GetTrackerFileData(TrackerFilePath, i, ref Car0Base, ref Problems, Rbrand))
                        {
                            ReportProblems(Problems);
                            excel.CloseTrackerData();
                            return;
                        }

                        InitialBase = new double[6];

                        if (FrameRoutines.ReadDefinerBaseData(RobotRootPath, ToolNumber, ref InitialBase, Rbrand.Company))
                        {
                            for (j = 3; j < 6; ++j)
                                Car0Base[j] = InitialBase[j];
                        }
                        else
                            Problems.Add("Unable to find " + mesbuf + " in the definer file.  This base is downloaded with default orientation");

                        FrameRoutines.WriteDefiner(RobotRootPath, mesbuf, Car0Base, ref LinesWritten, true, false, Rbrand, i, ref Problems);
                    }
                }
            }

            if (!LinesWritten.Equals(0))
                FrameRoutines.WriteDefiner(RobotRootPath, mesbuf, wAcar0_abc, ref LinesWritten, false, true, Rbrand, i, ref Problems);

            ReportProblems(Problems);
            excel.CloseTrackerData();
        }

        /* ProcessToolRadialData()
         * 
         *  Control of tool radial data processing.   Results in TOOL definition for tracker targets measured around J4, J5 and J6
         */
        private void ProcessToolRadialData(string RobotRootPath, string TrackerFilePath, ref ArrayList Problems)
        {
            List<Vector3> J4measurements = new List<Vector3>();
            List<Vector3> J5measurements = new List<Vector3>();
            List<Vector3> J6measurements = new List<Vector3>();
            List<Vector3> EOATmeasurements = new List<Vector3>();
            List<Vector3> J4angles = new List<Vector3>();
            List<Vector3> J5angles = new List<Vector3>();
            List<Vector3> J6angles = new List<Vector3>();
            List<Vector3> EOATangles = new List<Vector3>();
            double[] WristData = new double[12];

            //Read joint data recorded
            if (!excel.ReadToolData(TrackerFilePath, ref J4measurements, ref J4angles, ref J5measurements, ref J5angles,
                ref J6measurements, ref J6angles, ref EOATmeasurements, ref EOATangles, ref Problems, Rbrand))
            {
                excel.CloseTrackerData();

                ReportProblems(Problems);
                return;
            }

            //Read wrist parameters for this robot
            if (!excel.FindRobotWristData(TrackerFilePath, robotdata.RobotMechanismName, ref WristData, ref Problems))
            {
                excel.CloseTrackerData();

                ReportProblems(Problems);
                return;
            }

            //Adjust inital joint angle wrist data to reflect pose at time of experiment
            WristData[3] -= Utils.DegreesToRad(J5angles[0].x);
            //Dont think needed 3/21 WristData[7] -= J5angles[0].y;

            GetPlane J4Plane = new GetPlane(J4measurements);
            AxisOfRotation J4Axis = new AxisOfRotation(J4measurements, J4Plane.Normal, J4Plane.Distance);
            GetPlane J5Plane = new GetPlane(J5measurements);
            AxisOfRotation J5Axis = new AxisOfRotation(J5measurements, J5Plane.Normal, J5Plane.Distance);
            GetPlane J6Plane = new GetPlane(J6measurements);
            AxisOfRotation J6Axis = new AxisOfRotation(J6measurements, J6Plane.Normal, J6Plane.Distance);        

            Problems.Add("Max/Average J4 Radius = " + J4Axis.Radius.ToString("F2") + " fit error = (" + J4Plane.MaxError.ToString("F6") + "/" + J4Plane.AveError.ToString("F6") + ")");
            Problems.Add("Max/Average J5 Radius = " + J5Axis.Radius.ToString("F2") + " fit error = (" + J5Plane.MaxError.ToString("F6") + "/" + J5Plane.AveError.ToString("F6") + ")");
            Problems.Add("Max/Average J6 Radius = " + J6Axis.Radius.ToString("F2") + " fit error = (" + J6Plane.MaxError.ToString("F6") + "/" + J6Plane.AveError.ToString("F6") + ")");

            //Find the intersections
            Vector3 p1 = new Vector3();
            Vector3 p2 = new Vector3();
            Vector3 p3 = new Vector3();
            Vector3 p4 = new Vector3();
            Vector3 pa = new Vector3();
            Vector3 pb = new Vector3();
            J4Axis.Origin.PointSlopeToEndpoints(J4Axis.Normal, 10000.0, ref p1, ref p2);
            J5Axis.Origin.PointSlopeToEndpoints(J5Axis.Normal, 10000.0, ref p3, ref p4);
            List<Vector3> MidPoints = new List<Vector3>(3);

            Vector3.LineLineIntersect(p1, p2, p3, p4, ref pa, ref pb, ref Problems, "J4/J5 intersection");

            //Vector3 J4J5Midpoint = pa.MidPoint(pb);
            MidPoints.Add(pa.MidPoint(pb));

            J6Axis.Origin.PointSlopeToEndpoints(J6Axis.Normal, 10000.0, ref p3, ref p4);
            Vector3.LineLineIntersect(p1, p2, p3, p4, ref pa, ref pb, ref Problems, "J4/J6 intersection");

            //Vector3 J4J6Midpoint = pa.MidPoint(pb);
            MidPoints.Add(pa.MidPoint(pb));

            J5Axis.Origin.PointSlopeToEndpoints(J5Axis.Normal, 10000.0, ref p1, ref p2);
            Vector3.LineLineIntersect(p1, p2, p3, p4, ref pa, ref pb, ref Problems, "J5/J6 intersection");

            //Vector3 J5J6Midpoint = pa.MidPoint(pb);
            MidPoints.Add(pa.MidPoint(pb));

            double Max = 0.0, Ave = 0.0;

            int WorstPointNum;
            Vector3 Mp = new Vector3(MidPoints, ref Max, ref Ave, out WorstPointNum);

            Problems.Add("J456 Intersection Max/Average (" + Max.ToString("F3") + "," + Ave.ToString("F3") + "), Worst Point #" + (WorstPointNum+1).ToString());

            //Begin to build the transformation
            Transformation wAj4 = new Transformation();

            Vector TempA = new Vector(J4Axis.Normal);

            //Assign attack
            wAj4.attack_trans(TempA);

            Vector TempO;

            /* Note: We may need something robot or robot brand specific here.  For Fanuc robots, it is clear the rule is TempO is the negatvie
             *       of the the J5 Axis.  May not be true for others.
             */

            if (RobotData.RobotMechanismName.Equals("SRA210F") ||       
                RobotData.RobotMechanismName.Equals("SRA166") ||
                RobotData.RobotMechanismName.Equals("SC400L") ||
                RobotData.RobotMechanismName.Equals("MC350") ||
                RobotData.RobotMechanismName.Equals("NV06"))                 
            {
                TempO = (new Vector(J5Axis.Normal)).Scale(-1.0);
            }
            else
            {
                TempO = (new Vector(J5Axis.Normal)).Scale(-1.0);            //By default this will be the action
            }
            /*
            if (Rbrand.Company.Equals(Brand.RobotBrands.Nachi))
                TempO = new Vector(J5Axis.Normal);                                  //TODO:  EXPERIMENTAL 4/5/12 */

            //Assign normal
            wAj4.orient_trans(TempO);

            //Assign orient
            wAj4.norm_trans(TempO.CrossProduct(TempA));

            //Assign Point
            wAj4.point_trans(new Vector(Mp));

            /* Next, transform all the points to the J4 coordinate system, then to TF.
             */

            Transformation j4Aw = wAj4.inv_trans();
            List<Vector3> j4J4measurements = FrameRoutines.PointSetCsChange(j4Aw, J4measurements);
            List<Vector3> j4J5measurements = FrameRoutines.PointSetCsChange(j4Aw, J5measurements);
            List<Vector3> j4J6measurements = FrameRoutines.PointSetCsChange(j4Aw, J6measurements);

            //Transform each into the TOOLFRAME coordinate system
            List<Vector3> tfJ4measurements = FrameRoutines.PointSetJointTransform(j4J4measurements, J4angles, WristData);
            List<Vector3> tfJ5measurements = FrameRoutines.PointSetJointTransform(j4J5measurements, J5angles, WristData);
            List<Vector3> tfJ6measurements = FrameRoutines.PointSetJointTransform(j4J6measurements, J6angles, WristData);

            //Build total list
            List<Vector3> tfAll = new List<Vector3>();

            tfAll.Add(

            Vector3.AddToList(tfJ4measurements, ref tfAll);
            Vector3.AddToList(tfJ5measurements, ref tfAll);
            Vector3.AddToList(tfJ6measurements, ref tfAll);

            Vector3 AverageTool = new Vector3(tfAll, ref Max, ref Ave, out WorstPointNum);
            int SetNum = 4;
            
            //Find where the worst point number is
            if (WorstPointNum >= tfJ4measurements.Count)
            {
                ++SetNum;
                WorstPointNum -= tfJ4measurements.Count;

                if (WorstPointNum >= tfJ5measurements.Count)
                {
                    ++SetNum;
                    WorstPointNum -= tfJ5measurements.Count;

                    if (WorstPointNum >= tfJ6measurements.Count)
                    {
                        ++SetNum;
                        WorstPointNum -= tfJ5measurements.Count;
                    }
                }
            }

            Problems.Add("TOOL (" + AverageTool.x.ToString("F3") + ", " + AverageTool.y.ToString("F3") + ", " + AverageTool.z.ToString("F3") + ") Max/Average (" + Max.ToString("F3") + "," + Ave.ToString("F3") + "), Worst point J" + SetNum.ToString() + " Point #" + (WorstPointNum+1).ToString());

            //Transform present EOAT raw measurements into j4
            List<Vector3> j4EOATmeasurements = FrameRoutines.PointSetCsChange(j4Aw, EOATmeasurements, true);

            //Transform present EOAT to TOOLFRAME coordinate system
            List<Vector3> tfEOATmeasurements = FrameRoutines.PointSetJointTransform(j4EOATmeasurements, EOATangles, WristData, true);

            //Write any tool definitions to
            excel.WriteToolDefinitions(TrackerFilePath, tfEOATmeasurements, ref Problems);

            ReportProblems(Problems);

            if (WriteDebugDataCheckBox.Checked)
            {
                ArrayList DebugData = new ArrayList();

                Utils.AddVec3ListToArrayList(ref DebugData, J4measurements, "J4 Raw");
                Utils.AddVec3ListToArrayList(ref DebugData, J5measurements, "J5 Raw");
                Utils.AddVec3ListToArrayList(ref DebugData, J6measurements, "J6 Raw");
                Utils.AddVec3ListToArrayList(ref DebugData, j4J4measurements, "J4 wrt j4_axis");
                Utils.AddVec3ListToArrayList(ref DebugData, j4J5measurements, "J5 wrt j4_axis");
                Utils.AddVec3ListToArrayList(ref DebugData, j4J6measurements, "J6 wrt j4_axis");
                Utils.AddVec3ListToArrayList(ref DebugData, tfJ4measurements, "J4 wrt TOOLFRAME");
                Utils.AddVec3ListToArrayList(ref DebugData, tfJ5measurements, "J5 wrt TOOLFRAME");
                Utils.AddVec3ListToArrayList(ref DebugData, tfJ6measurements, "J6 wrt TOOLFRAME");
                Utils.AddTransToArrayList(ref DebugData, wAj4, "wAj4");
                Utils.AddVec3ToArrayList(ref DebugData, AverageTool, "Puck");

                Utils.ArrayListToFile(Path.Combine(RobotRootPath, "Debug.csv"), DebugData, true);
            }
        }

        #endregion

        public KUKAcar0()
        {
            InitializeComponent();
       
            this.excel = new ExcelIO(this);
            this.robotdata = new RobotData(this);
            UserPreferences.Read();

            RobotBrandListBox.SelectedIndex = UserPreferences.RobotBrandSelected; ;
            Rbrand = new Brand(RobotBrandListBox.SelectedIndex);

            InitializeMyDisplayValues();
        }

        private void FolderSelectButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select work folder";

            if (String.IsNullOrEmpty(fbd.SelectedPath))
                fbd.SelectedPath = "C:\\";

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                UserPreferences.WorkFolderName = FolderDisplayTextBox.Text = fbd.SelectedPath;
                UserPreferences.RobotMatrixFileName = null;
                RobotDataDisplayTextBox.Text = "Not Set";
                UserPreferences.Write();
                GetRobotDataButton.Visible = RobotDataDisplayTextBox.Visible = true;

                Brand.ReadConfiguration = false;
            }
            else
            {
                FolderDisplayTextBox.Text = "Not Set";
                GetRobotDataButton.Visible = RobotDataDisplayTextBox.Visible = false;
            }

            ProblemsTextBox.Clear();
        }

        private void ReportProblems(ArrayList Problems)
        {

            ProblemsTextBox.Text = String.Empty;

            if (Problems.Count > 0)
            {
                for (int i = 0; i < Problems.Count; ++i)
                    ProblemsTextBox.Text += (Problems[i].ToString() + "\n");
            }
            else
            {
                ProblemsTextBox.Text = "Successfully completed with no errors or warnings";
            }
        }


        private void CalculateRobot321Button_Click(object sender, EventArgs e)
        {
            ArrayList Problems = new ArrayList();
            ProblemsTextBox.Text = null;
            string RobotnameUse = RobotData.RobotName;

            while (RobotnameUse.EndsWith(" "))
                RobotnameUse = RobotnameUse.Substring(0, RobotnameUse.Length - 1);

            string RobotRootPath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotnameUse));
            string TrackerFilePath = Path.Combine(RobotRootPath, "TrackerFile.xls");

            if (excel.TrackerDataExcelOpen)
            {
                NotifyMessage("Tracker File is open", String.Format("Close file located at \r\n {0} \r\n and try again", TrackerFilePath));
                return;
            }

            if (!AnyBaseFramesChecked() || excel.CheckTrackerFile(TrackerFilePath, ref Problems, Rbrand))
            {
                if (Initial321RadioButton.Checked)
                    CalculateInitial321Fit();
                else if (RobotBasePointsRadioButton.Checked)
                    CalculateFinal3PointMethod(RobotRootPath, TrackerFilePath, ref Problems);
                else
                    ProcessToolRadialData(RobotRootPath, TrackerFilePath, ref Problems);
            }

            ReportProblems(Problems);
        }

        private void KUKAcar0_FormClosing(object sender, FormClosingEventArgs e)
        {
            excel.CloseMatrix();
            excel.CloseTrackerData();
        }

        private void Initial321RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            OperationSelectionControl();
        }

        private void RobotBasePointsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            OperationSelectionControl();
        }

        private void GetRobotDataButton_Click(object sender, EventArgs e)
        {                
            MyRobotData = (MyRobotData == null) ? (new RobotData()) : MyRobotData;

            MyRobotData.Activate();
            MyRobotData.ShowDialog(this);
            ProblemsTextBox.Clear();
        }

        private void KUKAcar0_Activated(object sender, EventArgs e)
        {
            CalculateRobot321Button.Enabled = ViewTrackerFileBbutton.Enabled = btnReadJointCoordsButton.Enabled = btnReadRobotBaseDataButton.Enabled = false;

            if (RobotData.StationName == null)
                TrackerDataFileTextBox.Text = "Not Set";
            else if (RobotData.RobotName == null)
            {
                RobotDataDisplayTextBox.Text = "System: " + RobotData.StationName + "   Robot: Not Set";
                TrackerDataFileTextBox.Text = "Not Set";
            }
            else
            {
                string RobotnameUse = RobotData.RobotName;

                while (RobotnameUse.EndsWith(" "))
                    RobotnameUse = RobotnameUse.Substring(0, RobotnameUse.Length - 1);

                RobotDataDisplayTextBox.Text = "System: " + RobotData.StationName + "   Robot: " + RobotnameUse + "(" + RobotData.RobotMechanismName + ")";

                string FolderPath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotnameUse));

                if (Directory.Exists(FolderPath))
                {
                    if (File.Exists(Path.Combine(FolderPath, "TrackerFile.xls")))
                    {
                        TrackerDataFileTextBox.Text = "TrackerFile.xls";
                        CalculateRobot321Button.Enabled = btnReadJointCoordsButton.Enabled = btnReadRobotBaseDataButton.Enabled = ViewTrackerFileBbutton.Enabled = true;
                    }
                    else
                        TrackerDataFileTextBox.Text = "Tracker Excel does NOT EXIST";
                }
                else
                    TrackerDataFileTextBox.Text = "Robot folder '" + FolderPath + "' does NOT EXIST";

                //My321PointFileTextBox.Text = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName));
            }
        }

        private void ViewTrackerFileBbutton_Click(object sender, EventArgs e)
        {
            string RobotnameUse = RobotData.RobotName;

            while (RobotnameUse.EndsWith(" "))
                RobotnameUse = RobotnameUse.Substring(0, RobotnameUse.Length - 1);

            string Fpath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, Path.Combine(RobotnameUse, "TrackerFile.xls")));

            excel.OpenExcelFile(Fpath, true);
        }

        private void RobotBrandListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserPreferences.RobotBrandSelected = RobotBrandListBox.SelectedIndex;
            Rbrand = new Brand(RobotBrandListBox.SelectedIndex);
            UserPreferences.Write();
            RobotData.RobotFramesChecked = new ArrayList();
        }


        private void ReadRobotBaseDataButton_Click(object sender, EventArgs e)
        {
            btnReadRobotBaseDataButton.Enabled = false;

            ArrayList Problems = new ArrayList();;
            List<Vector3> Mypoints = new List<Vector3>();

            if (UserPreferences.WorkFolderName == null)
                Problems.Add("You must set the root folder name first - Press Set root folder button");
            else if ((RobotData.StationName == null) || (RobotData.RobotName == null))
                Problems.Add("You must read the robot data first - Press Robot data button");
            else
            {
                string RobotRootPath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName));

                switch (Rbrand.Company)
                {
                    case Brand.RobotBrands.ABB:

                        Mypoints = ABB.ReadCar0BasePtsProgram(RobotRootPath, ref Problems);
                        break;

                    case Brand.RobotBrands.Fanuc:
                    case Brand.RobotBrands.FanucRJ:

                        Mypoints = Fanuc.ReadCar0BasePtsProgram(RobotRootPath, ref Problems);
                        break;

                    case Brand.RobotBrands.Nachi:

                        Mypoints = Nachi.ReadCar0BasePtsProgram(RobotRootPath, ref Problems);
                        break;

                    case Brand.RobotBrands.KUKA:

                        Mypoints = Kuka.ReadCar0BasePtsProgram(RobotRootPath, ref Problems);
                        break;

                    default:

                        Problems.Add("Unrecognized robot type in ReadRobotBaseDataButton_Click");
                        break;

                }

                if (Problems.Count.Equals(0))
                {
                    if (excel.WriteCar0BaseData(Path.Combine(RobotRootPath, "TrackerFile.xls"), Mypoints, ref Problems))
                        Problems.Add("Successfully read " + Mypoints.Count.ToString() + " Initial robot world base points.");
                }
            }

            ReportProblems(Problems);
        }

        private Boolean Check456Control(ref ArrayList Problems)
        {
            Boolean good = true;
            Vector Range, Average;
            int BadCount = 0;
            //Second level check of all 3 data sets to insure the other axes are held constant.  Checks of all 3 are performed, any failure forces operator to correct
            //Start check J4
            List<Boolean> J4Checks = Utils.CheckWristData(MyJ4Data, out Range, out Average, 0.01);

            btnReadJointCoordsButton.Enabled = true;
            SetAllJointValuesButton.Visible = J4setJ5ValueButton.Visible = J4_J5_TextBox.Visible = J4setJ6ValueButton.Visible = J4_J6_TextBox.Visible = J5setJ4ValueButton.Visible =
                J5_J4_TextBox.Visible = J5setJ6ValueButton.Visible = J5_J6_TextBox.Visible = J6setJ4ValueButton.Visible = J6_J4_TextBox.Visible = J6setJ5ValueButton.Visible = J6_J5_TextBox.Visible = false;

            if (J4Checks[1].Equals(false))
            {
                J4setJ5ValueButton.Visible = J4_J5_TextBox.Visible = true;

                J4_J5_TextBox.Text = Average.Vec[4].ToString("F3");

                Problems.Add(string.Format("J4 test joint 5 angles vary by: {0}", Range.Vec[4].ToString("F4")));

                good = btnReadJointCoordsButton.Enabled = false;
                ++BadCount;
            }

            if (J4Checks[2].Equals(false))
            {
                J4setJ6ValueButton.Visible = J4_J6_TextBox.Visible = true;

                J4_J6_TextBox.Text = Average.Vec[5].ToString("F3");

                Problems.Add(String.Format("J4 test joint 6 angles vary by: {0}",Range.Vec[5].ToString("F4")));

                good = btnReadJointCoordsButton.Enabled = false;
                ++BadCount;
            }

            List<Boolean> J5Checks = Utils.CheckWristData(MyJ5Data, out Range, out Average, 0.01);

            if (J5Checks[0].Equals(false))
            {
                J5setJ4ValueButton.Visible = J5_J4_TextBox.Visible = true;

                J5_J4_TextBox.Text = Average.Vec[3].ToString("F3");

                Problems.Add(string.Format("J5 test joint 4 angles vary by: {0}", Range.Vec[3].ToString("F4")));

                good = btnReadJointCoordsButton.Enabled = false;
                ++BadCount;
            }

            if (J5Checks[2].Equals(false))
            {
                J5setJ6ValueButton.Visible = J5_J6_TextBox.Visible = true;

                J5_J6_TextBox.Text = Average.Vec[5].ToString("F3");

                Problems.Add(string.Format("J5 test joint 6 angles vary by: {0}",Range.Vec[5].ToString("F4")));

                good = btnReadJointCoordsButton.Enabled = false;
                ++BadCount;
            }

            List<Boolean> J6Checks = Utils.CheckWristData(MyJ6Data, out Range, out Average, 0.01);

            if (J6Checks[0].Equals(false))
            {
                J6setJ4ValueButton.Visible = J6_J4_TextBox.Visible = true;

                J6_J4_TextBox.Text = Average.Vec[3].ToString("F3");

                Problems.Add(string.Format("J6 test joint 4 angles vary by: {0}", Range.Vec[3].ToString("F4")));

                good = btnReadJointCoordsButton.Enabled = false;
                ++BadCount;
            }

            if (J6Checks[1].Equals(false))
            {
                J6setJ5ValueButton.Visible = J6_J5_TextBox.Visible = true;

                J6_J5_TextBox.Text = Average.Vec[4].ToString("F3");
                Problems.Add(String.Format("J6 test joint 5 angles vary by: {0}",Range.Vec[4].ToString("F4")));

                good = btnReadJointCoordsButton.Enabled = false;
                ++BadCount;
            }

            SetAllJointValuesButton.Visible = (BadCount > 1);

            return good;
        }

        private void ReadJointCoordsButton_Click(object sender, EventArgs e)
        {
            btnReadJointCoordsButton.Enabled = false;
            ArrayList Problems = new ArrayList();

            if (String.IsNullOrEmpty(UserPreferences.WorkFolderName))
                Problems.Add("You must set the root folder name first - Press Set root folder button");
            else if ((string.IsNullOrEmpty(RobotData.StationName)) || string.IsNullOrEmpty(RobotData.RobotName))
                Problems.Add("You must read the robot data first - Press Robot data button");
            else
            {
                string RobotRootPath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName));

                switch (Rbrand.Company)
                {
                    case Brand.RobotBrands.ABB:

                        ABB.ReadCar0J456Targets(RobotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref Problems);
                        break;

                    case Brand.RobotBrands.Fanuc:
                    case Brand.RobotBrands.FanucRJ:

                        Fanuc.ReadCar0J456Targets(RobotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref Problems);
                        break;

                    case Brand.RobotBrands.KUKA:

                        Kuka.ReadCar0J456Targets(RobotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref Problems);
                        break;

                    case Brand.RobotBrands.Nachi:

                        Nachi.ReadCar0J456Targets(RobotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref Problems);
                        break;

                    default:

                        Problems.Add("Unrecognized robot type in ReadJointCoordsButton_Click");
                        break;
                }

                //Check then write them
                if (Problems.Count.Equals(0))
                {
                    Vector Range, Average;
                    List<Boolean> J123Check = Utils.CheckJ1toJ3Range(MyJ4Data, MyJ5Data, MyJ6Data, out Range, out Average, 0.01);

                    if (J123Check.Contains(false))
                    {
                        Problems.Add(String.Format("Major axis positions not constant false indicates not ok:\n\nJ1={0}, J2={1}, J3={2} Excel file was NOT written!!!", J123Check[0].ToString(), J123Check[1].ToString(), J123Check[2].ToString()));
                        Problems.Add("Correct, then try again");
                        SetJ123Button.Visible = J1_TextBox.Visible = J2_TextBox.Visible = J3_TextBox.Visible = true;
                        J1_TextBox.Text = Average.Vec[0].ToString("F3");
                        J2_TextBox.Text = Average.Vec[1].ToString("F3");
                        J3_TextBox.Text = Average.Vec[2].ToString("F3");

                        btnReadJointCoordsButton.Enabled = false;
                    }
                    else
                    {
                        Boolean good = Check456Control(ref Problems);

                        if (good)
                        {
                            if (excel.WriteJ456Data(Path.Combine(RobotRootPath, "TrackerFile.xls"), MyJ4Data, MyJ5Data, MyJ6Data, ref Problems))
                                Problems.Add(string.Format("Successfully processed {0} J4, {1} J5 & {2} J6 data records",MyJ4Data.Count.ToString(),MyJ5Data.Count.ToString(),MyJ6Data.Count.ToString()));
                        }
                        else
                            Problems.Add("Excel file was NOT written!!!!!   Correct, then try again.");
                    }
                }
            }

            ReportProblems(Problems);
        }


        private void SetJointButton_Click(object sender, EventArgs e)
        {
            switch ((sender as Button).Tag.ToString())
            {
                case "J4-J5":
                    SetAny456Jval(J4_J5_TextBox.Text, 4, 4);
                    break;
                case "J5-J4":
                    SetAny456Jval(J5_J4_TextBox.Text, 5, 3);
                    break;
                case "J6-J4":
                    SetAny456Jval(J6_J4_TextBox.Text, 6, 3);
                    break;
                case "J4-J6":
                    SetAny456Jval(J4_J6_TextBox.Text, 4, 5);
                    break;
                case "J6-J5":
                    SetAny456Jval(J6_J5_TextBox.Text, 6, 4);
                    break;
                case "J5-J6":
                    SetAny456Jval(J5_J6_TextBox.Text, 5, 5);
                    break;

            }
        }


        private void SetJ123Button_Click(object sender, EventArgs e)
        {
            ArrayList Problems = new ArrayList();
            double J1Val, J2Val, J3Val;

            //Do we have 3 valid numbers 
            if (Utils.CheckValidTextDoubleConvert(J1_TextBox.Text, out J1Val) && 
                Utils.CheckValidTextDoubleConvert(J2_TextBox.Text, out J2Val) && 
                Utils.CheckValidTextDoubleConvert(J3_TextBox.Text, out J3Val))
            {
                string RobotRootPath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName));
                Vector j123 = new Vector(3);

                j123.Vec[0] = J1Val;
                j123.Vec[1] = J2Val;
                j123.Vec[2] = J3Val;

                switch (Rbrand.Company)
                {
                    case Brand.RobotBrands.ABB:

                        ABB.SetJ123Values(RobotRootPath, j123, ref Problems);
                        break;

                    case Brand.RobotBrands.Nachi:

                        Nachi.SetJ123Values(RobotRootPath, j123, ref Problems);
                        break;

                    case Brand.RobotBrands.Fanuc:
                    case Brand.RobotBrands.FanucRJ:

                        Fanuc.SetJ123Values(RobotRootPath, j123, ref Problems);
                        break;

                    case Brand.RobotBrands.KUKA:

                        Kuka.SetJ123Values(RobotRootPath, j123, ref Problems);
                        break;

                    default:

                        Problems.Add(String.Format("Robot brand {0} not implemented.",Rbrand.Company.ToString()));
                        break;
                }

                if (Problems.Count.Equals(0))
                {
                    SetJ123Button.Visible = J1_TextBox.Visible = J2_TextBox.Visible = J3_TextBox.Visible = false;
                    btnReadJointCoordsButton.Enabled = true;

                    Problems.Add("Successfully fixed problem with joints 1-3 in the robot program.\n\nBE SURE TO LOAD THIS IN THE ROBOT!!!!");
                }

                ReportProblems(Problems);
            }
        }

        private void SetAny456Jval(string TextVal, int MovingJointNum, int AxisIndex)
        {
            ArrayList Problems = new ArrayList();
            double Jval;

            if (Utils.CheckValidTextDoubleConvert(TextVal, out Jval))
            {
                string RobotRootPath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName));

                switch (Rbrand.Company)
                {
                    case Brand.RobotBrands.Nachi:

                        Nachi.SetSingleJointAxisValue(RobotRootPath, Jval, MovingJointNum - 1, AxisIndex, ref Problems);
                        Nachi.ReadCar0J456Targets(RobotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref Problems);
                        break;

                    case Brand.RobotBrands.Fanuc:
                    case Brand.RobotBrands.FanucRJ:

                        Fanuc.SetSingleJointAxisValue(RobotRootPath, Jval, MovingJointNum, AxisIndex, ref Problems);
                        Fanuc.ReadCar0J456Targets(RobotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref Problems);
                        break;

                    case Brand.RobotBrands.ABB:

                        ABB.SetSingleJointAxisValue(RobotRootPath, Jval, MovingJointNum, AxisIndex, ref Problems);
                        ABB.ReadCar0J456Targets(RobotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref Problems);
                        break;

                    case Brand.RobotBrands.KUKA:

                        Kuka.SetSingleJointAxisValue(RobotRootPath, Jval, MovingJointNum, AxisIndex, ref Problems);
                        Kuka.ReadCar0J456Targets(RobotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref Problems);
                        break;

                    default:

                        Problems.Add("Robot brand " + Rbrand.Company.ToString() + " not implemented.");
                        break;
                }
            }

            if (Problems.Count.Equals(0))
            {
                Boolean good = Check456Control(ref Problems);

                if (good)
                {
                    Problems.Add("Successfully fixed problem with all joints in the robot program.\n\nBE SURE TO LOAD THIS IN THE ROBOT!!!!");
                }
            }

            ReportProblems(Problems);
        }

   
        private void SetAllJointValuesButton_Click(object sender, EventArgs e)
        {
            string J4_J5 = J4_J5_TextBox.Text;
            string J4_J6 = J4_J6_TextBox.Text;
            string J5_J4 = J5_J4_TextBox.Text;
            string J5_J6 = J5_J6_TextBox.Text;
            string J6_J4 = J6_J4_TextBox.Text;
            string J6_J5 = J6_J5_TextBox.Text;

            if (J4setJ5ValueButton.Visible)
                SetAny456Jval(J4_J5, 4, 4);

            if (J4setJ6ValueButton.Visible)
                SetAny456Jval(J4_J6, 4, 5);

            if (J5setJ4ValueButton.Visible)
                SetAny456Jval(J5_J4, 5, 3);

            if (J5setJ6ValueButton.Visible)
                SetAny456Jval(J5_J6, 5, 5);

            if (J6setJ4ValueButton.Visible)
                SetAny456Jval(J6_J4, 6, 3);

            if (J6setJ5ValueButton.Visible)
                SetAny456Jval(J6_J5, 6, 4);
        }

        private void ResetInfo(object sender, EventArgs e)
        {
            btnReadJointCoordsButton.Enabled = true;
            MyRobotData = null;
        }

#region NotifyMessages
        private void NotifyMessage(string title, string message)
        {
            notify.ShowBalloonTip(1000, title, message, notify.BalloonTipIcon);
        }
        private void NotifyMessage(int timeout)
        {
            notify.ShowBalloonTip(timeout);
        }
        private void NotifyMessage(string message)
        {
            notify.ShowBalloonTip(1000, notify.BalloonTipTitle, message, notify.BalloonTipIcon);
        }
        private void NotifyMessage(string message,int timeout,string title,ToolTipIcon icon)
        {
            this.notify.ShowBalloonTip(timeout, title, message, icon);
        }
#endregion
    }
}
