using System;
using System.Globalization;
using DstCompensatedDateTime.Classes.Interface;

namespace DstCompensatedDateTime.Classes.Controller
{
    public class TimeModule : ITimeModule
    {
        public DateTime Timestamp { get; set; }
        public DateTime UnixEpoch { get; }
        public int TimestampUnix { get; set;}
        public DaylightTime Daylight { get; set; }
        public TimeZone LocalZone { get; set; }
        public DateTime CurrentDate { get; set; }
        public int CurrentYear { get;}
        public DateTime DaylightStart { get;}
        public DateTime DaylightEnd { get; }
        public TimeSpan DaylightDelta { get; }

        private ITimeNow _myTimeNow;
        
        public TimeModule(ITimeNow timeNow)
        {
            _myTimeNow = timeNow;
            LocalZone = TimeZone.CurrentTimeZone;
            CurrentDate = DateTime.Now;
            CurrentYear = CurrentDate.Year;
            Daylight = LocalZone.GetDaylightChanges(CurrentYear);
            DaylightStart = Daylight.Start;
            DaylightEnd = Daylight.End;
            DaylightDelta = Daylight.Delta;
            UnixEpoch = DateTime.UnixEpoch;
            this.CurrentTimeUtc();
        }


        public void CurrentTimeUtc()
        {
            DateTime utcNow = _myTimeNow.CurrentUtc();
            TimeSpan unixNow = utcNow - UnixEpoch;
            TimestampUnix = (int)unixNow.TotalSeconds;
            _convertToLocalTime(utcNow);
        }

        public void CurrentTimeLocal()
        {
            Timestamp = DateTime.Now;
        }
        
        public void OffsetTime(int days, int hours, int minutes)
        {
            var Offset = new TimeSpan(days, hours, minutes, 0);
            DateTime timeAfter = Timestamp.Add(Offset);
            
            _compensateForTimeChangeDueToDst(timeAfter);
        }

        public void ExternalTimeCorrection(DateTime dt)
        {
            _compensateForTimeChangeDueToDst(dt);
        }

        private void _compensateForTimeChangeDueToDst(DateTime timeAfter)
        {
            var timeDifference = timeAfter.Hour - Timestamp.Hour;

            Console.WriteLine("{0} - {1}   Timediff = {2}", Timestamp , timeAfter , timeDifference);
            bool timeWentFromWinterToSummerTime = timeDifference == 1;
            bool timeWentFromSummerToWinterTime = timeDifference == -1;

            if (timeWentFromWinterToSummerTime)
            {
                // Compensate for the extra hour on destination timestamp
                Timestamp = timeAfter.AddHours(-1);
                Console.WriteLine("Time went from Winter to Summer, Result = {0}", Timestamp);
            }
            else if (timeWentFromSummerToWinterTime)
            {
                // Compensate for the missing hour on destination timestamp
                Timestamp = timeAfter.AddHours(+1);
                Console.WriteLine("Time went from Summer to Winter Result = {0}", Timestamp);
            }
            else
            {
                Timestamp = timeAfter;
                Console.WriteLine("Time did not cross from Winter to Summer or vice versa!");
            }
        }

        private void _convertToLocalTime(DateTime dt)
        {
            int summerTime = 2;
            int winterTime = 1;
            DateTime localNow = dt.ToLocalTime();
            
            Timestamp = dt.AddHours(localNow.IsDaylightSavingTime() ? summerTime : winterTime);
        }
        
        public void ShowDaylightSavingPeriod()
        {
            const string dataFmt = "{0,-30}{1}";
            const string timeFmt = "{0,-30}{1:yyyy-MM-dd HH:mm}";
            
            // Display the current date and time and show if they occur 
            // in daylight saving time.
            Console.WriteLine("\n" + timeFmt, "Current date and time:",
                CurrentDate);
            Console.WriteLine(dataFmt, "Daylight saving time?",
                LocalZone.IsDaylightSavingTime(CurrentDate));

            // Display the daylight saving time range for the current year.
            Console.WriteLine(
                "\nDaylight saving time for year {0}:", CurrentYear);
            Console.WriteLine("{0:yyyy-MM-dd HH:mm} to " +
                              "{1:yyyy-MM-dd HH:mm}, delta: {2}",
                DaylightStart, DaylightEnd, DaylightDelta);
        }

        public void FromUnixToDateTime(int unix)
        {
            DateTime rawUtcTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(unix);

            _convertToLocalTime(rawUtcTimestamp);
        }

        public void FromDateTimeToUnix(DateTime dt)
        {
            
        }
    }
}