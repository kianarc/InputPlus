# InputPlus

**InputPlus** is a custom Grasshopper plugin for Rhino 3D, built with .NET 7.0. It enhances the native input capabilities of Grasshopper by enabling robust data persistence for referenced geometry.

## 🚀 Features

- Stores object data (e.g., Breps, Curves) **inside the Grasshopper document**.
- Ensures that components retain their data **even if the original Rhino objects are deleted**.
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

Use the InputPlus components to reference Rhino objects like Breps and Curves. Once referenced, the geometry data is stored directly within the Grasshopper file, so it remains accessible even if the original object in Rhino is deleted.

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
