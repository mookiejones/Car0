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
    public class Brand
    {
        public event NotifyMessageEventHandler NotifyMessage;
        private void raiseNotify(string message, string title)
        {
            if (NotifyMessage!=null)
                NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
        }

        #region Public Variables
        public  Boolean ReadConfiguration = false;

        public enum RobotBrands { Undefined = -1, ABB = 0, Fanuc = 1, FanucRJ = 2, KUKA = 3, Nachi = 4 };
        public RobotBrands Company;
        public int Index, FirstCarriedSpot, FirstCarriedStud, FirstCarriedSeal, FirstCarriedMIG, FirstCar0, FirstPedTip, FirstGripCar0, FirstGripPin, MaxNumberRespot, MaxNumberGripperPin, FirstPedFrameNumber, SecondPedFrameNumber, ThirdPedFrameNumber, RobotNameNumberSubstringStart;
        public string BdStart, BdEnd, TdStart, TdEnd, Gc0Start, Gc0End, PdStart, PdEnd;

        public string SpotTcpNameFormat, GripTcpNameFormat, StudTcpNameFormat, NutTcpNameFormat, MigTcpNameFormat, RivetTcpNameFormat, SealTcpNameFormat, ScribeTcpNameFormat, VisionTcpNameFormat;
        public string PedSpotTcpNameFormat, PedStudTcpNameFormat, PedRivetTcpNameFormat, PedSealTcpNameFormat, PedScribeTcpNameFormat, FixtureCar0NameFormat, GripperCar0NameFormat;

        public Boolean CheckGripCar0, CheckFirstGripPin, CheckFirstPickFrame, CheckFirstDropFrame, CheckSubsequentPickFrame, CheckSubsequentDropFrame, CheckFirstPedFrame, CheckFirstGunTip, CheckFixtureCar0, CheckRespotCar0, UseGripperCar0, GripperCar0IsToolDef;
        #endregion
        #region Private Variables

        private  string[] BaseDataStartStrings = { "WOBJ__", "UFRAME ", "UFRAME ", "BASE_DATA[", "BASE[" };
        private  string[] BaseDataEndStrings = { "", "", "", "]", "]" };
        private  string[] ToolDataStartStrings = { "TDATA_", "UTOOL  ", "UTOOL  ", "TOOL_DATA[", "TOOL[" };
        private  string[] ToolDataEndStrings = { "", "", "", "]", "]" };
        private  string[] PedDataStartStrings = { "TDATA_", "UFRAME ", "UFRAME ", "BASE_DATA[", "TOOL[" };            //Sometimes pedestal is and is not a base
        private  string[] PedDataEndStrings = { "", "", "", "]", "]" };

        private  string[] GripperCar0StartStrings = { "WOBJ__", "UTOOL  ", "UTOOL  ", "TOOL_DATA[", "TOOL[" };
        private  string[] GripperCar0EndStrings = { "", "", "", "]", "]" };

        private  string[] SpotTcpNameFormats = { "<Robot>_tGun<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        private  string[] GripTcpNameFormats = { "<Robot>_tGrip<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        private  string[] StudTcpNameFormats = { "<Robot>_tStud<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        private  string[] NutTcpNameFormats = { "<Robot>_tNdrive<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        private  string[] MigTcpNameFormats = { "<Robot>_tMig<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        private  string[] RivetTcpNameFormats = { "<Robot>_tRiv<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        private  string[] SealTcpNameFormats = { "<Robot>_tGlue<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        private  string[] ScribeTcpNameFormats = { "<Robot>_tScribe<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        private  string[] VisionTcpNameFormats = { "<Robot>_tVision<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };

        private  string[] PedSpotTcpNameFormats = { "<Robot>_ST_tGun<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_tl<Num>" };
        private  string[] PedStudTcpNameFormats = { "<Robot>_ST_tStud<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_tl<Num>" };
        private  string[] PedRivetTcpNameFormats = { "<Robot>_ST_Riv<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_tl<Num>" };
        private  string[] PedSealTcpNameFormats = { "<Robot>_ST_Glue<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_tl<Num>" };
        private  string[] PedScribeTcpNameFormats = { "<Robot>_ST_tScribe<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_tl<Num>" };

        private  string[] FixtureCar0NameFormats = { "<Robot>_Wobj_<Station>_fx<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>" };
        private  string[] GripperCar0NameFormats = { "<Robot>_Wobj_<Station>_gp<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>" };

        private  int[] FirstCarriedSpots = { 1, 1, 1, 1, 1 };
        private  int[] FirstCarriedStuds = { 1, 1, 1, 4, 1 };
        private  int[] FirstCarriedSeals = { 1, 1, 1, 6, 1 };
        private  int[] FirstCarriedMIGs = { 1, 1, 1, 8, 1 };
        private  int[] FirstCar0s = { 1, 3, 3, 1, 1 };
        private  int[] FirstPedTips = { 1, 1, 1, 13, 20 };
        private  int[] FirstGripCar0s = { 1, 1, 1, 11, 30 };
        private  int[] FirstGripPins = { 1, 1, 1, 14, 10 };
        private  int[] MaxNumberRespots = { 1, 1, 1, 1, 3 };
        private  int[] MaxNumberGripperPins = { 5, 4, 4, 1, 5 };
        private  int[] FirstPedFrameNumbers = { 1, 1, 1, 13, 20 };
        private  int[] SecondPedFrameNumbers = { 2, 1, 1, 14, 21 };
        private  int[] ThirdPedFrameNumbers = { 3, 1, 1, 15, 22 };

        private  int[] RobotNameNumberSubstringStarts = { 5, 1, 1, 1, 1 };

        //Flags below are used to define default states for RobotData check boxes
        private  Boolean[] CheckGripCar0s = { false, false, false, true, false };
        private  Boolean[] CheckFirstGripPins = { true, true, true, true, true };
        private  Boolean[] CheckFirstPickFrames = { false, false, false, true, false };
        private  Boolean[] CheckFirstDropFrames = { false, false, false, true, false };
        private  Boolean[] CheckSubsequentPickFrames = { false, false, false, false, false };
        private  Boolean[] CheckSubsequentDropFrames = { false, false, false, false, false };
        private  Boolean[] CheckFirstPedFrames = { true, true, true, true, true };
        private  Boolean[] CheckFirstGunTips = { true, true, true, true, true };
        private  Boolean[] CheckFixtureCar0s = { true, true, true, true, true };
        private  Boolean[] CheckRespotCar0s = { false, false, false, false, false };
        private  Boolean[] UseGripperCar0s = { false, false, false, true, false };
        private  Boolean[] GripperCar0IsToolDefs = { true, false, false, false, false };

        #endregion
        #region Private Methods
      
        private  void ReadCar0Configuration()
        {
            if (!ReadConfiguration && (UserPreferences.WorkFolderName != null))
            {
                try
                {
                    string Fname = Path.Combine(UserPreferences.WorkFolderName, "Car0 Configuration.csv");

                    if (File.Exists(Fname))
                    {
                        ArrayList Data = Utils.FileToArrayList(Fname);
                        int i, j;

                        for (i = 1; i < Data.Count; ++i)
                        {
                            string[] items = System.Text.RegularExpressions.Regex.Split(Data[i].ToString(), ",");

                            //Use only lines fully defined
                            if (items.Length.Equals(6))
                            {
                                //Branch based on variable
                                switch (items[0].ToString())
                                {
                                    case "BaseDataStartStrings":
                                        for (j = 1; j < items.Length; ++j)
                                            BaseDataStartStrings[j - 1] = items[j];
                                        break;
                                    case "BaseDataEndStrings":
                                        for (j = 1; j < items.Length; ++j)
                                            BaseDataEndStrings[j - 1] = items[j];
                                        break;
                                    case "ToolDataStartStrings":
                                        for (j = 1; j < items.Length; ++j)
                                            ToolDataStartStrings[j - 1] = items[j];
                                        break;
                                    case "ToolDataEndStrings":
                                        for (j = 1; j < items.Length; ++j)
                                            ToolDataEndStrings[j - 1] = items[j];
                                        break;
                                    case "PedDataStartStrings":
                                        for (j = 1; j < items.Length; ++j)
                                            PedDataStartStrings[j - 1] = items[j];
                                        break;
                                    case "PedDataEndStrings":
                                        for (j = 1; j < items.Length; ++j)
                                            PedDataEndStrings[j - 1] = items[j];
                                        break;
                                    case "GripperCar0StartStrings":
                                        for (j = 1; j < items.Length; ++j)
                                            GripperCar0StartStrings[j - 1] = items[j];
                                        break;
                                    case "GripperCar0EndStrings":
                                        for (j = 1; j < items.Length; ++j)
                                            GripperCar0EndStrings[j - 1] = items[j];
                                        break;
                                    case "SpotTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            SpotTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "GripTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            GripTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "StudTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            StudTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "NutTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            NutTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "MigTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            MigTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "RivetTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            RivetTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "SealTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            SealTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "ScribeTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            ScribeTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "VisionTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            VisionTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "PedSpotTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            PedSpotTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "PedStudTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            PedStudTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "PedRivetTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            PedRivetTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "PedSealTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            PedSealTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "PedScribeTcpNameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            PedScribeTcpNameFormats[j - 1] = items[j];
                                        break;
                                    case "FixtureCar0NameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            FixtureCar0NameFormats[j - 1] = items[j];
                                        break;
                                    case "GripperCar0NameFormats":
                                        for (j = 1; j < items.Length; ++j)
                                            GripperCar0NameFormats[j - 1] = items[j];
                                        break;
                                    case "FirstCarriedSpots":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                FirstCarriedSpots[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "FirstCarriedStuds":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                FirstCarriedStuds[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "FirstCarriedSeals":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                FirstCarriedSeals[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "FirstCarriedMIGs":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                FirstCarriedMIGs[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "FirstCar0s":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                FirstCar0s[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "FirstPedTips":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                FirstPedTips[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "FirstGripCar0s":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                FirstGripCar0s[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "FirstGripPins":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                FirstGripPins[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "MaxNumberRespots":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                MaxNumberRespots[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "MaxNumberGripperPins":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                MaxNumberGripperPins[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "FirstPedFrameNumbers":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                FirstPedFrameNumbers[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "SecondPedFrameNumbers":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                SecondPedFrameNumbers[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "ThirdPedFrameNumbers":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                ThirdPedFrameNumbers[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "RobotNameNumberSubstringStarts":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                RobotNameNumberSubstringStarts[j - 1] = Convert.ToInt32(items[j]);
                                        }
                                        break;
                                    case "CheckGripCar0s":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                CheckGripCar0s[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "CheckFirstGripPins":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                CheckFirstGripPins[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "CheckFirstPickFrames":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                CheckFirstPickFrames[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "CheckFirstDropFrames":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                CheckFirstDropFrames[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "CheckSubsequentPickFrames":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                CheckSubsequentPickFrames[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "CheckSubsequentDropFrames":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                CheckSubsequentDropFrames[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "CheckFirstPedFrames":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                CheckFirstPedFrames[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "CheckFirstGunTips":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                CheckFirstGunTips[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "CheckFixtureCar0s":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                CheckFixtureCar0s[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "CheckRespotCar0s":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                CheckRespotCar0s[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "UseGripperCar0s":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                UseGripperCar0s[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                    case "GripperCar0IsToolDefs":
                                        for (j = 1; j < items.Length; ++j)
                                        {
                                            if (items[j] != null)
                                                GripperCar0IsToolDefs[j - 1] = (items[j].Equals("true"));
                                        }
                                        break;
                                }
                            }
                        }

                        ReadConfiguration = true;
                    }
                }
                catch (Exception ee)
                {
                    raiseNotify(ee.Message, "ReadCar0Configuration --> TELL AL!!!!");
                }

            }
        }
        #endregion
        #region Public Methods
        public Brand()
        {
            ReadCar0Configuration();
            Company = RobotBrands.Undefined;
            RobotNameNumberSubstringStart = ThirdPedFrameNumber = SecondPedFrameNumber = FirstPedFrameNumber = MaxNumberGripperPin = MaxNumberRespot = FirstGripPin = FirstGripCar0 = FirstPedTip = FirstCar0 = FirstCarriedMIG = FirstCarriedSeal = FirstCarriedStud = FirstCarriedSpot = Index = -1;
            PdStart = PdEnd = BdStart = BdEnd = TdStart = TdEnd = Gc0Start = Gc0End = "";

            SpotTcpNameFormat = GripTcpNameFormat = StudTcpNameFormat = NutTcpNameFormat = MigTcpNameFormat = RivetTcpNameFormat = SealTcpNameFormat = ScribeTcpNameFormat = VisionTcpNameFormat = "";
            PedSpotTcpNameFormat = PedStudTcpNameFormat = PedRivetTcpNameFormat = PedSealTcpNameFormat = PedScribeTcpNameFormat = FixtureCar0NameFormat = GripperCar0NameFormat = "";

            CheckGripCar0 = CheckFirstGripPin = CheckFirstPickFrame = CheckFirstDropFrame = CheckSubsequentPickFrame = CheckSubsequentDropFrame = CheckFirstPedFrame = CheckFirstGunTip = CheckRespotCar0 = UseGripperCar0 = GripperCar0IsToolDef = false;
        }
        public Brand(int t)
        {
            ReadCar0Configuration();

            Company = (RobotBrands)t;
            Index = t;
            BdStart = BaseDataStartStrings[t];
            BdEnd = BaseDataEndStrings[t];
            TdStart = ToolDataStartStrings[t];
            TdEnd = ToolDataEndStrings[t];
            Gc0Start = GripperCar0StartStrings[t];
            Gc0End = GripperCar0EndStrings[t];

            PdStart = PedDataStartStrings[t];
            PdEnd = PedDataEndStrings[t];

            FirstCarriedSpot = FirstCarriedSpots[t];
            FirstCarriedStud = FirstCarriedStuds[t];
            FirstCarriedSeal = FirstCarriedSeals[t];
            FirstCarriedMIG = FirstCarriedMIGs[t];
            FirstCar0 = FirstCar0s[t];
            FirstPedTip = FirstPedTips[t];
            FirstGripCar0 = FirstGripCar0s[t];
            FirstGripPin = FirstGripPins[t];
            MaxNumberRespot = MaxNumberRespots[t];
            MaxNumberGripperPin = MaxNumberGripperPins[t];
            CheckGripCar0 = CheckGripCar0s[t];
            CheckFirstGripPin = CheckFirstGripPins[t];
            CheckFirstPickFrame = CheckFirstPickFrames[t];
            CheckFirstDropFrame = CheckFirstDropFrames[t];
            CheckSubsequentPickFrame = CheckSubsequentPickFrames[t];
            CheckSubsequentDropFrame = CheckSubsequentDropFrames[t];
            CheckFirstPedFrame = CheckFirstPedFrames[t];
            CheckFirstGunTip = CheckFirstGunTips[t];
            CheckFixtureCar0 = CheckFixtureCar0s[t];
            CheckRespotCar0 = CheckRespotCar0s[t];
            UseGripperCar0 = UseGripperCar0s[t];
            GripperCar0IsToolDef = GripperCar0IsToolDefs[t];
            FirstPedFrameNumber = FirstPedFrameNumbers[t];
            SecondPedFrameNumber = SecondPedFrameNumbers[t];
            ThirdPedFrameNumber = ThirdPedFrameNumbers[t];
            RobotNameNumberSubstringStart = RobotNameNumberSubstringStarts[t];

            SpotTcpNameFormat = SpotTcpNameFormats[t];
            GripTcpNameFormat = GripTcpNameFormats[t];
            StudTcpNameFormat = StudTcpNameFormats[t];
            NutTcpNameFormat = NutTcpNameFormats[t];
            MigTcpNameFormat = MigTcpNameFormats[t];
            RivetTcpNameFormat = RivetTcpNameFormats[t];
            SealTcpNameFormat = SealTcpNameFormats[t];
            ScribeTcpNameFormat = ScribeTcpNameFormats[t];
            VisionTcpNameFormat = VisionTcpNameFormats[t];
            PedSpotTcpNameFormat = PedSpotTcpNameFormats[t];
            PedStudTcpNameFormat = PedStudTcpNameFormats[t];
            PedRivetTcpNameFormat = PedRivetTcpNameFormats[t];
            PedSealTcpNameFormat = PedSealTcpNameFormats[t];
            PedScribeTcpNameFormat = PedScribeTcpNameFormats[t];
            FixtureCar0NameFormat = FixtureCar0NameFormats[t];
            GripperCar0NameFormat = GripperCar0NameFormats[t];
        }

        public string BaseDataName(int BaseNum)
        {
            return BdStart + BaseNum.ToString().PadLeft(2) + BdEnd;
        }

        public string ToolDataName(int ToolNum)
        {
            return TdStart + ToolNum.ToString().PadLeft(2) + TdEnd;
        }

        public string PedDataName(int ToolNum)
        {
            return PdStart + ToolNum.ToString().PadLeft(2) + PdEnd;
        }
        #endregion
    }
}
