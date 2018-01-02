using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using CarZero.Helpers;
using CarZero.Robots;

namespace CarZero.ViewModels
{
    public class MainViewModel:ViewModelBase
    {
        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ?? (_instance = new MainViewModel());

        public const string NOT_SET = "Not Set";


        #region · Properties ·

        #region Robot

        private IRobot _robot = default(IRobot);

        public IRobot Robot
        {
            get { return _robot; }
            set
            {
                if (_robot == value) return;
                OnPropertyChanging(nameof(Robot));
                _robot = value;
                OnPropertyChanged(nameof(Robot));
            }
        }

        #endregion

        


        #region CanDoInitial


        private bool _canDoInitial = default(bool);

        public bool CanDoInitial
        {
            get { return _canDoInitial; }
            set
            {
                if (_canDoInitial == value) return;
                OnPropertyChanging(nameof(CanDoInitial));
                _canDoInitial = value;
                OnPropertyChanged(nameof(CanDoInitial));
            }
        }

        #endregion

        

        #region Problems

        private string _problems = default(string);

        public string Problems
        {
            get { return _problems; }
            set
            {
                if (_problems == value) return;
                OnPropertyChanging(nameof(Problems));
                _problems = value;
                OnPropertyChanged(nameof(Problems));
            }
        }

        #endregion

        


        public ObservableCollection<string> RobotTypes { get;set; }= new ObservableCollection<string>(RobotHelper.GetRobotTypes());

        #region SelectedRobotType

        private string _selectedRobotType = default(string);

        public string SelectedRobotType
        {
            get { return _selectedRobotType; }
            set
            {
                if (_selectedRobotType == value) return;
                OnPropertyChanging(nameof(SelectedRobotType));
                _selectedRobotType = value;
                OnPropertyChanged(nameof(SelectedRobotType));
            }
        }

        #endregion

        

        #region InitialFit

        private bool _initialFit = default(bool);

        public bool InitialFit
        {
            get { return _initialFit; }
            set
            {
                if (_initialFit == value) return;
                OnPropertyChanging(nameof(InitialFit));
                _initialFit = value;
                OnPropertyChanged(nameof(InitialFit));
            }
        }

        #endregion

        


        #region WriteDebug

        private bool _writeDebug = default(bool);

        public bool WriteDebug
        {
            get { return _writeDebug; }
            set
            {
                if (_writeDebug == value) return;
                OnPropertyChanging(nameof(WriteDebug));
                _writeDebug = value;
                OnPropertyChanged(nameof(WriteDebug));
            }
        }

        #endregion

        

        #region IsRadial

        private bool _isRadial = true;

        public bool IsRadial
        {
            get { return _isRadial; }
            set
            {
                if (_isRadial == value) return;
                OnPropertyChanging(nameof(IsRadial));
                _isRadial = value;
                OnPropertyChanged(nameof(IsNotRaidal));

                OnPropertyChanged(nameof(IsRadial));
            }
        }



        #endregion

        #region IsNotRaidal

        private bool _isNotRadial = default(bool);

        public bool IsNotRaidal
        {
            get { return _isNotRadial; }
            set
            {
                if (_isNotRadial == value) return;
                OnPropertyChanging(nameof(IsNotRaidal));
                _isNotRadial = value;
                OnPropertyChanged(nameof(IsNotRaidal));
                OnPropertyChanged(nameof(IsRadial));
            }
        }

        #endregion

        


        

        #region RootFolder

        private string _rootFolder = NOT_SET;

        public string RootFolder
        {
            get { return _rootFolder; }
            set
            {
                if (_rootFolder == value) return;
                OnPropertyChanging(nameof(RootFolder));
                _rootFolder = value;
                OnPropertyChanged(nameof(RootFolder));
            }
        }

        #endregion

        #region RobotData

        private string _robotData = NOT_SET;

        public string RobotData
        {
            get { return _robotData; }
            set
            {
                if (_robotData == value) return;
                OnPropertyChanging(nameof(RobotData));
                _robotData = value;
                OnPropertyChanged(nameof(RobotData));
            }
        }

        #endregion

        #region TrackerFile

        private string _trackerFile =NOT_SET;

        public string TrackerFile
        {
            get { return _trackerFile; }
            set
            {
                if (_trackerFile == value) return;
                OnPropertyChanging(nameof(TrackerFile));
                _trackerFile = value;
                OnPropertyChanged(nameof(TrackerFile));
            }
        }

        #endregion

        



        


        #endregion

        #region · Commands ·

        #region ReadRobotToolProgramCommand

        private RelayCommand _readRobotToolProgramCommand;
        public RelayCommand ReadRobotToolProgramCommand => _readRobotToolProgramCommand ?? (_readRobotToolProgramCommand = new RelayCommand(ExecuteReadRobotToolProgramCommand));

        private void ExecuteReadRobotToolProgramCommand()
        {
             var problems = new List<string>();
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

        #endregion

        private void ReportProblems(List<string> problems)
        {

        }

        #region ReadRobotBaseCommand

        private RelayCommand _readRobotBaseCommand;
        public RelayCommand ReadRobotBaseCommand => _readRobotBaseCommand ?? (_readRobotBaseCommand = new RelayCommand(ExecuteReadRobotBaseCommand));

        private void ExecuteReadRobotBaseCommand()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SetAllJointsCommand

        private RelayCommand _setAllJointsCommand;
        public RelayCommand SetAllJointsCommand => _setAllJointsCommand ?? (_setAllJointsCommand = new RelayCommand(ExecuteSetAllJointsCommand));

        private void ExecuteSetAllJointsCommand()
        {
            throw new NotImplementedException();
        }

        #endregion

        




        #endregion

        
    }
}
