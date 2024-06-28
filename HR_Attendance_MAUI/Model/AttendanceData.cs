using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR_Attendance_MAUI.Model
{
    public class AttendanceData
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string inTimeDate { get; set; }
        public string inTime { get; set; }
        public string outTimeDate { get; set; }
        public string outTime { get; set; }
        public string employee_ID { get; set; }
        public string empAttendenceDescription { get; set; }
        public string latIn { get; set; }
        public string lonIn { get; set; }
        public string latOut { get; set; }
        public string lonOut { get; set; }

    }
}
