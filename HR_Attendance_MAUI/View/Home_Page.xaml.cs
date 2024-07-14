using HR_Attendance_MAUI.Model;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Networking;
using Newtonsoft.Json;
//using ObjCRuntime;
using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using HR_Attendance_MAUI.Data;
using HR_Attendance_MAUI.Services;
using Microsoft.Maui.Controls;
//using Android.Webkit;





namespace HR_Attendance_MAUI;

//[QueryProperty(nameof(EmpLoginInfo), "EmpLoginInfo")]
public partial class Home_Page : ContentPage
{
    //LoginInfo loginInfo;
    string username;
    string? lat;
    string? lon;

    private System.Timers.Timer logoutTimer;
   
    private AttendanceDatabaseService2 _attendanceDatabaseService;

    LocationService location = new LocationService();

   /* public LoginInfo EmpLoginInfo
    {
        get => loginInfo;
        set
        {
            // loginInfo = value;
            //OnPropertyChanged();
            ShowMessage(value);
        }
    }*/



    public Home_Page()
    {
        InitializeComponent();
        //BindingContext = this;

        logoutTimer = new System.Timers.Timer(300000);

        logoutTimer.Start();
        logoutTimer.Elapsed += OnLogoutTimerElapsed;
        AppManager.LogoutTimer = logoutTimer;
    }

   
    
        
        
    
    //public async void ShowMessage(LoginInfo loginInfo)
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        //Getting Location 
        var locationData = await location.GetCurrentLocation();
        double latitude = locationData["Latitude"];
        double longitude = locationData["Longitude"];


        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
        _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);

        if (latitude != null || locationData.ContainsKey("Error"))
        {
            latitudeLabel.Text = "LATITUDE " + latitude.ToString();
            longitudeLabel.Text = "LONGITUDE " + longitude.ToString();
            lat = latitude.ToString();
            lon = longitude.ToString();
        }
        else
        {
            latitudeLabel.Text = "Latitude is not available";
            longitudeLabel.Text = "Longitude is not available";
        }

        username = await SecureStorage.GetAsync("username_key"); ;
        string a = username;
       


        AttendanceData attendanceData = await GetAttendanceData();

        if (attendanceData != null)
        {
            int id = attendanceData.id;
            string inTimeDate = attendanceData.inTimeDate;
            string inTime = attendanceData.inTime;
            string outTimeDate = attendanceData.outTimeDate;
            string outTime = attendanceData.outTime;
            string empAttendanceDescription = attendanceData.empAttendenceDescription;
            string LatIn = attendanceData.latIn;
            string LonIn = attendanceData.lonIn;
            string LatOut = attendanceData.latOut;
            string LonOut = attendanceData.lonOut;

            markInLabel.Text = inTime;
            markOutLabel.Text = outTime;
            remarksEntry.Text = empAttendanceDescription;
            if (id == -1)
            {
                markInBtn.IsEnabled = true;
                markInBtn.IsVisible = true;
                markOutBtn.IsVisible = false;


            }
            else if ((outTime == null || outTime == "") && (inTime != null || inTime != ""))
            {
                markInBtn.IsVisible = false;
                markOutBtn.IsVisible = true;
            }
            else
            {
                markInBtn.IsVisible = false;
                markOutBtn.IsVisible = false;
            }

        }
        else
        {
            markOutLabel.Text = "";
            markInLabel.Text = "";
            markInBtn.IsEnabled = true;
            markInBtn.IsVisible = true;
            markOutBtn.IsVisible = false;
        }

    }
    private async void OnMarkInButtonClicked(object sender, EventArgs e)
    {
        DateTimeOffset currentTime1 = DateTimeOffset.Now;
        DateTime currentTime = currentTime1.DateTime;
        string formattedTime1 = currentTime.ToString("HH:mm");
        currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);

        string currentDate = currentTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string formattedTime = ModifyText(formattedTime1);
      
        string remarks = remarksEntry.Text;

        if (remarks == null)
        {
            remarks = "";
        }
        markInBtn.IsEnabled = false;

        var attendanceData = new AttendanceData
        {
            inTimeDate = currentDate,
            inTime = formattedTime,
            outTimeDate = "",
            outTime = "",
            employee_ID = username,
            empAttendenceDescription = remarks,
            lonIn = lon,
            latIn = lat,
            lonOut = "",
            latOut = ""
        };

        bool attendanceMarked = await MarkInAttendanceAsync(attendanceData);

        if (attendanceMarked)
        {
            await DisplayAlert("Success", "Attendance is Marked Successfully", "OK");
            markInBtn.IsVisible = false;
            markOutBtn.IsVisible = true;
            markOutBtn.IsEnabled = true;
            markInLabel.Text = formattedTime;
            markInBtn.IsEnabled = true;

        }
        else
        {

        }
    }

    private async void OnMarkOutButtonClicked(object sender, EventArgs e)
    {
        DateTimeOffset currentTime1 = DateTimeOffset.Now;
        DateTime currentTime = currentTime1.DateTime;
        string formattedTime1 = currentTime.ToString("HH:mm");
        currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);

        string formattedTime = ModifyText(formattedTime1);
     
        string currentDate = currentTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

        string remarks = remarksEntry.Text;
        if (remarks == null)
        {
            remarks = "";
        }
        markOutBtn.IsEnabled = false;

        var attendanceData = new AttendanceData
        {
            inTimeDate = currentDate,
            inTime = markInLabel.Text,
            outTimeDate = currentDate,
            outTime = formattedTime,
            employee_ID = username,
            empAttendenceDescription = remarks,
            latIn = lat,
            lonIn = lon,
            latOut = lat,
            lonOut = lon
        };

        bool attendanceMarked = await MarkInAttendanceAsync(attendanceData);

        if (attendanceMarked)
        {
            markOutLabel.Text = formattedTime;
            await DisplayAlert("Success", "Attendance is Marked Successfully", "OK");
            markInBtn.IsVisible = false;
            markOutBtn.IsVisible = false;

        }
        else
        {

        }
    }


    private async Task<bool> MarkInAttendanceAsync(AttendanceData attendanceData)
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
        _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);
        string InTimeDate = attendanceData.inTimeDate;
        string InTime = attendanceData.inTime;
        string OutTimeDate = attendanceData.outTimeDate;
        string OutTime = attendanceData.outTime;
        string Employee_ID = attendanceData.employee_ID;
        string EmpAttendenceDescription = attendanceData.empAttendenceDescription;
        string LonIn = attendanceData.lonIn;
        string LatIn = attendanceData.latIn;
        string LonOut = attendanceData.lonOut;
        string LatOut = attendanceData.latOut;

        var networkAccess = Connectivity.NetworkAccess;
        if (networkAccess == NetworkAccess.Internet)
        {

            var markInData = new { InTimeDate = InTimeDate, InTime = InTime, OutTimeDate = OutTimeDate, OutTime = OutTime, Employee_ID = Employee_ID, EmpAttendenceDescription = EmpAttendenceDescription, LatIn = LatIn, LonIn = LonIn, LatOut = LatOut, LonOut = LonOut };

            HttpClient client;
            client = HttpClientFactory.CreateHttpClient();
            var response = await client.PostAsJsonAsync("api/Attendance/Store", markInData);

            if (response.IsSuccessStatusCode)
            {
                TodaySyncedAttendance empAttendanceData = new TodaySyncedAttendance();



                empAttendanceData.inTimeDate = InTimeDate;
                empAttendanceData.inTime = InTime;
                empAttendanceData.outTimeDate = OutTimeDate;
                empAttendanceData.outTime = OutTime;
                empAttendanceData.employee_ID = Employee_ID;
                empAttendanceData.empAttendenceDescription = EmpAttendenceDescription;
                empAttendanceData.latIn = LatIn;
                empAttendanceData.lonIn = LonIn;
                empAttendanceData.latOut = LatOut;
                empAttendanceData.lonOut = LonOut;


                if (OutTime == null || OutTime == "")
                {
                    _attendanceDatabaseService.InsertTodaySyncedAttendance(empAttendanceData);
                }
                else
                {
                    _attendanceDatabaseService.UpdateSyncedAttendanceByDateOffline(empAttendanceData);
                }

                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            AttendanceData empAttendanceData = new AttendanceData();

            empAttendanceData.inTimeDate = InTimeDate;
            empAttendanceData.inTime = InTime;
            empAttendanceData.outTimeDate = OutTimeDate;
            empAttendanceData.outTime = OutTime;
            empAttendanceData.employee_ID = GetId();
            empAttendanceData.empAttendenceDescription = EmpAttendenceDescription;
            empAttendanceData.latIn = LatIn;
            empAttendanceData.lonIn = LonIn;
            empAttendanceData.latOut = LatOut;
            empAttendanceData.lonOut = LonOut;

            _attendanceDatabaseService.UpdateAttendanceByDateOffline(empAttendanceData);
            return true;
        }

    }


    private async Task<AttendanceData> GetAttendanceData()
    {
        string employeeId = username;

        DateTimeOffset currentTime1 = DateTimeOffset.Now;
        DateTime currentTime = currentTime1.DateTime;
        string formattedTime1 = currentTime.ToString("HH:mm");
        currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);
        string currentDate = currentTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
        _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);

        AttendanceData empAttendanceData = new AttendanceData();
        /*if (_attendanceDatabaseService.GetAttendanceByDate(currentDate) != null)
        {
            empAttendanceData = _attendanceDatabaseService.GetAttendanceByDate(currentDate);
        }
        else
        {
            empAttendanceData = _attendanceDatabaseService.GetTodayAttendnaceOffline(currentDate);
        }*/

        empAttendanceData = _attendanceDatabaseService.GetTodayAttendnaceOffline(currentDate);


        if (empAttendanceData != null)
        {
            return empAttendanceData;
        }
        else
        {
            return null;
        }

    }

   

    private void OnLogoutTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        if (AppManager.IsLoggedIn)
        {
            AppManager.LogoutAsync();
        }
    }

    public static string GetId()
    {

#if ANDROID
        return Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
#elif IOS
        return UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();
#else
        return null;
#endif
    }


    public string ModifyText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException(nameof(text));
        }


        string filteredText = Regex.Replace(text, "[APM]", "");

        return filteredText.Replace(':', '.');
    }

}