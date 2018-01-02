using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Car0
{
     public class UserPreferences
     {
         public event NotifyMessageEventHandler NotifyMessage;
         private void raiseNotify(string message, string title)
        {
            if (NotifyMessage!=null)
                NotifyMessage(this,new NotifyMessageEventArgs(){Message = message,Title = title};
        }


        #region UserPreverence Constants
        public const int F321Xrow = 2, F321Yrow = 3, F321OriginRow = 4;
        public const string F321Xcolumn = "B", F321Ycolumn = "C", F321Zcolumn = "D";

        public const int FpointsInitP1Row = 8, FpointsInitP2Row = 9, FpointsInitP3Row = 10;
        public const string FpointsInitXcolumn = "B", FpointsInitYcolumn = "C", FpointsInitZcolumn = "D";

        public const int FpointsFinalBase1P1Row = 8;

        public  int[] FinalPointStartColNumbers = new int[8] { 6, 11,  16, 21, 26, 31, 36, 41};

        public const int J456StartRow = 2;
        public const int J4StartCol = 4;
        public const int J5StartCol = 12;
        public const int J6StartCol = 20;
        public const int EoatJasStartCol = 28;

        public const int ToolData1Row = 21;
        public const int GripPinData1Row = 30;

        public const string ToolDataXcolumn = "B", ToolDataYcolumn = "C", ToolDataZcolumn = "D";

        public const int BaseData13Row = 21, BaseData14Row = 22, BaseData15Row = 23;
        public const string BaseDataXcolumn = "G", BaseDataYcolumn = "H", BaseDataZcolumn = "I";

        #endregion

        #region User Preference Class Variables
        public static string WorkFolderName = null;
        public static string Excel321FileName = null;
        public static string MeasuredPointFileName = null;
        public static string RobotMatrixFileName = null;

        public  int OperationRadioButtonSelected = 1;
        public  int CurrentStyleSelected = -1;
        public  int CurrentRobotSelected = -1;

        public  int RobotBrandSelected = 0;
        #endregion

        #region User Preference Functions
        /* Write()
         * 
         *      Writes all defined user preferences to the file.
         */
        public  void Write()
        {
            string mesbuf;
            string mydocs = (true) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : "C:\\Documents and Settings\\aknasinski";
            string filepath = mydocs + "\\KUKA_Car0_Preferences.txt";

            try
            {
                using (StreamWriter fs = new System.IO.StreamWriter(@filepath))
                {
                    mesbuf = "Work Folder: " + WorkFolderName;
                    fs.WriteLine(mesbuf);

                    mesbuf = "Excel 321 File: " + Excel321FileName;
                    fs.WriteLine(mesbuf);

                    mesbuf = "Measured Point File: " + MeasuredPointFileName;
                    fs.WriteLine(mesbuf);

                    mesbuf = "RobotMatrixFileName: " + RobotMatrixFileName;
                    fs.WriteLine(mesbuf);

                    mesbuf = "OperationRadioButtonSelected: " + OperationRadioButtonSelected.ToString() ;
                    fs.WriteLine(mesbuf);

                    mesbuf = "CurrentStyleSelected: " + CurrentStyleSelected.ToString();
                    fs.WriteLine(mesbuf);

                    mesbuf = "CurrentRobotSelected: " + CurrentRobotSelected.ToString();
                    fs.WriteLine(mesbuf);

                    mesbuf = "RobotBrandSelected: " + RobotBrandSelected.ToString();
                    fs.WriteLine(mesbuf);

                    fs.Flush();
                }
            }
            catch (Exception e)
            {
               raiseNotify(e.ToString(),"UserPreferences");
            }
        }

        public  void Read()
        {
            try
            {
                string mydocs = (true) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : "C:\\Documents and Settings\\aknasinski";
                string filepath = mydocs + "\\KUKA_Car0_Preferences.txt";
                string line = null, mesbuf = null;

                WorkFolderName = Excel321FileName = MeasuredPointFileName = RobotMatrixFileName = null;
                OperationRadioButtonSelected = 1;
                CurrentStyleSelected = -1;
                CurrentRobotSelected = -1;
                using (System.IO.StreamReader myStream = new System.IO.StreamReader(filepath))
                {
                    while ((line = myStream.ReadLine()) != null)
                    {
                        if (line.Contains("Work Folder: "))
                        {
                            WorkFolderName = line.Substring(13);
                        }
                        else if (line.Contains("Excel 321 File: "))
                        {
                            Excel321FileName = line.Substring(16);
                        }
                        else if (line.Contains("Measured Point File: "))
                        {
                            MeasuredPointFileName = line.Substring(21);
                        }
                        else if (line.Contains("RobotMatrixFileName: "))
                        {
                            RobotMatrixFileName = line.Substring(21);
                        }
                        else if (line.Contains("OperationRadioButtonSelected: "))
                        {
                            mesbuf = line.Substring(30);

                            try
                            {
                                OperationRadioButtonSelected = Convert.ToInt32(mesbuf);
                            }
                            catch (Exception)
                            {
                                OperationRadioButtonSelected = 1;
                            }
                        }
                        else if (line.Contains("CurrentStyleSelected: "))
                        {
                            mesbuf = line.Substring(22);

                            try
                            {
                                CurrentStyleSelected = Convert.ToInt32(mesbuf);
                            }
                            catch (Exception)
                            {
                                CurrentStyleSelected = -1;
                            }
                        }
                        else if (line.Contains("CurrentRobotSelected: "))
                        {
                            mesbuf = line.Substring(22);

                            try
                            {
                                CurrentRobotSelected = Convert.ToInt32(mesbuf);
                            }
                            catch (Exception)
                            {
                                CurrentRobotSelected = -1;
                            }
                        }
                        else if (line.Contains("RobotBrandSelected: "))
                        {
                            mesbuf = line.Substring(20);

                            try
                            {
                                RobotBrandSelected = Convert.ToInt32(mesbuf);
                            }
                            catch (Exception)
                            {
                                RobotBrandSelected = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //System.Windows.Forms.raiseNotify(e.ToString());
            }
        }

        #endregion
    }
}
