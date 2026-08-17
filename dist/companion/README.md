# White Label Launcher Companion

This is a standalone Windows companion executable. It does not install a Playnite plug-in. Put `WhiteLabelLauncher.exe` and `WhiteLabelLauncher.ini` in the same directory as `Playnite.exe` or `Playnite.DesktopApp.exe`, then launch `WhiteLabelLauncher.exe` instead of launching Playnite directly.

The companion starts the adjacent Playnite executable with its working directory set to the Playnite folder. While Playnite is running, it periodically reapplies the configured window title. It supports the documented `--startdesktop` and `--startfullscreen` launch modes through `StartFullscreen` in the INI file. Any command-line arguments passed to `WhiteLabelLauncher.exe` are forwarded to Playnite.

## Same-folder layout

```text
Playnite folder/
├── Playnite.exe or Playnite.DesktopApp.exe
├── WhiteLabelLauncher.exe
└── WhiteLabelLauncher.ini
```

Edit `WhiteLabelLauncher.ini` to change the visible window title. To start Fullscreen mode, set `StartFullscreen=true`. To start Desktop mode, set it to `false`.

## Build

On Windows, install Visual Studio Build Tools with the .NET Framework 4.6.2 targeting pack. Run `build-on-windows.ps1`. The generated bundle contains the executable and configuration file.

## Limitation

A standalone companion can safely control the Playnite process and its top-level window, but Windows does not allow it to directly manipulate every WPF control inside another process without unsupported code injection or internal Playnite modifications. Therefore this version changes the title bar and provides the correct self-contained launch path; a custom Playnite Desktop/Fullscreen theme remains necessary to remove every logo, menu label, settings surface, and built-in visual element.
