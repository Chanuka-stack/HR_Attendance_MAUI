using Foundation;
using Microsoft.Extensions.Logging;
using UIKit;
using HR_Attendance_MAUI.Services;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls;

namespace HR_Attendance_MAUI
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        private NSTimer fetchTimer;

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        // Ensure the correct method signature for FinishedLaunching
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            return base.FinishedLaunching(application, launchOptions);
        }

        // PerformFetch method to handle background fetch
        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            var backgroundService = DependencyService.Get<IBackgroundService>();

            Task.Run(async () =>
            {
                backgroundService.Start();
                completionHandler(UIBackgroundFetchResult.NewData);
            });
        }
    }
}
