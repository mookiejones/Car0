using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;


namespace Car0
{
    public partial class RobotData : Form
    {
        #region Private Variables
        private const int AppTypePadWidth = 37;

        private  OpenFileDialog FileBrowser = new OpenFileDialog();
        private  DialogResult FileReady = System.Windows.Forms.DialogResult.None;
        private  Boolean Initializing = false;
        private  Boolean ShowingRobots = false;

        private  List<AppToolName> ToolNames = new List<AppToolName>();
        private  ArrayList FrameDescriptions = new ArrayList();

        private  ArrayList SystemCodes = new ArrayList();

        private  Boolean ChecksChanged = false;

        #endregion
        #region Public Variables
        private object parent = null;
        internal  ArrayList RobotFrames = new ArrayList();
        internal  ArrayList RobotFramesChecked = new ArrayList();
        internal  List<FrameType> FrameTypes = new List<FrameType>();
        internal  List<int> FrameNumbers = new List<int>();

        internal  string StationName = null, RobotName = null, RobotMechanismName = null, StationCode = null, RobotStationName = null;

        internal enum FrameType { Undefined = -1, MigGunTip, NutGunTip, RivetGunTip, ScribeGun, SealGunTip, SpotGunTip, StudGunTip, GripPin, FixtureCar0, GripperCar0, PickTool, DropTool, PedRivetGunTip, PedSealGunTip, PedSpotGunTip, PedScribeGunTip, PedStudGunTip, LaserTip, PierceTip, VisionTcp };
        #endregion

        #region Private Methods
        private void ShowSystems()
        {
            int i;
            ArrayList SysNames = new ArrayList();

            if (ExcelIO.BuildMatyrixSystemList(ref SysNames, ref SystemCodes))
            {
                StationSelectListBox.Items.Clear();

                for (i = 0; i < SysNames.Count; ++i)
                    StationSelectListBox.Items.Add(SysNames[i].ToString());

                if (UserPreferences.CurrentStyleSelected != -1)
                    StationSelectListBox.SelectedIndex = UserPreferences.CurrentStyleSelected;
                
                MakeFoldersButton.Visible = ((UserPreferences.WorkFolderName != null) &&
                    Directory.Exists(UserPreferences.WorkFolderName) &&
                    Environment.UserName.Equals("aknasinski") &&
                    (SysNames.Count > 0));

                CheckRobotsButton.Visible = ((UserPreferences.WorkFolderName != null) &&
                    Directory.Exists(UserPreferences.WorkFolderName) &&
                    Environment.UserName.Equals("aknasinski") &&
                    (SysNames.Count > 0));

            }
        }

        private void ShowRobots()
        {
            ShowingRobots = true;
            int i;
            ArrayList RobotNames = new ArrayList(); ;
            ArrayList RobotMechanismNames = new ArrayList();

            if (UserPreferences.CurrentStyleSelected == -1)
            {
                SelectRobotLabel.Visible = false;
                RobotSelectListBox.Visible = false;
            }
            else
            {
                SelectRobotLabel.Visible = true;
                RobotSelectListBox.Visible = true;

                if (ExcelIO.BuildMatrixRobotList(ref RobotNames, ref RobotMechanismNames, StationSelectListBox.Items[UserPreferences.CurrentStyleSelected].ToString()))
                {
                    RobotSelectListBox.Items.Clear();

                    for (i = 0; i < RobotNames.Count; ++i)
                        RobotSelectListBox.Items.Add(RobotNames[i].ToString() + "(" + RobotMechanismNames[i].ToString() + ")");

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

        private  void SetCheckedIndex(ref CheckedListBox Box, ArrayList LastChecked, int Indx, Boolean DefaultValue)
        {
            if (Indx < Box.Items.Count)
            {
                if (LastChecked.Count.Equals(0))
                    Box.SetItemChecked(Indx, DefaultValue);
                else
                    Box.SetItemChecked(Indx, LastChecked.Contains(Indx.ToString()));
            }
        }

        private void ShowRobotDataLabels()
        {
            string RobotMechanismType = null;
            ArrayList MyRobApps = new ArrayList();            
            string mesbuf = null, Description = null;
            Brand MyBrand = new Brand(UserPreferences.RobotBrandSelected);

            if ((UserPreferences.CurrentStyleSelected != -1) && (UserPreferences.CurrentRobotSelected != -1))
            {
                string RobName = (RobotSelectListBox.Items[UserPreferences.CurrentRobotSelected].ToString().Contains("(")) ?
                    RobotSelectListBox.Items[UserPreferences.CurrentRobotSelected].ToString().Substring(0, RobotSelectListBox.Items[UserPreferences.CurrentRobotSelected].ToString().IndexOf("(")) :
                    RobotSelectListBox.Items[UserPreferences.CurrentRobotSelected].ToString();
                ArrayList LastCheckedIndices = Utils.FileToArrayList(Path.Combine(Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName)), "CheckedIndices.txt"));

                if (ExcelIO.GetRobotData(StationSelectListBox.Items[UserPreferences.CurrentStyleSelected].ToString(), RobName, ref RobotMechanismType, ref RobotStationName, ref MyRobApps))
                {
                    int NextCarriedSpot = MyBrand.FirstCarriedSpot, NextCarriedStud = MyBrand.FirstCarriedStud, NextCarriedSteal = MyBrand.FirstCarriedSeal, NextCarriedMig = MyBrand.FirstCarriedMIG;  //, NextCarriedLaser = 9;
                    int NextGripCar0 = MyBrand.FirstGripCar0, NextGripperPin = MyBrand.FirstGripPin;
                    int NextCar0 = MyBrand.FirstCar0, NextPedTip = MyBrand.FirstPedTip;
                    int GripCount = 1, WeldGunCount = 1, SealGunCount = 1, FastenerGunCount = 1, MigGunCount = 1, PedWeldGunCount = 1;
                    Boolean DefiningChecks = (RobotFramesChecked.Count == 0);

                    RobotTypeLabel.Text = RobotMechanismType;
                    RobotTypeLabel.Visible = true;

                    ApplicationFramesCheckedListBox.Items.Clear();
                    ApplicationFramesCheckedListBox.Sorted = false;

                    if (DefiningChecks)
                    {
                        RobotFrames = new ArrayList();
                        RobotFramesChecked = new ArrayList();
                        ToolNames = new List<AppToolName>();
                        FrameTypes = new List<FrameType>();
                        FrameNumbers = new List<int>();
                        FrameDescriptions = new ArrayList();
                        Initializing = true;

                        for (int i = 0; i < MyRobApps.Count; ++i)
                        {
                            mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                            if (MyRobApps[i].ToString().Contains("M/H"))
                            {
                                if (MyBrand.UseGripperCar0)
                                {
                                    Description = " @Grip " + GripCount.ToString() + " Car 0";
                                    mesbuf += MyBrand.Gc0Start + NextGripCar0.ToString().PadLeft(2) + MyBrand.Gc0End + Description;
                                    ApplicationFramesCheckedListBox.Items.Add(mesbuf);

                                    if (MyBrand.GripperCar0IsToolDef)
                                        ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                    else
                                        ToolNames.Add(new AppToolName());             //Null space keeper for Non TOOL

                                    FrameTypes.Add(FrameType.GripperCar0);
                                    FrameNumbers.Add(NextGripCar0);
                                    FrameDescriptions.Add(Description);

                                    SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckGripCar0);
                                    //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckGripCar0);
                                }

                                for (int j = 0; j < MyBrand.MaxNumberGripperPin; ++j)
                                {
                                    Description = " @Grip " + GripCount.ToString() + " Pin " + (j + 1).ToString();
                                    mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                    mesbuf += MyBrand.TdStart + NextGripperPin.ToString().PadLeft(2) + MyBrand.TdEnd + Description;
                                    ApplicationFramesCheckedListBox.Items.Add(mesbuf);

                                    SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, j.Equals(0) & MyBrand.CheckFirstGripPin);
                                    //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, j.Equals(0) & MyBrand.CheckFirstGripPin);
                                    ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                    FrameTypes.Add(FrameType.GripPin);
                                    FrameNumbers.Add(NextGripperPin);
                                    FrameDescriptions.Add(Description);
                                    ++NextGripperPin;
                                }

                                Description = " @Pick 1 location";
                                mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());             //Null space keeper for Non TOOL

                                FrameTypes.Add(FrameType.PickTool);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPickFrame);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPickFrame);

                                ++NextCar0;

                                Description = " @Place 1 location";
                                mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());             //Null space keeper for Non TOOL
                                FrameTypes.Add(FrameType.DropTool);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstDropFrame);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstDropFrame);

                                ++NextCar0;

                                Description = " @Pick 2 location";
                                mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());             //Null space keeper for Non TOOL
                                FrameTypes.Add(FrameType.PickTool);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckSubsequentPickFrame);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckSubsequentPickFrame);

                                ++NextCar0;

                                Description = " @Place 2 location";
                                mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());             //Null space keeper for Non TOOL
                                FrameTypes.Add(FrameType.DropTool);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckSubsequentDropFrame);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckSubsequentDropFrame);

                                ++NextCar0;

                                ++GripCount;
                                ++NextGripCar0;
                            }
                            else if (MyRobApps[i].ToString().Contains("Ped"))
                            {
                                if (MyRobApps[i].ToString().Contains("Dual"))
                                {
                                    if (MyRobApps[i].ToString().Contains("Seal"))
                                    {
                                        Description = " @Dual Ped Seal Tip 1 ";
                                        mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                        mesbuf += MyBrand.PdStart + NextPedTip.ToString().PadLeft(2) + MyBrand.PdEnd + Description;
                                        ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                        ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                        FrameTypes.Add(FrameType.SealGunTip);
                                        FrameDescriptions.Add(Description);
                                        FrameNumbers.Add(NextPedTip);

                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);
                                        //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);

                                        ++NextPedTip;

                                        Description = " @Dual Ped Seal Tip 2 ";
                                        mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                        mesbuf += MyBrand.PdStart + NextPedTip.ToString().PadLeft(2) + MyBrand.PdEnd + Description;
                                        ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                        ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                        FrameTypes.Add(FrameType.SealGunTip);
                                        FrameDescriptions.Add(Description);
                                        FrameNumbers.Add(NextPedTip);

                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);
                                        //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);

                                        ++NextPedTip;
                                    }
                                    else
                                    {
                                        Description = " @Dual Ped Weld Gun Tip 1 ";
                                        mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                        mesbuf += MyBrand.PdStart + NextPedTip.ToString().PadLeft(2) + MyBrand.PdEnd + Description;
                                        ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                        ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                        FrameTypes.Add(FrameType.PedSpotGunTip);
                                        FrameDescriptions.Add(Description);
                                        FrameNumbers.Add(NextPedTip);

                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);
                                        //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);

                                        ++NextPedTip;

                                        Description = " @Dual Ped Weld Gun Tip 2 ";
                                        mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                        mesbuf += MyBrand.PdStart + NextPedTip.ToString().PadLeft(2) + MyBrand.PdEnd + Description;
                                        ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                        ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                        FrameTypes.Add(FrameType.PedSpotGunTip);
                                        FrameDescriptions.Add(Description);
                                        FrameNumbers.Add(NextPedTip);

                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);
                                        //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);

                                        ++NextPedTip;
                                    }
                                }
                                else
                                {
                                    if (MyRobApps[i].ToString().Contains("Stud"))
                                    {
                                        Description = " @Ped Stud Gun " + FastenerGunCount.ToString();
                                        mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                        mesbuf += MyBrand.PdStart + NextPedTip.ToString().PadLeft(2) + MyBrand.PdEnd + Description;
                                        ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                        ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                        FrameTypes.Add(FrameType.PedStudGunTip);
                                        FrameDescriptions.Add(Description);
                                        FrameNumbers.Add(NextPedTip);

                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);
                                        //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);

                                        ++FastenerGunCount;
                                        ++NextPedTip;
                                    }
                                    else if (MyRobApps[i].ToString().Contains("Nut"))
                                    {
                                        Description = " @Ped Nut Gun " + FastenerGunCount.ToString();
                                        mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                        mesbuf += MyBrand.PdStart + NextPedTip.ToString().PadLeft(2) + MyBrand.PdEnd + Description;
                                        ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                        ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                        FrameTypes.Add(FrameType.PedStudGunTip);
                                        FrameDescriptions.Add(Description);
                                        FrameNumbers.Add(NextPedTip);

                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);
                                        //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);

                                        ++FastenerGunCount;
                                        ++NextPedTip;
                                    }
                                    else if (MyRobApps[i].ToString().Contains("Seal"))
                                    {
                                        Description = " @Ped Seal Gun " + SealGunCount.ToString();
                                        mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                        mesbuf += MyBrand.PdStart + NextPedTip.ToString().PadLeft(2) + MyBrand.PdEnd + Description;
                                        ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                        ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                        FrameTypes.Add(FrameType.PedSealGunTip);
                                        FrameDescriptions.Add(Description);
                                        FrameNumbers.Add(NextPedTip);

                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);
                                        //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);

                                        ++SealGunCount;
                                        ++NextPedTip;
                                    }
                                    else if (MyRobApps[i].ToString().Contains("Weld") || MyRobApps[i].ToString().Contains("W/Gun"))  //Designed to get air and servo
                                    {
                                        Description = " @Ped Weld Gun " + PedWeldGunCount.ToString();
                                        mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                        mesbuf += MyBrand.PdStart + NextPedTip.ToString().PadLeft(2) + MyBrand.PdEnd + Description;
                                        ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                        ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                        FrameTypes.Add(FrameType.PedSpotGunTip);
                                        FrameDescriptions.Add(Description);
                                        FrameNumbers.Add(NextPedTip);

                                        SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);
                                        //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstPedFrame);

                                        ++PedWeldGunCount;
                                        ++NextPedTip;
                                    }
                                }
                            }
                            else if (MyRobApps[i].ToString().Contains("W/Gun")) //Spot - don't care if Air or Servo
                            {
                                Description = " @Weld Gun " + WeldGunCount.ToString();
                                mesbuf += MyBrand.TdStart + NextCarriedSpot.ToString().PadLeft(2) + MyBrand.TdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                FrameTypes.Add(FrameType.SpotGunTip);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCarriedSpot);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);

                                Description = " @Weld fixutre Car 0";
                                mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);

                                ++NextCar0;

                                for (int j = 0; j < MyBrand.MaxNumberRespot; ++j)
                                {
                                    Description = " @Respot #" + (j + 1).ToString() + " Car 0";
                                    mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                    mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                    ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                    ToolNames.Add(new AppToolName());

                                    SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckRespotCar0);
                                    //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckRespotCar0);
                                    FrameTypes.Add(FrameType.FixtureCar0);
                                    FrameDescriptions.Add(Description);
                                    FrameNumbers.Add(NextCar0);

                                    ++NextCar0;
                                }

                                ++NextCarriedSpot;
                                ++WeldGunCount;
                            }
                            else if (MyRobApps[i].ToString().Contains("Mig"))
                            {
                                Description = " @MIG Gun " + MigGunCount.ToString();
                                mesbuf += MyBrand.TdStart + NextCarriedMig.ToString().PadLeft(2) + MyBrand.TdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                FrameTypes.Add(FrameType.MigGunTip);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCarriedMig);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);

                                Description = " @MIG fixutre Car 0";
                                mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);

                                ++NextCar0;

                                ++NextCarriedMig;
                                ++MigGunCount;
                            }
                            else if (MyRobApps[i].ToString().Contains("Laser"))
                            {
                                Description = " @Laser Gun " + MigGunCount.ToString();
                                mesbuf += MyBrand.TdStart + NextCarriedMig.ToString().PadLeft(2) + MyBrand.TdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                FrameTypes.Add(FrameType.LaserTip);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCarriedMig);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);

                                Description = " @Laser fixutre Car 0";
                                mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);

                                ++NextCar0;

                                ++NextCarriedMig;
                                ++MigGunCount;
                            }
                            else if (MyRobApps[i].ToString().Contains("Piercing"))
                            {
                                Description = " @Pierce Gun " + MigGunCount.ToString();
                                mesbuf += MyBrand.TdStart + NextCarriedMig.ToString().PadLeft(2) + MyBrand.TdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                FrameTypes.Add(FrameType.PierceTip);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCarriedMig);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);

                                Description = " @Pierce fixutre Car 0";
                                mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);

                                ++NextCar0;

                                ++NextCarriedMig;
                                ++MigGunCount;
                            }
                            else if (MyRobApps[i].ToString().Contains("Seal"))
                            {
                                Description = " @MIG Gun " + SealGunCount.ToString();
                                mesbuf += MyBrand.TdStart + NextCarriedSteal.ToString().PadLeft(2) + MyBrand.TdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                FrameTypes.Add(FrameType.SealGunTip);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCarriedSteal);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);

                                Description = " @MIG fixutre Car 0";
                                mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);

                                ++NextCar0;

                                ++NextCarriedSteal;
                                ++SealGunCount;
                            }
                            else if (MyRobApps[i].ToString().Contains("Stud"))
                            {
                                Description = " @Stud Gun " + FastenerGunCount.ToString();
                                mesbuf += MyBrand.TdStart + NextCarriedStud.ToString().PadLeft(2) + MyBrand.TdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName(MyRobApps[i].ToString()));
                                FrameTypes.Add(FrameType.StudGunTip);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCarriedStud);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);

                                Description = " @Stud fixutre Car 0";
                                mesbuf = MyRobApps[i].ToString().PadRight(AppTypePadWidth);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                ++NextCar0;

                                ++NextCarriedStud;
                                ++FastenerGunCount;
                            }
                            else if (MyRobApps[i].ToString().Contains("Scriber"))
                            {
                                Description = " Scriber Gun " + FastenerGunCount.ToString();
                                mesbuf += MyBrand.TdStart + NextCarriedStud.ToString().PadLeft(2) + MyBrand.TdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.ScribeGun);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCarriedStud);      //TODO: Make NextCarriedScriber

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);

                                Description = " Scriber fixutre Car 0";
                                mesbuf = MyRobApps[i].ToString().PadRight(37);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                ++NextCar0;

                                ++NextCarriedStud;
                                ++FastenerGunCount;
                            }
                            else if (MyRobApps[i].ToString().Contains("Vision"))
                            {
                                Description = " Carried Vision " + FastenerGunCount.ToString();
                                mesbuf += MyBrand.TdStart + NextCarriedStud.ToString().PadLeft(2) + MyBrand.TdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.VisionTcp);            //TODO: Maxe a FirstCarriedVision
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCarriedStud);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFirstGunTip);

                                Description = " Vision fixutre Car 0";
                                mesbuf = MyRobApps[i].ToString().PadRight(37);
                                mesbuf += MyBrand.BdStart + NextCar0.ToString().PadLeft(2) + MyBrand.BdEnd + Description;
                                ApplicationFramesCheckedListBox.Items.Add(mesbuf);
                                ToolNames.Add(new AppToolName());
                                FrameTypes.Add(FrameType.FixtureCar0);
                                FrameDescriptions.Add(Description);
                                FrameNumbers.Add(NextCar0);

                                SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, MyBrand.CheckFixtureCar0);
                                ++NextCar0;

                                ++NextCarriedStud;
                                ++FastenerGunCount;
                            }
                        }

                        ApplicationFramesCheckedListBox.Sorted = false;     //Sorting gets everything out of sync!!!!!!!!!!!!!!!!!!!!!
                        Initializing = false;

                        for (int i = 0; i < ApplicationFramesCheckedListBox.Items.Count; ++i)
                        {
                            RobotFrames.Add(ApplicationFramesCheckedListBox.Items[i]);
                            RobotFramesChecked.Add(ApplicationFramesCheckedListBox.CheckedIndices.Contains(i));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < RobotFramesChecked.Count; ++i)
                        {
                            ApplicationFramesCheckedListBox.Items.Add(RobotFrames[i].ToString());
                            SetCheckedIndex(ref ApplicationFramesCheckedListBox, LastCheckedIndices, ApplicationFramesCheckedListBox.Items.Count - 1, RobotFramesChecked[i].Equals(true));                                    
                            //ApplicationFramesCheckedListBox.SetItemChecked(ApplicationFramesCheckedListBox.Items.Count - 1, RobotFramesChecked[i].Equals(true));
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
        #endregion
        #region Public Methods
        #endregion

        public RobotData(object parent)
        {
            InitializeComponent();
            RobotMatrixTextBox.Text = UserPreferences.RobotMatrixFileName;
            BadApplicationsRichTextBox.Visible = BadAppLabel.Visible = false;

            Initializing = true;
            ShowSystems();
            Initializing = false;

            this.parent = parent;
        }

        public RobotData()
        {
            InitializeComponent();
            RobotMatrixTextBox.Text = UserPreferences.RobotMatrixFileName;
            BadApplicationsRichTextBox.Visible = BadAppLabel.Visible = false;

            Initializing = true;
            ShowSystems();
            Initializing = false;
        }

        private void WriteChecked()
        {
            if (WriteAcceptedChecksCheckBox.Checked)
            {
                int i;
                ArrayList MyCheckedIndices = new ArrayList();

                MyCheckedIndices.Add("#Each line below contains the index of a checked item");
                                
                for (i = 0; i < ApplicationFramesCheckedListBox.CheckedIndices.Count; ++i)
                    MyCheckedIndices.Add(ApplicationFramesCheckedListBox.CheckedIndices[i]);

                string Fpath = Path.Combine(Path.Combine(UserPreferences.WorkFolderName, Path.Combine(RobotData.StationName, RobotData.RobotName)), "CheckedIndices.txt");

                Utils.ArrayListToFile(Fpath, MyCheckedIndices, true);
            }
        }

        private void RobotData_FormClosing(object sender, FormClosingEventArgs e)
        {
            KUKAcar0.MyRobotData = null;

            if (ChecksChanged && MessageBox.Show("Save changes in checked frame selections", "FRAME SELECTIONS HAVE CHANGED!!!", MessageBoxButtons.YesNo).Equals(DialogResult.Yes))
                WriteChecked();

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

            RobotName = StationName = RobotMechanismName = null;

            if (FileReady == System.Windows.Forms.DialogResult.OK)
            {
                UserPreferences.RobotMatrixFileName = RobotMatrixTextBox.Text = FileBrowser.FileName;
                UserPreferences.CurrentStyleSelected = -1;
                ShowSystems();

                UserPreferences.Write();
            }
        }

        private void StationSelectListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserPreferences.CurrentStyleSelected = StationSelectListBox.SelectedIndex;

            if (StationSelectListBox.SelectedIndex == -1)
            {
                StationName = StationCode = null;
            }
            else
            {
                StationName = StationSelectListBox.Items[StationSelectListBox.SelectedIndex].ToString();
                StationCode = (SystemCodes[StationSelectListBox.SelectedIndex] != null) ? SystemCodes[StationSelectListBox.SelectedIndex].ToString().ToLower() : "";
            }
            //RobotSelectListBox.SelectedIndex = -1;
            RobotSelectListBox.Items.Clear();
            RobotName = RobotMechanismName = null;
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
            btnOK.Enabled = true;
        }

        private void MakeFoldersButton_Click(object sender, EventArgs e)
        {
            int i,j;
            ArrayList RobotNames = new ArrayList();
            ArrayList RobotMechanismNames = new ArrayList();
            string FolderName = null;
            string BlankExcelName = Path.Combine(UserPreferences.WorkFolderName, "TrackerFile.xls");
            Boolean BlankExcelExists = File.Exists(BlankExcelName);

            for (i = 0; i < StationSelectListBox.Items.Count; ++i)
            {
                if (ExcelIO.BuildMatrixRobotList(ref RobotNames, ref RobotMechanismNames, StationSelectListBox.Items[i].ToString()))
                {
                    for (j = 0; j < RobotNames.Count; ++j)
                    {
                        FolderName = Path.Combine(UserPreferences.WorkFolderName, Path.Combine(StationSelectListBox.Items[i].ToString(), RobotNames[j].ToString()));

                        Directory.CreateDirectory(FolderName);

                        if (BlankExcelExists)
                            File.Copy(BlankExcelName, Path.Combine(FolderName, "TrackerFile.xls"));
                    }
                }
            }
        }

        private void ApplicationFramesCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int i;

            if (ChecksChanged = !Initializing)
            {
                RobotFramesChecked = new ArrayList();

                for (i = 0; i < ApplicationFramesCheckedListBox.Items.Count; ++i)
                {
                    if (e.Index.Equals(i))
                        RobotFramesChecked.Add(e.NewValue.ToString().Equals("Checked"));
                    else
                    {
                        if (ApplicationFramesCheckedListBox.CheckedIndices.Contains(i))
                            RobotFramesChecked.Add(true);
                        else
                            RobotFramesChecked.Add(false);
                    }
                }
            }
        }

        private void RobotData_Deactivate(object sender, EventArgs e)
        {
            if (!ShowingRobots)
               ExcelIO.CloseMatrix();
        }

        private void CheckRobotsButton_Click(object sender, EventArgs e)
        {
            ArrayList SysNames = new ArrayList();
            ArrayList BadApps = new ArrayList();

            if (ExcelIO.BuildMatyrixSystemList(ref SysNames, ref SystemCodes))
            {
                int i = 0;
                CheckProgressBar.Value = 0;
                CheckProgressBar.Refresh();

                foreach (string System in SysNames)
                {
                    ArrayList RobotNames = new ArrayList();
                    ArrayList RobotMechanismNames = new ArrayList();
                    string RobotMechanismType = null;
                    ArrayList MyRobApps = new ArrayList();                    

                    if (ExcelIO.BuildMatrixRobotList(ref RobotNames, ref RobotMechanismNames, System))
                    {
                        foreach (string RobName in RobotNames)
                        {
                            if (ExcelIO.GetRobotData(System, RobName, ref RobotMechanismType, ref RobotStationName, ref MyRobApps))
                            {
                                foreach (string Application in MyRobApps)
                                {
                                    Boolean GoodApp = false;

                                    if (Application.Contains("M/H"))
                                        GoodApp = true;
                                    else if (Application.Contains("Ped"))
                                    {
                                        if (Application.Contains("Dual"))
                                        {
                                            if (Application.Contains("Seal"))
                                                GoodApp = true;
                                            else if (Application.Contains("Weld") || Application.Contains("W/Gun"))
                                                GoodApp = true;
                                        }
                                        else
                                        {
                                            if (Application.Contains("Stud"))
                                                GoodApp = true;
                                            else if (Application.Contains("Nut"))
                                                GoodApp = true;
                                            else if (Application.Contains("Seal"))
                                                GoodApp = true;
                                            else if (Application.Contains("Weld") || Application.Contains("W/Gun"))
                                                GoodApp = true;
                                        }
                                    }
                                    else if (Application.Contains("W/Gun"))
                                        GoodApp = true;
                                    else if (Application.Contains("Mig"))
                                        GoodApp = true;
                                    else if (Application.Contains("Seal"))
                                        GoodApp = true;
                                    else if (Application.Contains("Stud"))
                                        GoodApp = true;
                                    else if (Application.Contains("Scriber"))
                                        GoodApp = true;
                                    else if (Application.Contains("Vision"))
                                        GoodApp = true;
                                    else if (Application.Contains("Laser Cutting"))
                                        GoodApp = true;
                                    else if (Application.Contains("Piercing"))
                                        GoodApp = true;

                                    if (!GoodApp)
                                        BadApps.Add(System + " " + RobName + " Application = '" + Application + "'");
                                }

                            } //THere
                        }
                    }
                    
                    ++i;
                    CheckProgressBar.Value = 100 * i / SysNames.Count;
                    CheckProgressBar.Refresh();
                }

                if (BadApps.Count > 0)
                {
                    BadApplicationsRichTextBox.Visible = BadAppLabel.Visible = true;
                    BadApplicationsRichTextBox.Text = null;

                    foreach (string bogus in BadApps)
                        BadApplicationsRichTextBox.Text += (bogus + "\n");
                }
                else
                {
                    this.MaximumSize = new System.Drawing.Size(615, 350);
                    this.Size = new System.Drawing.Size(615, 350);
                }
            }
        }
    }
}
