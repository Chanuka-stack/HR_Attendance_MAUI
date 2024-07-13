using HR_Attendance_MAUI.Services;
using HR_Attendance_MAUI.Data;
using HR_Attendance_MAUI.Model;

namespace HR_Attendance_MAUI
{
    public partial class App : Application
    {
        private readonly IBackgroundService _backgroundService;
        private AttendanceDatabaseService2 _attendanceDatabaseService;

        public App(IBackgroundService backgroundService)
        //public App(IBackgroundService backgroundService)
        {

            InitializeComponent();

            Application.Current.UserAppTheme = AppTheme.Light;
            MainPage = new AppShell();


      
            _backgroundService = backgroundService;
            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
            _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);
            List<AttendanceData> attenadanceDataList = _attendanceDatabaseService.GetAllAttendances();

            if (attenadanceDataList.Count != 0)
            {
                if (!_backgroundService.IsForeGroundServiceRunning()) {
                    _backgroundService.Start();
                }
                    
            }
            else {
                _backgroundService.Stop();
            }
           
       
        }

        /* protected override void OnStart()
         {
             base.OnStart();
             _backgroundService.Start();
         }

         protected override void OnSleep()
         {
             base.OnSleep();
             _backgroundService.Stop();
         }

         protected override void OnResume()
         {
             base.OnResume();
             _backgroundService.Start();
         }*/

        protected override void OnSleep()
        {
            base.OnSleep();
            AttendanceDatabaseService2 _attendanceDatabaseService;

            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
            _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);
            List<AttendanceData> attenadanceDataList = _attendanceDatabaseService.GetAllAttendances();
            if (attenadanceDataList.Count != 0)
            {
                if (!_backgroundService.IsForeGroundServiceRunning())
                {
                    _backgroundService.Start();
                }

            }
            else
            {
                _backgroundService.Stop();
            }
            
            

            
        }
    }
}
