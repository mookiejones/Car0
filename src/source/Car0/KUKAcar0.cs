using CarZero.Classes;

namespace CarZero
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    public partial class KUKAcar0 : Form
    {
  
        public KUKAcar0()
        {
            InitializeComponent();
            UserPreferences.Read();
            RobotBrandListBox.SelectedIndex = UserPreferences.RobotBrandSelected;
            Rbrand = new Brand(RobotBrandListBox.SelectedIndex);
            InitializeMyDisplayValues();
        }

        private bool AnyBaseFramesChecked()
        {

            for (var i = 0; i < RobotData.RobotFramesChecked.Count; i++)
            {
                if (!((!RobotData.RobotFramesChecked[i].Equals(true) || !RobotData.RobotFrames[i].ToString().Contains(Rbrand.BdStart)) || RobotData.RobotFrames[i].ToString().Contains("Ped")))
                {
                    return true;
                }
            }
            return false;
        }

        private bool AnyBaseFramesChecked(bool dummy)
        {
            for (var i = 0; i < RobotData.RobotFramesChecked.Count; i++)
            {
                if (RobotData.RobotFramesChecked[i].Equals(true) && (RobotData.RobotFrames[i].ToString().Contains(Rbrand.BdStart) || (RobotData.RobotFrames[i].ToString().Contains(Rbrand.PdStart) && RobotData.RobotFrames[i].ToString().Contains("Ped"))))
                {
                    return true;
                }
            }
            return false;
        }

        private void CalculateFinal3PointMethod(string RobotRootPath, string TrackerFilePath, ref ArrayList Problems)
        {
            List<Vector> rfpts = null;
            List<Vector> tdpts = null;
            var t = new Transformation();
            Transformation transformation2 = null;
            var frameDef = new double[6];
            var numArray2 = new double[6];
            var numArray3 = new double[6];
            var elementPts = new double[6];
            var numArray5 = new double[6];
            var aveError = 0.0;
            var ew = 0.0;
            var toolNum = 0;
            var linesWritten = 0;
            var worst = -1;
            string elementName = null;
            var flag = false;
            if (AnyBaseFramesChecked(true) && ((rfpts = ExcelIO.GetTrackerFileData(TrackerFilePath, "Initial wrt Robot World", ref Problems, Rbrand)) == null))
            {
                ReportProblems(Problems);
                ExcelIO.CloseTrackerData();
            }
            else
            {
                var ind = 0;
                while (ind < RobotData.RobotFramesChecked.Count)
                {
                    if (RobotData.RobotFramesChecked[ind].Equals(true))
                    {
                        int num4;
                        if (RobotData.RobotFrames[ind].ToString().Contains(Rbrand.BdStart) || (RobotData.RobotFrames[ind].ToString().Contains(Rbrand.PdStart) && RobotData.RobotFrames[ind].ToString().Contains("Ped")))
                        {
                            elementName = null;
                            if (!(!RobotData.RobotFrames[ind].ToString().Contains("Ped") || flag))
                            {
                                elementName = Rbrand.BdStart + " 1" + Rbrand.BdEnd;
                            }
                            else if (RobotData.RobotFrames[ind].ToString().Contains(Rbrand.BdStart))
                            {
                                elementName = RobotData.RobotFrames[ind].ToString().Substring(RobotData.RobotFrames[ind].ToString().IndexOf(Rbrand.BdStart));
                                if ((Rbrand.BdEnd.Length > 0) && elementName.Contains(Rbrand.BdEnd))
                                {
                                    elementName = elementName.Substring(0, elementName.IndexOf(Rbrand.BdEnd) + 1);
                                }
                            }
                            else if (RobotData.RobotFrames[ind].ToString().Contains(Rbrand.PdStart))
                            {
                                elementName = RobotData.RobotFrames[ind].ToString().Substring(RobotData.RobotFrames[ind].ToString().IndexOf(Rbrand.PdStart));
                                if ((Rbrand.PdEnd.Length > 0) && elementName.Contains(Rbrand.PdEnd))
                                {
                                    elementName = elementName.Substring(0, elementName.IndexOf(Rbrand.PdEnd) + 1);
                                }
                            }
                            if (elementName != null)
                            {
                                tdpts = ExcelIO.GetTrackerFileData(TrackerFilePath, elementName, ref Problems, Rbrand);
                                if (tdpts == null)
                                {
                                    ReportProblems(Problems);
                                    ExcelIO.CloseTrackerData();
                                    return;
                                }
                            }
                            else if (RobotData.FrameTypes[ind].Equals(FrameType.PedSpotGunTip) && (t != null))
                            {
                                transformation2 = new Transformation(t);
                            }
                            if (!RobotData.RobotFrames[ind].ToString().Contains("Ped") || !flag)
                            {
                                if ((tdpts != null) && rfpts.Count.Equals(tdpts.Count))
                                {
                                    if (!FrameRoutines.where(rfpts, tdpts, ref t, ref aveError, ref ew, ref worst, ref Problems))
                                    {
                                        ReportProblems(Problems);
                                        ExcelIO.CloseTrackerData();
                                        return;
                                    }
                                    if (Rbrand.Company.Equals(Brand.RobotBrands.KUKA))
                                    {
                                        t.trans_RPY(ref frameDef[0], ref frameDef[1], ref frameDef[2], ref frameDef[5], ref frameDef[4], ref frameDef[3]);
                                    }
                                    else
                                    {
                                        t.trans_RPY(ref frameDef[0], ref frameDef[1], ref frameDef[2], ref frameDef[3], ref frameDef[4], ref frameDef[5]);
                                    }
                                    num4 = 3;
                                    while (num4 < 6)
                                    {
                                        frameDef[num4] = Utils.RadToDegrees(frameDef[num4]);
                                        num4++;
                                    }
                                    if (RobotData.RobotFrames[ind].ToString().Contains("Ped"))
                                    {
                                        transformation2 = new Transformation(t);
                                    }
                                    else
                                    {
                                        FrameRoutines.WriteDefiner(RobotRootPath, elementName, frameDef, ref linesWritten, true, false, Rbrand, ind, ref Problems);
                                    }
                                    Problems.Add("Average / Worst fit error = " + aveError.ToString("F3") + "/" + ew.ToString("F3") + ", worst point is #" + worst.ToString());
                                }
                                else if (tdpts != null)
                                {
                                    Problems.Add("Not the same number of initial points = " + rfpts.Count.ToString() + " as final points = " + tdpts.Count.ToString());
                                }
                                else
                                {
                                    Problems.Add("Not the same number of initial points = " + rfpts.Count.ToString() + " as final points = null");
                                }
                            }
                            if (RobotData.RobotFrames[ind].ToString().Contains("Ped"))
                            {
                                if (transformation2 == null)
                                {
                                    Problems.Add("Program bug: wAtracker is null");
                                    ReportProblems(Problems);
                                    return;
                                }
                                elementName = RobotData.RobotFrames[ind].ToString().Substring(RobotData.RobotFrames[ind].ToString().IndexOf(Rbrand.PdStart));
                                if ((Rbrand.BdEnd.Length > 0) && elementName.Contains(Rbrand.BdEnd))
                                {
                                    elementName = elementName.Substring(0, elementName.IndexOf(Rbrand.BdEnd) + 1);
                                }
                                var vector = new Vector(3);
                                if (!ExcelIO.GetTrackerFileData(TrackerFilePath, ind, ref vector, ref Problems, Rbrand))
                                {
                                    ReportProblems(Problems);
                                    ExcelIO.CloseTrackerData();
                                    return;
                                }
                                var b = new Transformation(Transformation.SpecialTransformType.Identity);
                                b.point_trans(vector);
                                vector = transformation2.mult_trans(b).trans_point();
                                num4 = 0;
                                while (num4 < 3)
                                {
                                    frameDef[num4] = vector.Vec[num4];
                                    num4++;
                                }
                                numArray3 = new double[6];
                                toolNum = RobotData.FrameNumbers[ind];
                                if (FrameRoutines.ReadDefinerPedData(RobotRootPath, toolNum, ref numArray3, Rbrand.Company))
                                {
                                    num4 = 3;
                                    while (num4 < 6)
                                    {
                                        frameDef[num4] = numArray3[num4];
                                        num4++;
                                    }
                                }
                                else
                                {
                                    num4 = 3;
                                    while (num4 < 6)
                                    {
                                        frameDef[num4] = 0.0;
                                        num4++;
                                    }
                                }
                                FrameRoutines.WriteDefiner(RobotRootPath, elementName, frameDef, ref linesWritten, true, false, Rbrand, ind, ref Problems);
                            }
                            flag = true;
                        }
                        else if (RobotData.RobotFrames[ind].ToString().Contains(Rbrand.TdStart))
                        {
                            toolNum = RobotData.FrameNumbers[ind];
                            elementPts = new double[6];
                            if (!ExcelIO.GetTrackerFileData(TrackerFilePath, ind, ref elementPts, ref Problems, Rbrand))
                            {
                                ReportProblems(Problems);
                                ExcelIO.CloseTrackerData();
                                return;
                            }
                            numArray2 = new double[6];
                            if (FrameRoutines.ReadDefinerToolData(RobotRootPath, toolNum, ref numArray2, Rbrand.Company))
                            {
                                num4 = 3;
                                while (num4 < 6)
                                {
                                    elementPts[num4] = numArray2[num4];
                                    num4++;
                                }
                            }
                            else
                            {
                                Problems.Add("Unable to find " + elementName + " in the definer file.  This tool is downloaded with default orientation");
                            }
                            elementName = RobotData.RobotFrames[ind].ToString().Substring(RobotData.RobotFrames[ind].ToString().IndexOf(Rbrand.TdStart));
                            if ((Rbrand.TdEnd.Length > 0) && elementName.Contains(Rbrand.TdEnd))
                            {
                                elementName = elementName.Substring(0, elementName.IndexOf(Rbrand.TdEnd) + 1);
                            }
                            FrameRoutines.WriteDefiner(RobotRootPath, elementName, elementPts, ref linesWritten, true, false, Rbrand, ind, ref Problems);
                        }
                        else if (RobotData.RobotFrames[ind].ToString().Contains("BASE_DATA["))
                        {
                            toolNum = RobotData.FrameNumbers[ind];
                            numArray5 = new double[6];
                            if (!ExcelIO.GetTrackerFileData(TrackerFilePath, ind, ref numArray5, ref Problems, Rbrand))
                            {
                                ReportProblems(Problems);
                                ExcelIO.CloseTrackerData();
                                return;
                            }
                            numArray3 = new double[6];
                            if (FrameRoutines.ReadDefinerBaseData(RobotRootPath, toolNum, ref numArray3, Rbrand.Company))
                            {
                                for (num4 = 3; num4 < 6; num4++)
                                {
                                    numArray5[num4] = numArray3[num4];
                                }
                            }
                            else
                            {
                                Problems.Add("Unable to find " + elementName + " in the definer file.  This base is downloaded with default orientation");
                            }
                            FrameRoutines.WriteDefiner(RobotRootPath, elementName, numArray5, ref linesWritten, true, false, Rbrand, ind, ref Problems);
                        }
                    }
                    ind++;
                }
                if (!linesWritten.Equals(0))
                {
                    FrameRoutines.WriteDefiner(RobotRootPath, elementName, frameDef, ref linesWritten, false, true, Rbrand, ind, ref Problems);
                }
                ReportProblems(Problems);
                ExcelIO.CloseTrackerData();
            }
        }

        private void CalculateInitial321Fit()
        {
            var numArray = new double[3];
            var numArray2 = new double[3];
            var numArray3 = new double[3];
            var numArray4 = new double[6];
        }

        private void CalculateRobot321Button_Click(object sender, EventArgs e)
        {
            var problems = new ArrayList();
            ProblemsTextBox.Text = null;
            var robotName = RobotData.RobotName;
            while (robotName.EndsWith(" "))
            {
                robotName = robotName.Substring(0, robotName.Length - 1);
            }
            var str2 = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, robotName));
            var filePath = Path.Combine(str2, "TrackerFile.xls");
            if (ExcelIO.TrackerDataExcelOpen)
            {
                MessageBox.Show("Close the TrackerFile and try again");
            }
            else
            {
                if (!AnyBaseFramesChecked() || ExcelIO.CheckTrackerFile(filePath, ref problems, Rbrand))
                {
                    if (Initial321RadioButton.Checked)
                    {
                        CalculateInitial321Fit();
                    }
                    else if (RobotBasePointsRadioButton.Checked)
                    {
                        CalculateFinal3PointMethod(str2, filePath, ref problems);
                    }
                    else
                    {
                        ProcessToolRadialData(str2, filePath, ref problems);
                    }
                }
                ReportProblems(problems);
            }
        }

        private bool Check456Control(ref ArrayList Problems)
        {
            Vector vector;
            Vector vector2;
            double num2;
            var flag = true;
            var num = 0;
            var list = Utils.CheckWristData(MyJ4Data, out vector, out vector2, 0.01);
            ReadJointCoordsButton.Visible = true;
            SetAllJointValuesButton.Visible = J4setJ5ValueButton.Visible = J4_J5_TextBox.Visible = J4setJ6ValueButton.Visible = J4_J6_TextBox.Visible = J5setJ4ValueButton.Visible = J5_J4_TextBox.Visible = J5setJ6ValueButton.Visible = J5_J6_TextBox.Visible = J6setJ4ValueButton.Visible = J6_J4_TextBox.Visible = J6setJ5ValueButton.Visible = J6_J5_TextBox.Visible = false;
            var flag3 = list[1];
            if (flag3.Equals(false))
            {
                J4setJ5ValueButton.Visible = J4_J5_TextBox.Visible = true;
                num2 = vector2.Vec[4];
                J4_J5_TextBox.Text = num2.ToString("F3");
                num2 = vector.Vec[4];
                Problems.Add("J4 test joint 5 angles vary by: " + num2.ToString("F4"));
                flag = ReadJointCoordsButton.Visible = false;
                num++;
            }
            flag3 = list[2];
            if (flag3.Equals(false))
            {
                J4setJ6ValueButton.Visible = J4_J6_TextBox.Visible = true;
                num2 = vector2.Vec[5];
                J4_J6_TextBox.Text = num2.ToString("F3");
                num2 = vector.Vec[5];
                Problems.Add("J4 test joint 6 angles vary by: " + num2.ToString("F4"));
                flag = ReadJointCoordsButton.Visible = false;
                num++;
            }
            var list2 = Utils.CheckWristData(MyJ5Data, out vector, out vector2, 0.01);
            flag3 = list2[0];
            if (flag3.Equals(false))
            {
                J5setJ4ValueButton.Visible = J5_J4_TextBox.Visible = true;
                num2 = vector2.Vec[3];
                J5_J4_TextBox.Text = num2.ToString("F3");
                num2 = vector.Vec[3];
                Problems.Add("J5 test joint 4 angles vary by: " + num2.ToString("F4"));
                flag = ReadJointCoordsButton.Visible = false;
                num++;
            }
            flag3 = list2[2];
            if (flag3.Equals(false))
            {
                J5setJ6ValueButton.Visible = J5_J6_TextBox.Visible = true;
                num2 = vector2.Vec[5];
                J5_J6_TextBox.Text = num2.ToString("F3");
                num2 = vector.Vec[5];
                Problems.Add("J5 test joint 6 angles vary by: " + num2.ToString("F4"));
                flag = ReadJointCoordsButton.Visible = false;
                num++;
            }
            var list3 = Utils.CheckWristData(MyJ6Data, out vector, out vector2, 0.01);
            flag3 = list3[0];
            if (flag3.Equals(false))
            {
                J6setJ4ValueButton.Visible = J6_J4_TextBox.Visible = true;
                num2 = vector2.Vec[3];
                J6_J4_TextBox.Text = num2.ToString("F3");
                num2 = vector.Vec[3];
                Problems.Add("J6 test joint 4 angles vary by: " + num2.ToString("F4"));
                flag = ReadJointCoordsButton.Visible = false;
                num++;
            }
            flag3 = list3[1];
            if (flag3.Equals(false))
            {
                J6setJ5ValueButton.Visible = J6_J5_TextBox.Visible = true;
                num2 = vector2.Vec[4];
                J6_J5_TextBox.Text = num2.ToString("F3");
                Problems.Add("J6 test joint 5 angles vary by: " + vector.Vec[4].ToString("F4"));
                flag = ReadJointCoordsButton.Visible = false;
                num++;
            }
            SetAllJointValuesButton.Visible = num > 1;
            return flag;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FolderSelectButton_Click(object sender, EventArgs e)
        {
            folderBrowser1.Description = "Select work folder";
            if (folderBrowser1.SelectedPath == "")
            {
                folderBrowser1.SelectedPath = @"c:\";
            }
            if (folderBrowser1.ShowDialog() == DialogResult.OK)
            {
                UserPreferences.WorkFolderName = FolderDisplayTextBox.Text = folderBrowser1.SelectedPath;
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

        private void GetRobotDataButton_Click(object sender, EventArgs e)
        {
            MyRobotData = (MyRobotData == null) ? new RobotData() : MyRobotData;
            MyRobotData.Activate();
            MyRobotData.Show();
            ProblemsTextBox.Clear();
        }

        private void Initial321RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            OperationSelectionControl();
        }


        private void InitializeMyDisplayValues()
        {
            Initializing = true;
            switch (UserPreferences.OperationRadioButtonSelected)
            {
                case 1:
                    Initial321RadioButton.Checked = true;
                    break;

                case 2:
                    RobotBasePointsRadioButton.Checked = true;
                    break;

                case 3:
                    RadialToolDataRadioButton.Checked = true;
                    break;
            }
            FolderDisplayTextBox.Text = UserPreferences.WorkFolderName;
            GetRobotDataButton.Visible = RobotDataDisplayTextBox.Visible = (UserPreferences.WorkFolderName != null) && !UserPreferences.WorkFolderName.Equals("Not Set");
            Initializing = false;
        }

        private void J4setJ5ValueButton_Click(object sender, EventArgs e)
        {
            SetAny456Jval(J4_J5_TextBox.Text, 4, 4);
        }

        private void J4setJ6ValueButton_Click(object sender, EventArgs e)
        {
            SetAny456Jval(J4_J6_TextBox.Text, 4, 5);
        }

        private void J5setJ4ValueButton_Click(object sender, EventArgs e)
        {
            SetAny456Jval(J5_J4_TextBox.Text, 5, 3);
        }

        private void J5setJ6ValueButton_Click(object sender, EventArgs e)
        {
            SetAny456Jval(J5_J6_TextBox.Text, 5, 5);
        }

        private void J6setJ4ValueButton_Click(object sender, EventArgs e)
        {
            SetAny456Jval(J6_J4_TextBox.Text, 6, 3);
        }

        private void J6setJ5ValueButton_Click(object sender, EventArgs e)
        {
            SetAny456Jval(J6_J5_TextBox.Text, 6, 4);
        }

        private void KUKAcar0_Activated(object sender, EventArgs e)
        {
            CalculateRobot321Button.Visible = ViewTrackerFileBbutton.Visible = ReadJointCoordsButton.Visible = ReadRobotBaseDataButton.Visible = false;
            if (RobotData.StationName == null)
            {
                TrackerDataFileTextBox.Text = "Not Set";
            }
            else if (RobotData.RobotName == null)
            {
                RobotDataDisplayTextBox.Text = "System: " + RobotData.StationName + "   Robot: Not Set";
                TrackerDataFileTextBox.Text = "Not Set";
            }
            else
            {
                var robotName = RobotData.RobotName;
                while (robotName.EndsWith(" "))
                {
                    robotName = robotName.Substring(0, robotName.Length - 1);
                }
                RobotDataDisplayTextBox.Text = "System: " + RobotData.StationName + "   Robot: " + robotName + "(" + RobotData.RobotMechanismName + ")";
                var path = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, robotName));
                if (Directory.Exists(path))
                {
                    if (File.Exists(Path.Combine(path, "TrackerFile.xls")))
                    {
                        TrackerDataFileTextBox.Text = "TrackerFile.xls";
                        CalculateRobot321Button.Visible = ReadJointCoordsButton.Visible = ReadRobotBaseDataButton.Visible = ViewTrackerFileBbutton.Visible = true;
                    }
                    else
                    {
                        TrackerDataFileTextBox.Text = "Tracker Excel does NOT EXIST";
                    }
                }
                else
                {
                    TrackerDataFileTextBox.Text = "Robot folder '" + path + "' does NOT EXIST";
                }
            }
        }

        private void KUKAcar0_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExcelIO.CloseMatrix();
            ExcelIO.CloseTrackerData();
        }

        private void OperationSelectionControl()
        {
            var initializing = Initializing;
            if (Initial321RadioButton.Checked)
            {
                CalculateRobot321Button.Text = "Calculate Car 0 using 3-2-1";
                UserPreferences.OperationRadioButtonSelected = 1;
                WriteDebugDataCheckBox.Visible = false;
                Initializing = true;
                Initializing = initializing;
            }
            else if (RobotBasePointsRadioButton.Checked)
            {
                CalculateRobot321Button.Text = "Calculate Car 0 using reference points";
                UserPreferences.OperationRadioButtonSelected = 2;
                WriteDebugDataCheckBox.Visible = false;
                Initializing = true;
                Initializing = initializing;
            }
            else
            {
                CalculateRobot321Button.Text = "Calculate radial tool data";
                UserPreferences.OperationRadioButtonSelected = 3;
                WriteDebugDataCheckBox.Visible = true;
                Initializing = true;
                Initializing = initializing;
            }
            if (!Initializing)
            {
                UserPreferences.Write();
            }
        }

        private void ProcessToolRadialData(string RobotRootPath, string TrackerFilePath, ref ArrayList Problems)
        {
            var list = new List<Vector3>();
            var list2 = new List<Vector3>();
            var list3 = new List<Vector3>();
            var eOATvals = new List<Vector3>();
            var list5 = new List<Vector3>();
            var list6 = new List<Vector3>();
            var list7 = new List<Vector3>();
            var eOATangs = new List<Vector3>();
            var wristData = new double[12];
            if (!ExcelIO.ReadToolData(TrackerFilePath, ref list, ref list5, ref list2, ref list6, ref list3, ref list7, ref eOATvals, ref eOATangs, ref Problems, Rbrand))
            {
                ExcelIO.CloseTrackerData();
                ReportProblems(Problems);
            }
            else if (!ExcelIO.FindRobotWristData(TrackerFilePath, RobotData.RobotMechanismName, ref wristData, ref Problems))
            {
                ExcelIO.CloseTrackerData();
                ReportProblems(Problems);
            }
            else
            {
                int num3;
                Vector vector9;
                wristData[3] -= Utils.DegreesToRad(list6[0].x);
                var plane = new GetPlane(list);
                var rotation = new AxisOfRotation(list, plane.Normal, plane.Distance);
                var plane2 = new GetPlane(list2);
                var rotation2 = new AxisOfRotation(list2, plane2.Normal, plane2.Distance);
                var plane3 = new GetPlane(list3);
                var rotation3 = new AxisOfRotation(list3, plane3.Normal, plane3.Distance);
                Problems.Add("Max/Average J4 Radius = " + rotation.Radius.ToString("F2") + " fit error = (" + plane.MaxError.ToString("F6") + "/" + plane.AveError.ToString("F6") + ")");
                Problems.Add("Max/Average J5 Radius = " + rotation2.Radius.ToString("F2") + " fit error = (" + plane2.MaxError.ToString("F6") + "/" + plane2.AveError.ToString("F6") + ")");
                Problems.Add("Max/Average J6 Radius = " + rotation3.Radius.ToString("F2") + " fit error = (" + plane3.MaxError.ToString("F6") + "/" + plane3.AveError.ToString("F6") + ")");
                var sp = new Vector3();
                var ep = new Vector3();
                var vector3 = new Vector3();
                var vector4 = new Vector3();
                var pa = new Vector3();
                var pb = new Vector3();
                rotation.Origin.PointSlopeToEndpoints(rotation.Normal, 10000.0, ref sp, ref ep);
                rotation2.Origin.PointSlopeToEndpoints(rotation2.Normal, 10000.0, ref vector3, ref vector4);
                var vecs = new List<Vector3>(3);
                Vector3.LineLineIntersect(sp, ep, vector3, vector4, ref pa, ref pb, ref Problems, "J4/J5 intersection");
                vecs.Add(pa.MidPoint(pb));
                rotation3.Origin.PointSlopeToEndpoints(rotation3.Normal, 10000.0, ref vector3, ref vector4);
                Vector3.LineLineIntersect(sp, ep, vector3, vector4, ref pa, ref pb, ref Problems, "J4/J6 intersection");
                vecs.Add(pa.MidPoint(pb));
                rotation2.Origin.PointSlopeToEndpoints(rotation2.Normal, 10000.0, ref sp, ref ep);
                Vector3.LineLineIntersect(sp, ep, vector3, vector4, ref pa, ref pb, ref Problems, "J5/J6 intersection");
                vecs.Add(pa.MidPoint(pb));
                var max = 0.0;
                var ave = 0.0;
                var b = new Vector3(vecs, ref max, ref ave, out num3);
                var strArray = new string[6];
                strArray[0] = "J456 Intersection Max/Average (";
                strArray[1] = max.ToString("F3");
                strArray[2] = ",";
                strArray[3] = ave.ToString("F3");
                strArray[4] = "), Worst Point #";
                var num5 = num3 + 1;
                strArray[5] = num5.ToString();
                Problems.Add(string.Concat(strArray));
                var fAt = new Transformation();
                var a = new Vector(rotation.Normal);
                fAt.attack_trans(a);
                if (((RobotData.RobotMechanismName.Equals("SRA210F") || RobotData.RobotMechanismName.Equals("SRA166")) || (RobotData.RobotMechanismName.Equals("SC400L") || RobotData.RobotMechanismName.Equals("MC350"))) || RobotData.RobotMechanismName.Equals("NV06"))
                {
                    vector9 = new Vector(rotation2.Normal).Scale(-1.0);
                }
                else
                {
                    vector9 = new Vector(rotation2.Normal).Scale(-1.0);
                }
                fAt.orient_trans(vector9);
                fAt.norm_trans(vector9.CrossProduct(a));
                fAt.point_trans(new Vector(b));
                var toAfrom = fAt.inv_trans();
                var list10 = FrameRoutines.PointSetCsChange(toAfrom, list);
                var list11 = FrameRoutines.PointSetCsChange(toAfrom, list2);
                var list12 = FrameRoutines.PointSetCsChange(toAfrom, list3);
                var newStuff = FrameRoutines.PointSetJointTransform(list10, list5, wristData);
                var list14 = FrameRoutines.PointSetJointTransform(list11, list6, wristData);
                var list15 = FrameRoutines.PointSetJointTransform(list12, list7, wristData);
                var buildList = new List<Vector3>();
                Vector3.AddToList(newStuff, ref buildList);
                Vector3.AddToList(list14, ref buildList);
                Vector3.AddToList(list15, ref buildList);
                var vec = new Vector3(buildList, ref max, ref ave, out num3);
                var num4 = 4;
                if (num3 >= newStuff.Count)
                {
                    num4++;
                    num3 -= newStuff.Count;
                    if (num3 >= list14.Count)
                    {
                        num4++;
                        num3 -= list14.Count;
                        if (num3 >= list15.Count)
                        {
                            num4++;
                            num3 -= list14.Count;
                        }
                    }
                }
                strArray = new string[] { "TOOL (", vec.x.ToString("F3"), ", ", vec.y.ToString("F3"), ", ", vec.z.ToString("F3"), ") Max/Average (", max.ToString("F3"), ",", ave.ToString("F3"), "), Worst point J", num4.ToString(), " Point #", (num3 + 1).ToString() };
                Problems.Add(string.Concat(strArray));
                var toolData = FrameRoutines.PointSetJointTransform(FrameRoutines.PointSetCsChange(toAfrom, eOATvals, true), eOATangs, wristData, true);
                ExcelIO.WriteToolDefinitions(TrackerFilePath, toolData, ref Problems);
                ReportProblems(Problems);
                if (WriteDebugDataCheckBox.Checked)
                {
                    var buildArrList = new ArrayList();
                    Utils.AddVec3ListToArrayList(ref buildArrList, list, "J4 Raw");
                    Utils.AddVec3ListToArrayList(ref buildArrList, list2, "J5 Raw");
                    Utils.AddVec3ListToArrayList(ref buildArrList, list3, "J6 Raw");
                    Utils.AddVec3ListToArrayList(ref buildArrList, list10, "J4 wrt j4_axis");
                    Utils.AddVec3ListToArrayList(ref buildArrList, list11, "J5 wrt j4_axis");
                    Utils.AddVec3ListToArrayList(ref buildArrList, list12, "J6 wrt j4_axis");
                    Utils.AddVec3ListToArrayList(ref buildArrList, newStuff, "J4 wrt TOOLFRAME");
                    Utils.AddVec3ListToArrayList(ref buildArrList, list14, "J5 wrt TOOLFRAME");
                    Utils.AddVec3ListToArrayList(ref buildArrList, list15, "J6 wrt TOOLFRAME");
                    Utils.AddTransToArrayList(ref buildArrList, fAt, "wAj4");
                    Utils.AddVec3ToArrayList(ref buildArrList, vec, "Puck");
                    Utils.ArrayListToFile(Path.Combine(RobotRootPath, "Debug.csv"), buildArrList, true);
                }
            }
        }

        private void ReadJointCoordsButton_Click(object sender, EventArgs e)
        {
            var problems = new ArrayList();
            if (UserPreferences.WorkFolderName == null)
            {
                problems.Add("You must set the root folder name first - Press Set root folder button");
            }
            else if ((RobotData.StationName == null) || (RobotData.RobotName == null))
            {
                problems.Add("You must read the robot data first - Press Robot data button");
            }
            else
            {
                var robotRootPath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName));
                switch (Rbrand.Company)
                {
                    case Brand.RobotBrands.ABB:
                        ABB.ReadCar0J456Targets(robotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref problems);
                        break;

                    case Brand.RobotBrands.Fanuc:
                    case Brand.RobotBrands.FanucRJ:
                        Fanuc.ReadCar0J456Targets(robotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref problems);
                        break;

                    case Brand.RobotBrands.KUKA:
                        Kuka.ReadCar0J456Targets(robotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref problems);
                        break;

                    case Brand.RobotBrands.Nachi:
                        Nachi.ReadCar0J456Targets(robotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref problems);
                        break;

                    default:
                        problems.Add("Unrecognized robot type in ReadJointCoordsButton_Click");
                        break;
                }
                if (problems.Count.Equals(0))
                {
                    Vector vector;
                    Vector vector2;
                    var list2 = Utils.CheckJ1toJ3Range(MyJ4Data, MyJ5Data, MyJ6Data, out vector, out vector2, 0.01);
                    if (list2.Contains(false))
                    {
                        var strArray = new string[7];
                        strArray[0] = "Major axis positions not constant false indicates not ok:\n\nJ1=";
                        var flag3 = list2[0];
                        strArray[1] = flag3.ToString();
                        strArray[2] = ", J2=";
                        flag3 = list2[1];
                        strArray[3] = flag3.ToString();
                        strArray[4] = ", J3=";
                        strArray[5] = list2[2].ToString();
                        strArray[6] = " Excel file was NOT written!!!!!";
                        problems.Add(string.Concat(strArray));
                        problems.Add("Correct, then try again");
                        SetJ123Button.Visible = J1_TextBox.Visible = J2_TextBox.Visible = J3_TextBox.Visible = true;
                        var num2 = vector2.Vec[0];
                        J1_TextBox.Text = num2.ToString("F3");
                        num2 = vector2.Vec[1];
                        J2_TextBox.Text = num2.ToString("F3");
                        J3_TextBox.Text = vector2.Vec[2].ToString("F3");
                        ReadJointCoordsButton.Visible = false;
                    }
                    else if (Check456Control(ref problems))
                    {
                        if (ExcelIO.WriteJ456Data(Path.Combine(robotRootPath, "TrackerFile.xls"), MyJ4Data, MyJ5Data, MyJ6Data, ref problems))
                        {
                            problems.Add("Successfully processed " + MyJ4Data.Count.ToString() + " J4, " + MyJ5Data.Count.ToString() + " J5 & " + MyJ6Data.Count.ToString() + " J6 data records.");
                        }
                    }
                    else
                    {
                        problems.Add("Excel file was NOT written!!!!!   Correct, then try again.");
                    }
                }
            }
            ReportProblems(problems);
        }

        private void ReadRobotBaseDataButton_Click(object sender, EventArgs e)
        {
            var problems = new ArrayList();
            var baseData = new List<Vector3>();
            if (UserPreferences.WorkFolderName == null)
            {
                problems.Add("You must set the root folder name first - Press Set root folder button");
            }
            else if ((RobotData.StationName == null) || (RobotData.RobotName == null))
            {
                problems.Add("You must read the robot data first - Press Robot data button");
            }
            else
            {
                var robotRootPath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName));
                switch (Rbrand.Company)
                {
                    case Brand.RobotBrands.ABB:
                        baseData = ABB.ReadCar0BasePtsProgram(robotRootPath, ref problems);
                        break;

                    case Brand.RobotBrands.Fanuc:
                    case Brand.RobotBrands.FanucRJ:
                        baseData = Fanuc.ReadCar0BasePtsProgram(robotRootPath, ref problems);
                        break;

                    case Brand.RobotBrands.KUKA:
                        baseData = Kuka.ReadCar0BasePtsProgram(robotRootPath, ref problems);
                        break;

                    case Brand.RobotBrands.Nachi:
                        baseData = Nachi.ReadCar0BasePtsProgram(robotRootPath, ref problems);
                        break;

                    default:
                        problems.Add("Unrecognized robot type in ReadRobotBaseDataButton_Click");
                        break;
                }
                if (problems.Count.Equals(0) && ExcelIO.WriteCar0BaseData(Path.Combine(robotRootPath, "TrackerFile.xls"), baseData, ref problems))
                {
                    problems.Add("Successfully read " + baseData.Count.ToString() + " Initial robot world base points.");
                }
            }
            ReportProblems(problems);
        }

        private void ReportProblems(ArrayList Problems)
        {
            ProblemsTextBox.Text = null;
            if (Problems.Count > 0)
            {
                for (var i = 0; i < Problems.Count; i++)
                {
                    ProblemsTextBox.Text = ProblemsTextBox.Text + Problems[i].ToString() + "\n";
                }
            }
            else
            {
                ProblemsTextBox.Text = "Successfully completed with no errors or warnings";
            }
        }

        private void RobotBasePointsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            OperationSelectionControl();
        }

        private void RobotBrandListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserPreferences.RobotBrandSelected = RobotBrandListBox.SelectedIndex;
            Rbrand = new Brand(RobotBrandListBox.SelectedIndex);
            UserPreferences.Write();
            RobotData.RobotFramesChecked = new ArrayList();
        }

        private void SetAllJointValuesButton_Click(object sender, EventArgs e)
        {
            var text = J4_J5_TextBox.Text;
            var textVal = J4_J6_TextBox.Text;
            var str3 = J5_J4_TextBox.Text;
            var str4 = J5_J6_TextBox.Text;
            var str5 = J6_J4_TextBox.Text;
            var str6 = J6_J5_TextBox.Text;
            if (J4setJ5ValueButton.Visible)
            {
                SetAny456Jval(text, 4, 4);
            }
            if (J4setJ6ValueButton.Visible)
            {
                SetAny456Jval(textVal, 4, 5);
            }
            if (J5setJ4ValueButton.Visible)
            {
                SetAny456Jval(str3, 5, 3);
            }
            if (J5setJ6ValueButton.Visible)
            {
                SetAny456Jval(str4, 5, 5);
            }
            if (J6setJ4ValueButton.Visible)
            {
                SetAny456Jval(str5, 6, 3);
            }
            if (J6setJ5ValueButton.Visible)
            {
                SetAny456Jval(str6, 6, 4);
            }
        }

        private void SetAny456Jval(string TextVal, int MovingJointNum, int AxisIndex)
        {
            double num;
            var problems = new ArrayList();
            if (Utils.CheckValidTextDoubleConvert(TextVal, out num))
            {
                var robotRootPath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName));
                switch (Rbrand.Company)
                {
                    case Brand.RobotBrands.ABB:
                        ABB.SetSingleJointAxisValue(robotRootPath, num, MovingJointNum, AxisIndex, ref problems);
                        ABB.ReadCar0J456Targets(robotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref problems);
                        goto Label_012E;

                    case Brand.RobotBrands.Fanuc:
                    case Brand.RobotBrands.FanucRJ:
                        Fanuc.SetSingleJointAxisValue(robotRootPath, num, MovingJointNum, AxisIndex, ref problems);
                        Fanuc.ReadCar0J456Targets(robotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref problems);
                        goto Label_012E;

                    case Brand.RobotBrands.KUKA:
                        Kuka.SetSingleJointAxisValue(robotRootPath, num, MovingJointNum, AxisIndex, ref problems);
                        Kuka.ReadCar0J456Targets(robotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref problems);
                        goto Label_012E;

                    case Brand.RobotBrands.Nachi:
                        Nachi.SetSingleJointAxisValue(robotRootPath, num, MovingJointNum - 1, AxisIndex, ref problems);
                        Nachi.ReadCar0J456Targets(robotRootPath, out MyJ4Data, out MyJ5Data, out MyJ6Data, ref problems);
                        goto Label_012E;
                }
                problems.Add("Robot brand " + Rbrand.Company.ToString() + " not implemented.");
            }
        Label_012E:
            if (problems.Count.Equals(0) && Check456Control(ref problems))
            {
                problems.Add("Successfully fixed problem with all joints in the robot program.\n\nBE SURE TO LOAD THIS IN THE ROBOT!!!!");
            }
            ReportProblems(problems);
        }

        private void SetJ123Button_Click(object sender, EventArgs e)
        {
            double num;
            double num2;
            double num3;
            var problems = new ArrayList();
            if ((Utils.CheckValidTextDoubleConvert(J1_TextBox.Text, out num) && Utils.CheckValidTextDoubleConvert(J2_TextBox.Text, out num2)) && Utils.CheckValidTextDoubleConvert(J3_TextBox.Text, out num3))
            {
                var robotRootPath = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName));
                var vector = new Vector(3);
                vector.Vec[0] = num;
                vector.Vec[1] = num2;
                vector.Vec[2] = num3;
                switch (Rbrand.Company)
                {
                    case Brand.RobotBrands.ABB:
                        ABB.SetJ123Values(robotRootPath, vector, ref problems);
                        break;

                    case Brand.RobotBrands.Fanuc:
                    case Brand.RobotBrands.FanucRJ:
                        Fanuc.SetJ123Values(robotRootPath, vector, ref problems);
                        break;

                    case Brand.RobotBrands.KUKA:
                        Kuka.SetJ123Values(robotRootPath, vector, ref problems);
                        break;

                    case Brand.RobotBrands.Nachi:
                        Nachi.SetJ123Values(robotRootPath, vector, ref problems);
                        break;

                    default:
                        problems.Add("Robot brand " + Rbrand.Company.ToString() + " not implemented.");
                        break;
                }
                if (problems.Count.Equals(0))
                {
                    SetJ123Button.Visible = J1_TextBox.Visible = J2_TextBox.Visible = J3_TextBox.Visible = false;
                    ReadJointCoordsButton.Visible = true;
                    problems.Add("Successfully fixed problem with joints 1-3 in the robot program.\n\nBE SURE TO LOAD THIS IN THE ROBOT!!!!");
                }
                ReportProblems(problems);
            }
        }

        private void ViewTrackerFileBbutton_Click(object sender, EventArgs e)
        {
            var robotName = RobotData.RobotName;
            while (robotName.EndsWith(" "))
            {
                robotName = robotName.Substring(0, robotName.Length - 1);
            }
            ExcelIO.OpenExcelFile(Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, Path.Combine(robotName, "TrackerFile.xls"))), true);
        }

       
    }
}

