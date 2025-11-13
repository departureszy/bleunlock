// UnlockSettings.cs

using System;

namespace BLEAutoUnlock.Core.Models
{
    public class UnlockSettings
    {
        public double UnlockDistance { get; set; } = 2.0;
        public double LockDistance { get; set; } = 4.0;
        public int ScanInterval { get; set; } = 2000;
        public int RssiSampleSize { get; set; } = 5;
        public int UnlockDelay { get; set; } = 3;
        public int LockDelay { get; set; } = 5;
        public int DeviceTimeout { get; set; } = 30;
        public bool EnableAutoUnlock { get; set; } = false;
        public bool EnableAutoLock { get; set; } = true;
        public double PathLossExponent { get; set; } = 2.5;
        public int ReferenceTxPower { get; set; } = -59;
        public bool UseDeviceFingerprint { get; set; } = true;
        public bool ShowLockNotification { get; set; } = false;
        public bool ShowUnlockNotification { get; set; } = false;
        public TimeRange WorkingHours { get; set; }
        public bool OnlyDuringWorkingHours { get; set; } = false;
    }

    public class TimeRange
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsInRange(TimeSpan time)
        {
            return time >= StartTime && time <= EndTime;
        }
    }
}