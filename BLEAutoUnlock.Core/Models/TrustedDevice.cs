using System;
using System.Collections.Generic;

namespace BLEAutoUnlock.Core.Models
{
    /// <summary>
    /// 受信任的蓝牙设备
    /// </summary>
    public class TrustedDevice
    {
        /// <summary>
        /// 设备唯一标识符
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 蓝牙 MAC 地址
        /// </summary>
        public ulong BluetoothAddress { get; set; }

        /// <summary>
        /// MAC 地址的字符串表示 (格式: AA:BB:CC:DD:EE:FF)
        /// </summary>
        public string MacAddress
        {
            get => FormatMacAddress(BluetoothAddress);
            set => BluetoothAddress = ParseMacAddress(value);
        }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后检测时间
        /// </summary>
        public DateTime? LastSeen { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 设备指纹（用于额外验证）
        /// </summary>
        public DeviceFingerprint? Fingerprint { get; set; }

        /// <summary>
        /// 格式化 MAC 地址
        /// </summary>
        public static string FormatMacAddress(ulong address)
        {
            byte[] bytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                bytes[5 - i] = (byte)((address >> (i * 8)) & 0xFF);
            }
            return BitConverter.ToString(bytes).Replace('-', ':');
        }

        /// <summary>
        /// 解析 MAC 地址字符串
        /// </summary>
        public static ulong ParseMacAddress(string macAddress)
        {
            string cleaned = macAddress.Replace(":", "").Replace("-", "");
            if (cleaned.Length != 12)
                throw new ArgumentException("Invalid MAC address format");

            ulong address = 0;
            for (int i = 0; i < 6; i++)
            {
                address = (address << 8) | Convert.ToByte(cleaned.Substring(i * 2, 2), 16);
            }
            return address;
        }

        public override string ToString() => $"{Name} ({MacAddress})";
    }

    /// <summary>
    /// 设备指纹（用于增强安全性）
    /// </summary>
    public class DeviceFingerprint
    {
        public string? LocalName { get; set; }
        public List<string> ServiceUuids { get; set; } = new();
        public byte[]? ManufacturerData { get; set; }
        public short? TxPower { get; set; }

        /// <summary>
        /// 检查指纹是否匹配
        /// </summary>
        public bool Matches(DeviceFingerprint other)
        {
            if (other == null) return false;

            // 本地名称匹配
            bool nameMatch = string.IsNullOrEmpty(LocalName) || LocalName == other.LocalName;

            // 服务 UUID 匹配（至少一个相同）
            bool serviceMatch = !ServiceUuids.Any() || 
                                ServiceUuids.Intersect(other.ServiceUuids).Any();

            // 制造商数据匹配
            bool mfgMatch = ManufacturerData == null || 
                           (other.ManufacturerData != null && 
                            ManufacturerData.SequenceEqual(other.ManufacturerData));

            return nameMatch && serviceMatch && mfgMatch;
        }
    }
}