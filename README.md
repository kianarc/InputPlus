# InputPlus

**InputPlus** is a custom Grasshopper plugin for Rhino 3D, built with .NET 7.0. It enhances the native input capabilities of Grasshopper by providing advanced UI components and utilities to streamline geometry and data interaction.

## 🚀 Features

- Extended input components for parametric design workflows.
- Seamless integration with Grasshopper 8 and Rhino 8.
- Lightweight, performance-optimized `.gha` plugin.
- .NET 7.0 runtime support with desktop UI compatibility.

## 📦 Requirements

- Rhino 8
- Grasshopper 8 (version `8.0.23304.9001` or newer)
- [.NET 7.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## 🛠 Installation

1. Download the latest release from the [Releases](#) tab (or compile from source if available).
2. Place the `InputPlus.gha` file into your Grasshopper Components folder:
   ```
   %AppData%\Grasshopper\Libraries
   ```
3. Restart Rhino and open Grasshopper. You should see the **InputPlus** tab available.

## 🧪 Usage

Drag and drop the new input components from the **InputPlus** tab in the Grasshopper canvas.

*(Add images or gifs here demonstrating usage if available.)*

## 🧩 Development

This plugin targets:

- .NETCoreApp, Version=7.0
- Grasshopper 8 SDK
- RhinoCommon SDK

To build from source (when code is available):

```bash
dotnet build -c Release
```

## 📄 License

MIT License (or specify if different)
