using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using HR_Attendance_MAUI.Platforms.Android.Services;
using Plugin.Fingerprint;

namespace HR_Attendance_MAUI
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnPostCreate(Bundle? savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            CrossFingerprint.SetCurrentActivityResolver(()=>this);
        }

        //Added 7/1/2024
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartService(new Intent(this, typeof(AndroidBackgroundService)));
        }
    }
}
