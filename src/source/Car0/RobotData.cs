

using CarZero.Classes;

namespace CarZero
{ 
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public partial class RobotData : Form
    {
      

        public RobotData()
        {
            InitializeComponent();
            RobotMatrixTextBox.Text = UserPreferences.RobotMatrixFileName;
            MaximumSize = new Size(0x267, 350);
            Size = new Size(0x267, 350);
            BadApplicationsRichTextBox.Visible = BadAppLabel.Visible = false;
            Initializing = true;
            ShowSystems();
            Initializing = false;
        }

        private void ApplicationFramesCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (ChecksChanged = !Initializing)
            {
                RobotFramesChecked = new ArrayList();
                for (var i = 0; i < ApplicationFramesCheckedListBox.Items.Count; i++)
                {
                    if (e.Index.Equals(i))
                    {
                        RobotFramesChecked.Add(e.NewValue.ToString().Equals("Checked"));
                    }
                    else if (ApplicationFramesCheckedListBox.CheckedIndices.Contains(i))
                    {
                        RobotFramesChecked.Add(true);
                    }
                    else
                    {
                        RobotFramesChecked.Add(false);
                    }
                }
            }
        }

        private void CheckRobotsButton_Click(object sender, EventArgs e)
        {
            var systemNames = new ArrayList();
            var list2 = new ArrayList();
            if (ExcelIO.BuildMatyrixSystemList(ref systemNames, ref SystemCodes))
            {
                MaximumSize = new Size(0x267, 600);
                Size = new Size(0x267, 600);
                var num = 0;
                CheckProgressBar.Value = 0;
                CheckProgressBar.Refresh();
                foreach (string str in systemNames)
                {
                    var robotNames = new ArrayList();
                    var robotMechanismNames = new ArrayList();
                    string robotMechanismType = null;
                    var robotApps = new ArrayList();
                    if (ExcelIO.BuildMatrixRobotList(ref robotNames, ref robotMechanismNames, str))
                    {
                        foreach (string str3 in robotNames)
                        {
                            if (ExcelIO.GetRobotData(str, str3, ref robotMechanismType, ref RobotStationName, ref robotApps))
                            {
                                foreach (string str4 in robotApps)
                                {
                                    var flag = false;
                                    if (str4.Contains("M/H"))
                                    {
                                        flag = true;
                                    }
                                    else if (str4.Contains("Ped"))
                                    {
                                        if (str4.Contains("Dual"))
                                        {
                                            if (str4.Contains("Seal"))
                                            {
                                                flag = true;
                                            }
                                            else if (str4.Contains("Weld") || str4.Contains("W/Gun"))
                                            {
                                                flag = true;
                                            }
                                        }
                                        else if (str4.Contains("Stud"))
                                        {
                                            flag = true;
                                        }
                                        else if (str4.Contains("Nut"))
                                        {
                                            flag = true;
                                        }
                                        else if (str4.Contains("Seal"))
                                        {
                                            flag = true;
                                        }
                                        else if (str4.Contains("Weld") || str4.Contains("W/Gun"))
                                        {
                                            flag = true;
                                        }
                                    }
                                    else if (str4.Contains("W/Gun"))
                                    {
                                        flag = true;
                                    }
                                    else if (str4.Contains("Mig"))
                                    {
                                        flag = true;
                                    }
                                    else if (str4.Contains("Seal"))
                                    {
                                        flag = true;
                                    }
                                    else if (str4.Contains("Stud"))
                                    {
                                        flag = true;
                                    }
                                    else if (str4.Contains("Scriber"))
                                    {
                                        flag = true;
                                    }
                                    else if (str4.Contains("Vision"))
                                    {
                                        flag = true;
                                    }
                                    else if (str4.Contains("Laser Cutting"))
                                    {
                                        flag = true;
                                    }
                                    else if (str4.Contains("Piercing"))
                                    {
                                        flag = true;
                                    }
                                    if (!flag)
                                    {
                                        list2.Add(str + " " + str3 + " Application = '" + str4 + "'");
                                    }
                                }
                            }
                        }
                    }
                    num++;
                    CheckProgressBar.Value = (100 * num) / systemNames.Count;
                    CheckProgressBar.Refresh();
                }
                if (list2.Count > 0)
                {
                    BadApplicationsRichTextBox.Visible = BadAppLabel.Visible = true;
                    BadApplicationsRichTextBox.Text = null;
                    foreach (string str5 in list2)
                    {
                        BadApplicationsRichTextBox.Text = BadApplicationsRichTextBox.Text + str5 + "\n";
                    }
                }
                else
                {
                    MaximumSize = new Size(0x267, 350);
                    Size = new Size(0x267, 350);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private void MakeFoldersButton_Click(object sender, EventArgs e)
        {
            var robotNames = new ArrayList();
            var robotMechanismNames = new ArrayList();
            string path = null;
            var str2 = Path.Combine(UserPreferences.WorkFolderName, "TrackerFile.xls");
            var flag = File.Exists(str2);
            for (var i = 0; i < StationSelectListBox.Items.Count; i++)
            {
                if (ExcelIO.BuildMatrixRobotList(ref robotNames, ref robotMechanismNames, StationSelectListBox.Items[i].ToString()))
                {
                    for (var j = 0; j < robotNames.Count; j++)
                    {
                        path = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(StationSelectListBox.Items[i].ToString(), robotNames[j].ToString()));
                        Directory.CreateDirectory(path);
                        if (flag)
                        {
                            File.Copy(str2, Path.Combine(path, "TrackerFile.xls"));
                        }
                    }
                }
            }
        }

        private void RobotData_Deactivate(object sender, EventArgs e)
        {
            if (!ShowingRobots)
            {
                ExcelIO.CloseMatrix();
            }
        }

        private void RobotData_FormClosing(object sender, FormClosingEventArgs e)
        {
            KUKAcar0.MyRobotData = null;
            if (ChecksChanged && MessageBox.Show("Save changes in checked frame selections", "FRAME SELECTIONS HAVE CHANGED!!!", MessageBoxButtons.YesNo).Equals(DialogResult.Yes))
            {
                WriteChecked();
            }
            ExcelIO.CloseMatrix();
        }

        private void RobotMatrixFileButton_Click(object sender, EventArgs e)
        {
            FileBrowser.Title = "Select Excel Robot Matrix File";
            FileBrowser.InitialDirectory = UserPreferences.WorkFolderName;
            FileBrowser.Filter = "Excel Files (*.xls; *.xlsx) | *.xls;*.xlsx";
            FileReady = FileBrowser.ShowDialog();
            ExcelIO.CloseMatrix();
            RobotFrames = new ArrayList();
            RobotFramesChecked = new ArrayList();
            RobotName = StationName = (string) (RobotMechanismName = null);
            if (FileReady == DialogResult.OK)
            {
                UserPreferences.RobotMatrixFileName = RobotMatrixTextBox.Text = FileBrowser.FileName;
                UserPreferences.CurrentStyleSelected = -1;
                ShowSystems();
                UserPreferences.Write();
            }
        }

        private void RobotSelectListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserPreferences.CurrentRobotSelected = RobotSelectListBox.SelectedIndex;
            RobotName = (RobotSelectListBox.SelectedIndex == -1) ? null : RobotSelectListBox.Items[RobotSelectListBox.SelectedIndex].ToString();
            if (RobotName.Contains("("))
            {
                RobotMechanismName = RobotName.Substring(RobotName.IndexOf("(") + 1);
                RobotMechanismName = RobotMechanismName.Replace(")", "");
                RobotName = RobotName.Substring(0, RobotName.IndexOf("("));
            }
            if (!Initializing)
            {
                RobotFrames = new ArrayList();
                RobotFramesChecked = new ArrayList();
            }
            UserPreferences.Write();
            ShowRobotDataLabels();
        }

        private void SetCheckedIndex(ref CheckedListBox Box, ArrayList LastChecked, int Indx, bool DefaultValue)
        {
            if (Indx < Box.Items.Count)
            {
                if (LastChecked.Count.Equals(0))
                {
                    Box.SetItemChecked(Indx, DefaultValue);
                }
                else
                {
                    Box.SetItemChecked(Indx, LastChecked.Contains(Indx.ToString()));
                }
            }
        }

        private void ShowRobotDataLabels()
        {
            string robotMechanismType = null;
            var robotApps = new ArrayList();
            string item = null;
            string str3 = null;
            Brand brand = new Brand(UserPreferences.RobotBrandSelected);
            if ((UserPreferences.CurrentStyleSelected != -1) && (UserPreferences.CurrentRobotSelected != -1))
            {
                var targetRobot = RobotSelectListBox.Items[UserPreferences.CurrentRobotSelected].ToString().Contains("(") ? RobotSelectListBox.Items[UserPreferences.CurrentRobotSelected].ToString().Substring(0, RobotSelectListBox.Items[UserPreferences.CurrentRobotSelected].ToString().IndexOf("(")) : RobotSelectListBox.Items[UserPreferences.CurrentRobotSelected].ToString();
                ArrayList lastChecked = Utils.FileToArrayList(Path.Combine(Path.Combine(UserPreferences.WorkFolderName, Path.Combine(StationName, RobotName)), "CheckedIndices.txt"));
                if (ExcelIO.GetRobotData(StationSelectListBox.Items[UserPreferences.CurrentStyleSelected].ToString(), targetRobot, ref robotMechanismType, ref RobotStationName, ref robotApps))
                {
                    int num;
                    int firstCarriedSpot = brand.FirstCarriedSpot;
                    int firstCarriedStud = brand.FirstCarriedStud;
                    int firstCarriedSeal = brand.FirstCarriedSeal;
                    int firstCarriedMIG = brand.FirstCarriedMIG;
                    int num7 = brand.FirstGripCar0;
                    int firstGripPin = brand.FirstGripPin;
                    int num9 = brand.FirstCar0;
                    int firstPedTip = brand.FirstPedTip;
                    var num11 = 1;
                    var num12 = 1;
                    var num13 = 1;
                    var num14 = 1;
                    var num15 = 1;
                    var num16 = 1;
                    var flag = RobotFramesChecked.Count == 0;
                    RobotTypeLabel.Text = robotMechanismType;
                    RobotTypeLabel.Visible = true;
                    ApplicationFramesCheckedListBox.Items.Clear();
                    ApplicationFramesCheckedListBox.Sorted = false;
                    if (flag)
                    {
                        RobotFrames = new ArrayList();
                        RobotFramesChecked = new ArrayList();
                        ToolNames = new List<AppToolName>();
                        FrameTypes = new List<FrameType>();
                        FrameNumbers = new List<int>();
                        FrameDescriptions = new ArrayList();
                        Initializing = true;
                        for (num = 0; num < robotApps.Count; num++)
                        {
                            int num2;
                            string str5;
                            item = robotApps[num].ToString().PadRight(0x25);
                            if (robotApps[num].ToString().Contains("M/H"))
                            {
                                if (brand.UseGripperCar0)
                                {
                                    str3 = " @Grip " + num11.ToString() + " Car 0";
                                    str5 = item;
                                    item = str5 + brand.Gc0Start + num7.ToString().PadLeft(2) + brand.Gc0End + str3;
                                    ApplicationFramesCheckedListBox.Items.Add(item);
                                    if (brand.GripperCar0IsToolDef)
                                    {
                                        ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                    }
                                    else
                                    {
                                        ToolNames.Add(new AppToolName());
                                    }
                                    FrameTypes.Add(FrameType.GripperCar0);
                                    FrameNumbers.Add(num7);
                                    FrameDescriptions.Add(str3);
                                    SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckGripCar0);
                                }
                                num2 = 0;
                                while (num2 < brand.MaxNumberGripperPin)
                                {
                                    var num17 = num2 + 1;
                                    str3 = " @Grip " + num11.ToString() + " Pin " + num17.ToString();
                                    str5 = robotApps[num].ToString().PadRight(0x25);
                                    item = str5 + brand.TdStart + firstGripPin.ToString().PadLeft(2) + brand.TdEnd + str3;
                                    ApplicationFramesCheckedListBox.Items.Add(item);
                                    SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, num2.Equals(0) & brand.CheckFirstGripPin);
                                    ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                    FrameTypes.Add(FrameType.GripPin);
                                    FrameNumbers.Add(firstGripPin);
                                    FrameDescriptions.Add(str3);
                                    firstGripPin++;
                                    num2++;
                                }
                                str3 = " @Pick 1 location";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.PickTool);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstPickFrame);
                                num9++;
                                str3 = " @Place 1 location";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.DropTool);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstDropFrame);
                                num9++;
                                str3 = " @Pick 2 location";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.PickTool);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckSubsequentPickFrame);
                                num9++;
                                str3 = " @Place 2 location";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.DropTool);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckSubsequentDropFrame);
                                num9++;
                                num11++;
                                num7++;
                            }
                            else if (robotApps[num].ToString().Contains("Ped"))
                            {
                                if (robotApps[num].ToString().Contains("Dual"))
                                {
                                    if (robotApps[num].ToString().Contains("Seal"))
                                    {
                                        str3 = " @Dual Ped Seal Tip 1 ";
                                        str5 = robotApps[num].ToString().PadRight(0x25);
                                        item = str5 + brand.PdStart + firstPedTip.ToString().PadLeft(2) + brand.PdEnd + str3;
                                        ApplicationFramesCheckedListBox.Items.Add(item);
                                        ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                        FrameTypes.Add(FrameType.SealGunTip);
                                        FrameDescriptions.Add(str3);
                                        FrameNumbers.Add(firstPedTip);
                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstPedFrame);
                                        firstPedTip++;
                                        str3 = " @Dual Ped Seal Tip 2 ";
                                        str5 = robotApps[num].ToString().PadRight(0x25);
                                        item = str5 + brand.PdStart + firstPedTip.ToString().PadLeft(2) + brand.PdEnd + str3;
                                        ApplicationFramesCheckedListBox.Items.Add(item);
                                        ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                        FrameTypes.Add(FrameType.SealGunTip);
                                        FrameDescriptions.Add(str3);
                                        FrameNumbers.Add(firstPedTip);
                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstPedFrame);
                                        firstPedTip++;
                                    }
                                    else
                                    {
                                        str3 = " @Dual Ped Weld Gun Tip 1 ";
                                        str5 = robotApps[num].ToString().PadRight(0x25);
                                        item = str5 + brand.PdStart + firstPedTip.ToString().PadLeft(2) + brand.PdEnd + str3;
                                        ApplicationFramesCheckedListBox.Items.Add(item);
                                        ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                        FrameTypes.Add(FrameType.PedSpotGunTip);
                                        FrameDescriptions.Add(str3);
                                        FrameNumbers.Add(firstPedTip);
                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstPedFrame);
                                        firstPedTip++;
                                        str3 = " @Dual Ped Weld Gun Tip 2 ";
                                        str5 = robotApps[num].ToString().PadRight(0x25);
                                        item = str5 + brand.PdStart + firstPedTip.ToString().PadLeft(2) + brand.PdEnd + str3;
                                        ApplicationFramesCheckedListBox.Items.Add(item);
                                        ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                        FrameTypes.Add(FrameType.PedSpotGunTip);
                                        FrameDescriptions.Add(str3);
                                        FrameNumbers.Add(firstPedTip);
                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstPedFrame);
                                        firstPedTip++;
                                    }
                                }
                                else if (robotApps[num].ToString().Contains("Stud"))
                                {
                                    str3 = " @Ped Stud Gun " + num14.ToString();
                                    str5 = robotApps[num].ToString().PadRight(0x25);
                                    item = str5 + brand.PdStart + firstPedTip.ToString().PadLeft(2) + brand.PdEnd + str3;
                                    ApplicationFramesCheckedListBox.Items.Add(item);
                                    ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                    FrameTypes.Add(FrameType.PedStudGunTip);
                                    FrameDescriptions.Add(str3);
                                    FrameNumbers.Add(firstPedTip);
                                    SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstPedFrame);
                                    num14++;
                                    firstPedTip++;
                                }
                                else if (robotApps[num].ToString().Contains("Nut"))
                                {
                                    str3 = " @Ped Nut Gun " + num14.ToString();
                                    str5 = robotApps[num].ToString().PadRight(0x25);
                                    item = str5 + brand.PdStart + firstPedTip.ToString().PadLeft(2) + brand.PdEnd + str3;
                                    ApplicationFramesCheckedListBox.Items.Add(item);
                                    ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                    FrameTypes.Add(FrameType.PedStudGunTip);
                                    FrameDescriptions.Add(str3);
                                    FrameNumbers.Add(firstPedTip);
                                    SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstPedFrame);
                                    num14++;
                                    firstPedTip++;
                                }
                                else if (robotApps[num].ToString().Contains("Seal"))
                                {
                                    str3 = " @Ped Seal Gun " + num13.ToString();
                                    str5 = robotApps[num].ToString().PadRight(0x25);
                                    item = str5 + brand.PdStart + firstPedTip.ToString().PadLeft(2) + brand.PdEnd + str3;
                                    ApplicationFramesCheckedListBox.Items.Add(item);
                                    ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                    FrameTypes.Add(FrameType.PedSealGunTip);
                                    FrameDescriptions.Add(str3);
                                    FrameNumbers.Add(firstPedTip);
                                    SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstPedFrame);
                                    num13++;
                                    firstPedTip++;
                                }
                                else if (robotApps[num].ToString().Contains("Weld") || robotApps[num].ToString().Contains("W/Gun"))
                                {
                                    str3 = " @Ped Weld Gun " + num16.ToString();
                                    str5 = robotApps[num].ToString().PadRight(0x25);
                                    item = str5 + brand.PdStart + firstPedTip.ToString().PadLeft(2) + brand.PdEnd + str3;
                                    ApplicationFramesCheckedListBox.Items.Add(item);
                                    ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                    FrameTypes.Add(FrameType.PedSpotGunTip);
                                    FrameDescriptions.Add(str3);
                                    FrameNumbers.Add(firstPedTip);
                                    SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstPedFrame);
                                    num16++;
                                    firstPedTip++;
                                }
                            }
                            else if (robotApps[num].ToString().Contains("W/Gun"))
                            {
                                str3 = " @Weld Gun " + num12.ToString();
                                str5 = item;
                                item = str5 + brand.TdStart + firstCarriedSpot.ToString().PadLeft(2) + brand.TdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                FrameTypes.Add(FrameType.SpotGunTip);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(firstCarriedSpot);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstGunTip);
                                str3 = " @Weld fixutre Car 0";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFixtureCar0);
                                num9++;
                                for (num2 = 0; num2 < brand.MaxNumberRespot; num2++)
                                {
                                    str3 = " @Respot #" + ((num2 + 1)).ToString() + " Car 0";
                                    str5 = robotApps[num].ToString().PadRight(0x25);
                                    item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                    ApplicationFramesCheckedListBox.Items.Add(item);
                                    ToolNames.Add(new AppToolName());
                                    SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckRespotCar0);
                                    FrameTypes.Add(FrameType.FixtureCar0);
                                    FrameDescriptions.Add(str3);
                                    FrameNumbers.Add(num9);
                                    num9++;
                                }
                                firstCarriedSpot++;
                                num12++;
                            }
                            else if (robotApps[num].ToString().Contains("Mig"))
                            {
                                str3 = " @MIG Gun " + num15.ToString();
                                str5 = item;
                                item = str5 + brand.TdStart + firstCarriedMIG.ToString().PadLeft(2) + brand.TdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                FrameTypes.Add(FrameType.MigGunTip);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(firstCarriedMIG);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstGunTip);
                                str3 = " @MIG fixutre Car 0";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFixtureCar0);
                                num9++;
                                firstCarriedMIG++;
                                num15++;
                            }
                            else if (robotApps[num].ToString().Contains("Laser"))
                            {
                                str3 = " @Laser Gun " + num15.ToString();
                                str5 = item;
                                item = str5 + brand.TdStart + firstCarriedMIG.ToString().PadLeft(2) + brand.TdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                FrameTypes.Add(FrameType.LaserTip);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(firstCarriedMIG);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstGunTip);
                                str3 = " @Laser fixutre Car 0";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFixtureCar0);
                                num9++;
                                firstCarriedMIG++;
                                num15++;
                            }
                            else if (robotApps[num].ToString().Contains("Piercing"))
                            {
                                str3 = " @Pierce Gun " + num15.ToString();
                                str5 = item;
                                item = str5 + brand.TdStart + firstCarriedMIG.ToString().PadLeft(2) + brand.TdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                FrameTypes.Add(FrameType.PierceTip);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(firstCarriedMIG);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstGunTip);
                                str3 = " @Pierce fixutre Car 0";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFixtureCar0);
                                num9++;
                                firstCarriedMIG++;
                                num15++;
                            }
                            else if (robotApps[num].ToString().Contains("Seal"))
                            {
                                str3 = " @MIG Gun " + num13.ToString();
                                str5 = item;
                                item = str5 + brand.TdStart + firstCarriedSeal.ToString().PadLeft(2) + brand.TdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                FrameTypes.Add(FrameType.SealGunTip);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(firstCarriedSeal);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstGunTip);
                                str3 = " @MIG fixutre Car 0";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFixtureCar0);
                                num9++;
                                firstCarriedSeal++;
                                num13++;
                            }
                            else if (robotApps[num].ToString().Contains("Stud"))
                            {
                                str3 = " @Stud Gun " + num14.ToString();
                                str5 = item;
                                item = str5 + brand.TdStart + firstCarriedStud.ToString().PadLeft(2) + brand.TdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName(robotApps[num].ToString()));
                                FrameTypes.Add(FrameType.StudGunTip);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(firstCarriedStud);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstGunTip);
                                str3 = " @Stud fixutre Car 0";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFixtureCar0);
                                num9++;
                                firstCarriedStud++;
                                num14++;
                            }
                            else if (robotApps[num].ToString().Contains("Scriber"))
                            {
                                str3 = " Scriber Gun " + num14.ToString();
                                str5 = item;
                                item = str5 + brand.TdStart + firstCarriedStud.ToString().PadLeft(2) + brand.TdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.ScribeGun);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(firstCarriedStud);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstGunTip);
                                str3 = " Scriber fixutre Car 0";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFixtureCar0);
                                num9++;
                                firstCarriedStud++;
                                num14++;
                            }
                            else if (robotApps[num].ToString().Contains("Vision"))
                            {
                                str3 = " Carried Vision " + num14.ToString();
                                str5 = item;
                                item = str5 + brand.TdStart + firstCarriedStud.ToString().PadLeft(2) + brand.TdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.VisionTcp);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(firstCarriedStud);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFirstGunTip);
                                str3 = " Vision fixutre Car 0";
                                str5 = robotApps[num].ToString().PadRight(0x25);
                                item = str5 + brand.BdStart + num9.ToString().PadLeft(2) + brand.BdEnd + str3;
                                ApplicationFramesCheckedListBox.Items.Add(item);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(str3);
                                FrameNumbers.Add(num9);
                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, brand.CheckFixtureCar0);
                                num9++;
                                firstCarriedStud++;
                                num14++;
                            }
                        }
                        ApplicationFramesCheckedListBox.Sorted = false;
                        Initializing = false;
                        for (num = 0; num < ApplicationFramesCheckedListBox.Items.Count; num++)
                        {
                            RobotFrames.Add(ApplicationFramesCheckedListBox.Items[num]);
                            RobotFramesChecked.Add(ApplicationFramesCheckedListBox.CheckedIndices.Contains(num));
                        }
                    }
                    else
                    {
                        for (num = 0; num < RobotFramesChecked.Count; num++)
                        {
                            ApplicationFramesCheckedListBox.Items.Add(RobotFrames[num].ToString());
                            SetCheckedIndex(ref ApplicationFramesCheckedListBox, lastChecked, ApplicationFramesCheckedListBox.Items.Count - 1, RobotFramesChecked[num].Equals(true));
                        }
                        ApplicationFramesCheckedListBox.Sorted = false;
                    }
                }
            }
            else
            {
                RobotTypeLabel.Visible = false;
            }
        }

        private void ShowRobots()
        {
            ShowingRobots = true;
            var robotNames = new ArrayList();
            var robotMechanismNames = new ArrayList();
            if (UserPreferences.CurrentStyleSelected == -1)
            {
                SelectRobotLabel.Visible = false;
                RobotSelectListBox.Visible = false;
            }
            else
            {
                SelectRobotLabel.Visible = true;
                RobotSelectListBox.Visible = true;
                if (ExcelIO.BuildMatrixRobotList(ref robotNames, ref robotMechanismNames, StationSelectListBox.Items[UserPreferences.CurrentStyleSelected].ToString()))
                {
                    RobotSelectListBox.Items.Clear();
                    for (var i = 0; i < robotNames.Count; i++)
                    {
                        RobotSelectListBox.Items.Add(robotNames[i].ToString() + "(" + robotMechanismNames[i].ToString() + ")");
                    }
                    if (UserPreferences.CurrentRobotSelected != -1)
                    {
                        try
                        {
                            RobotSelectListBox.SelectedIndex = UserPreferences.CurrentRobotSelected;
                        }
                        catch (Exception)
                        {
                            RobotSelectListBox.SelectedIndex = UserPreferences.CurrentRobotSelected = -1;
                        }
                    }
                }
            }
            ShowingRobots = false;
        }

        private void ShowSystems()
        {
            var systemNames = new ArrayList();
            if (ExcelIO.BuildMatyrixSystemList(ref systemNames, ref SystemCodes))
            {
                StationSelectListBox.Items.Clear();
                for (var i = 0; i < systemNames.Count; i++)
                {
                    StationSelectListBox.Items.Add(systemNames[i].ToString());
                }
                if (UserPreferences.CurrentStyleSelected != -1)
                {
                    StationSelectListBox.SelectedIndex = UserPreferences.CurrentStyleSelected;
                }
                MakeFoldersButton.Visible = (((UserPreferences.WorkFolderName != null) && Directory.Exists(UserPreferences.WorkFolderName)) && Environment.UserName.Equals("aknasinski")) && (systemNames.Count > 0);
                CheckRobotsButton.Visible = (((UserPreferences.WorkFolderName != null) && Directory.Exists(UserPreferences.WorkFolderName)) && Environment.UserName.Equals("aknasinski")) && (systemNames.Count > 0);
            }
        }

        private void StationSelectListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserPreferences.CurrentStyleSelected = StationSelectListBox.SelectedIndex;
            if (StationSelectListBox.SelectedIndex == -1)
            {
                StationName = (string) (StationCode = null);
            }
            else
            {
                StationName = StationSelectListBox.Items[StationSelectListBox.SelectedIndex].ToString();
                StationCode = (SystemCodes[StationSelectListBox.SelectedIndex] != null) ? SystemCodes[StationSelectListBox.SelectedIndex].ToString().ToLower() : "";
            }
            RobotSelectListBox.Items.Clear();
            RobotName = (string) (RobotMechanismName = null);
            ApplicationFramesCheckedListBox.Items.Clear();
            RobotSelectListBox.SelectedIndex = -1;
            if (!Initializing)
            {
                RobotFrames = new ArrayList();
                RobotFramesChecked = new ArrayList();
            }
            UserPreferences.Write();
            ShowRobots();
        }

        private void WriteChecked()
        {
            if (WriteAcceptedChecksCheckBox.Checked)
            {
                var al = new ArrayList();
                al.Add("#Each line below contains the index of a checked item");
                for (var i = 0; i < ApplicationFramesCheckedListBox.CheckedIndices.Count; i++)
                {
                    al.Add(ApplicationFramesCheckedListBox.CheckedIndices[i]);
                }
                Utils.ArrayListToFile(Path.Combine(Path.Combine(UserPreferences.WorkFolderName, Path.Combine(StationName, RobotName)), "CheckedIndices.txt"), al, true);
            }
        }

      
    }
}

