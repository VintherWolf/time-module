using System;
using System.Globalization;
using DstCompensatedDateTime.Classes.Interface;

namespace DstCompensatedDateTime.Classes.Controller
{
    public class TimeModule : ITimeModule
    {
        public DateTime Timestamp { get; set; }
        public DaylightTime Daylight { get; set; }
        public TimeZone LocalZone { get; set; }
        public DateTime CurrentDate { get; set; }
        public int CurrentYear { get;}
        public DateTime DaylightStart { get;}
        public DateTime DaylightEnd { get; }
        public TimeSpan DaylightDelta { get; }
        
        public TimeModule()
        {
            LocalZone = TimeZone.CurrentTimeZone;
            CurrentDate = DateTime.Now;
            CurrentYear = CurrentDate.Year;
            Daylight = LocalZone.GetDaylightChanges(CurrentYear);
            DaylightStart = Daylight.Start;
            DaylightEnd = Daylight.End;
            DaylightDelta = Daylight.Delta;
        }


        public void CurrentTimeUtc()
        {
            DateTime utcNow = DateTime.UtcNow;

            _compensateNowTimeForDst(utcNow);
        }

        public void CurrentTimeLocal()
        {
            Timestamp = DateTime.Now;
        }
        
        public void OffsetTime(int days, int hours, int minutes)
        {
            var Offset = new TimeSpan(days, hours, minutes, 0);
            DateTime timeAfter = Timestamp.Add(Offset);
            
            _compensateOffsetTimeForDst(timeAfter);
        }

        public void ExternalTimeCorrection(DateTime dt)
        {
            _compensateOffsetTimeForDst(dt);
        }

        private void _compensateOffsetTimeForDst(DateTime timeAfter)
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

        private void _compensateNowTimeForDst(DateTime dt)
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
            throw new NotImplementedException();
        }

        public void FromDateimeToUnix(DateTime dt)
        {
            throw new NotImplementedException();
        }
    }
}