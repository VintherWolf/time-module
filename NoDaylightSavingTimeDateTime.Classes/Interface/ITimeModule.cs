using System;

namespace DstCompensatedDateTime.Classes.Interface
{
    public interface ITimeModule
    {
        public void CurrentTimeUtc();
        public void CurrentTimeLocal();
        public void OffsetTime(int days, int hours, int minutes);
        public void ExternalTimeCorrection(DateTime dt);
        public void ShowDaylightSavingPeriod();
        public void FromUnixToDateTime(int unix);
        public void FromDateTimeToUnix(DateTime dt);

    }
}