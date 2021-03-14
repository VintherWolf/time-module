using System;

namespace DstCompensatedDateTime.Classes.Interface
{
    public interface ITimeNow
    {
        public DateTime CurrentUtc();
        public DateTime CurrentLocal();
    }
}