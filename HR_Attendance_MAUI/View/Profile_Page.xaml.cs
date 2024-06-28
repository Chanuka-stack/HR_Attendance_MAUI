using HR_Attendance_MAUI.Model;
using Microsoft.Maui.Controls;
using Plugin.Fingerprint.Abstractions;


namespace HR_Attendance_MAUI;

public partial class Profile_Page : ContentPage
{
    private readonly IFingerprint _fingerprint;
    public Profile_Page()
	{
		InitializeComponent();
      

        // Your code for creating the list and binding it to ItemsSource
        List<ProfileListItem> profiles = new List<ProfileListItem>();
        profiles.Add(new ProfileListItem { Name = "Alice", Description = "Desc", ImageSource = "fp.jfif" });
        //profiles.Add(new ProfileListItem { Name = "Bob", Description = "...", ImageSource = "lo.jfif" });

        //listView.ItemsSource = profiles;
        
    }

    private void OnLogoutButtonClicked(object sender, EventArgs e)
    {
        AppManager.LogoutAsync();
        AppManager.LogoutTimer?.Stop();
    }
    private async void onFinSettingsTapped(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync("//FinPrintReg_Page");
        await Navigation.PushAsync(new FinPrintReg_Page(_fingerprint));
    }
   
}