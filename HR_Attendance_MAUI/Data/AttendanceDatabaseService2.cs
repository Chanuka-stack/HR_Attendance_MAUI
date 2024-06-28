using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR_Attendance_MAUI.Model;
using SQLite;

namespace HR_Attendance_MAUI.Data
{
    public class AttendanceDatabaseService2
    {
        private SQLiteConnection _db;

        public AttendanceDatabaseService2(string dbPath)
        {
            _db = new SQLiteConnection(dbPath);
            _db.CreateTable<AttendanceData>();  // Create the table

            _db.CreateTable<TodaySyncedAttendance>();

        }

        public void InsertAttendance(AttendanceData attendance)
        {
            _db.Insert(attendance);  // Insert a record into the table
        }



        public List<AttendanceData> GetAllAttendances()
        {
            return _db.Table<AttendanceData>()
                      .Select(a => new AttendanceData
                      {
                          inTimeDate = a.inTimeDate,
                          inTime = a.inTime,
                          outTimeDate = a.outTimeDate,
                          outTime = a.outTime,
                          employee_ID = a.employee_ID,
                          empAttendenceDescription = a.empAttendenceDescription,
                          latIn = a.latIn,
                          lonIn = a.lonIn,
                          latOut = a.latOut,
                          lonOut = a.lonOut
                      })
                      .ToList();
        }
        /* public List<AttendanceData> GetAllAttendances()
         {
             return _db.Table<AttendanceData>().ToList();  // Retrieve all records
         }*/

        public AttendanceData GetAttendanceById(int id)
        {
            return _db.Table<AttendanceData>().FirstOrDefault(x => x.id == id);  // Retrieve a record by ID
        }

        public void DeleteAttendance(int id)
        {
            _db.Delete<AttendanceData>(id);  // Delete a record by ID
        }

        public void UpdateAttendance(AttendanceData attendance)
        {
            _db.Update(attendance);  // Update the record
        }

        public void DeleteAllAttendances()
        {
            _db.Execute("DELETE FROM AttendanceData");  // Delete all records
            _db.Execute("VACUUM");  // Optional: Reclaim space after deletion
        }
        public AttendanceData GetAttendanceByDate(string date)
        {
            return _db.Table<AttendanceData>()
                      .FirstOrDefault(x => x.inTimeDate == date);  // Return the first record matching the date
        }
        public void DeleteAllExceptCurrentDate(string currentDate)
        {

            _db.Execute($"DELETE FROM AttendanceData WHERE InTimeDate != ?", currentDate);

            _db.Execute("VACUUM");
        }

        public void StoreTodaysRecordsAndDeleteAll(string currentDate)
        {

            var record = _db.Table<AttendanceData>()
                                  .FirstOrDefault(a => a.inTimeDate == currentDate);

            var existingRecord = _db.Table<TodaySyncedAttendance>()
                                   .FirstOrDefault(a => a.inTimeDate == currentDate);

            if (existingRecord != null)
            {
                var todaysAttendance = new TodaySyncedAttendance
                {

                    outTimeDate = record.outTimeDate,
                    outTime = record.outTime,
                    employee_ID = record.employee_ID,
                    empAttendenceDescription = record.empAttendenceDescription,
                    latOut =record.latOut,
                    lonOut = record.lonOut
                };
                _db.Update(todaysAttendance);
            }
            else
            {
                var todaysAttendance = new TodaySyncedAttendance
                {
                    inTimeDate = record.inTimeDate,
                    inTime = record.inTime,
                    outTimeDate = record.outTimeDate,
                    outTime = record.outTime,
                    employee_ID = record.employee_ID,
                    empAttendenceDescription = record.empAttendenceDescription,
                    latIn = record.latIn,
                    lonIn = record.lonIn,
                    latOut = record.latOut,
                    lonOut = record.lonOut
                };
                _db.Insert(todaysAttendance);
            }


            _db.DeleteAll<AttendanceData>();
        }

        public AttendanceData GetTodayAttendnaceOffline(string currentDate)
        {
            _db.Execute($"DELETE FROM TodaySyncedAttendance WHERE InTimeDate != ?", currentDate);

            _db.Execute("VACUUM");

            var record = _db.Table<TodaySyncedAttendance>()
                            .Where(a => a.inTimeDate == currentDate)
                            .FirstOrDefault();

            if (record == null)
            {
                return null;
            }

            var todaysAttendance = new AttendanceData
            {
                inTimeDate = record.inTimeDate,
                inTime = record.inTime,
                outTimeDate = record.outTimeDate,
                outTime = record.outTime,
                employee_ID = record.employee_ID,
                empAttendenceDescription = record.empAttendenceDescription,
                latIn = record.latIn,
                lonIn = record.lonIn,
                latOut = record.latOut,
                lonOut = record.lonOut
            };

            return todaysAttendance;
        }

        public int GetTotalAttendanceCount()
        {
            return _db.Table<AttendanceData>().Count();
        }


        public void UpdateAttendanceByDateOffline(AttendanceData attendanceData)
        {
            var recordToUpdate = _db.Table<AttendanceData>()
                .FirstOrDefault(a => a.inTimeDate == attendanceData.inTimeDate && a.employee_ID == attendanceData.employee_ID);


            if (recordToUpdate != null)
            {

                recordToUpdate.outTime = attendanceData.outTime;
                recordToUpdate.outTimeDate = attendanceData.outTimeDate;
                recordToUpdate.empAttendenceDescription = attendanceData.empAttendenceDescription;
                recordToUpdate.latOut = attendanceData.latOut;
                recordToUpdate.lonOut = attendanceData.lonOut;


                _db.Update(recordToUpdate);
            }
            else
            {
                _db.Insert(attendanceData);
            }
        }
        public void InsertTodaySyncedAttendance(TodaySyncedAttendance attendance)
        {
            _db.Insert(attendance);
        }

        public void UpdateSyncedAttendanceByDateOffline(TodaySyncedAttendance attendanceData)
        {
            var recordToUpdate = _db.Table<TodaySyncedAttendance>()
                .FirstOrDefault(a => a.inTimeDate == attendanceData.inTimeDate && a.employee_ID == attendanceData.employee_ID);


            if (recordToUpdate != null)
            {

                recordToUpdate.outTime = attendanceData.outTime;
                recordToUpdate.outTimeDate = attendanceData.outTimeDate;
                recordToUpdate.empAttendenceDescription = attendanceData.empAttendenceDescription;
                recordToUpdate.latOut = attendanceData.latOut;
                recordToUpdate.lonOut = attendanceData.lonOut;


                _db.Update(recordToUpdate);
            }
        }

    }
}
