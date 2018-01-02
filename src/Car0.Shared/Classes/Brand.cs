#if WPF
using System.Windows;
    #else
using System.Windows.Forms;
#endif
namespace CarZero
{
    using System;
    using System.IO;

    internal class Brand
    {
        private static string[] BaseDataEndStrings = new string[] { "", "", "", "]", "]" };
        private static string[] BaseDataStartStrings = new string[] { "WOBJ__", "UFRAME ", "UFRAME ", "BASE_DATA[", "BASE[" };
        public string BdEnd;
        public string BdStart;
        public bool CheckFirstDropFrame;
        private static bool[] CheckFirstDropFrames;
        public bool CheckFirstGripPin;
        private static bool[] CheckFirstGripPins;
        public bool CheckFirstGunTip;
        private static bool[] CheckFirstGunTips;
        public bool CheckFirstPedFrame;
        private static bool[] CheckFirstPedFrames;
        public bool CheckFirstPickFrame;
        private static bool[] CheckFirstPickFrames;
        public bool CheckFixtureCar0;
        private static bool[] CheckFixtureCar0s;
        public bool CheckGripCar0;
        private static bool[] CheckGripCar0s;
        public bool CheckRespotCar0;
        private static bool[] CheckRespotCar0s;
        public bool CheckSubsequentDropFrame;
        private static bool[] CheckSubsequentDropFrames;
        public bool CheckSubsequentPickFrame;
        private static bool[] CheckSubsequentPickFrames;
        public RobotBrands Company;
        public int FirstCar0;
        private static int[] FirstCar0s = new int[] { 1, 3, 3, 1, 1 };
        public int FirstCarriedMIG;
        private static int[] FirstCarriedMIGs = new int[] { 1, 1, 1, 8, 1 };
        public int FirstCarriedSeal;
        private static int[] FirstCarriedSeals = new int[] { 1, 1, 1, 6, 1 };
        public int FirstCarriedSpot;
        private static int[] FirstCarriedSpots = new int[] { 1, 1, 1, 1, 1 };
        public int FirstCarriedStud;
        private static int[] FirstCarriedStuds = new int[] { 1, 1, 1, 4, 1 };
        public int FirstGripCar0;
        private static int[] FirstGripCar0s = new int[] { 1, 1, 1, 11, 30 };
        public int FirstGripPin;
        private static int[] FirstGripPins = new int[] { 1, 1, 1, 14, 10 };
        public int FirstPedFrameNumber;
        private static int[] FirstPedFrameNumbers = new int[] { 1, 1, 1, 13, 20 };
        public int FirstPedTip;
        private static int[] FirstPedTips = new int[] { 1, 1, 1, 13, 20 };
        public string FixtureCar0NameFormat;
        private static string[] FixtureCar0NameFormats = new string[] { "<Robot>_Wobj_<Station>_fx<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>" };
        public string Gc0End;
        public string Gc0Start;
        private static string[] GripperCar0EndStrings = new string[] { "", "", "", "]", "]" };
        public bool GripperCar0IsToolDef;
        private static bool[] GripperCar0IsToolDefs;
        public string GripperCar0NameFormat;
        private static string[] GripperCar0NameFormats = new string[] { "<Robot>_Wobj_<Station>_gp<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>" };
        private static string[] GripperCar0StartStrings = new string[] { "WOBJ__", "UTOOL  ", "UTOOL  ", "TOOL_DATA[", "TOOL[" };
        public string GripTcpNameFormat;
        private static string[] GripTcpNameFormats = new string[] { "<Robot>_tGrip<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        public int Index;
        public int MaxNumberGripperPin;
        private static int[] MaxNumberGripperPins = new int[] { 5, 4, 4, 1, 5 };
        public int MaxNumberRespot;
        private static int[] MaxNumberRespots = new int[] { 1, 1, 1, 1, 3 };
        public string MigTcpNameFormat;
        private static string[] MigTcpNameFormats = new string[] { "<Robot>_tMig<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        public string NutTcpNameFormat;
        private static string[] NutTcpNameFormats = new string[] { "<Robot>_tNdrive<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        public string PdEnd;
        public string PdStart;
        private static string[] PedDataEndStrings = new string[] { "", "", "", "]", "]" };
        private static string[] PedDataStartStrings = new string[] { "TDATA_", "UFRAME ", "UFRAME ", "BASE_DATA[", "TOOL[" };
        public string PedRivetTcpNameFormat;
        private static string[] PedRivetTcpNameFormats = new string[] { "<Robot>_ST_Riv<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_tl<Num>" };
        public string PedScribeTcpNameFormat;
        private static string[] PedScribeTcpNameFormats = new string[] { "<Robot>_ST_tScribe<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_tl<Num>" };
        public string PedSealTcpNameFormat;
        private static string[] PedSealTcpNameFormats = new string[] { "<Robot>_ST_Glue<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_tl<Num>" };
        public string PedSpotTcpNameFormat;
        private static string[] PedSpotTcpNameFormats = new string[] { "<Robot>_ST_tGun<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_tl<Num>" };
        public string PedStudTcpNameFormat;
        private static string[] PedStudTcpNameFormats = new string[] { "<Robot>_ST_tStud<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_uf<Num>", "<Robot>_tl<Num>" };
        public static bool ReadConfiguration = false;
        public string RivetTcpNameFormat;
        private static string[] RivetTcpNameFormats = new string[] { "<Robot>_tRiv<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        public int RobotNameNumberSubstringStart;
        private static int[] RobotNameNumberSubstringStarts = new int[] { 5, 1, 1, 1, 1 };
        public string ScribeTcpNameFormat;
        private static string[] ScribeTcpNameFormats = new string[] { "<Robot>_tScribe<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        public string SealTcpNameFormat;
        private static string[] SealTcpNameFormats = new string[] { "<Robot>_tGlue<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        public int SecondPedFrameNumber;
        private static int[] SecondPedFrameNumbers = new int[] { 2, 1, 1, 14, 0x15 };
        public string SpotTcpNameFormat;
        private static string[] SpotTcpNameFormats = new string[] { "<Robot>_tGun<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        public string StudTcpNameFormat;
        private static string[] StudTcpNameFormats = new string[] { "<Robot>_tStud<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };
        public string TdEnd;
        public string TdStart;
        public int ThirdPedFrameNumber;
        private static int[] ThirdPedFrameNumbers = new int[] { 3, 1, 1, 15, 0x16 };
        private static string[] ToolDataEndStrings = new string[] { "", "", "", "]", "]" };
        private static string[] ToolDataStartStrings = new string[] { "TDATA_", "UTOOL  ", "UTOOL  ", "TOOL_DATA[", "TOOL[" };
        public bool UseGripperCar0;
        private static bool[] UseGripperCar0s;
        public string VisionTcpNameFormat;
        private static string[] VisionTcpNameFormats = new string[] { "<Robot>_tVision<Num>", "<Robot>_ut<Num>", "<Robot>_ut<Num>", "<Robot>_tl<Num>", "<Robot>_tl<Num>" };

        static Brand()
        {
            var flagArray = new bool[5];
            flagArray[3] = true;
            CheckGripCar0s = flagArray;
            CheckFirstGripPins = new bool[] { true, true, true, true, true };
            flagArray = new bool[5];
            flagArray[3] = true;
            CheckFirstPickFrames = flagArray;
            flagArray = new bool[5];
            flagArray[3] = true;
            CheckFirstDropFrames = flagArray;
            flagArray = new bool[5];
            CheckSubsequentPickFrames = flagArray;
            flagArray = new bool[5];
            CheckSubsequentDropFrames = flagArray;
            CheckFirstPedFrames = new bool[] { true, true, true, true, true };
            CheckFirstGunTips = new bool[] { true, true, true, true, true };
            CheckFixtureCar0s = new bool[] { true, true, true, true, true };
            flagArray = new bool[5];
            CheckRespotCar0s = flagArray;
            flagArray = new bool[5];
            flagArray[3] = true;
            UseGripperCar0s = flagArray;
            flagArray = new bool[5];
            flagArray[0] = true;
            GripperCar0IsToolDefs = flagArray;
        }

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
            Company = (RobotBrands) t;
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
            return (BdStart + BaseNum.ToString().PadLeft(2) + BdEnd);
        }

        public string PedDataName(int ToolNum)
        {
            return (PdStart + ToolNum.ToString().PadLeft(2) + PdEnd);
        }

        private  void ReadCar0Configuration()
        {
            if (!ReadConfiguration && (UserPreferences.WorkFolderName != null))
            {
                try
                {
                    var path = Path.Combine(UserPreferences.WorkFolderName, "Car0 Configuration.csv");
                    if (File.Exists(path))
                    {
                        var list = Utils.FileToArrayList(path);
                        for (var i = 1; i < list.Count; i++)
                        {
                            var strArray = list[i].ToString().Split(new char[] { ',' });
                            var length = strArray.Length;
                            if (length.Equals(6))
                            {
                                int num2;
                                switch (strArray[0])
                                {
                                    case "BaseDataStartStrings":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        BaseDataStartStrings[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                        break;
                                    case "BaseDataEndStrings":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        BaseDataEndStrings[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "ToolDataStartStrings":

                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        ToolDataStartStrings[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "ToolDataEndStrings":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        ToolDataEndStrings[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "PedDataStartStrings":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        PedDataStartStrings[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "PedDataEndStrings":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        PedDataEndStrings[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "GripperCar0StartStrings":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        GripperCar0StartStrings[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                        break;
                                    case "GripperCar0EndStrings":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        GripperCar0EndStrings[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "SpotTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        SpotTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "GripTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        GripTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "StudTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        StudTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "NutTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        NutTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "MigTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        MigTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                        break;
                                    case "RivetTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        RivetTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "SealTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        SealTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "ScribeTcpNameFormats":

                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        ScribeTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "VisionTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        VisionTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "PedSpotTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        PedSpotTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "PedStudTcpNameFormats":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        PedStudTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "PedRivetTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        PedRivetTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "PedSealTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        PedSealTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "PedScribeTcpNameFormats":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        PedScribeTcpNameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
break;
                                    case "FixtureCar0NameFormats":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        FixtureCar0NameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "GripperCar0NameFormats":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        GripperCar0NameFormats[num2 - 1] = strArray[num2];
                                        num2++;
                                    }
                                break;
                                    case "FirstCarriedSpots":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            FirstCarriedSpots[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                    case "FirstCarriedStuds":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            FirstCarriedStuds[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                    case "FirstCarriedSeals":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            FirstCarriedSeals[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                    case "FirstCarriedMIGs":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            FirstCarriedMIGs[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                    case "FirstCar0s":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            FirstCar0s[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                    case "FirstPedTips":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            FirstPedTips[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                    case "FirstGripCar0s":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            FirstGripCar0s[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                    case "FirstGripPins":
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            FirstGripPins[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                    case "MaxNumberRespots":

                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            MaxNumberRespots[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                case "MaxNumberGripperPins":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            MaxNumberGripperPins[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                case "FirstPedFrameNumbers":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            FirstPedFrameNumbers[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                case "SecondPedFrameNumbers":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            SecondPedFrameNumbers[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                case "ThirdPedFrameNumbers":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            ThirdPedFrameNumbers[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                case "RobotNameNumberSubstringStarts":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            RobotNameNumberSubstringStarts[num2 - 1] = Convert.ToInt32(strArray[num2]);
                                        }
                                        num2++;
                                    }
                                break;
                                case "CheckGripCar0s":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            CheckGripCar0s[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                case "CheckFirstGripPins":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            CheckFirstGripPins[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                case "CheckFirstPickFrames":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            CheckFirstPickFrames[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                case "CheckFirstDropFrames":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            CheckFirstDropFrames[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                case "CheckSubsequentPickFrames":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            CheckSubsequentPickFrames[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                case "CheckSubsequentDropFrames":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            CheckSubsequentDropFrames[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                case "CheckFirstPedFrames":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            CheckFirstPedFrames[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                case "CheckFirstGunTips":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            CheckFirstGunTips[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                case "CheckFixtureCar0s":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            CheckFixtureCar0s[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                case "CheckRespotCar0s":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            CheckRespotCar0s[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                case "UseGripperCar0s":
                                
                                    num2 = 1;
                                    while (num2 < strArray.Length)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            UseGripperCar0s[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                        num2++;
                                    }
                                break;
                                    case "GripperCar0IsToolDefs":
                                    for (num2 = 1; num2 < strArray.Length; num2++)
                                    {
                                        if (strArray[num2] != null)
                                        {
                                            GripperCar0IsToolDefs[num2 - 1] = strArray[num2].Equals("true");
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        ReadConfiguration = true;
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "ReadCar0Configuration --> TELL AL!!!!");
                }
            }
        }

        public string ToolDataName(int ToolNum)
        {
            return (TdStart + ToolNum.ToString().PadLeft(2) + TdEnd);
        }

        public enum RobotBrands
        {
            ABB = 0,
            Fanuc = 1,
            FanucRJ = 2,
            KUKA = 3,
            Nachi = 4,
            Undefined = -1
        }
    }
}

