using Android.App;
using Android.Content;
//using Microsoft.Extensions.DependencyInjection.Abstractions;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using HR_Attendance_MAUI.Data;
using HR_Attendance_MAUI.Model;
using HR_Attendance_MAUI.Services;
using System.Net.Http.Json;
using Microsoft.Maui.Controls;


[assembly: Dependency(typeof(ForegroundService))]
namespace HR_Attendance_MAUI.Platforms.Android.Services
{
    [Service]
    public class AndroidBackgroundService : Service, IBackgroundService
    {
        public static bool IsForegroundServiceRunning;
        private Timer _timer;
        // private ILogger<AndroidBackgroundService> _logger;
        private readonly TimeSpan _delay = TimeSpan.FromMinutes(5);
        private AttendanceDatabaseService2 _attendanceDatabaseService;

        public AndroidBackgroundService()
        {

        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            //_logger.LogInformation("Background service starting.");
            //_timer = new Timer(DoWork, null, 0, 600000); // Run every 10 seconds


            //_timer = new Timer(DoWork, null, 0, Timeout.Infinite);
            var intent = new Intent(Platform.AppContext, typeof(AndroidBackgroundService));
            Platform.AppContext.StartForegroundService(intent);
            //StartService(new Intent(this, typeof(AndroidBackgroundService)));

        }

        public void Stop()
        {
            //_logger.LogInformation("Background service stopping.");
            //_timer?.Dispose();
            var intent = new Intent(Platform.AppContext, typeof(AndroidBackgroundService));
            Platform.AppContext.StopService(intent);
        }

        

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            Task.Run(async () =>
            {
                while (IsForegroundServiceRunning)
                {

                    var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "AttendanceDB.db3");
                    _attendanceDatabaseService = new AttendanceDatabaseService2(dbPath);
                    List<AttendanceData> attenadanceDataList = _attendanceDatabaseService.GetAllAttendances();

                    if (attenadanceDataList.Count == 0)
                    {
                        Stop();
                       
                    }
                    var networkAccess = Connectivity.NetworkAccess;
                    if (networkAccess == NetworkAccess.Internet)
                    {
                        bool result = await SyncAttendanceData();
                    }
                    else { 
                    
                    }
                        System.Diagnostics.Debug.WriteLine("foreground Service is Running");
                    Thread.Sleep(200000);
                }
            });

            string channelID = "ForeGroundServiceChannel";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                var notfificationChannel = new NotificationChannel(channelID, channelID, NotificationImportance.Low);
                notificationManager.CreateNotificationChannel(notfificationChannel);
            }

            var notificationBuilder = new NotificationCompat.Builder(this, channelID)
                                         .SetContentTitle("ForeGroundServiceStarted")
                                         .SetSmallIcon(Resource.Drawable.bhome)
                                         .SetContentText("Service Running in Foreground")
                                         .SetPriority(1)
                                         .SetOngoing(true)
                                         .SetChannelId(channelID)
                                         .SetAutoCancel(true);


            StartForeground(1001, notificationBuilder.Build());
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            IsForegroundServiceRunning = true;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            IsForegroundServiceRunning = false;
        }

        public bool IsForeGroundServiceRunning()
        {
            return IsForegroundServiceRunning;
        }
      
        //private async Task<bool> SyncAttendanceData()
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
                string inTimeDate1 = attendanceData.outTime;
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
                string a = ex.Message;
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                return false;
            }



        }

    }
}
