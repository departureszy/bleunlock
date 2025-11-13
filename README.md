# BLE Auto Unlock - Windows 蓝牙自动解锁系统

<div align="center">

![Windows](https://img.shields.io/badge/Windows-10%2F11-0078D6?logo=windows&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green.svg)
![Bluetooth](https://img.shields.io/badge/Bluetooth-LE%205.0-blue?logo=bluetooth&logoColor=white)

**通过蓝牙距离感应自动解锁/锁定 Windows 电脑**

[功能特性](#-功能特性) • [快速开始](#-快速开始) • [工作原理](#-工作原理) • [文档](#-文档)

</div>

---

## 📖 项目简介

BLE Auto Unlock 是一款基于蓝牙 BLE（低功耗蓝牙）技术的 Windows 自动解锁软件。当您携带手机或智能手环靠近电脑时自动解锁，离开时自动锁屏，无需开发手机 APP。

### 🎯 适用场景

- 🏢 **办公场景**：离开座位自动锁屏，回到座位自动解锁
- 🏠 **家庭使用**：保护隐私，便捷访问
- 🔐 **安全增强**：物理距离作为认证因素

---

## ✨ 功能特性

### 核心功能

- ✅ **无需手机 APP** - 利用手机系统原生蓝牙广播
- ✅ **智能距离检测** - 基于 RSSI 信号强度精准计算距离
- ✅ **自动锁屏/解锁** - 可配置距离阈值和延迟时间
- ✅ **多设备支持** - 可添加多个信任设备（手机、手环等）
- ✅ **低功耗** - 被动扫描模式，不影响手机续航

### 安全特性

- 🔐 **设备指纹验证** - 多因素识别防止伪造
- 🔐 **加密存储** - 使用 Windows DPAPI 加密设备信息
- 🔐 **防误触机制** - 延迟触发 + 距离双重确认
- 🔐 **审计日志** - 记录所有解锁/锁屏事件

### 高级特性

- ⚙️ **距离算法优化** - 中位数滤波 + 移动平均，抗干扰
- ⚙️ **可配置参数** - 距离阈值、扫描频率、环境系数等
- ⚙️ **时间策略** - 仅在工作时间启用
- ⚙️ **系统托盘** - 常驻后台，便捷管理

---

## 🚀 快速开始

### 系统要求

- **操作系统**：Windows 10 (1809+) 或 Windows 11
- **蓝牙硬件**：支持 BLE 4.0+ 的蓝牙适配器
- **开发环境**：Visual Studio 2022 + .NET 8.0 SDK

### 安装步骤

#### 方式一：直接运行（开发者）
1. **克隆仓库**
```bash
git clone https://github.com/departureszy/bleunlock.git
cd bleunlock
```
2. **还原依赖**
```bash
dotnet restore
```
3. **构建项目**
```bash
dotnet build --configuration Release
```
4. **运行 UI 程序**
```bash
cd BLEAutoUnlock.UI/bin/Release/net8.0-windows10.0.19041.0
BLEAutoUnlock.UI.exe
```

#### 方式二：安装为 Windows 服务（推荐）
```powershell
# 以管理员身份运行
cd BLEAutoUnlock.Service/bin/Release/net8.0-windows10.0.19041.0

# 安装服务
sc create "BLEAutoUnlock" binPath= "%CD%\BLEAutoUnlock.Service.exe"
sc description "BLEAutoUnlock" "Bluetooth Auto Unlock Service"
sc start "BLEAutoUnlock"
```

---

## 🔧 使用指南

### 1️⃣ 添加信任设备
1. 确保手机蓝牙已开启
2. 打开 BLE Auto Unlock UI 程序
3. 点击"设备管理" → "扫描设备"
4. 在列表中找到您的手机（查看手机蓝牙设置确认名称）
5. 点击"添加为信任设备"

### 2️⃣ 配置参数
进入"设置"界面，调整以下参数：

| 参数 | 说明 | 推荐值 |
|------|------|--------|
| **解锁距离** | 设备靠近多少米时解锁 | 1.5 - 2.0 米 |
| **锁屏距离** | 设备离开多少米时锁屏 | 3.0 - 5.0 米 |
| **扫描间隔** | 多久扫描一次 | 2000 毫秒 |
| **解锁延迟** | 检测到设备后延迟多久解锁 | 3 秒 |
| **锁屏延迟** | 设备离开后延迟多久锁屏 | 5 秒 |

### 3️⃣ 启用自动功能
- ✅ 勾选"启用自动解锁"
- ✅ 勾选"启用自动锁屏"
- 💡 **重要提示**：标准 Windows API 无法直接实现自动解锁，需要配合 Credential Provider 或 Companion Device Framework

---

## 🛠️ 工作原理

### 技术架构
```
┌─────────────────────────────────────────┐
│        手机/手环（无需 APP）             │
│     系统蓝牙广播 MAC 地址                │
└──────────────┬──────────────────────────┘
               │ BLE Advertisement
               ▼
┌─────────────────────────────────────────┐
│         PC - BLE 扫描器                 │
│  1. 被动扫描蓝牙广播                     │
│  2. 获取 RSSI 信号强度                  │
│  3. 计算距离（路径损耗模型）             │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│      距离判断引擎                        │
│  - RSSI 滤波（中位数 + 移动平均）        │
│  - 距离公式：d = 10^((TxPower-RSSI)/10N) │
│  - 状态机：远离 ↔ 靠近                   │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│       会话管理器                         │
│  - 距离 < 2m → 触发解锁（需 CP/CDF）     │
│  - 距离 > 4m → 执行锁屏                  │
└─────────────────────────────────────────┘
```

### 距离计算公式

使用**路径损耗模型**（Log-Distance Path Loss Model）：

```
Distance (米) = 10 ^ ((TxPower - RSSI) / (10 * N))

其中：
- TxPower: 发射功率（1米处参考 RSSI，默认 -59dBm）
- RSSI: 接收信号强度指示（实时测量值）
- N: 路径损耗指数（环境系数，范围 2.0-4.0）
  · 2.0 = 自由空间
  · 2.5 = 办公室（推荐）
  · 3.0 = 有障碍物
  · 4.0 = 密集障碍
```

### 信号滤波算法

为提高准确性，采用**两级滤波**：

1. **中位数滤波** - 去除突变干扰
2. **移动平均** - 平滑信号波动

---

## 📚 文档

- [安装指南](docs/INSTALLATION.md) - 详细安装步骤
- [使用手册](docs/USAGE.md) - 完整功能说明
- [开发指南](docs/DEVELOPMENT.md) - 二次开发文档
- [故障排除](docs/TROUBLESHOOTING.md) - 常见问题解决

---

## ⚠️ 重要说明

### 关于自动解锁

**当前版本的自动锁屏功能可以正常工作，但自动解锁功能受限于 Windows 安全机制。**

要实现完整的自动解锁，需要以下方案之一：

1. **Credential Provider** (推荐)
   - 开发自定义凭据提供程序集成到 Windows 登录界面
   - 安全性高，用户体验好
   - 需要 C++ 开发和系统级权限

2. **Companion Device Framework**
   - 使用 Windows 10+ 的配套设备框架
   - 官方支持，与 Windows Hello 集成
   - 需要 UWP 应用

3. **当前实现**
   - 本项目提供完整的距离检测和设备管理框架
   - 自动锁屏功能完全可用
   - 为未来集成 CP/CDF 预留接口

### 隐私声明

- ✅ 所有数据本地存储，不上传云端
- ✅ 使用 Windows DPAPI 加密敏感信息
- ✅ 仅扫描蓝牙 MAC 地址，不获取其他信息
- ✅ 开源透明，代码可审计

---

## 🗺️ 开发路线图

### ✅ Phase 1 - 核心功能（已完成）
- [x] BLE 被动扫描
- [x] 距离计算算法
- [x] 设备管理
- [x] 自动锁屏
- [x] 设置持久化

### 🔄 Phase 2 - 自动解锁（进行中）
- [ ] Credential Provider 开发
- [ ] Companion Device Framework 集成
- [ ] Windows Hello 集成

### 📋 Phase 3 - 功能增强（计划中）
- [ ] 智能手环支持（小米、华为等）
- [ ] 地理围栏（仅在特定位置生效）
- [ ] 多用户支持
- [ ] 云同步设置

### 🎨 Phase 4 - 用户体验（计划中）
- [ ] 现代化 UI（WinUI 3）
- [ ] 安装程序（MSI）
- [ ] 自动更新
- [ ] 多语言支持

---

## 🤝 贡献指南

欢迎贡献代码、报告问题或提出建议！

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

---

## 📄 许可证

本项目采用 [MIT License](LICENSE) 开源协议。

---

## 👨‍💻 作者

**departureszy**

- GitHub: [@departureszy](https://github.com/departureszy)
- 项目链接: [https://github.com/departureszy/bleunlock](https://github.com/departureszy/bleunlock)

---

## 🙏 致谢

感谢以下项目和资源的启发：

- [Windows Bluetooth LE API](https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/bluetooth)
- [Credential Provider Framework](https://docs.microsoft.com/en-us/windows/win32/secauthn/credential-providers-in-windows)
- [Companion Device Framework](https://docs.microsoft.com/en-us/windows/uwp/security/companion-device-unlock)

---

<div align="center">

**如果这个项目对您有帮助，请给个 ⭐ Star！**

Made with ❤️ by departureszy

</div>
