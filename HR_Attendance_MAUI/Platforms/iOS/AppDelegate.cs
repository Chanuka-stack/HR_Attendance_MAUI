using Foundation;
using Microsoft.Extensions.Logging;
using UIKit;

namespace HR_Attendance_MAUI
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        //Added 7-1-2024
        private readonly ILogger<AppDelegate> _logger;

        public AppDelegate(ILogger<AppDelegate> logger)
        {
            _logger = logger;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            return base.FinishedLaunching(application, launchOptions);
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            _logger.LogInformation("Background fetch is performing work.");
            // Your background task logic here
            completionHandler(UIBackgroundFetchResult.NewData);
        }
    }
}
