using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using System.Threading;
using Microsoft.Extensions.Logging;
using HR_Attendance_MAUI.Services;
using HR_Attendance_MAUI.Data;
using HR_Attendance_MAUI.Model;
using System.Net.Http.Json;
using System.Text.Json;
//using Microsoft.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;


namespace HR_Attendance_MAUI.Platforms.Android.Services
{
    [Service]
    public class AndroidBackgroundService : Service, IBackgroundService
    {
        private Timer _timer;
        // private ILogger<AndroidBackgroundService> _logger;
        private readonly TimeSpan _delay = TimeSpan.FromMinutes(5);
        private AttendanceDatabaseService2 _attendanceDatabaseService;

        public AndroidBackgroundService()
        {

        }
        /*public AndroidBackgroundService(ILogger<AndroidBackgroundService> logger)
        {
            _logger = logger;
        }*/

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public void Start()
        {
            //_logger.LogInformation("Background service starting.");
            //_timer = new Timer(DoWork, null, 0, 600000); // Run every 10 seconds

         
                _timer = new Timer(DoWork, null, 0, Timeout.Infinite);
            
            
        }

        public void Stop()
        {
            //_logger.LogInformation("Background service stopping.");
            _timer?.Dispose();
        }

        private async void DoWork(object state)
        {
            //_logger.LogInformation("Background service is doing work.");
            // Your background task logic here
            var networkAccess = Connectivity.NetworkAccess;
            if (networkAccess == NetworkAccess.Internet) {
                var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
                _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);
                List<AttendanceData> attenadanceDataList = _attendanceDatabaseService.GetAllAttendances();

                if (attenadanceDataList.Count != 0)
                {
                    bool result = await SyncAttendanceData();
                    if (result == true)
                    {
                        //_logger.LogInformation("Synced Successfully");
                        Stop();
                        await Task.Delay(_delay);
                        Start();
                    }
                    else
                    {
                        //_logger.LogInformation("Not Synced Successfully");
                        Stop();
                        await Task.Delay(_delay);
                        Start();
                    }
                    
                }
                else {
                    Stop();
                    await Task.Delay(_delay);
                    Start();

                }

                
            }
                


        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            /*if (_logger == null)
            {
                _logger = (ILogger<AndroidBackgroundService>)App.Current.Services.GetService(typeof(ILogger<AndroidBackgroundService>));
            }*/

            Start();
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            Stop();
            base.OnDestroy();
        }

        private async Task<bool> SyncAttendanceData()
        {

            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
            _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);
            List<AttendanceData> attenadanceDataList = _attendanceDatabaseService.GetAllAttendances();

            if (attenadanceDataList.Count == 0) {
                return false;
            }

            foreach (AttendanceData attendanceData in attenadanceDataList)
            {
                string inTimeDate = attendanceData.inTimeDate;
                string inTimeDate1 = attendanceData.inTime;
                string inTimeDate2 = attendanceData.lonOut;
                string inTimeDate3 = attendanceData.latOut;
            }

            DateTimeOffset currentTime1 = DateTimeOffset.Now;
            DateTime currentTime = currentTime1.DateTime;
            //string formattedTime1 = currentTime.ToString("HH:mm");
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);
            string currentDate = currentTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            HttpClient client;
            try
            {
                client = HttpClientFactory.CreateHttpClient();

                var response = await client.PostAsJsonAsync("api/Attendance/StoreList", attenadanceDataList);

                if (response.IsSuccessStatusCode)
                {
                    //_attendanceDatabaseService.DeleteAllExceptCurrentDate(currentDate);
                    _attendanceDatabaseService.StoreTodaysRecordsAndDeleteAll(currentDate);


                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Error", ex.Message, "OK");
                return false;
            }



        }

    }
}
