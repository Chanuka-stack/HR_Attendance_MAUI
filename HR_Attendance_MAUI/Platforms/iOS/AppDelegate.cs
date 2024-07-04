using Foundation;
using Microsoft.Extensions.Logging;
using UIKit;
using HR_Attendance_MAUI.Services;


namespace HR_Attendance_MAUI
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        private NSTimer fetchTimer;

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        //Added
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            return true;
        }

       /* public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            var backgroundService = MauiApplication.Current.Services.GetService<IBackgroundService>();
            backgroundService?.Start();
            completionHandler(UIBackgroundFetchResult.NewData);
        }*/
    }
}
