using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR_Attendance_MAUI.Services
{
    public static class HttpClientFactory
    {
        public static HttpClient CreateHttpClient()
        {
            HttpClient client;

            #if DEBUG
                HttpsClientHandlerService handler = new HttpsClientHandlerService();
                client = new HttpClient(handler.GetPlatformMessageHandler());
                client.BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5026" : "http://localhost:5026");
            #else
                client = new HttpClient();
                client.BaseAddress = new Uri("http://apps.emetsoft.com:91/HRA_APIv3/");
            #endif

            
            return client;
        }
    }
}
