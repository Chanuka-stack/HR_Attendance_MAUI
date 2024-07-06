using HR_Attendance_MAUI.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using HR_Attendance_MAUI.Data;
using HR_Attendance_MAUI.Model;
using System.Net.Http.Json;
using System.Text.Json;

namespace HR_Attendance_MAUI.Platforms.iOS
{
    public class iOSBackgroundService : IBackgroundService

    {
        public static bool IsForegroundServiceRunning;
        private CancellationTokenSource _cts;
        private AttendanceDatabaseService2 _attendanceDatabaseService;
        public void Start()
        {
            _cts = new CancellationTokenSource();
            Device.StartTimer(TimeSpan.FromMinutes(5), () =>
            {
                RunAsync(_cts.Token).ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Console.WriteLine("Background task completed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Background task failed.");
                    }
                });

                // Return true to keep the timer recurring
                return true;
            });
        }


        public bool IsForeGroundServiceRunning()
        {
            return IsForegroundServiceRunning;
        }
        public void Stop()
        {
            _cts?.Cancel();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            // Your background task logic here
            bool result = await SyncAttendanceData();
            await Task.Delay(5000, cancellationToken); // Simulate a task that takes time
        }

        private async Task<bool> SyncAttendanceData()
        {

            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
            _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);
            List<AttendanceData> attenadanceDataList = _attendanceDatabaseService.GetAllAttendances();

            if (attenadanceDataList.Count == 0)
            {
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
