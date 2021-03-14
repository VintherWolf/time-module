using System;
using DstCompensatedDateTime.Classes.Interface;
using NUnit.Framework;
using NSubstitute;
using TimeModule = DstCompensatedDateTime.Classes.Controller.TimeModule;

namespace DstCompensatedDateTime.Test.Unit
{
    [TestFixture]
    public class TimeModuleTest
    {
        private TimeModule _uut;
        private ITimeNow stubTimeNow;

        [SetUp]
        public void Setup()
        {
            stubTimeNow = Substitute.For<ITimeNow>();
            _uut = new TimeModule(stubTimeNow);
        }

        #region CurrentTimeUtc

        [Test]
        public void CurrentTimeUtc_TimestampIsCorrectUtcNowTime()
        {
            /* Arrange */
            string format = "MM/dd/yyyy HH:mm";
            var utcNow = DateTime.Now.ToString(format);

            /* Act */
            stubTimeNow.CurrentUtc().Returns(DateTime.UtcNow);
            _uut.CurrentTimeUtc();

            /* Assert */
            Assert.That(_uut.Timestamp.ToString(format), Is.EqualTo(utcNow));
        }

        [Test]
        public void CurrentTimeUtc_UnixEpochIsCorrect()
        {
            /* Arrange */
            var unixEpoch = DateTime.UnixEpoch;

            /* Act */
            _uut.CurrentTimeUtc();

            /* Assert */
            Assert.That(_uut.UnixEpoch, Is.EqualTo(unixEpoch));
        }

        [Test]
        public void CurrentTimeUtc_UnixTimestampIsCorrect()
        {
            /* Arrange */
            // Epoch + 1700000000s = 14.11.2023 22.13.20 GMT
            int ExpectedUnixTime = 1700000000;
            DateTime stubDate = new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc);

            /* Act */
            stubTimeNow.CurrentUtc().Returns(stubDate);
            _uut.CurrentTimeUtc();

            /* Assert */
            Assert.That(_uut.TimestampUnix, Is.EqualTo(ExpectedUnixTime));
        }

        #endregion CurrentTimeUtc

        #region CurrentTimeLocal

        [Test]
        public void CurrentTimeLocal_TimestampIsCorrectLocalNowTime()
        {
            /* Arrange */
            string format = "MM/dd/yyyy HH:mm";
            var localNow = DateTime.Now.ToString(format);

            /* Act */
            _uut.CurrentTimeLocal();

            /* Assert */
            Assert.That(_uut.Timestamp.ToString(format), Is.EqualTo(localNow));
        }

        #endregion CurrentTimeLocal

        #region OffsetTime

        [Test]
        public void OffsetTime_FromSummerToWinterTime_HoursIsCorrect()
        {
            /* Arrange */
            string format = "HH";
            var offsetFromSummerToWinterTime = new DateTime(2021,12,01,2,0,0).ToString(format);
            _uut.Timestamp = new DateTime(2021,06,1,2,0,0);

            /* Act */
            _uut.OffsetTime(180,0,0);
            Console.WriteLine("Timestamp: {0}",  _uut.Timestamp.ToString());

            /* Assert */
            Assert.That(_uut.Timestamp.ToString(format), Is.EqualTo(offsetFromSummerToWinterTime));
        }


        [Test]
        public void OffsetTime_FromWinterToSummerTime_HoursIsCorrect()
        {
            /* Arrange */
            string format = "HH";
            var offsetFromWinterToSummerTime = new DateTime(2021, 05, 02, 1, 0, 0).ToString(format);
            _uut.Timestamp = new DateTime(2021, 03, 13, 1, 0, 0);

            /* Act */
            _uut.OffsetTime(30, 0, 0);

            /* Assert */
            Assert.That(_uut.Timestamp.ToString(format), Is.EqualTo(offsetFromWinterToSummerTime));
        }

        #endregion offsetTime

        #region ExternalTimeCorrection
        [Test]
        public void ExternalTimeCorrection_FromWinterToSummerTime_HoursIsCorrect()
        {
            /* Arrange */
            string format = "HH";
            var externalTimeIsInSummerTime = new DateTime(2021, 05, 02, 2, 0, 0);
            // Expected to compensate -1 hour to keep in UTC +1 (Winter time)
            var strExtTime = externalTimeIsInSummerTime.AddHours(-1).ToString(format);

            _uut.Timestamp = new DateTime(2021, 03, 13, 1, 0, 0);

            /* Act */
            _uut.ExternalTimeCorrection(externalTimeIsInSummerTime);

            /* Assert */
            Assert.That(_uut.Timestamp.ToString(format), Is.EqualTo(strExtTime));
        }

        [Test]
        public void ExternalTimeCorrection_FromSummerToWinterTime_HoursIsCorrect()
        {
            /* Arrange */
            string format = "HH";

            // Source Timestamp: 13-05-2021 02:00:00
            _uut.Timestamp = new DateTime(2021, 05, 13, 2, 0, 0);
            // Destination Timestamp: 02-01-2021 01:00:00 + 1Hour
            var externalTimeIsInWinterTime = new DateTime(2021, 01, 02, 1, 0, 0);
            // +1 hour: Expected to compensate +1 hour to keep in UTC +2 (Summer time)
            var strExtTime = externalTimeIsInWinterTime.AddHours(1).ToString(format);

            /* Act */
            _uut.ExternalTimeCorrection(externalTimeIsInWinterTime);

            /* Assert */
            Assert.That(_uut.Timestamp.ToString(format), Is.EqualTo(strExtTime));
        }

        #endregion ExternalimeCorrection

        #region DaylightSavingPeriod
        [Test]
        public void DaylightSavingPeriod_Start_CorrectStartDate()
        {
            /* Arrange */
            string format = "yy:MM:dd";
            var dstStartDateDk = new DateTime(2021, 03, 28, 2, 0, 0).ToString(format);

            /* Act */


            /* Assert */
            Assert.That(_uut.DaylightStart.ToString(format), Is.EqualTo(dstStartDateDk));
        }

        [Test]
        public void DaylightSavingPeriod_End_CorrectEndDate()
        {
            /* Arrange */
            string format = "yy:MM:dd";
            var dstEndDateDk = new DateTime(2021, 10, 31, 3, 0, 0).ToString(format);

            /* Act */

            /* Assert */
            Assert.That(_uut.DaylightEnd.ToString(format), Is.EqualTo(dstEndDateDk));
        }
        #endregion DaylightSavingPeriod

        #region FromUnixToDateTime

        [Test]
        public void FromDateTimeToUnix_ValidUnixTime_TimestampIsCorrect()
        {
            /* Arrange */
            // Epoch + 1700000000s = 14.11.2023 23.13.20 Local Time
            int stubUnixTime = 1700000000;
            DateTime expectedDate = new DateTime(2023, 11, 14, 23, 13, 20, DateTimeKind.Utc);

            /* Act */
            _uut.FromUnixToDateTime(stubUnixTime);

            /* Assert */
            Assert.That(_uut.Timestamp, Is.EqualTo(expectedDate));
        }

        #endregion FromUnixToDateTime

        #region FromDateTimeToUnix

        [Test]
        public void FromDateTimeToUnix_ValidDateTime_TimestampUnixIsCorrect()
        {
            /* Arrange */
            // Epoch + 1700000000s = 14.11.2023 22.13.20 GMT
            int ExpectedUnixTime = 1700000000;
            DateTime stubDate = new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc);

            /* Act */
            _uut.FromDateTimeToUnix(stubDate);

            /* Assert */
            Assert.That(_uut.TimestampUnix, Is.EqualTo(ExpectedUnixTime));
        }

        #endregion FromDateTimeToUnix

    }
}