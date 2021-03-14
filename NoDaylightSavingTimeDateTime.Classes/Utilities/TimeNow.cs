using System;
using DstCompensatedDateTime.Classes.Interface;

namespace DstCompensatedDateTime.Classes.Utilities
{
    public class TimeNow : ITimeNow
    {
        public DateTime CurrentUtc()
        {
            return DateTime.UtcNow;
        }

        public DateTime CurrentLocal()
        {
            return DateTime.Now;
        }
    }
}