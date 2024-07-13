using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR_Attendance_MAUI
{
    public static class AppManager
    {
        public static bool IsLoggedIn { get; set; } = false;
        public static System.Timers.Timer LogoutTimer { get; set; }
        public static async Task LogoutAsync()
        {
            IsLoggedIn = false;
            //Application.Current.MainPage = new MainPage();
            SecureStorage.RemoveAll();
            await Shell.Current.GoToAsync("//MainPage?clearHistory=true");
        }
    }
}
 