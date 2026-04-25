# NeonFtool

NeonFtool is a lightweight, high-performance automation utility built with .NET 8 for Windows. It provides advanced keyboard macro and spamming capabilities with a focus on ease of use, dynamic targeting, and low-level efficiency.

![Banner](https://img.shields.io/badge/Platform-Windows-blue)
![Framework](https://img.shields.io/badge/.NET-8.0-purple)
![License](https://img.shields.io/badge/License-MIT-green)

## 🚀 Key Features

- **Dynamic Targeting**: Use Regex patterns to match target window titles. No need to constantly re-select windows when they restart or change titles.
- **Multiple Spammer Groups**: Configure up to 10 independent spamming slots, each with its own hotkey, interval, and target.
- **Parallel Execution**: Run multiple spammers simultaneously or configure them to stop others when triggered.
- **Integrated Overlay**: A draggable, lockable UI overlay that anchors to your target application, providing real-time status updates without intrusive injection.
- **Global Hotkeys**: Control your macros from anywhere with customizable global hotkeys (CTRL/ALT combinations).
- **Intelligent Interruption**: Automatically stop all active spammers when specific keys (1-0, F1-F12) are pressed to prevent macro interference during manual play.
- **Window Manager**: Built-in utility to manage active windows, including the ability to hide/show specific processes.
- **Persistence**: All configurations, including window regexes and intervals, are automatically saved and restored on startup.

## 🛠️ Getting Started

### Prerequisites
- [Windows 10/11](https://www.microsoft.com/windows)
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation
1. Download the latest release from the [Releases](https://github.com/NeonSpectrum/NeonFtool/releases) page.
2. Extract the ZIP file to a folder of your choice.
3. Run `NeonFtool.exe`.

## 📖 How to Use

1. **Target a Window**: Enter a regex pattern in the "Window" text box (e.g., `^Notepad` or `.*Chrome.*`). The tool will automatically find matching processes.
2. **Configure Keys**: Select the F-Key or Skill Key you wish to spam.
3. **Set Interval**: Adjust the interval (in milliseconds) to control the speed of the spammer.
4. **Assign Hotkey**: Choose a global hotkey to toggle the spammer on/off.
5. **Start Spamming**: Press the assigned hotkey or click the "Start" button.

## 💻 Building from Source

To build NeonFtool locally, you will need Visual Studio 2022 or the .NET 8 SDK.

```powershell
# Clone the repository
git clone https://github.com/NeonSpectrum/NeonFtool.git

# Navigate to the project directory
cd NeonFtool

# Build the project
dotnet build -c Release
```

The compiled binaries will be located in `NeonFtool/bin/Release/net8.0-windows/`.

## 🛡️ Security & Integrity

NeonFtool uses standard Windows API calls (`SendMessage`, `PostMessage`, and low-level hooks) to interact with windows. It **does not** use memory injection or read/write process memory, making it safer to use with applications that have integrity checks.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
*Created with ❤️ by NeonSpectrum*
