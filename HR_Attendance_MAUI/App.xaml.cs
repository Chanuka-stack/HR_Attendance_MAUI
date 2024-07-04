using HR_Attendance_MAUI.Services;

namespace HR_Attendance_MAUI
{
    public partial class App : Application
    {
        private readonly IBackgroundService _backgroundService;

        public App(IBackgroundService backgroundService)
        //public App(IBackgroundService backgroundService)
        {

            InitializeComponent();

            Application.Current.UserAppTheme = AppTheme.Light;
            MainPage = new AppShell();


      
            _backgroundService = backgroundService;
            _backgroundService.Start();
       
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
    }
}
