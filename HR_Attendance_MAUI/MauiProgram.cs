
using Microsoft.Extensions.Logging;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint;
using Plugin.Maui.Biometric;
using HR_Attendance_MAUI.Services;

#if ANDROID
using HR_Attendance_MAUI.Platforms.Android.Services;
#elif IOS
using HR_Attendance_MAUI.Platforms.iOS.Service; 
#endif


namespace HR_Attendance_MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<FinPrintReg_Page>();
            builder.Services.AddSingleton(typeof(IFingerprint), CrossFingerprint.Current);

#if DEBUG
            builder.Logging.AddDebug();
#endif
#if ANDROID
        builder.Services.AddSingleton<IBackgroundService, AndroidBackgroundService>();
#elif IOS
            builder.Services.AddSingleton<IBackgroundService, iOSBackgroundService>();
#endif

            return builder.Build();
        }
    }


}
