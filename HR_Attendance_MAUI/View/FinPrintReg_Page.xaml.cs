using Newtonsoft.Json;
using System.Net.Http.Json;
using Plugin.Maui.Biometric;
using Plugin.Fingerprint.Abstractions;

/* Unmerged change from project 'HR_Attendance_MAUI (net8.0-maccatalyst)'
Before:
//using Android.DeviceLock;
After:
using HR_Attendance_MAUI.Services;
//using Android.DeviceLock;
*/

/* Unmerged change from project 'HR_Attendance_MAUI (net8.0-windows10.0.19041.0)'
Before:
//using Android.DeviceLock;
After:
using HR_Attendance_MAUI.Services;
//using Android.DeviceLock;
*/
using HR_Attendance_MAUI.Services;
using Plugin.Fingerprint;

//using Android.DeviceLock;
#if ANDROID
using Android.Provider;
using Android.Content;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
#endif

#if IOS
using UIKit;
#endif
namespace HR_Attendance_MAUI;

public partial class FinPrintReg_Page : ContentPage
{
    private readonly IFingerprint fingerprint;
    public FinPrintReg_Page(IFingerprint fingerprint)
	{
		InitializeComponent();
        this.fingerprint = fingerprint;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        EmployeeIDEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
    }
    private async void OnRegistrationButtonClicked(object sender, EventArgs e)
    {

        string employee_id = EmployeeIDEntry.Text;
        string password = PasswordEntry.Text;
        //var request = new AuthenticationRequestConfiguration("Validate that you have fingers", "Because without them you will not be able to access");
        //var result = await fingerprint.AuthenticateAsync(request);
        var request = new AuthenticationRequestConfiguration
("Login using biometrics", "Confirm login with your biometrics")
        {
            FallbackTitle = "Use PIN",
            AllowAlternativeAuthentication = true,
        };

        var result = await CrossFingerprint.Current.AuthenticateAsync(request);
        var deviceId = "";
        
        bool isAuthenticated = await AuthenticateUserAsync(employee_id, password);

        //if (isAuthenticated)
        if (result.Authenticated && isAuthenticated)      
        {
#if ANDROID
            var context = Android.App.Application.Context;
            deviceId = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
#endif

#if IOS
        deviceId = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
#endif
        }
        else
        {
            await DisplayAlert("Error", "Invalid Login", "OK");
        }


        if (deviceId != "")
        {
            bool isRegistered = await RegisterDevice(employee_id, deviceId);

            if (isRegistered)
            {

                await DisplayAlert("Success", "Registration Successful", "OK");
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await DisplayAlert("Error", "Try again to register", "OK");

            }
        }
        else {

           if (result.Authenticated == false) {
                await DisplayAlert("Error", "Invalid Finger", "OK");
            }

            if (isAuthenticated) {
                await DisplayAlert("Error", "Invalid Credentials", "OK");
            }
        }

        

       
    }

    private async Task<bool> RegisterDevice(string employee_id, string fingerprint_id)
    {
        var Data = new { Employee_ID = employee_id, Device_ID = fingerprint_id };

        HttpClient client;
        client = HttpClientFactory.CreateHttpClient();

        var response = await client.PostAsJsonAsync("api/Device", Data);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private async Task<bool> AuthenticateUserAsync(string username, string password)
    {
        var loginData = new { Username = username, Password = password };

        HttpClient client;
        client = HttpClientFactory.CreateHttpClient();

        var response = await client.PostAsJsonAsync("api/Auth", loginData);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private async void OnBackButtonClicked(object sender, EventArgs e) {
        await Shell.Current.GoToAsync("//MainPage");
    }




}
