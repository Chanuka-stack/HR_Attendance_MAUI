using HR_Attendance_MAUI.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace HR_Attendance_MAUI.Platforms.iOS
{
    public class iOSBackgroundService : IBackgroundService
    {
        private readonly ILogger<iOSBackgroundService> _logger;

        public iOSBackgroundService(ILogger<iOSBackgroundService> logger)
        {
            _logger = logger;
        }

        public void Start()
        {
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
        }

        public void Stop()
        {
            // iOS does not have a straightforward way to stop background fetch
        }
    }

}
