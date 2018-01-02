#if WPF
using System.Windows;
    #else
using System.Windows.Forms;
#endif
using CarZero.Classes;

namespace CarZero
{
    using Microsoft.Office.Interop.Excel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class ExcelIO
    {
        private static int AppDataEndColumnNum = 0x1d;
        private static int AppDataStartColumnNum = 0x10;
        private static bool ClosingMatrixForRead = false;
        private static bool ClosingTrackerDataForRead = false;
        private static int EoatDefEndRow = 0x10;
        private static int EoatDefStartRow = 2;
        private static int HeadingsRow = 3;
        private static string LastTrackerDataFile = null;
        private static int MatrixFileStartRow = 6;
        private static Worksheet MatrixMatSheet = null;
        private static bool MatrixOpen = false;
        private static Workbook MatrixoWb = null;
        private static Microsoft.Office.Interop.Excel.Application MatrixoXL = null;
        private static Sheets MatrixWorksheets = null;
        private static string[] MeasColNames = new string[] { 
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", 
            "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", 
            "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", 
            "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", 
            "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA", "CB", 
            "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", 
            "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ"
         };
        public static ArrayList RobotAppTypes = new ArrayList();
        private static string RobotNameColumn = "G";
        public static ArrayList RobotTypes = new ArrayList();
        private static int RobotTypesEndColumnNum = 0x1c;
        private static int RobotTypesStartColumnNum = 0x13;
        private static string StationNumberColumn = "";
        private static string SystemCodeColumn = "";
        private static string SystemNameColumn = "B";
        private static string SystemNumColumn = "A";
        public static bool TrackerDataExcelOpen = false;
        private static Workbook TrackerDataoWb = null;
        private static Microsoft.Office.Interop.Excel.Application TrackerDataoXL = null;
        private static bool TrackerDataSaving = false;
        private static Worksheet TrackerMatSheet = null;
        private static Sheets TrackerWorksheets = null;
        private static RobTypeMethod TypeMethod = RobTypeMethod.NamesInColums;

        public static bool BuildMatrixRobotList(ref ArrayList RobotNames, ref ArrayList RobotMechanismNames, string TargetSystemName)
        {
            string str = null;
            string item = null;
            string robotMechanismName = null;
            string str4 = null;
            string str5 = null;
            if (CheckBuildRobotDataArrays() && TryOpenMatrix())
            {
                try
                {
                    var matrixFileStartRow = MatrixFileStartRow;
                    var flag = false;
                    RobotNames = new ArrayList();
                    while (!flag)
                    {
                        str4 = ProgStringCell(SystemNumColumn, matrixFileStartRow, MatrixMatSheet);
                        str5 = ProgStringCell(SystemNameColumn, matrixFileStartRow, MatrixMatSheet);
                        item = ProgStringCell(RobotNameColumn, matrixFileStartRow, MatrixMatSheet);
                        robotMechanismName = GetRobotMechanismName(matrixFileStartRow);
                        if (((str4 == null) || (str5 == null)) || (item == null))
                        {
                            flag = true;
                        }
                        else
                        {
                            str = str5 + "(" + str4 + ")";
                            if (TargetSystemName.Equals(str) && !RobotNames.Contains(item))
                            {
                                if (item == null)
                                {
                                    RobotNames.Add("UNKNOWN");
                                }
                                else
                                {
                                    RobotNames.Add(item);
                                }
                                if (robotMechanismName == null)
                                {
                                    RobotMechanismNames.Add("UNKNOWN");
                                }
                                else
                                {
                                    RobotMechanismNames.Add(robotMechanismName);
                                }
                            }
                        }
                        matrixFileStartRow++;
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString() + "\n\nThis is a problem with the ROBOT MATRIX file selected!!!\n\n");
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool BuildMatyrixSystemList(ref ArrayList SystemNames, ref ArrayList SystemCodes)
        {
            string item = null;
            string str2 = null;
            string str3 = null;
            string str4 = null;
            RobotTypes = new ArrayList();
            RobotAppTypes = new ArrayList();
            SystemCodes = new ArrayList();
            if (TryOpenMatrix() && ReadMatrixMap())
            {
                try
                {
                    var matrixFileStartRow = MatrixFileStartRow;
                    var flag = false;
                    while (!flag)
                    {
                        str2 = ProgStringCell(SystemNumColumn, matrixFileStartRow, MatrixMatSheet);
                        str3 = ProgStringCell(SystemNameColumn, matrixFileStartRow, MatrixMatSheet);
                        if (SystemCodeColumn.Length > 0)
                        {
                            str4 = ProgStringCell(SystemCodeColumn, matrixFileStartRow, MatrixMatSheet);
                        }
                        if ((str2 == null) || (str3 == null))
                        {
                            flag = true;
                        }
                        else
                        {
                            item = str3 + "(" + str2 + ")";
                            if (!SystemNames.Contains(item))
                            {
                                SystemNames.Add(item);
                                SystemCodes.Add(str4);
                            }
                        }
                        matrixFileStartRow++;
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString() + "\n\nThis is a problem with the ROBOT MATRIX file selected!!!\n\n");
                    return false;
                }
                return true;
            }
            return false;
        }

        private static bool CheckBuildRobotDataArrays()
        {
            if (RobotTypes.Count == 0)
            {
                string str = null;
                if (TryOpenMatrix())
                {
                    try
                    {
                        int num;
                        if (TypeMethod.Equals(RobTypeMethod.RangeColumns))
                        {
                            for (num = RobotTypesStartColumnNum; num <= RobotTypesEndColumnNum; num++)
                            {
                                str = ProgStringCell(MeasColNames[num], HeadingsRow, MatrixMatSheet);
                                RobotTypes.Add(str);
                            }
                        }
                        for (num = AppDataStartColumnNum; num <= AppDataEndColumnNum; num++)
                        {
                            str = ProgStringCell(MeasColNames[num], HeadingsRow, MatrixMatSheet);
                            RobotAppTypes.Add(str);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.ToString() + "\n\nThis is a problem with the ROBOT MATRIX file selected!!!\n\n");
                        return false;
                    }
                    return true;
                }
                return false;
            }
            return true;
        }

        public static bool CheckTrackerFile(string FilePath, ref ArrayList Problems, Brand Kind)
        {
            if (!OpenExcelFile(FilePath, false))
            {
                return false;
            }
            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet) TrackerWorksheets.get_Item(1);
            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;
            Problems = new ArrayList();
            try
            {
                if (UserPreferences.OperationRadioButtonSelected == 1)
                {
                    if (!((DoubleDataValid("B", 2, TrackerMatSheet) && DoubleDataValid("C", 2, TrackerMatSheet)) && DoubleDataValid("D", 2, TrackerMatSheet)))
                    {
                        Problems.Add("Robot 321 Fit points: Xref row not completely defined");
                    }
                    if (!((DoubleDataValid("B", 3, TrackerMatSheet) && DoubleDataValid("C", 3, TrackerMatSheet)) && DoubleDataValid("D", 3, TrackerMatSheet)))
                    {
                        Problems.Add("Robot 321 Fit points: Yref row not completely defined");
                    }
                    if (!((DoubleDataValid("B", 4, TrackerMatSheet) && DoubleDataValid("C", 4, TrackerMatSheet)) && DoubleDataValid("D", 4, TrackerMatSheet)))
                    {
                        Problems.Add("Robot 321 Fit points: OriginRef row not completely defined");
                    }
                }
                else if (UserPreferences.OperationRadioButtonSelected == 2)
                {
                    if (!((DoubleDataValid("B", 8, TrackerMatSheet) && DoubleDataValid("C", 8, TrackerMatSheet)) && DoubleDataValid("D", 8, TrackerMatSheet)))
                    {
                        Problems.Add("Initial wrt Robot World: Base pt1 row not completely defined");
                    }
                    if (!((DoubleDataValid("B", 9, TrackerMatSheet) && DoubleDataValid("C", 9, TrackerMatSheet)) && DoubleDataValid("D", 9, TrackerMatSheet)))
                    {
                        Problems.Add("Initial wrt Robot World: Base pt2 row not completely defined");
                    }
                    if (!((DoubleDataValid("B", 10, TrackerMatSheet) && DoubleDataValid("C", 10, TrackerMatSheet)) && DoubleDataValid("D", 10, TrackerMatSheet)))
                    {
                        Problems.Add("Initial wrt Robot World: Base pt3 row not completely defined");
                    }
                    var flag = false;
                    for (var i = 0; i < RobotData.RobotFramesChecked.Count; i++)
                    {
                        flag = false;
                        if (RobotData.RobotFramesChecked[i].Equals(true))
                        {
                            var index = 0;
                            while (index < 8)
                            {
                                if (RobotData.RobotFrames[i].ToString().Contains(Kind.BaseDataName(index + 1)))
                                {
                                    for (var j = 0; j < 3; j++)
                                    {
                                        if (!((DoubleDataValid(MeasColNames[UserPreferences.FinalPointStartColNumbers[index]], 8 + j, TrackerMatSheet) && DoubleDataValid(MeasColNames[UserPreferences.FinalPointStartColNumbers[index] + 1], 8 + j, TrackerMatSheet)) && DoubleDataValid(MeasColNames[UserPreferences.FinalPointStartColNumbers[index] + 2], 8 + j, TrackerMatSheet)))
                                        {
                                            var strArray = new string[5];
                                            strArray[0] = "Final wrt Car 0 ";
                                            strArray[1] = Kind.BaseDataName(index + 1);
                                            strArray[2] = ": Car0 pt";
                                            var num4 = j + 1;
                                            strArray[3] = num4.ToString();
                                            strArray[4] = " row not completely defined";
                                            Problems.Add(string.Concat(strArray));
                                        }
                                    }
                                    flag = true;
                                }
                                index++;
                            }
                            if (!flag)
                            {
                                index = 0;
                                while (index < 14)
                                {
                                    if (RobotData.RobotFrames[i].ToString().Contains(Kind.ToolDataName(index + 1)))
                                    {
                                        if (!((DoubleDataValid("B", 0x15 + index, TrackerMatSheet) && DoubleDataValid("C", 0x15 + index, TrackerMatSheet)) && DoubleDataValid("D", 0x15 + index, TrackerMatSheet)))
                                        {
                                            Problems.Add("EOAT Definitions Wrt TOOLFRAME: TOOL_DATA[" + ((index + 1)).ToString() + "] row not completely defined");
                                        }
                                        flag = true;
                                    }
                                    index++;
                                }
                            }
                            if (!flag)
                            {
                                for (index = 0; index < 3; index++)
                                {
                                    if (RobotData.RobotFrames[i].ToString().Contains(Kind.PedDataName(Kind.FirstPedFrameNumber + index)) && !((DoubleDataValid("G", 0x15 + index, TrackerMatSheet) && DoubleDataValid("H", 0x15 + index, TrackerMatSheet)) && DoubleDataValid("I", 0x15 + index, TrackerMatSheet)))
                                    {
                                        Problems.Add("BASE Definitions wrt Robot World: " + Kind.PedDataName(Kind.FirstPedFrameNumber + index) + " row not completely defined");
                                    }
                                    flag = true;
                                }
                            }
                            if (!flag)
                            {
                                Problems.Add("Error: Unrecognized frame = '" + RobotData.RobotFrames[i].ToString() + "'");
                                CloseTrackerData();
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                CloseTrackerData();
                return false;
            }
            CloseTrackerData();
            return (Problems.Count == 0);
        }

        public static void CloseMatrix()
        {
            if (MatrixOpen)
            {
                ClosingMatrixForRead = true;
                if (MatrixMatSheet != null)
                {
                    NAR(MatrixMatSheet);
                    MatrixMatSheet = null;
                }
                if (MatrixoWb != null)
                {
                    MatrixoWb.Close(false, false, false);
                }
                ClosingMatrixForRead = false;
                MatrixOpen = false;
            }
        }

        public static void CloseTrackerData()
        {
            CloseTrackerData(false, null);
        }

        public static void CloseTrackerData(bool Saveit, string FilePath)
        {
            if (TrackerDataExcelOpen)
            {
                ClosingTrackerDataForRead = true;
                if (TrackerMatSheet != null)
                {
                    NAR(TrackerMatSheet);
                    TrackerMatSheet = null;
                }
                TrackerDataoWb.Close(Saveit, FilePath, false);
                ClosingTrackerDataForRead = false;
            }
        }

        private static bool DoubleDataValid(string Col, int Row, Worksheet Sheet)
        {
            try
            {
                var str = ProgStringCell(Col, Row, Sheet);
                if ((str == null) || (str.Length == 0))
                {
                    return false;
                }
                var num = Convert.ToDouble(str);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool FindRobotWristData(string FilePath, string RobotMechanismType, ref double[] WristData, ref ArrayList Problems)
        {
            var row = 2;
            var flag = false;
            var strArray = new string[] { "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" };
            if (!OpenExcelFile(FilePath, false))
            {
                return false;
            }
            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet) TrackerWorksheets.get_Item(3);
            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;
            try
            {
                string str = null;
                do
                {
                    row++;
                    str = ProgStringCell("A", row, TrackerMatSheet);
                }
                while ((str != null) && !str.Equals(RobotMechanismType));
                if ((str != null) && str.Equals(RobotMechanismType))
                {
                    var val = 0.0;
                    var index = 0;
                    while (index < 12)
                    {
                        if (GetDoubleValAtRowCol(strArray[index], row, ref val))
                        {
                            switch (index)
                            {
                                case 2:
                                case 3:
                                case 6:
                                case 7:
                                case 10:
                                case 11:
                                    WristData[index] = Utils.DegreesToRad(val);
                                    goto Label_01B7;
                            }
                            WristData[index] = val;
                        }
                        else
                        {
                            Problems.Add("Incomplete wrist data record for robot type " + RobotMechanismType);
                            break;
                        }
                    Label_01B7:
                        index++;
                    }
                    flag = index.Equals(12);
                }
                CloseTrackerData();
            }
            catch (Exception exception)
            {
                Problems.Add(exception.Message);
                return false;
            }
            if (!flag)
            {
                Problems.Add("Wrist data not found for robot type:" + RobotMechanismType);
            }
            return flag;
        }

        private static bool GetColumnAndRows(int index, ref int RowNum, ref string Xcol, ref string Ycol, ref string Zcol, Brand Kind)
        {
            FrameType type = RobotData.FrameTypes[index];
            int num = RobotData.FrameNumbers[index];
            switch (type)
            {
                case FrameType.MigGunTip:
                case FrameType.LaserTip:
                case FrameType.PierceTip:
                    Xcol = "B";
                    Ycol = "C";
                    Zcol = "D";
                    RowNum = (0x15 + num) - Kind.FirstCarriedMIG;
                    break;

                case FrameType.NutGunTip:
                case FrameType.RivetGunTip:
                case FrameType.ScribeGun:
                case FrameType.StudGunTip:
                case FrameType.VisionTcp:
                    Xcol = "B";
                    Ycol = "C";
                    Zcol = "D";
                    RowNum = (0x15 + num) - Kind.FirstCarriedStud;
                    break;

                case FrameType.SealGunTip:
                    Xcol = "B";
                    Ycol = "C";
                    Zcol = "D";
                    RowNum = (0x15 + num) - Kind.FirstCarriedSeal;
                    break;

                case FrameType.SpotGunTip:
                    Xcol = "B";
                    Ycol = "C";
                    Zcol = "D";
                    RowNum = (0x15 + num) - Kind.FirstCarriedSpot;
                    break;

                case FrameType.GripPin:
                    Xcol = "B";
                    Ycol = "C";
                    Zcol = "D";
                    RowNum = (30 + num) - Kind.FirstGripPin;
                    break;

                case FrameType.FixtureCar0:
                case FrameType.GripperCar0:
                case FrameType.PickTool:
                case FrameType.DropTool:
                    Xcol = MeasColNames[UserPreferences.FinalPointStartColNumbers[num - 1]];
                    Ycol = MeasColNames[UserPreferences.FinalPointStartColNumbers[num - 1] + 1];
                    Zcol = MeasColNames[UserPreferences.FinalPointStartColNumbers[num - 1] + 2];
                    break;

                case FrameType.PedRivetGunTip:
                case FrameType.PedSealGunTip:
                case FrameType.PedSpotGunTip:
                case FrameType.PedScribeGunTip:
                case FrameType.PedStudGunTip:
                    Xcol = "G";
                    Ycol = "H";
                    Zcol = "I";
                    RowNum = (0x15 + num) - Kind.FirstPedFrameNumber;
                    break;
            }
            return true;
        }

        private static bool GetColumnAndRows(string ElementName, ref int RowNum, ref string Xcol, ref string Ycol, ref string Zcol, Brand Kind)
        {
            int num;
            for (num = 2; num >= 0; num--)
            {
                if (ElementName.Contains(Kind.PedDataName(num + Kind.FirstPedFrameNumber)))
                {
                    Xcol = "G";
                    Ycol = "H";
                    Zcol = "I";
                    RowNum = 0x15 + num;
                    return true;
                }
            }
            for (num = 13; num >= 0; num--)
            {
                if (ElementName.Contains(Kind.ToolDataName(num + 1)))
                {
                    Xcol = "B";
                    Ycol = "C";
                    Zcol = "D";
                    RowNum = 0x15 + num;
                    return true;
                }
            }
            return false;
        }

        private static bool GetColumnAndRows(string ElementName, int ElementNumber, ref int RowNum, ref string Xcol, ref string Ycol, ref string Zcol, Brand Kind)
        {
            if (ElementName.Equals("Robot 321 Fit points"))
            {
                Xcol = "B";
                Ycol = "C";
                Zcol = "D";
                switch (ElementNumber)
                {
                    case 0:
                        RowNum = 2;
                        return true;

                    case 1:
                        RowNum = 3;
                        return true;

                    case 2:
                        RowNum = 4;
                        return true;
                }
            }
            else if (ElementName.Equals("Initial wrt Robot World"))
            {
                Xcol = "B";
                Ycol = "C";
                Zcol = "D";
                if (ElementNumber < 10)
                {
                    RowNum = 8 + ElementNumber;
                    return true;
                }
            }
            else
            {
                for (var i = 0; i < 8; i++)
                {
                    if (ElementName.Contains(Kind.BaseDataName(i + 1)))
                    {
                        Xcol = MeasColNames[UserPreferences.FinalPointStartColNumbers[i]];
                        Ycol = MeasColNames[UserPreferences.FinalPointStartColNumbers[i] + 1];
                        Zcol = MeasColNames[UserPreferences.FinalPointStartColNumbers[i] + 2];
                        if (ElementNumber < 10)
                        {
                            RowNum = 8 + ElementNumber;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool GetDoubleValAtRowCol(string Column, int Row, ref double Val)
        {
            try
            {
                if (ProgStringCell(Column, Row, TrackerMatSheet) == null)
                {
                    return false;
                }
                Val = Convert.ToDouble(ProgStringCell(Column, Row, TrackerMatSheet));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool GetRobotData(string TargetSystemName, string TargetRobot, ref string RobotMechanismType, ref string StationName, ref ArrayList RobotApps)
        {
            if (CheckBuildRobotDataArrays())
            {
                string str = null;
                string str2 = null;
                string str3 = null;
                string str4 = null;
                if (TryOpenMatrix())
                {
                    var matrixFileStartRow = MatrixFileStartRow;
                    try
                    {
                        var num2 = 0;
                        var index = 0;
                        var flag = false;
                        var flag2 = false;
                        while (!flag)
                        {
                            str3 = ProgStringCell(SystemNumColumn, matrixFileStartRow, MatrixMatSheet);
                            str4 = ProgStringCell(SystemNameColumn, matrixFileStartRow, MatrixMatSheet);
                            str2 = ProgStringCell(RobotNameColumn, matrixFileStartRow, MatrixMatSheet);
                            if (((str3 == null) || (str4 == null)) || (str2 == null))
                            {
                                flag = true;
                            }
                            else
                            {
                                str = str4 + "(" + str3.ToString() + ")";
                                if (TargetSystemName.Equals(str) && str2.Equals(TargetRobot))
                                {
                                    flag2 = true;
                                    break;
                                }
                            }
                            matrixFileStartRow++;
                        }
                        if (!flag2)
                        {
                            MessageBox.Show("Error: target robot '" + TargetRobot + "' not found in Robot Matrix");
                            return false;
                        }
                        flag2 = false;
                        RobotMechanismType = GetRobotMechanismName(matrixFileStartRow);
                        StationName = (StationNumberColumn.Length > 0) ? ProgStringCell(StationNumberColumn, matrixFileStartRow, MatrixMatSheet) : "Unknown";
                        RobotApps = new ArrayList();
                        num2 = 0;
                        for (index = AppDataStartColumnNum; index <= AppDataEndColumnNum; index++)
                        {
                            str = ProgStringCell(MeasColNames[index], matrixFileStartRow, MatrixMatSheet);
                            if ((str != null) && (((str.Equals("1") || str.Equals("2")) || (str.Equals("3") || str.Equals("4"))) || str.Equals("5")))
                            {
                                var num5 = Convert.ToInt32(str);
                                for (var i = 0; i < num5; i++)
                                {
                                    RobotApps.Add(RobotAppTypes[num2].ToString());
                                }
                            }
                            num2++;
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.ToString() + "\n\nThis is a problem with the ROBOT MATRIX file selected!!!\n\n");
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        private static string GetRobotMechanismName(int RobotRow)
        {
            if (TypeMethod.Equals(RobTypeMethod.NamesInColums))
            {
                return ProgStringCell(MeasColNames[RobotTypesStartColumnNum], RobotRow, MatrixMatSheet);
            }
            string str2 = null;
            for (var i = RobotTypesStartColumnNum; i <= RobotTypesEndColumnNum; i++)
            {
                str2 = ProgStringCell(MeasColNames[i], RobotRow, MatrixMatSheet);
                if ((str2 != null) && str2.Equals("1"))
                {
                    return ((i < RobotTypes.Count) ? RobotTypes[i - RobotTypesStartColumnNum].ToString() : "UNKNOWN");
                }
            }
            return "UNKNOWN";
        }

        public static List<Vector> GetTrackerFileData(string FilePath, string ElementName, ref ArrayList Problems, Brand Kind)
        {
            var rowNum = 0;
            var strArray = new string[3];
            if (!OpenExcelFile(FilePath, false))
            {
                return null;
            }
            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet) TrackerWorksheets.get_Item(1);
            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;
            var list = new List<Vector>();
            var val = 0.0;
            try
            {
                var b = new Vector(3);
                for (var i = 0; i < 10; i++)
                {
                    if (!GetColumnAndRows(ElementName, i, ref rowNum, ref strArray[0], ref strArray[1], ref strArray[2], Kind))
                    {
                        Problems.Add(ElementName + " not known.");
                        return null;
                    }
                    for (var j = 0; j < 3; j++)
                    {
                        if (!GetDoubleValAtRowCol(strArray[j], rowNum, ref val))
                        {
                            if (i < 3)
                            {
                                Problems.Add(ElementName + " item at column '" + strArray[i] + "' row = " + rowNum.ToString() + " is not defined");
                                return null;
                            }
                            return list;
                        }
                        b.Vec[j] = val;
                    }
                    list.Add(new Vector(b));
                }
            }
            catch (Exception)
            {
                return null;
            }
            return list;
        }

        public static bool GetTrackerFileData(string FilePath, int index, ref double[] ElementPts, ref ArrayList Problems, Brand Kind)
        {
            var rowNum = 0;
            var strArray = new string[3];
            if (!OpenExcelFile(FilePath, false))
            {
                return false;
            }
            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet) TrackerWorksheets.get_Item(1);
            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;
            try
            {
                if (!GetColumnAndRows(index, ref rowNum, ref strArray[0], ref strArray[1], ref strArray[2], Kind))
                {
                    Problems.Add(RobotData.FrameTypes[index].ToString() + " not known.");
                    return false;
                }
                for (var i = 0; i < 3; i++)
                {
                    if (!GetDoubleValAtRowCol(strArray[i], rowNum, ref ElementPts[i]))
                    {
                        Problems.Add(RobotData.FrameTypes[index].ToString() + " item at column '" + strArray[i] + "' row = " + rowNum.ToString() + " is not defined");
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool GetTrackerFileData(string FilePath, int index, ref Vector ElementPts, ref ArrayList Problems, Brand Kind)
        {
            var rowNum = 0;
            var strArray = new string[3];
            if (!OpenExcelFile(FilePath, false))
            {
                return false;
            }
            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet) TrackerWorksheets.get_Item(1);
            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;
            try
            {
                var val = 0.0;
                if (!GetColumnAndRows(index, ref rowNum, ref strArray[0], ref strArray[1], ref strArray[2], Kind))
                {
                    Problems.Add(RobotData.FrameTypes[index].ToString() + " not known.");
                    return false;
                }
                for (var i = 0; i < 3; i++)
                {
                    if (!GetDoubleValAtRowCol(strArray[i], rowNum, ref val))
                    {
                        Problems.Add(RobotData.FrameTypes[index].ToString() + " item at column '" + strArray[i] + "' row = " + rowNum.ToString() + " is not defined");
                        return false;
                    }
                    ElementPts.Vec[i] = val;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool GetTrackerFileData(string FilePath, string ElementName, ref double[,] ElementPts, ref ArrayList Problems, Brand Kind)
        {
            var rowNum = 0;
            var strArray = new string[3];
            if (!OpenExcelFile(FilePath, false))
            {
                return false;
            }
            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet) TrackerWorksheets.get_Item(1);
            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;
            Problems = new ArrayList();
            try
            {
                for (var i = 0; i < 3; i++)
                {
                    if (!GetColumnAndRows(ElementName, i, ref rowNum, ref strArray[0], ref strArray[1], ref strArray[2], Kind))
                    {
                        Problems.Add(ElementName + " not known.");
                        return false;
                    }
                    for (var j = 0; j < 3; j++)
                    {
                        if (!GetDoubleValAtRowCol(strArray[j], rowNum, ref ElementPts[i, j]))
                        {
                            Problems.Add(ElementName + " item at column '" + strArray[i] + "' row = " + rowNum.ToString() + " is not defined");
                            return false;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static void MatrixoXL_WindowDeactivate(Workbook Wb, Window Wn)
        {
            MatrixOpen = false;
        }

        private static void MatrixoXL_WorkbookDeactivate(Workbook Wb)
        {
            if (!ClosingMatrixForRead)
            {
                NAR(MatrixoWb);
            }
            MatrixoWb = null;
            if (ClosingMatrixForRead)
            {
                NAR(MatrixWorksheets);
                MatrixWorksheets = null;
            }
            MatrixoXL.Quit();
            NAR(MatrixoXL);
            MatrixoXL = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void NAR(object O)
        {
            try
            {
                Marshal.ReleaseComObject(O);
            }
            catch (Exception)
            {
            }
            finally
            {
                O = null;
            }
        }

        public static bool OpenExcelFile(string FilePath, bool makevisible)
        {
            if (!TrackerDataExcelOpen)
            {
                if (!File.Exists(FilePath))
                {
                    MessageBox.Show("Error: Selected Excel Tracker Data file '" + FilePath + "' was not found!!");
                    return false;
                }
                if (TrackerDataExcelOpen)
                {
                    MessageBox.Show("Warning: the following Tracker Data file is still open:\n\n" + LastTrackerDataFile + "\n\nPlease close this (saving if you wish) and try again.");
                    return false;
                }
                if (TrackerDataoXL != null)
                {
                    TrackerDataoXL.Quit();
                    while (TrackerDataExcelOpen)
                    {
                        Thread.Sleep(10);
                    }
                }
                #if !WPF
                TrackerDataoXL = new ApplicationClass();
                TrackerDataoXL.WorkbookBeforeSave+=TrackerDataoXL_WorkbookBeforeSave;
                TrackerDataoXL.WindowDeactivate+=TrackerDataoXL_WindowDeactivate;
                TrackerDataoXL.WorkbookDeactivate+=TrackerDataoXL_WorkbookDeactivate;
                TrackerDataoWb = TrackerDataoXL.Workbooks.Open(FilePath, 0, false, 5, "", "", false, (XlPlatform) 2, "", true, false, 0, false, false, 0);
                TrackerDataoXL.Visible = makevisible;
                TrackerDataExcelOpen = true;
                LastTrackerDataFile = FilePath;
    #else
                #warning Need to fix
                #endif
            }
            return true;
        }

        private static string ProgStringCell(string Column, int Row, Worksheet MatSheet)
        {
            string str = null;
            string str2 = null;
            if (Column != null)
            {
                str2 = Column + Row.ToString();
                var range = MatSheet.get_Range(str2, str2);
                if (range.Value2 != null)
                {
                    str = range.Value2.ToString();
                }
            }
            return str;
        }

        public static bool Read123Data(ref double[] Origin, ref double[] Xref, ref double[] Yref)
        {
            var path = Path.Combine(UserPreferences.WorkFolderName, UserPreferences.Excel321FileName);
            if (!File.Exists(path))
            {
                MessageBox.Show("Error: Selected Excel 3-2-1 file '" + UserPreferences.Excel321FileName + "' was not found!!");
                return false;
            }
            try
            {
                #if !WPF
                string str2 = null;
                Microsoft.Office.Interop.Excel.Application application = new ApplicationClass();
                var workbook = application.Workbooks.Open(path, 0, true, 5, "", "", true, (XlPlatform) 2, "", false, false, 0, false, false, false);
                var matSheet = (Worksheet) application.Worksheets.get_Item(1);
                application.DisplayAlerts = false;
                workbook.Saved = true;
                str2 = ProgStringCell("B", 4, matSheet);
                Origin[0] = Convert.ToDouble(str2);
                str2 = ProgStringCell("C", 4, matSheet);
                Origin[1] = Convert.ToDouble(str2);
                str2 = ProgStringCell("D", 4, matSheet);
                Origin[2] = Convert.ToDouble(str2);
                str2 = ProgStringCell("B", 2, matSheet);
                Xref[0] = Convert.ToDouble(str2);
                str2 = ProgStringCell("C", 2, matSheet);
                Xref[1] = Convert.ToDouble(str2);
                str2 = ProgStringCell("D", 2, matSheet);
                Xref[2] = Convert.ToDouble(str2);
                str2 = ProgStringCell("B", 3, matSheet);
                Yref[0] = Convert.ToDouble(str2);
                str2 = ProgStringCell("C", 3, matSheet);
                Yref[1] = Convert.ToDouble(str2);
                str2 = ProgStringCell("D", 3, matSheet);
                Yref[2] = Convert.ToDouble(str2);
                workbook.Close(false, false, false);
                application.Quit();
    #else
                #warning Fix this
                #endif
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString() + "\n\nThis is a problem with the WELD MATRIX file selected!!!\n\n");
                return false;
            }
            return true;
        }

        private static bool ReadMatrixMap()
        {
            try
            {
                string filePath = null;
                if (UserPreferences.RobotMatrixFileName.Contains(".xlsx"))
                {
                    filePath = UserPreferences.RobotMatrixFileName.Replace(".xlsx", ".txt");
                }
                else if (UserPreferences.RobotMatrixFileName.Contains(".xls"))
                {
                    filePath = UserPreferences.RobotMatrixFileName.Replace(".xls", ".txt");
                }
                if (filePath == null)
                {
                    MessageBox.Show("Illegal .extension for Matrix file name = '" + UserPreferences.RobotMatrixFileName + "'", "ReadMatrixMap");
                    return false;
                }
                var list = Utils.FileToArrayList(filePath);
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i].ToString().Contains("MatrixFileStartRow:"))
                    {
                        MatrixFileStartRow = Utils.IntFromString(list[i].ToString(), "MatrixFileStartRow:");
                    }
                    else if (list[i].ToString().Contains("HeadingsRow:"))
                    {
                        HeadingsRow = Utils.IntFromString(list[i].ToString(), "HeadingsRow:");
                    }
                    else if (list[i].ToString().Contains("EoatDefStartRow:"))
                    {
                        EoatDefStartRow = Utils.IntFromString(list[i].ToString(), "EoatDefStartRow:");
                    }
                    else if (list[i].ToString().Contains("EoatDefEndRow:"))
                    {
                        EoatDefEndRow = Utils.IntFromString(list[i].ToString(), "EoatDefEndRow:");
                    }
                    else if (list[i].ToString().Contains("RobotTypesStartColumnNum:"))
                    {
                        RobotTypesStartColumnNum = Utils.IntFromString(list[i].ToString(), "RobotTypesStartColumnNum:");
                    }
                    else if (list[i].ToString().Contains("RobotTypesEndColumnNum:"))
                    {
                        RobotTypesEndColumnNum = Utils.IntFromString(list[i].ToString(), "RobotTypesEndColumnNum:");
                    }
                    else if (list[i].ToString().Contains("AppDataStartColumnNum:"))
                    {
                        AppDataStartColumnNum = Utils.IntFromString(list[i].ToString(), "AppDataStartColumnNum:");
                    }
                    else if (list[i].ToString().Contains("AppDataEndColumnNum:"))
                    {
                        AppDataEndColumnNum = Utils.IntFromString(list[i].ToString(), "AppDataEndColumnNum:");
                    }
                    else if (list[i].ToString().Contains("SystemNumColumn:"))
                    {
                        SystemNumColumn = Utils.StrFromString(list[i].ToString(), "SystemNumColumn:");
                    }
                    else if (list[i].ToString().Contains("SystemNameColumn:"))
                    {
                        SystemNameColumn = Utils.StrFromString(list[i].ToString(), "SystemNameColumn:");
                    }
                    else if (list[i].ToString().Contains("SystemCodeColumn:"))
                    {
                        SystemCodeColumn = Utils.StrFromString(list[i].ToString(), "SystemCodeColumn:");
                    }
                    else if (list[i].ToString().Contains("StationNumberColumn:"))
                    {
                        StationNumberColumn = Utils.StrFromString(list[i].ToString(), "StationNumberColumn:");
                    }
                    else if (list[i].ToString().Contains("RobotNameColumn:"))
                    {
                        RobotNameColumn = Utils.StrFromString(list[i].ToString(), "RobotNameColumn:");
                    }
                    else if (list[i].ToString().Contains("RobotTypesMethod:"))
                    {
                        TypeMethod = Utils.StrFromString(list[i].ToString(), "RobotTypesMethod:").Equals("NamesInColums") ? RobTypeMethod.NamesInColums : RobTypeMethod.RangeColumns;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "ReadMatrixMap");
                return false;
            }
            return true;
        }

        public static bool ReadToolData(string FilePath, ref List<Vector3> J4vals, ref List<Vector3> J4angs, ref List<Vector3> J5vals, ref List<Vector3> J5angs, ref List<Vector3> J6vals, ref List<Vector3> J6angs, ref List<Vector3> EOATvals, ref List<Vector3> EOATangs, ref ArrayList Problems, Brand Kind)
        {
            var row = 2;
            var flag = false;
            var strArray = new string[] { "B", "C", "D", "J", "K", "L", "R", "S", "T" };
            var strArray2 = new string[] { "E", "F", "G", "M", "N", "O", "U", "V", "W" };
            var strArray3 = new string[] { "Z", "AA", "AB" };
            var strArray4 = new string[] { "AC", "AD", "AE" };
            if (!OpenExcelFile(FilePath, false))
            {
                return false;
            }
            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet) TrackerWorksheets.get_Item(2);
            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;
            try
            {
                bool flag2;
                var v = new Vector3();
                var vector2 = new Vector3();
                do
                {
                    flag2 = false;
                    if ((((GetDoubleValAtRowCol(strArray[0], row, ref v.x) && GetDoubleValAtRowCol(strArray[1], row, ref v.y)) && (GetDoubleValAtRowCol(strArray[2], row, ref v.z) && GetDoubleValAtRowCol(strArray2[0], row, ref vector2.x))) && GetDoubleValAtRowCol(strArray2[1], row, ref vector2.y)) && GetDoubleValAtRowCol(strArray2[2], row, ref vector2.z))
                    {
                        J4vals.Add(new Vector3(v));
                        J4angs.Add(new Vector3(vector2));
                        flag = flag2 = true;
                    }
                    if ((((GetDoubleValAtRowCol(strArray[3], row, ref v.x) && GetDoubleValAtRowCol(strArray[4], row, ref v.y)) && (GetDoubleValAtRowCol(strArray[5], row, ref v.z) && GetDoubleValAtRowCol(strArray2[3], row, ref vector2.x))) && GetDoubleValAtRowCol(strArray2[4], row, ref vector2.y)) && GetDoubleValAtRowCol(strArray2[5], row, ref vector2.z))
                    {
                        J5vals.Add(new Vector3(v));
                        J5angs.Add(new Vector3(vector2));
                        flag = flag2 = true;
                    }
                    if ((((GetDoubleValAtRowCol(strArray[6], row, ref v.x) && GetDoubleValAtRowCol(strArray[7], row, ref v.y)) && (GetDoubleValAtRowCol(strArray[8], row, ref v.z) && GetDoubleValAtRowCol(strArray2[6], row, ref vector2.x))) && GetDoubleValAtRowCol(strArray2[7], row, ref vector2.y)) && GetDoubleValAtRowCol(strArray2[8], row, ref vector2.z))
                    {
                        J6vals.Add(new Vector3(v));
                        J6angs.Add(new Vector3(vector2));
                        flag = flag2 = true;
                    }
                    row++;
                }
                while (flag2);
                var val = 0.0;
                var num4 = 0.0;
                for (row = EoatDefStartRow; row < EoatDefEndRow; row++)
                {
                    var vector3 = new Vector(3);
                    var vector4 = new Vector(3);
                    var flag3 = true;
                    for (var i = 0; i < 3; i++)
                    {
                        flag3 &= GetDoubleValAtRowCol(strArray3[i], row, ref val) && GetDoubleValAtRowCol(strArray4[i], row, ref num4);
                        if (!flag3)
                        {
                            break;
                        }
                        vector3.Vec[i] = val;
                        vector4.Vec[i] = num4;
                    }
                    if (flag3)
                    {
                        v = new Vector3(vector3);
                        vector2 = new Vector3(vector4);
                    }
                    else
                    {
                        v = new Vector3();
                        vector2 = new Vector3();
                    }
                    EOATvals.Add(new Vector3(v));
                    EOATangs.Add(new Vector3(vector2));
                }
                CloseTrackerData();
            }
            catch (Exception exception)
            {
                Problems.Add(exception.Message);
                return false;
            }
            if (!flag)
            {
                Problems.Add("No joint radial data was found");
            }
            return flag;
        }

        private static void TrackerDataoXL_WindowDeactivate(Workbook Wb, Window Wn)
        {
            TrackerDataSaving = false;
            TrackerDataExcelOpen = false;
        }

        private static void TrackerDataoXL_WorkbookBeforeSave(Workbook Wb, bool SaveAsUI, ref bool Cancel)
        {
            TrackerDataSaving = true;
        }

        private static void TrackerDataoXL_WorkbookDeactivate(Workbook Wb)
        {
            if (!ClosingTrackerDataForRead)
            {
                NAR(TrackerDataoWb);
            }
            TrackerDataoWb = null;
            if (ClosingTrackerDataForRead)
            {
                NAR(TrackerWorksheets);
                TrackerWorksheets = null;
            }
            TrackerDataoXL.Quit();
            NAR(TrackerDataoXL);
            TrackerDataoXL = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static bool TryOpenMatrix()
        {
            if (!MatrixOpen)
            {
                if ((UserPreferences.RobotMatrixFileName == null) || UserPreferences.RobotMatrixFileName.Length.Equals(0))
                {
                    return false;
                }
                if (!File.Exists(UserPreferences.RobotMatrixFileName))
                {
                    MessageBox.Show("Error: Selected Excel Robot Matrix file '" + UserPreferences.RobotMatrixFileName + "' was not found!!");
                    return false;
                }
                if (!ReadMatrixMap())
                {
                    return false;
                }
                #if !WPF
                MatrixoXL = new ApplicationClass();
                MatrixoXL.WindowDeactivate+=MatrixoXL_WindowDeactivate;
                MatrixoXL.WorkbookDeactivate+=MatrixoXL_WorkbookDeactivate;

                MatrixoWb = MatrixoXL.Workbooks.Open(UserPreferences.RobotMatrixFileName, 0, false, 5, "", "", true, (XlPlatform)2, "", false, false, 0, false, false, false);
                MatrixWorksheets = MatrixoWb.Worksheets;
                MatrixMatSheet = (Worksheet) MatrixWorksheets.get_Item(1);
                MatrixoXL.DisplayAlerts = false;
                MatrixoWb.Saved = true;
                MatrixOpen = true;
    #else
                #warning fix this
                #endif
            }
            return true;
        }

        public static bool WriteCar0BaseData(string FilePath, List<Vector3> BaseData, ref ArrayList Problems)
        {
            try
            {
                if (BaseData.Count > 0)
                {
                    if (!OpenExcelFile(FilePath, false))
                    {
                        return false;
                    }
                    TrackerWorksheets = TrackerDataoWb.Worksheets;
                    TrackerMatSheet = (Worksheet) TrackerWorksheets.get_Item(1);
                    TrackerDataoXL.DisplayAlerts = false;
                    TrackerDataoWb.Saved = true;
                    TrackerDataExcelOpen = true;
                    var num = 0;
                    string str = null;
                    for (var i = 0; i < BaseData.Count; i++)
                    {
                        num = i + 8;
                        str = "B" + num.ToString();
                        TrackerMatSheet.get_Range(str, str).Value2=BaseData[i].x;
                        str = "C" + num.ToString();
                        TrackerMatSheet.get_Range(str, str).Value2=BaseData[i].y;
                        str = "D" + num.ToString();
                        TrackerMatSheet.get_Range(str, str).Value2=BaseData[i].z;
                    }
                    CloseTrackerData(true, FilePath);
                }
            }
            catch (Exception exception)
            {
                Problems.Add(exception.Message);
                return false;
            }
            return true;
        }

        public static bool WriteJ456Data(string FilePath, List<Vector> J4Data, List<Vector> J5Data, List<Vector> J6Data, ref ArrayList Problems)
        {
            try
            {
                if (((J4Data.Count > 2) && (J5Data.Count > 2)) && (J5Data.Count > 2))
                {
                    int num2;
                    int num3;
                    double num4;
                    if (!OpenExcelFile(FilePath, false))
                    {
                        return false;
                    }
                    TrackerWorksheets = TrackerDataoWb.Worksheets;
                    TrackerMatSheet = (Worksheet) TrackerWorksheets.get_Item(2);
                    TrackerDataoXL.DisplayAlerts = false;
                    TrackerDataoWb.Saved = true;
                    TrackerDataExcelOpen = true;
                    var num = 0;
                    string str = null;
                    for (num2 = 0; num2 < J4Data.Count; num2++)
                    {
                        num = num2 + 2;
                        num3 = 0;
                        while (num3 < 3)
                        {
                            str = MeasColNames[4 + num3] + num.ToString();
                            num4 = J4Data[num2].Vec[num3 + 3];
                            TrackerMatSheet.get_Range(str, str).Value2 = num4.ToString();
                            num3++;
                        }
                    }
                    for (num2 = 0; num2 < J5Data.Count; num2++)
                    {
                        num = num2 + 2;
                        num3 = 0;
                        while (num3 < 3)
                        {
                            str = MeasColNames[12 + num3] + num.ToString();
                            num4 = J5Data[num2].Vec[num3 + 3];
                            TrackerMatSheet.get_Range(str, str).Value2 = num4.ToString();
                            num3++;
                        }
                    }
                    for (num2 = 0; num2 < J6Data.Count; num2++)
                    {
                        num = num2 + 2;
                        num3 = 0;
                        while (num3 < 3)
                        {
                            str = MeasColNames[20 + num3] + num.ToString();
                            num4 = J6Data[num2].Vec[num3 + 3];
                            TrackerMatSheet.get_Range(str, str).Value2=num4.ToString();
                            num3++;
                        }
                    }
                    num = 2;
                    for (num3 = 0; num3 < 3; num3++)
                    {
                        str = MeasColNames[0x1c + num3] + num.ToString();
                        num4 = J6Data[J6Data.Count - 1].Vec[num3 + 3];
                        TrackerMatSheet.get_Range(str, str).Value2=num4.ToString();
                    }
                    CloseTrackerData(true, FilePath);
                }
            }
            catch (Exception exception)
            {
                Problems.Add(exception.Message);
                return false;
            }
            return true;
        }

        public static bool WriteToolDefinitions(string FilePath, List<Vector3> ToolData, ref ArrayList Problems)
        {
            try
            {
                int num;
                var flag = false;
                for (num = 0; num < ToolData.Count; num++)
                {
                    if (!((ToolData[num].x.Equals((double) 0.0) && ToolData[num].y.Equals((double) 0.0)) && ToolData[num].z.Equals((double) 0.0)))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    if (!OpenExcelFile(FilePath, false))
                    {
                        return false;
                    }
                    TrackerWorksheets = TrackerDataoWb.Worksheets;
                    TrackerMatSheet = (Worksheet) TrackerWorksheets.get_Item(1);
                    TrackerDataoXL.DisplayAlerts = false;
                    TrackerDataoWb.Saved = true;
                    TrackerDataExcelOpen = true;
                    var num2 = 0;
                    string str = null;
                    for (num = 0; num < ToolData.Count; num++)
                    {
                        if (!((ToolData[num].x.Equals((double) 0.0) && ToolData[num].y.Equals((double) 0.0)) && ToolData[num].z.Equals((double) 0.0)))
                        {
                            num2 = num + 0x15;
                            str = "B" + num2.ToString();
                            TrackerMatSheet.get_Range(str, str).Value2 = ToolData[num].x;
                            str = "C" + num2.ToString();
                            TrackerMatSheet.get_Range(str, str).Value2 = ToolData[num].y;
                            str = "D" + num2.ToString();
                            TrackerMatSheet.get_Range(str, str).Value2 = ToolData[num].z;
                        }
                    }
                    CloseTrackerData(true, FilePath);
                }
            }
            catch (Exception exception)
            {
                Problems.Add(exception.Message);
                return false;
            }
            return true;
        }

        private enum RobTypeMethod
        {
            NamesInColums,
            RangeColumns
        }
    }
}

