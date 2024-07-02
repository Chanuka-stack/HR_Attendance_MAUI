using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using System.Net.Http;
using HR_Attendance_MAUI.Model;
using Plugin.Maui.Biometric;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint;
using System.Net;
using System.Net.Http.Headers;
using HR_Attendance_MAUI.Services;



namespace HR_Attendance_MAUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly IFingerprint fingerprint;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            /*var networkAccess = Connectivity.NetworkAccess;
            if (networkAccess == NetworkAccess.Internet) {
                UsernameEntry.Text = string.Empty;
                PasswordEntry.Text = string.Empty;
                LoginBtnOffline.IsVisible = false;
                UsernameEntry.IsVisible = true;
                PasswordEntry.IsVisible = true;
                LoginBtn.IsVisible = true;
            }
            else {
                UsernameEntry.IsVisible = false;
                PasswordEntry.IsVisible = false;
                LoginBtn.IsVisible = false;
                LoginBtnOffline.IsVisible = true;
            }*/
        }
        public MainPage(IFingerprint fingerprint)
        {
            InitializeComponent();
            this.fingerprint = fingerprint;

        }

        private async void OnLoginButtonClicked(object sender, EventArgs e) {

            //var request = new AuthenticationRequestConfiguration("Validate that you have fingers", "Because without them you will not be able to access");

            //var result = await fingerprint.AuthenticateAsync(request);
            var request = new AuthenticationRequestConfiguration
("Login using biometrics", "Confirm login with your biometrics")
            {
                FallbackTitle = "Use PIN",
                AllowAlternativeAuthentication = true,
            };

            var result = await CrossFingerprint.Current.AuthenticateAsync(request);

            if (result.Authenticated)
            {
                LoginInfo loginInfo = new LoginInfo
                {
                    Username = "Offline",
                    Longitude = 0,
                    Latitude = 0
                };

                var navigationParameter = new ShellNavigationQueryParameters
                        {
                            { "EmpLoginInfo", loginInfo }
                        };
                AppManager.IsLoggedIn = true;
                await Shell.Current.GoToAsync("//Home_Page", navigationParameter);
            }
            else {
                await DisplayAlert("Error", "Invalid Login", "OK");
            }
            
        }
        /*private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;


            bool isAuthenticated = await AuthenticateUserAsync(username, password);

            if (isAuthenticated)
            {
                LoginInfo loginInfo = new LoginInfo
                {
                    Username = username,
                    Longitude = 0,
                    Latitude = 0
                };

                var navigationParameter = new ShellNavigationQueryParameters
                        {
                            { "EmpLoginInfo", loginInfo }
                        };
                AppManager.IsLoggedIn = true;
                await Shell.Current.GoToAsync("//Home_Page", navigationParameter);
            }
            else
            {
                await DisplayAlert("Error", "Invalid username or password", "OK");
            }
        }*/

        private async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            var loginData = new { Username = username, Password = password };

            HttpClient client;

            try
            {
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
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK"); 
                return false;
            }
            
           
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FinPrintReg_Page");
        }

        



    }




}
