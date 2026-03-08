# AC_DLSS - AiComi Plugin Template

This is a template for developing BepInEx plugins for **AiComi** using IL2CPP and .NET 6.0.

## Quick Start

1. Rename the project folders and files from `ExamplePlugin` → `AC_YourPluginName`
2. Update the namespace in `ExamplePlugin.cs` from `AC_ExamplePlugin` to `AC_YourPluginName`
3. Update the `GUID` and `PluginName` constants
4. Update the output path in `PluginCode.csproj` if needed
5. Build and the plugin will auto-deploy to `bin/BepInEx/plugins/`

## Tech Stack

- **Engine**: Unity IL2CPP
- **BepInEx**: 6.x (Unity)
- **Base Class**: `BepInEx.Unity.IL2CPP.BasePlugin`
- **Target Framework**: `.net6.0`
- **Harmony**: `HarmonyLib` (Postfix/Prefix Patches)
- **Packages**: `IllusionLibs.Aicomi.AllPackages` (2024.10.3)

## Plugin Structure

```text
AC_YourPluginName/
├── ExamplePlugin.cs      # Main plugin class (rename as needed)
├── PluginCode.csproj     # SDK-style project file
├── packages.config       # (no longer needed with SDK style)
└── Properties/
    └── AssemblyInfo.cs   # Assembly metadata
```

## Key Differences from Standard BepInEx

### Inheritance

- Use `BasePlugin` (not `BaseUnityPlugin`) from `BepInEx.Unity.IL2CPP`
- Entry point is `Load()` method (not `Awake()`)
- Static logger must be initialized: `Logger = Log;`

### IL2CPP Reflection

- Standard .NET reflection won't work for native game types
- Use dnSpy to browse `BepInEx/interop/` DLLs for method/field locations
- Private methods in interop assemblies are often accessible

### Harmony Patches

```csharp
[HarmonyPostfix]
[HarmonyPatch(typeof(GameClass), nameof(GameClass.GameMethod))]
private static void AfterMethod(GameClass __instance)
{
    // __instance = the object the method was called on
}
```

## Debugging

1. **Log in game**: `UnityEngine.Debug.Log(...)` → appears as `[Message: Unity]` in BepInEx log
2. **Find fields**: RuntimeUnityEditor (F7) → Object Browser
3. **Find methods**: dnSpy → `BepInEx/interop/` DLLs
4. **IL2CPP Reflection**: Avoid standard Type.GetFields() — use game's native methods

## Configuration

```csharp
// Normal setting
Config.Bind("Section", "Key", defaultValue, "Description");

// Advanced setting (only visible when Debug mode enabled)
Config.Bind("Debug", "Key", false,
    new ConfigDescription("Description", tags: new[] { "Advanced" })
);
```

## Building & Deployment

Build the project and the plugin DLL will automatically copy to `bin/BepInEx/plugins/AC_YourPluginName/`

For releases, use the included `release.ps1` script.
