using System.Collections.ObjectModel;


namespace CarZero.ViewModels
{
    public class RobotDataViewModel:ViewModelBase
    {

        #region Binding Properties

        private bool _foldersbuttonvisible = true;
        public bool FoldersButtonVisible { get { return _foldersbuttonvisible; } set { _foldersbuttonvisible = value;OnPropertyChanged("FoldersButtonVisible"); } }

        private bool _checkrobotsbuttonvisible = true;
        public bool CheckRobotsButtonVisible { get { return _checkrobotsbuttonvisible; } set { _checkrobotsbuttonvisible = value;OnPropertyChanged("CheckRobotsButtonVisible"); } }

        private int _currentstyleselected = 0;
        public int CurrentStyleSelected { get { return _currentstyleselected; } set { _currentstyleselected = value;OnPropertyChanged("CurrentStyleSelected"); } }

        private string _unknownapplicationstext = string.Empty;
        public string UnknownApplicationsText
        {
            get { return _unknownapplicationstext; }
            set { _unknownapplicationstext = value;OnPropertyChanged("UnknownApplicationsText"); }
        }
        private bool _badapplicationsvisible= false;
        public bool BadApplicationsVisible
        {
            get { return _badapplicationsvisible; }
            set { _badapplicationsvisible = value;OnPropertyChanged("BadApplicationsVisible"); }
        }

        public ObservableCollection<string> SystemNames { get; set; }
        public ObservableCollection<string> SystemCodes { get; set; }
        #endregion

        void ShowSystems()
        {
//           if (ExcelIO.BuildMatyrixSystemList(ref SystemNames, ref SystemCodes))





        }
    }
}
