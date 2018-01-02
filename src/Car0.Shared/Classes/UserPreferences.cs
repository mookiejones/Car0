#if WPF
using System.Windows;
    #else
using System.Windows.Forms;
#endif
namespace CarZero
{
    using System;
    using System.IO; 
    internal class UserPreferences
    {
        public const int BaseData13Row = 0x15;
        public const int BaseData14Row = 0x16;
        public const int BaseData15Row = 0x17;
        public const string BaseDataXcolumn = "G";
        public const string BaseDataYcolumn = "H";
        public const string BaseDataZcolumn = "I";
        public static int CurrentRobotSelected = -1;
        public static int CurrentStyleSelected = -1;
        public const int EoatJasStartCol = 0x1c;
        public static string Excel321FileName = null;
        public const int F321OriginRow = 4;
        public const string F321Xcolumn = "B";
        public const int F321Xrow = 2;
        public const string F321Ycolumn = "C";
        public const int F321Yrow = 3;
        public const string F321Zcolumn = "D";
        public static int[] FinalPointStartColNumbers = new int[] { 6, 11, 0x10, 0x15, 0x1a, 0x1f, 0x24, 0x29 };
        public const int FpointsFinalBase1P1Row = 8;
        public const int FpointsInitP1Row = 8;
        public const int FpointsInitP2Row = 9;
        public const int FpointsInitP3Row = 10;
        public const string FpointsInitXcolumn = "B";
        public const string FpointsInitYcolumn = "C";
        public const string FpointsInitZcolumn = "D";
        public const int GripPinData1Row = 30;
        public const int J456StartRow = 2;
        public const int J4StartCol = 4;
        public const int J5StartCol = 12;
        public const int J6StartCol = 20;
        public static string MeasuredPointFileName = null;
        public static int OperationRadioButtonSelected = 1;
        public static int RobotBrandSelected = 0;
        public static string RobotMatrixFileName = null;
        public const int ToolData1Row = 0x15;
        public const string ToolDataXcolumn = "B";
        public const string ToolDataYcolumn = "C";
        public const string ToolDataZcolumn = "D";
        public static string WorkFolderName = null;

        public static void Read()
        {
            try
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\KUKA_Car0_Preferences.txt";
                string str3 = null;
                string str4 = null;
                WorkFolderName = Excel321FileName = MeasuredPointFileName = (string) (RobotMatrixFileName = null);
                OperationRadioButtonSelected = 1;
                CurrentStyleSelected = -1;
                CurrentRobotSelected = -1;
                var reader = new StreamReader(path);
                while ((str3 = reader.ReadLine()) != null)
                {
                    if (str3.Contains("Work Folder: "))
                    {
                        WorkFolderName = str3.Substring(13);
                    }
                    else if (str3.Contains("Excel 321 File: "))
                    {
                        Excel321FileName = str3.Substring(0x10);
                    }
                    else if (str3.Contains("Measured Point File: "))
                    {
                        MeasuredPointFileName = str3.Substring(0x15);
                    }
                    else if (str3.Contains("RobotMatrixFileName: "))
                    {
                        RobotMatrixFileName = str3.Substring(0x15);
                    }
                    else if (str3.Contains("OperationRadioButtonSelected: "))
                    {
                        str4 = str3.Substring(30);
                        try
                        {
                            OperationRadioButtonSelected = Convert.ToInt32(str4);
                        }
                        catch (Exception)
                        {
                            OperationRadioButtonSelected = 1;
                        }
                    }
                    else if (str3.Contains("CurrentStyleSelected: "))
                    {
                        str4 = str3.Substring(0x16);
                        try
                        {
                            CurrentStyleSelected = Convert.ToInt32(str4);
                        }
                        catch (Exception)
                        {
                            CurrentStyleSelected = -1;
                        }
                    }
                    else if (str3.Contains("CurrentRobotSelected: "))
                    {
                        str4 = str3.Substring(0x16);
                        try
                        {
                            CurrentRobotSelected = Convert.ToInt32(str4);
                        }
                        catch (Exception)
                        {
                            CurrentRobotSelected = -1;
                        }
                    }
                    else if (str3.Contains("RobotBrandSelected: "))
                    {
                        str4 = str3.Substring(20);
                        try
                        {
                            RobotBrandSelected = Convert.ToInt32(str4);
                        }
                        catch (Exception)
                        {
                            RobotBrandSelected = 0;
                        }
                    }
                }
                reader.Close();
            }
            catch (Exception)
            {
            }
        }

        public static void Write()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\KUKA_Car0_Preferences.txt";
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    var str = "Work Folder: " + WorkFolderName;
                    writer.WriteLine(str);
                    str = "Excel 321 File: " + Excel321FileName;
                    writer.WriteLine(str);
                    str = "Measured Point File: " + MeasuredPointFileName;
                    writer.WriteLine(str);
                    str = "RobotMatrixFileName: " + RobotMatrixFileName;
                    writer.WriteLine(str);
                    str = "OperationRadioButtonSelected: " + OperationRadioButtonSelected.ToString();
                    writer.WriteLine(str);
                    str = "CurrentStyleSelected: " + CurrentStyleSelected.ToString();
                    writer.WriteLine(str);
                    str = "CurrentRobotSelected: " + CurrentRobotSelected.ToString();
                    writer.WriteLine(str);
                    str = "RobotBrandSelected: " + RobotBrandSelected.ToString();
                    writer.WriteLine(str);
                    writer.Flush();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }
    }
}

