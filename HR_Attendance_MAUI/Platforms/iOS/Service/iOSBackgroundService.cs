using Foundation;
using UIKit;
using HR_Attendance_MAUI.Services;
using HR_Attendance_MAUI.Data;
using HR_Attendance_MAUI.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.IO;
using Microsoft.Maui.Controls;
using HR_Attendance_MAUI.Platforms.iOS.Service;

[assembly: Dependency(typeof(iOSBackgroundService))]
namespace HR_Attendance_MAUI.Platforms.iOS.Service
{
    [Register("iOSBackgroundService")]
    public class iOSBackgroundService : NSObject, IBackgroundService
    {
        private nint _taskId;
        private Timer _timer;
        private AttendanceDatabaseService2 _attendanceDatabaseService;
        private readonly TimeSpan _delay = TimeSpan.FromMinutes(5);

        public static bool IsForegroundServiceRunning;

        public void Start()
        {
            IsForegroundServiceRunning = true;
            _taskId = UIApplication.SharedApplication.BeginBackgroundTask("BackgroundService", OnExpiration);

            Task.Run(async () =>
            {
                while (IsForegroundServiceRunning)
                {
                    var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
                    _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);
                    List<AttendanceData> attendanceDataList = _attendanceDatabaseService.GetAllAttendances();

                    if (attendanceDataList.Count == 0)
                    {
                        Stop();
                    }

                    var networkAccess = Connectivity.NetworkAccess;
                    if (networkAccess == NetworkAccess.Internet)
                    {
                        bool result = await SyncAttendanceData();
                    }

                    Console.WriteLine("Background Service is Running");
                    await Task.Delay(_delay);
                }

                UIApplication.SharedApplication.EndBackgroundTask(_taskId);
                _taskId = UIApplication.BackgroundTaskInvalid;
            });
        }

        public void Stop()
        {
            IsForegroundServiceRunning = false;
            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }

        public bool IsForeGroundServiceRunning()
        {
            return IsForegroundServiceRunning;
        }
        private void OnExpiration()
        {
            Stop();
        }

        private async Task<bool> SyncAttendanceData()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
            _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);
            List<AttendanceData> attendanceDataList = _attendanceDatabaseService.GetAllAttendances();

            if (attendanceDataList.Count == 0)
            {
                return false;
            }

            foreach (AttendanceData attendanceData in attendanceDataList)
            {
                string inTimeDate = attendanceData.inTimeDate;
                string inTimeDate1 = attendanceData.outTime;
                string inTimeDate2 = attendanceData.lonOut;
                string inTimeDate3 = attendanceData.latOut;
            }

            DateTimeOffset currentTime1 = DateTimeOffset.Now;
            DateTime currentTime = currentTime1.DateTime;
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);
            string currentDate = currentTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            HttpClient client;
            try
            {
                client = HttpClientFactory.CreateHttpClient();
                var response = await client.PostAsJsonAsync("api/Attendance/StoreList", attendanceDataList);

                if (response.IsSuccessStatusCode)
                {
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
                string errorMessage = ex.Message;
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                return false;
            }
        }
    }
}
