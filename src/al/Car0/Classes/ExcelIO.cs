using System;
using System.Collections.Generic;
using System.Collections;

using System.Text;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Runtime.InteropServices;

namespace Car0
{
    public class ExcelIO
    {

        private object parent = null;

        public ExcelIO() { }
        public ExcelIO(object Parent)
        {
            this.parent = Parent;
        }
        #region ExcelIO Variables

        public event NotifyMessageEventHandler NotifyMessage;
        private void raiseNotify(string message, string title)
        {
            if (NotifyMessage!=null)
                NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
        }

        enum RobTypeMethod { NamesInColums = 0, RangeColumns = 1 };

        private  int MatrixFileStartRow = 6;
        private  string SystemNumColumn = "A";
        private  string SystemNameColumn = "B";
        private  string SystemCodeColumn = "";
        private  string StationNumberColumn = "";
        private  string RobotNameColumn = "G";  // "E" for B515, "G" for FLEX-N-GATE

        private  int EoatDefStartRow = 2;
        private  int EoatDefEndRow = 16;

        private  RobTypeMethod TypeMethod = RobTypeMethod.NamesInColums; 

        private  int HeadingsRow = 3;
        private  int RobotTypesStartColumnNum = 19;
        private  int RobotTypesEndColumnNum = 28;
        private  int AppDataStartColumnNum = 16;
        private  int AppDataEndColumnNum = 29;

        private  string[] MeasColNames = new string[] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                           "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
                                           "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
                                           "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ"};


        public  ArrayList RobotTypes = new ArrayList();
        public  ArrayList RobotAppTypes = new ArrayList();

        private  Boolean MatrixOpen = false, ClosingMatrixForRead = false;
        private  Microsoft.Office.Interop.Excel.Application MatrixoXL = null;
        private  Workbook MatrixoWb = null;
        private  Microsoft.Office.Interop.Excel.Sheets MatrixWorksheets = null;
        private  Microsoft.Office.Interop.Excel.Worksheet MatrixMatSheet = null;

        private  Microsoft.Office.Interop.Excel.Application TrackerDataoXL = null;
        private  Workbook TrackerDataoWb = null;
        private  Microsoft.Office.Interop.Excel.Sheets TrackerWorksheets = null;
        private  Microsoft.Office.Interop.Excel.Worksheet TrackerMatSheet = null;
        public  Boolean TrackerDataExcelOpen = false;
      //  private  Boolean TrackerDataSaving = false;
        private  Boolean ClosingTrackerDataForRead = false;
        private  string LastTrackerDataFile = null;

        #endregion

        private string ProgStringCell(string Column, int Row, Worksheet MatSheet)
        {
            Range oRng;
            string rc = null, mesbuf = null;

            if (Column != null)
            {
                mesbuf = Column + Row.ToString();

                oRng = MatSheet.get_Range(mesbuf, mesbuf);

                if (oRng.Value2 != null)
                    rc = oRng.Value2.ToString();
            }

            return rc;
        }

        public  Boolean Read123Data(ref double[] Origin, ref double[] Xref, ref double[] Yref)
        {
            string Fpath = Path.Combine(UserPreferences.WorkFolderName,UserPreferences.Excel321FileName);

            if (!File.Exists(@Fpath))
            {
                raiseNotify(String.Format("Selected Excel 3-2-1 file '{0}' was not found!!",UserPreferences.Excel321FileName),"Error");
                return false;
            }

            try
            {
                
                Microsoft.Office.Interop.Excel.Application oXL = new Microsoft.Office.Interop.Excel.Application();
                Workbook oWb = oXL.Workbooks.Open(Fpath, 0, true, 5, "", "", true, XlPlatform.xlWindows, "", false, false, 0, false, false, false);

                Microsoft.Office.Interop.Excel.Sheets Worksheets = oXL.Worksheets;
                Microsoft.Office.Interop.Excel.Worksheet MatSheet = (Worksheet)Worksheets.get_Item(1);

                oXL.DisplayAlerts = false;
                oWb.Saved = true;

                Origin[0] = Convert.ToDouble(ProgStringCell(UserPreferences.F321Xcolumn, UserPreferences.F321OriginRow, MatSheet));
                Origin[1] = Convert.ToDouble(ProgStringCell(UserPreferences.F321Ycolumn, UserPreferences.F321OriginRow, MatSheet));
                Origin[2] = Convert.ToDouble(ProgStringCell(UserPreferences.F321Zcolumn, UserPreferences.F321OriginRow, MatSheet));

                 Xref[0] = Convert.ToDouble(ProgStringCell(UserPreferences.F321Xcolumn, UserPreferences.F321Xrow, MatSheet));
                 Xref[1] = Convert.ToDouble(ProgStringCell(UserPreferences.F321Ycolumn, UserPreferences.F321Xrow, MatSheet));

                 Xref[2] = Convert.ToDouble(ProgStringCell(UserPreferences.F321Zcolumn, UserPreferences.F321Xrow, MatSheet));
               
                Yref[0] = Convert.ToDouble(ProgStringCell(UserPreferences.F321Xcolumn, UserPreferences.F321Yrow, MatSheet));
               
                Yref[1] = Convert.ToDouble(ProgStringCell(UserPreferences.F321Ycolumn, UserPreferences.F321Yrow, MatSheet));
                Yref[2] = Convert.ToDouble(ProgStringCell(UserPreferences.F321Zcolumn, UserPreferences.F321Yrow, MatSheet));
       
                oWb.Close(false, false, false);
                oXL.Quit();
            }
           catch (Exception err)
            {
               raiseNotify(err.ToString() , "\n\nThis is a problem with the WELD MATRIX file selected!!!\n\n");
                return false;
            }

            return true;
        }

        private FileInfo matrixfile;
        public FileInfo MatrixFile
        {
            get { return matrixfile; }
            set { matrixfile = value; }
        }

        private Boolean ReadMatrixMap()
        {
            try
            {
                string TextFileName = null;

                if (UserPreferences.RobotMatrixFileName.Contains(".xlsx"))
                    TextFileName = UserPreferences.RobotMatrixFileName.Replace(".xlsx", ".txt");
                else if (UserPreferences.RobotMatrixFileName.Contains(".xls"))
                    TextFileName = UserPreferences.RobotMatrixFileName.Replace(".xls", ".txt");

                if (TextFileName == null)
                {
                    raiseNotify("Illegal .extension for Matrix file name = '" + UserPreferences.RobotMatrixFileName + "'", "ReadMatrixMap");
                    return false;
                }

                ArrayList MapLines = Utils.FileToArrayList(TextFileName);
                int i;

                for (i = 0; i < MapLines.Count; ++i)
                {
                    if (MapLines[i].ToString().Contains("MatrixFileStartRow:"))
                        MatrixFileStartRow = Utils.IntFromString(MapLines[i].ToString(), "MatrixFileStartRow:");
                    else if (MapLines[i].ToString().Contains("HeadingsRow:"))
                        HeadingsRow = Utils.IntFromString(MapLines[i].ToString(), "HeadingsRow:");
                    else if (MapLines[i].ToString().Contains("EoatDefStartRow:"))
                        EoatDefStartRow = Utils.IntFromString(MapLines[i].ToString(), "EoatDefStartRow:");
                    else if (MapLines[i].ToString().Contains("EoatDefEndRow:"))
                        EoatDefEndRow = Utils.IntFromString(MapLines[i].ToString(), "EoatDefEndRow:");
                    else if (MapLines[i].ToString().Contains("RobotTypesStartColumnNum:"))
                        RobotTypesStartColumnNum = Utils.IntFromString(MapLines[i].ToString(), "RobotTypesStartColumnNum:");
                    else if (MapLines[i].ToString().Contains("RobotTypesEndColumnNum:"))
                        RobotTypesEndColumnNum = Utils.IntFromString(MapLines[i].ToString(), "RobotTypesEndColumnNum:");
                    else if (MapLines[i].ToString().Contains("AppDataStartColumnNum:"))
                        AppDataStartColumnNum = Utils.IntFromString(MapLines[i].ToString(), "AppDataStartColumnNum:");
                    else if (MapLines[i].ToString().Contains("AppDataEndColumnNum:"))
                        AppDataEndColumnNum = Utils.IntFromString(MapLines[i].ToString(), "AppDataEndColumnNum:");
                    else if (MapLines[i].ToString().Contains("SystemNumColumn:"))
                        SystemNumColumn = Utils.StrFromString(MapLines[i].ToString(), "SystemNumColumn:");
                    else if (MapLines[i].ToString().Contains("SystemNameColumn:"))
                        SystemNameColumn = Utils.StrFromString(MapLines[i].ToString(), "SystemNameColumn:");
                    else if (MapLines[i].ToString().Contains("SystemCodeColumn:"))
                        SystemCodeColumn = Utils.StrFromString(MapLines[i].ToString(), "SystemCodeColumn:");
                    else if (MapLines[i].ToString().Contains("StationNumberColumn:"))
                        StationNumberColumn = Utils.StrFromString(MapLines[i].ToString(), "StationNumberColumn:");
                    else if (MapLines[i].ToString().Contains("RobotNameColumn:"))
                        RobotNameColumn = Utils.StrFromString(MapLines[i].ToString(), "RobotNameColumn:");
                    else if (MapLines[i].ToString().Contains("RobotTypesMethod:"))
                    {
                        string mesbuf = Utils.StrFromString(MapLines[i].ToString(), "RobotTypesMethod:");

                        TypeMethod = mesbuf.Equals("NamesInColums") ? RobTypeMethod.NamesInColums : RobTypeMethod.RangeColumns;
                    }
                }

            }
            catch (Exception d)
            {
                raiseNotify(d.Message, "ReadMatrixMap");
                return false;
            }

            return true;
        }

        private Boolean TryOpenMatrix()
        {

            FileInfo fi = new FileInfo(@UserPreferences.RobotMatrixFileName);

            // Prevent from corrupting file
            if (!File.Exists(fi.FullName))
                throw new ExcelMatrixException(String.Format("Error: Selected Excel Robot Matric file '{0}' was not found!",fi.FullName));

            if (fi.Length.Equals(0))
                throw new ExcelMatrixException(String.Format("Error: Selected Excel Robot Matrix File '{0}'contains no data", fi.FullName));

            //Is is already open
            if (!MatrixOpen)
            {
                if ((UserPreferences.RobotMatrixFileName == null) || UserPreferences.RobotMatrixFileName.Length.Equals(0))
                    return false;

                if (!File.Exists(@UserPreferences.RobotMatrixFileName))
                {
                    raiseNotify("Selected Excel Robot Matrix file '" + UserPreferences.RobotMatrixFileName + "' was not found!!","Error");
                    return false;
                }

                if (!ReadMatrixMap())
                    return false;

                MatrixoXL = new Microsoft.Office.Interop.Excel.Application();
                MatrixoXL.WorkbookDeactivate += new AppEvents_WorkbookDeactivateEventHandler(MatrixoXL_WorkbookDeactivate);
                MatrixoXL.WindowDeactivate += new AppEvents_WindowDeactivateEventHandler(MatrixoXL_WindowDeactivate);
                MatrixoWb = MatrixoXL.Workbooks.Open(UserPreferences.RobotMatrixFileName, 0, false, 5, "", "", true, XlPlatform.xlWindows, "", false, false, 0, false, false, false);

                MatrixWorksheets = MatrixoWb.Worksheets;
                MatrixMatSheet = (Worksheet)MatrixWorksheets.get_Item(1);

                MatrixoXL.DisplayAlerts = false;
                MatrixoWb.Saved = true;
                MatrixOpen = true;
            }

            return true;
        }

         void MatrixoXL_WindowDeactivate(Workbook Wb, Window Wn)
        {
            MatrixOpen = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect")]
         void MatrixoXL_WorkbookDeactivate(Workbook Wb)
        {
            if (!ClosingMatrixForRead)
                NAR(MatrixoWb);

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

        public  void CloseMatrix()
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
                    MatrixoWb.Close(false, false, false);

                ClosingMatrixForRead = false;
                MatrixOpen = false;
            }
        }

        private  void NAR(object O)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(O);
            }
            catch (Exception)
            {
            }
            finally
            {
                O = null;
            }
        }
        public  Boolean BuildMatyrixSystemList(ref ArrayList SystemNames, ref ArrayList SystemCodes)
        {
            string mesbuf = null, SysNum = null, SysName = null, SysCode = null;

            //Initialize the robot data arrays
            RobotTypes = new ArrayList();
            RobotAppTypes = new ArrayList();
            SystemCodes = new ArrayList();

            if (TryOpenMatrix() && ReadMatrixMap())
            {
                try
                {
                    int RowNum = MatrixFileStartRow;     //Hard coded TODO un-hard code
                    Boolean done = false;

                    while (!done)
                    {
                        SysNum = ProgStringCell(SystemNumColumn, RowNum, MatrixMatSheet);
                        SysName = ProgStringCell(SystemNameColumn, RowNum, MatrixMatSheet);

                        if (SystemCodeColumn.Length > 0)
                            SysCode = ProgStringCell(SystemCodeColumn, RowNum, MatrixMatSheet);

                        if ((SysNum == null) || (SysName == null))
                            done = true;
                        else
                        {
                            mesbuf = SysName + "(" + SysNum + ")";

                            if (!SystemNames.Contains(mesbuf))
                            {
                                SystemNames.Add(mesbuf);
                                SystemCodes.Add(SysCode);
                            }
                        }

                        ++RowNum;
                    }

                }
                catch (Exception err)
                {
                    raiseNotify(err.ToString() ,"\n\nThis is a problem with the ROBOT MATRIX file selected!!!\n\n");
                    return false;
                }

                return true;
            }

            return false;
        }

        public  Boolean BuildMatrixRobotList(ref ArrayList RobotNames, ref ArrayList RobotMechanismNames,string TargetSystemName)
        {
            string mesbuf = null, RobotName = null, RobotMechanismName = null, SysNum = null, SysName = null;

            if (!CheckBuildRobotDataArrays())
                return false;

            if (TryOpenMatrix())
            {
                try
                {
                    int RowNum = MatrixFileStartRow;     //Hard coded TODO un-hard code
                    Boolean done = false;

                    RobotNames = new ArrayList();

                    while (!done)
                    {
                        SysNum = ProgStringCell(SystemNumColumn, RowNum, MatrixMatSheet);
                        SysName = ProgStringCell(SystemNameColumn, RowNum, MatrixMatSheet);
                        RobotName = ProgStringCell(RobotNameColumn, RowNum, MatrixMatSheet);
                        RobotMechanismName = GetRobotMechanismName(RowNum);

                        if ((SysNum == null) || (SysName == null) || (RobotName == null))
                            done = true;
                        else
                        {
                            mesbuf = SysName + "(" + SysNum + ")";

                            if (TargetSystemName.Equals(mesbuf) && !RobotNames.Contains(RobotName))
                            {
                                if (RobotName == null)
                                    RobotNames.Add("UNKNOWN");
                                else
                                    RobotNames.Add(RobotName);

                                if (RobotMechanismName == null)
                                    RobotMechanismNames.Add("UNKNOWN");
                                else
                                    RobotMechanismNames.Add(RobotMechanismName);
                            }
                        }

                        ++RowNum;
                    }
                }
                catch (Exception err)
                {
                   raiseNotify(err.ToString() , "\n\nThis is a problem with the ROBOT MATRIX file selected!!!\n\n");
                    return false;
                }

                return true;
            }

            return false;
        }

        private  Boolean CheckBuildRobotDataArrays()
        {
            if (RobotTypes.Count == 0)
            {
                int i;
                string mesbuf = null;

                if (TryOpenMatrix())
                {
                    try
                    {
                        if (TypeMethod.Equals(RobTypeMethod.RangeColumns))
                        {
                            for (i = RobotTypesStartColumnNum; i <= RobotTypesEndColumnNum; ++i)
                            {
                                mesbuf = ProgStringCell(MeasColNames[i], HeadingsRow, MatrixMatSheet);
                                RobotTypes.Add(mesbuf);
                            }
                        }

                        for (i = AppDataStartColumnNum; i <= AppDataEndColumnNum; ++i)
                        {
                            mesbuf = ProgStringCell(MeasColNames[i], HeadingsRow, MatrixMatSheet);
                            RobotAppTypes.Add(mesbuf);
                        }
                    }
                    catch (Exception err)
                    {
                       raiseNotify(err.ToString() , "\n\nThis is a problem with the ROBOT MATRIX file selected!!!\n\n");
                        return false;
                    }
                    return true;
                }

                return false;
            }

            return true;
        }

        private  string GetRobotMechanismName(int RobotRow)
        {
            string MechanismName = "UNKNOWN";

            if (TypeMethod.Equals(RobTypeMethod.NamesInColums))
                MechanismName = ProgStringCell(MeasColNames[RobotTypesStartColumnNum], RobotRow, MatrixMatSheet);
            else
            {
                int i;
                string mesbuf = null;

                for (i = RobotTypesStartColumnNum; i <= RobotTypesEndColumnNum; ++i)
                {
                    mesbuf = ProgStringCell(MeasColNames[i], RobotRow, MatrixMatSheet);

                    if ((mesbuf != null) && mesbuf.Equals("1"))
                    {
                        MechanismName = (i < RobotTypes.Count) ? RobotTypes[i - RobotTypesStartColumnNum].ToString() : "UNKNOWN";
                        break;
                    }
                }
            }

            return MechanismName;
        }

        public  Boolean GetRobotData(string TargetSystemName, string TargetRobot, ref string RobotMechanismType, ref string StationName, ref ArrayList RobotApps)
        {
            if (!CheckBuildRobotDataArrays())
                return false;

            string mesbuf = null, RobotName = null, SysNum = null, SysName = null;


            if (TryOpenMatrix())
            {
                int RobotRow = MatrixFileStartRow;     //Hard coded TODO un-hard code

                try
                {
                    //Attempt to find the robot row
                    int i = 0, MyColumnNum = 0;
                    Boolean done = false, Found = false;

                    while (!done)
                    {
                        SysNum = ProgStringCell(SystemNumColumn, RobotRow, MatrixMatSheet);
                        SysName = ProgStringCell(SystemNameColumn, RobotRow, MatrixMatSheet);
                        RobotName = ProgStringCell(RobotNameColumn, RobotRow, MatrixMatSheet);

                        if ((SysNum == null) || (SysName == null) || (RobotName == null))
                            done = true;
                        else
                        {
                            mesbuf = SysName + "(" + SysNum.ToString() + ")";

                            if (TargetSystemName.Equals(mesbuf) && RobotName.Equals(TargetRobot))
                            {
                                Found = true;
                                break;
                            }
                        }

                        ++RobotRow;
                    }

                    //Did we find it?
                    if (!Found)
                    {
                        raiseNotify("Target robot '" + TargetRobot + "' not found in Robot Matrix","Error");
                        return false;
                    }

                    //Try to find the robot type
                    Found = false;

                    RobotMechanismType = GetRobotMechanismName(RobotRow);

                    StationName = (StationNumberColumn.Length > 0) ? ProgStringCell(StationNumberColumn, RobotRow, MatrixMatSheet) : "Unknown";

                    //Build the robot application list
                    RobotApps = new ArrayList();
                    i = 0;

                    for (MyColumnNum = AppDataStartColumnNum; MyColumnNum <= AppDataEndColumnNum; ++MyColumnNum)
                    {
                        mesbuf = ProgStringCell(MeasColNames[MyColumnNum], RobotRow, MatrixMatSheet);

                        if ((mesbuf != null) && (mesbuf.Equals("1") || mesbuf.Equals("2") || mesbuf.Equals("3") || mesbuf.Equals("4") || mesbuf.Equals("5")))
                        {
                            int k, n = Convert.ToInt32(mesbuf);

                            for (k=0; k<n; ++k)
                                RobotApps.Add(RobotAppTypes[i].ToString());
                        }

                        ++i;
                    }

                }
                catch (Exception err)
                {
                   raiseNotify(err.ToString() , "\n\nThis is a problem with the ROBOT MATRIX file selected!!!\n\n");
                    return false;
                }
                return true;
            }

            return false;
        }

        public  Boolean OpenExcelFile(string FilePath,Boolean makevisible)
        {
            if (!TrackerDataExcelOpen)
            {
                if (!File.Exists(@FilePath))
                {
                    raiseNotify("Selected Excel Tracker Data file '" + FilePath + "' was not found!!","Error");
                    return false;
                }

                if (TrackerDataExcelOpen)
                {
                    raiseNotify("The following Tracker Data file is still open:\n\n" + LastTrackerDataFile + "\n\nPlease close this (saving if you wish) and try again.","Warning");
                    return false;
                }

                if (TrackerDataoXL != null)
                {
                    TrackerDataoXL.Quit();

                    //Wait for exit
                    while (TrackerDataExcelOpen)
                    {
                        Thread.Sleep(10);
                    }

                }

                TrackerDataoXL = new Microsoft.Office.Interop.Excel.Application();

                TrackerDataoXL.WorkbookBeforeSave += new AppEvents_WorkbookBeforeSaveEventHandler(TrackerDataoXL_WorkbookBeforeSave);
                TrackerDataoXL.WindowDeactivate += new AppEvents_WindowDeactivateEventHandler(TrackerDataoXL_WindowDeactivate);
                TrackerDataoXL.WorkbookDeactivate += new AppEvents_WorkbookDeactivateEventHandler(TrackerDataoXL_WorkbookDeactivate);
                TrackerDataoWb = TrackerDataoXL.Workbooks.Open(@FilePath, 0, false, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, false, false, 0);

                TrackerDataoXL.Visible = makevisible;
                TrackerDataExcelOpen = true;

                LastTrackerDataFile = FilePath;
            }

            return true;
        }

         void TrackerDataoXL_WindowDeactivate(Workbook Wb, Window Wn)
        {
  //          TrackerDataSaving = false;
            TrackerDataExcelOpen = false;
        }

         void TrackerDataoXL_WorkbookBeforeSave(Workbook Wb, bool SaveAsUI, ref bool Cancel)
        {
//            TrackerDataSaving = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect")]
        private void TrackerDataoXL_WorkbookDeactivate(Workbook Wb)
        {
            if (!ClosingTrackerDataForRead)
                NAR(TrackerDataoWb);

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

        public  void CloseTrackerData(Boolean Saveit, string FilePath)
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

        public  void CloseTrackerData()
        {
            CloseTrackerData(false, null);
        }
    
        private  Boolean DoubleDataValid(string Col, int Row, Microsoft.Office.Interop.Excel.Worksheet Sheet)
        {
            try
            {
                string mesbuf = ProgStringCell(Col,Row,Sheet);

                if ((mesbuf == null) || (mesbuf.Length == 0))
                    return false;

                double x = Convert.ToDouble(mesbuf);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public  Boolean CheckTrackerFile(string FilePath, ref ArrayList Problems, Brand Kind)
        {
            if (!OpenExcelFile(@FilePath, false))
                return false;

            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet)TrackerWorksheets.get_Item(1);

            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;

            Problems = new ArrayList();

            try
            {
                //If Checking presense of Robot 321 fit points
                if (UserPreferences.OperationRadioButtonSelected == 1)
                {
                    if (!(DoubleDataValid(UserPreferences.F321Xcolumn, UserPreferences.F321Xrow, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.F321Ycolumn, UserPreferences.F321Xrow, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.F321Zcolumn, UserPreferences.F321Xrow, TrackerMatSheet)))
                        Problems.Add("Robot 321 Fit points: Xref row not completely defined");

                    if (!(DoubleDataValid(UserPreferences.F321Xcolumn, UserPreferences.F321Yrow, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.F321Ycolumn, UserPreferences.F321Yrow, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.F321Zcolumn, UserPreferences.F321Yrow, TrackerMatSheet)))
                        Problems.Add("Robot 321 Fit points: Yref row not completely defined");

                    if (!(DoubleDataValid(UserPreferences.F321Xcolumn, UserPreferences.F321OriginRow, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.F321Ycolumn, UserPreferences.F321OriginRow, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.F321Zcolumn, UserPreferences.F321OriginRow, TrackerMatSheet)))
                        Problems.Add("Robot 321 Fit points: OriginRef row not completely defined");
                }
                else if (UserPreferences.OperationRadioButtonSelected == 2)
                {
                    if (!(DoubleDataValid(UserPreferences.FpointsInitXcolumn, UserPreferences.FpointsInitP1Row, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.FpointsInitYcolumn, UserPreferences.FpointsInitP1Row, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.FpointsInitZcolumn, UserPreferences.FpointsInitP1Row, TrackerMatSheet)))
                        Problems.Add("Initial wrt Robot World: Base pt1 row not completely defined");

                    if (!(DoubleDataValid(UserPreferences.FpointsInitXcolumn, UserPreferences.FpointsInitP2Row, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.FpointsInitYcolumn, UserPreferences.FpointsInitP2Row, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.FpointsInitZcolumn, UserPreferences.FpointsInitP2Row, TrackerMatSheet)))
                        Problems.Add("Initial wrt Robot World: Base pt2 row not completely defined");

                    if (!(DoubleDataValid(UserPreferences.FpointsInitXcolumn, UserPreferences.FpointsInitP3Row, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.FpointsInitYcolumn, UserPreferences.FpointsInitP3Row, TrackerMatSheet) &&
                        DoubleDataValid(UserPreferences.FpointsInitZcolumn, UserPreferences.FpointsInitP3Row, TrackerMatSheet)))
                        Problems.Add("Initial wrt Robot World: Base pt3 row not completely defined");

                    int i, j, k;
                    Boolean Fountit = false;

                    //Loop through all RobotData.RobotFramesChecked
                    for (i = 0; i < RobotData.RobotFramesChecked.Count; ++i)
                    {
                        Fountit = false;

                        if (RobotData.RobotFramesChecked[i].Equals(true))
                        {
                            //Look for supported base data types
                            for (j = 0; j < 8; ++j)
                            {
                                if (RobotData.RobotFrames[i].ToString().Contains(Kind.BaseDataName(j+1)))
                                {
                                    for (k = 0; k < 3; ++k)         //Just need to have the first 3 (minimum) defined
                                    {
                                        if (!(DoubleDataValid(MeasColNames[UserPreferences.FinalPointStartColNumbers[j]], UserPreferences.FpointsFinalBase1P1Row + k, TrackerMatSheet) &&
                                            DoubleDataValid(MeasColNames[UserPreferences.FinalPointStartColNumbers[j] + 1], UserPreferences.FpointsFinalBase1P1Row + k, TrackerMatSheet) &&
                                            DoubleDataValid(MeasColNames[UserPreferences.FinalPointStartColNumbers[j] + 2], UserPreferences.FpointsFinalBase1P1Row + k, TrackerMatSheet)))
                                            Problems.Add("Final wrt Car 0 " + Kind.BaseDataName(j + 1) + ": Car0 pt" + (k + 1).ToString() + " row not completely defined");
                                    }

                                    Fountit = true;
                                }
                            }

                            if (!Fountit)
                            {
                                //Look for supported tool data types
                                for (j = 0; j < 14; ++j)
                                {
                                    if (RobotData.RobotFrames[i].ToString().Contains(Kind.ToolDataName(j+1)))
                                    {
                                        if (!(DoubleDataValid(UserPreferences.ToolDataXcolumn, UserPreferences.ToolData1Row + j, TrackerMatSheet) &&
                                            DoubleDataValid(UserPreferences.ToolDataYcolumn, UserPreferences.ToolData1Row + j, TrackerMatSheet) &&
                                            DoubleDataValid(UserPreferences.ToolDataZcolumn, UserPreferences.ToolData1Row + j, TrackerMatSheet)))
                                            Problems.Add("EOAT Definitions Wrt TOOLFRAME: TOOL_DATA[" + (j + 1).ToString() + "] row not completely defined");

                                        Fountit = true;
                                    }
                                }
                            }
                            if (!Fountit)
                            {
                                for (j = 0; j < 3; ++j)
                                {
                                    if (RobotData.RobotFrames[i].ToString().Contains(Kind.PedDataName(Kind.FirstPedFrameNumber + j)))
                                    {
                                        if (!(DoubleDataValid(UserPreferences.BaseDataXcolumn, UserPreferences.BaseData13Row + j, TrackerMatSheet) &&
                                            DoubleDataValid(UserPreferences.BaseDataYcolumn, UserPreferences.BaseData13Row + j, TrackerMatSheet) &&
                                            DoubleDataValid(UserPreferences.BaseDataZcolumn, UserPreferences.BaseData13Row + j, TrackerMatSheet)))
                                            Problems.Add("BASE Definitions wrt Robot World: " + Kind.PedDataName(Kind.FirstPedFrameNumber + j) + " row not completely defined");
                                    }

                                    Fountit = true;
                                }
                            }  
                            if (!Fountit)
                            {
                                //Unrecognized frame is being sought after
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

        private  Boolean GetColumnAndRows(string ElementName, int ElementNumber, ref int RowNum, ref string Xcol, ref string Ycol, ref string Zcol, Brand Kind)
        {
            if (ElementName.Equals("Robot 321 Fit points"))
            {
                Xcol = UserPreferences.F321Xcolumn;
                Ycol = UserPreferences.F321Ycolumn;
                Zcol = UserPreferences.F321Zcolumn;

                switch (ElementNumber)
                {
                    case 0:

                        RowNum = UserPreferences.F321Xrow;
                        return true;

                    case 1:

                        RowNum = UserPreferences.F321Yrow;
                        return true;

                    case 2:

                        RowNum = UserPreferences.F321OriginRow;
                        return true;
                }
            }
            else if (ElementName.Equals("Initial wrt Robot World"))
            {
                Xcol = UserPreferences.FpointsInitXcolumn;
                Ycol = UserPreferences.FpointsInitYcolumn;
                Zcol = UserPreferences.FpointsInitZcolumn;

                if (ElementNumber < 10)
                {
                    RowNum = UserPreferences.FpointsInitP1Row + ElementNumber;
                    return true;
                }
            }
            else     
            {
                int i;

                for (i = 0; i < 8; ++i)
                {
                    if (ElementName.Contains(Kind.BaseDataName(i + 1)))  //Was equal (3/22/12)
                    {
                        Xcol = MeasColNames[UserPreferences.FinalPointStartColNumbers[i]];
                        Ycol = MeasColNames[UserPreferences.FinalPointStartColNumbers[i] + 1];
                        Zcol = MeasColNames[UserPreferences.FinalPointStartColNumbers[i] + 2];

                        if (ElementNumber < 10)
                        {
                            RowNum = UserPreferences.FpointsFinalBase1P1Row + ElementNumber;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private  Boolean GetColumnAndRows(string ElementName, ref int RowNum, ref string Xcol, ref string Ycol, ref string Zcol, Brand Kind)
        {
            int i;

            //Process the pedestal names
            for (i = 2; i >= 0; --i)
            {
                if (ElementName.Contains(Kind.PedDataName(i + Kind.FirstPedFrameNumber)))       //Was Equals 3/22/12 :: Was Kind.ToolDataName(...)
                {
                    Xcol = UserPreferences.BaseDataXcolumn;
                    Ycol = UserPreferences.BaseDataYcolumn;
                    Zcol = UserPreferences.BaseDataZcolumn;

                    RowNum = UserPreferences.ToolData1Row + i;
                    return true;
                }
            }

            //Process tool names
            for (i = 13; i >= 0; --i)
            {
                if (ElementName.Contains(Kind.ToolDataName(i + 1)))       //Was Equals 3/22/12
                {
                    Xcol = UserPreferences.ToolDataXcolumn;
                    Ycol = UserPreferences.ToolDataYcolumn;
                    Zcol = UserPreferences.ToolDataZcolumn;

                    RowNum = UserPreferences.ToolData1Row + i;
                    return true;
                }
            }

            return false;
        }
        private  Boolean GetColumnAndRows(int index, ref int RowNum, ref string Xcol, ref string Ycol, ref string Zcol, Brand Kind)
        {
            RobotData.FrameType Ftype = RobotData.FrameTypes[index];
            int Fnum = RobotData.FrameNumbers[index];

            switch (Ftype)
            {
                case RobotData.FrameType.DropTool:
                case RobotData.FrameType.FixtureCar0:
                case RobotData.FrameType.GripperCar0:
                case RobotData.FrameType.PickTool:

                    Xcol = MeasColNames[UserPreferences.FinalPointStartColNumbers[Fnum - 1]];
                    Ycol = MeasColNames[UserPreferences.FinalPointStartColNumbers[Fnum - 1] + 1];
                    Zcol = MeasColNames[UserPreferences.FinalPointStartColNumbers[Fnum - 1] + 2];

                    break;

                case RobotData.FrameType.PedRivetGunTip:
                case RobotData.FrameType.PedScribeGunTip:
                case RobotData.FrameType.PedSealGunTip:
                case RobotData.FrameType.PedSpotGunTip:
                case RobotData.FrameType.PedStudGunTip:

                    Xcol = UserPreferences.BaseDataXcolumn;
                    Ycol = UserPreferences.BaseDataYcolumn;
                    Zcol = UserPreferences.BaseDataZcolumn;

                    RowNum = UserPreferences.ToolData1Row + Fnum - Kind.FirstPedFrameNumber;
                    break;

                case RobotData.FrameType.GripPin:

                    Xcol = UserPreferences.ToolDataXcolumn;
                    Ycol = UserPreferences.ToolDataYcolumn;
                    Zcol = UserPreferences.ToolDataZcolumn;

                    RowNum = UserPreferences.GripPinData1Row + Fnum - Kind.FirstGripPin;
                    break;

                case RobotData.FrameType.MigGunTip:
                case RobotData.FrameType.LaserTip:
                case RobotData.FrameType.PierceTip:

                    Xcol = UserPreferences.ToolDataXcolumn;
                    Ycol = UserPreferences.ToolDataYcolumn;
                    Zcol = UserPreferences.ToolDataZcolumn;

                    RowNum = UserPreferences.ToolData1Row + Fnum - Kind.FirstCarriedMIG;
                    break;

                case RobotData.FrameType.NutGunTip:
                case RobotData.FrameType.StudGunTip:
                case RobotData.FrameType.ScribeGun:         //TODO: Make FirstCarriedScriber
                case RobotData.FrameType.RivetGunTip:       //TODO: This is not assigned anywhere
                case RobotData.FrameType.VisionTcp:         //TODO: Make FirstCarriedVision

                    Xcol = UserPreferences.ToolDataXcolumn;
                    Ycol = UserPreferences.ToolDataYcolumn;
                    Zcol = UserPreferences.ToolDataZcolumn;

                    RowNum = UserPreferences.ToolData1Row + Fnum - Kind.FirstCarriedStud;
                    break;

                case RobotData.FrameType.SealGunTip:

                    Xcol = UserPreferences.ToolDataXcolumn;
                    Ycol = UserPreferences.ToolDataYcolumn;
                    Zcol = UserPreferences.ToolDataZcolumn;

                    RowNum = UserPreferences.ToolData1Row + Fnum - Kind.FirstCarriedSeal;
                    break;

                case RobotData.FrameType.SpotGunTip:

                    Xcol = UserPreferences.ToolDataXcolumn;
                    Ycol = UserPreferences.ToolDataYcolumn;
                    Zcol = UserPreferences.ToolDataZcolumn;

                    RowNum = UserPreferences.ToolData1Row + Fnum - Kind.FirstCarriedSpot;
                    break;
            }

            return true;
        }

        public  Boolean GetDoubleValAtRowCol(string Column, int Row, ref double Val)
        {
            try
            {
                string mesbuf = ProgStringCell(Column, Row, TrackerMatSheet);

                if (mesbuf == null)
                    return false;

                Val = Convert.ToDouble(ProgStringCell(Column, Row, TrackerMatSheet));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public  Boolean GetTrackerFileData(string FilePath, string ElementName, ref double[,] ElementPts, ref ArrayList Problems, Brand Kind)
        {
            int i, j, RowNum = 0;
            string[] MeasColNames = new string[3];

            if (!OpenExcelFile(@FilePath, false))
                return false;

            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet)TrackerWorksheets.get_Item(1);

            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;

            Problems = new ArrayList();

            try
            {
                for (i = 0; i < 3; ++i)
                {
                    if (!GetColumnAndRows(ElementName, i, ref RowNum, ref MeasColNames[0], ref MeasColNames[1], ref MeasColNames[2], Kind))
                    {
                        Problems.Add(ElementName + " not known.");
                        return false;
                    }

                    for (j = 0; j < 3; ++j)
                    {
                        if (!GetDoubleValAtRowCol(MeasColNames[j], RowNum, ref ElementPts[i, j]))
                        {
                            Problems.Add(ElementName + " item at column '" + MeasColNames[i] + "' row = " + RowNum.ToString() + " is not defined");
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

        public  List<Vector> GetTrackerFileData(string FilePath, string ElementName, ref ArrayList Problems, Brand Kind)
        {
            int i, j, RowNum = 0;
            string[] MeasColNames = new string[3];

            if (!OpenExcelFile(@FilePath, false))
                return null;

            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet)TrackerWorksheets.get_Item(1);

            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;

            List<Vector> ElementPts = new List<Vector>();
            double val = 0.0;

            try
            {
                Vector vals = new Vector(3);

                for (i = 0; i < 10; ++i)
                {
                    if (!GetColumnAndRows(ElementName, i, ref RowNum, ref MeasColNames[0], ref MeasColNames[1], ref MeasColNames[2], Kind))
                    {
                        Problems.Add(ElementName + " not known.");

                        return null;
                    }

                    for (j = 0; j < 3; ++j)
                    {
                        if (!GetDoubleValAtRowCol(MeasColNames[j], RowNum, ref val))
                        {
                            if (i < 3)
                            {
                                Problems.Add(ElementName + " item at column '" + MeasColNames[i] + "' row = " + RowNum.ToString() + " is not defined");
                                return null;
                            }

                            return ElementPts;      //Normal end for < 10 points
                        }
                        vals.Vec[j] = val;
                    }

                    ElementPts.Add(new Vector(vals));
                }
            }
            catch (Exception)
            {
                return null;
            }

            return ElementPts;
        }

        //Second converted
        public  Boolean GetTrackerFileData(string FilePath, int index, ref double[] ElementPts, ref ArrayList Problems, Brand Kind)
        {
            int i, RowNum = 0;
            string[] MeasColNames = new string[3];

            if (!OpenExcelFile(@FilePath, false))
                return false;

            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet)TrackerWorksheets.get_Item(1);

            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;

            //Stop removing problems 6/20/11 Problems = new ArrayList();

            try
            {
                if (!GetColumnAndRows(index, ref RowNum, ref MeasColNames[0], ref MeasColNames[1], ref MeasColNames[2], Kind))
                {
                    Problems.Add(RobotData.FrameTypes[index].ToString() + " not known.");
                    return false;
                }

                for (i = 0; i < 3; ++i)
                {
                    if (!GetDoubleValAtRowCol(MeasColNames[i], RowNum, ref ElementPts[i]))
                    {
                        Problems.Add(RobotData.FrameTypes[index].ToString() + " item at column '" + MeasColNames[i] + "' row = " + RowNum.ToString() + " is not defined");
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

        //Converting
        public  Boolean GetTrackerFileData(string FilePath, int index, ref Vector ElementPts, ref ArrayList Problems, Brand Kind)
        {
            int i, RowNum = 0;
            string[] MeasColNames = new string[3];

            if (!OpenExcelFile(@FilePath, false))
                return false;

            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet)TrackerWorksheets.get_Item(1);

            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;

            //Stop removing problems 6/20/11 Problems = new ArrayList();

            try
            {
                double val = 0.0;

                if (!GetColumnAndRows(index, ref RowNum, ref MeasColNames[0], ref MeasColNames[1], ref MeasColNames[2], Kind))
                {
                    Problems.Add(RobotData.FrameTypes[index].ToString() + " not known.");
                    return false;
                }

                for (i = 0; i < 3; ++i)
                {
                    if (!GetDoubleValAtRowCol(MeasColNames[i], RowNum, ref val))
                    {
                        Problems.Add(RobotData.FrameTypes[index].ToString() + " item at column '" + MeasColNames[i] + "' row = " + RowNum.ToString() + " is not defined");
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

        public  Boolean ReadToolData(string FilePath, ref List<Vector3> J4vals, ref List<Vector3> J4angs, ref List<Vector3> J5vals, ref List<Vector3> J5angs, ref List<Vector3> J6vals, ref List<Vector3> J6angs, ref List<Vector3> EOATvals, ref List<Vector3> EOATangs, ref ArrayList Problems, Brand Kind)
        {
            int RowNum = 2, j;
            Boolean GotAnyAtAll = false;
            string[] MeasColNames = new string[9] { "B", "C", "D", "J", "K", "L", "R", "S", "T" };
            string[] JasColNames = new string[9] { "E", "F", "G", "M", "N", "O", "U", "V", "W" };
            string[] EoatCols = new string[3] { "Z", "AA", "AB" };
            string[] EoatJasCols = new string[3] { "AC", "AD", "AE" };

            if (!OpenExcelFile(@FilePath, false))
                return false;

            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet)TrackerWorksheets.get_Item(2);     //Attempting to read 2nd worksheet

            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;

            try
            {
                Boolean GotAnySet;
                Vector3 vals = new Vector3();
                Vector3 jvals = new Vector3();

                //Process the wrist radial Data
                do
                {
                    GotAnySet = false;

                    //Check J4 set
                    if (GetDoubleValAtRowCol(MeasColNames[0], RowNum, ref vals.x) && GetDoubleValAtRowCol(MeasColNames[1], RowNum, ref vals.y) && GetDoubleValAtRowCol(MeasColNames[2], RowNum, ref vals.z) &&
                        GetDoubleValAtRowCol(JasColNames[0], RowNum, ref jvals.x) && GetDoubleValAtRowCol(JasColNames[1], RowNum, ref jvals.y) && GetDoubleValAtRowCol(JasColNames[2], RowNum, ref jvals.z))
                    {
                        J4vals.Add(new Vector3(vals));
                        J4angs.Add(new Vector3(jvals));
                        GotAnyAtAll = GotAnySet = true;
                    }

                    //Check J5 set
                    if (GetDoubleValAtRowCol(MeasColNames[3], RowNum, ref vals.x) && GetDoubleValAtRowCol(MeasColNames[4], RowNum, ref vals.y) && GetDoubleValAtRowCol(MeasColNames[5], RowNum, ref vals.z) &&
                        GetDoubleValAtRowCol(JasColNames[3], RowNum, ref jvals.x) && GetDoubleValAtRowCol(JasColNames[4], RowNum, ref jvals.y) && GetDoubleValAtRowCol(JasColNames[5], RowNum, ref jvals.z))
                    {
                        J5vals.Add(new Vector3(vals));
                        J5angs.Add(new Vector3(jvals));
                        GotAnyAtAll = GotAnySet = true;
                    }

                    //Check J6 set
                    if (GetDoubleValAtRowCol(MeasColNames[6], RowNum, ref vals.x) && GetDoubleValAtRowCol(MeasColNames[7], RowNum, ref vals.y) && GetDoubleValAtRowCol(MeasColNames[8], RowNum, ref vals.z) &&
                        GetDoubleValAtRowCol(JasColNames[6], RowNum, ref jvals.x) && GetDoubleValAtRowCol(JasColNames[7], RowNum, ref jvals.y) && GetDoubleValAtRowCol(JasColNames[8], RowNum, ref jvals.z))
                    {
                        J6vals.Add(new Vector3(vals));
                        J6angs.Add(new Vector3(jvals));
                        GotAnyAtAll = GotAnySet = true;
                    }

                    ++RowNum;
                } while (GotAnySet);

                //Process the EOAT definition data
                Boolean GotAll;
                double ev = 0.0, ejv = 0.0;

                for (RowNum = EoatDefStartRow; RowNum < EoatDefEndRow; ++RowNum)
                {
                    Vector Evals = new Vector(3);
                    Vector EJvals = new Vector(3);
                    GotAll = true;

                    for (j = 0; j < 3; ++j)
                    {
                        GotAll &= (GetDoubleValAtRowCol(EoatCols[j], RowNum, ref ev) && GetDoubleValAtRowCol(EoatJasCols[j], RowNum, ref ejv));

                        if (GotAll)
                        {
                            Evals.Vec[j] = ev;
                            EJvals.Vec[j] = ejv;
                        }
                        else
                            break;
                    }

                    if (GotAll)
                    {
                        vals = new Vector3(Evals);
                        jvals = new Vector3(EJvals);
                    }
                    else
                    {
                        vals = new Vector3();
                        jvals = new Vector3();
                    }

                    EOATvals.Add(new Vector3(vals));
                    EOATangs.Add(new Vector3(jvals));
                }

                CloseTrackerData();
            }
            catch (Exception e)
            {
                Problems.Add(e.Message);
                return false;
            }

            if (!GotAnyAtAll)
            {
                Problems.Add("No joint radial data was found");
            }

            return GotAnyAtAll;
        }



        public  Boolean FindRobotWristData(string FilePath, string RobotMechanismType, ref double[] WristData, ref ArrayList Problems)
        {
            int RowNum = 2;
            Boolean FoundIt = false;
            string[] MeasColNames = new string[12] { "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" };

            if (!OpenExcelFile(@FilePath, false))
                return false;

            TrackerWorksheets = TrackerDataoWb.Worksheets;
            TrackerMatSheet = (Worksheet)TrackerWorksheets.get_Item(3);     //Attempting to read 3rd worksheet

            TrackerDataoXL.DisplayAlerts = false;
            TrackerDataoWb.Saved = true;
            TrackerDataExcelOpen = true;

            try
            {
                string MechTypeRead = null;

                do
                {
                    ++RowNum;
                    MechTypeRead = ProgStringCell("A", RowNum, TrackerMatSheet);
                } while ((MechTypeRead != null) && !MechTypeRead.Equals(RobotMechanismType));

                //Continue if we have found the mechanism type of interest
                if ((MechTypeRead != null) && MechTypeRead.Equals(RobotMechanismType))
                {
                    int i;
                    double val = 0.0;

                    for (i = 0; i < 12; ++i)
                    {
                        if (GetDoubleValAtRowCol(MeasColNames[i], RowNum, ref val))
                        {
                            switch (i)
                            {
                                case 2:
                                case 3:
                                case 6:
                                case 7:
                                case 10:
                                case 11:
                                    WristData[i] = Utils.DegreesToRad(val);
                                    break;

                                default:

                                    WristData[i] = val;
                                    break;
                            }
                        }
                        else
                        {
                            Problems.Add("Incomplete wrist data record for robot type " + RobotMechanismType);
                            break;
                        }
                    }

                    FoundIt = i.Equals(12);
                }

                CloseTrackerData();
            }
            catch (Exception e)
            {
                Problems.Add(e.Message);
                return false;
            }

            if (!FoundIt)
            {
                Problems.Add("Wrist data not found for robot type:" + RobotMechanismType);
            }

            return FoundIt;
        }

        public  Boolean WriteToolDefinitions(string FilePath, List<Vector3> ToolData, ref ArrayList Problems)
        {
            int i = 0;
            try
            {
                Boolean GotAny = false;

                //See if we Got any
                for ( i = 0; i < ToolData.Count; ++i)
                {
                    if (!(ToolData[i].x.Equals(0.0) && ToolData[i].y.Equals(0.0) && ToolData[i].z.Equals(0.0)))
                    {
                        GotAny = true;
                        break;
                    }
                }

                if (GotAny)
                {
                    if (!OpenExcelFile(@FilePath, false))
                        return false;

                    TrackerWorksheets = TrackerDataoWb.Worksheets;
                    TrackerMatSheet = (Worksheet)TrackerWorksheets.get_Item(1);     //Attempting to read 1st worksheet

                    TrackerDataoXL.DisplayAlerts = false;
                    TrackerDataoWb.Saved = true;
                    TrackerDataExcelOpen = true;
                    int RowUse = 0;
                    Range oRng;
                    string mesbuf = null;

                    //Loop through the data
                    for (i = 0; i < ToolData.Count; ++i)
                    {
                        if (!(ToolData[i].x.Equals(0.0) && ToolData[i].y.Equals(0.0) && ToolData[i].z.Equals(0.0)))
                        {
                            RowUse = i + UserPreferences.ToolData1Row;
                            mesbuf = UserPreferences.ToolDataXcolumn + RowUse.ToString();

                            oRng = TrackerMatSheet.get_Range(mesbuf, mesbuf);
                            oRng.Value2 = ToolData[i].x;

                            mesbuf = UserPreferences.ToolDataYcolumn + RowUse.ToString();
                            oRng = TrackerMatSheet.get_Range(mesbuf, mesbuf);
                            oRng.Value2 = ToolData[i].y;

                            mesbuf = UserPreferences.ToolDataZcolumn + RowUse.ToString();
                            oRng = TrackerMatSheet.get_Range(mesbuf, mesbuf);
                            oRng.Value2 = ToolData[i].z;                            
                        }
                    }

                    CloseTrackerData(true, FilePath);                
                }
            }
            catch (Exception e)
            {
                Problems.Add(e.Message);
                return false;
            }

            return true;
        }

        public  Boolean WriteCar0BaseData(string FilePath, List<Vector3> BaseData, ref ArrayList Problems)
        {
            try
            {
                if (BaseData.Count > 0)
                {
                    if (!OpenExcelFile(@FilePath, false))
                        return false;

                    TrackerWorksheets = TrackerDataoWb.Worksheets;
                    TrackerMatSheet = (Worksheet)TrackerWorksheets.get_Item(1);     //Attempting to read 1st worksheet

                    TrackerDataoXL.DisplayAlerts = false;
                    TrackerDataoWb.Saved = true;
                    TrackerDataExcelOpen = true;
                    int RowUse = 0, i;
                    Range oRng;
                    string mesbuf = null;

                    //Loop through the data
                    for (i = 0; i < BaseData.Count; ++i)
                    {
                        RowUse = i + UserPreferences.FpointsFinalBase1P1Row;
                        mesbuf = UserPreferences.ToolDataXcolumn + RowUse.ToString();

                        oRng = TrackerMatSheet.get_Range(mesbuf, mesbuf);
                        oRng.Value2 = BaseData[i].x;

                        mesbuf = UserPreferences.ToolDataYcolumn + RowUse.ToString();
                        oRng = TrackerMatSheet.get_Range(mesbuf, mesbuf);
                        oRng.Value2 = BaseData[i].y;

                        mesbuf = UserPreferences.ToolDataZcolumn + RowUse.ToString();
                        oRng = TrackerMatSheet.get_Range(mesbuf, mesbuf);
                        oRng.Value2 = BaseData[i].z;
                    }

                    CloseTrackerData(true, FilePath);
                }

            }
            catch (Exception e)
            {
                Problems.Add(e.Message);
                return false;
            }

            return true;
        }
        public  Boolean WriteJ456Data(string FilePath, List<Vector> J4Data, List<Vector> J5Data, List<Vector> J6Data, ref ArrayList Problems)
        {

            try
            {
                if ((J4Data.Count > 2) && (J5Data.Count > 2) && (J5Data.Count > 2))
                {
                    if (!OpenExcelFile(@FilePath, false))
                        return false;

                    TrackerWorksheets = TrackerDataoWb.Worksheets;
                    TrackerMatSheet = (Worksheet)TrackerWorksheets.get_Item(2);     //Attempting to read 1st worksheet

                    TrackerDataoXL.DisplayAlerts = false;
                    TrackerDataoWb.Saved = true;
                    TrackerDataExcelOpen = true;
                    int RowUse = 0, i, j;
                    Range oRng;
                    string mesbuf = null;

                    //Process J4Data
                    for (i = 0; i < J4Data.Count; ++i)
                    {
                        RowUse = i + UserPreferences.J456StartRow;

                        for (j = 0; j < 3; ++j)
                        {
                            mesbuf = MeasColNames[UserPreferences.J4StartCol + j] + RowUse.ToString();

                            oRng = TrackerMatSheet.get_Range(mesbuf, mesbuf);
                            oRng.Value2 = J4Data[i].Vec[j + 3].ToString();
                        }
                    }

                    //Process J5Data
                    for (i = 0; i < J5Data.Count; ++i)
                    {
                        RowUse = i + UserPreferences.J456StartRow;

                        for (j = 0; j < 3; ++j)
                        {
                            mesbuf = MeasColNames[UserPreferences.J5StartCol + j] + RowUse.ToString();

                            oRng = TrackerMatSheet.get_Range(mesbuf, mesbuf);
                            oRng.Value2 = J5Data[i].Vec[j + 3].ToString();
                        }
                    }

                    //Process J6Data
                    for (i = 0; i < J6Data.Count; ++i)
                    {
                        RowUse = i + UserPreferences.J456StartRow;

                        for (j = 0; j < 3; ++j)
                        {
                            mesbuf = MeasColNames[UserPreferences.J6StartCol + j] + RowUse.ToString();

                            oRng = TrackerMatSheet.get_Range(mesbuf, mesbuf);
                            oRng.Value2 = J6Data[i].Vec[j + 3].ToString();
                        }
                    }

                    RowUse = UserPreferences.J456StartRow;

                    //Fill in possible wrist angles for EOAT measurement
                    for (j = 0; j < 3; ++j)
                    {
                        mesbuf = MeasColNames[UserPreferences.EoatJasStartCol + j] + RowUse.ToString();

                        oRng = TrackerMatSheet.get_Range(mesbuf, mesbuf);
                        oRng.Value2 = J6Data[J6Data.Count - 1].Vec[j + 3].ToString();
                    }

                    CloseTrackerData(true, FilePath);
                }
            }
            catch (Exception e)
            {
                Problems.Add(e.Message);
                return false;
            }

            return true;
        }
    }
}
