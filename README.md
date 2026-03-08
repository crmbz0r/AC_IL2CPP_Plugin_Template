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

## Plugin Skeleton

```csharp
using BepInEx;
using BepInEx.Unity.IL2CPP;   // not BepInEx alone!
using HarmonyLib;

[BepInPlugin(GUID, PluginName, Version)]
public class Plugin : BasePlugin   // not BaseUnityPlugin!
{
    public const string GUID = "com.author.myplugin";
    public const string PluginName = "My Plugin";
    public const string Version = "1.0.0";

    internal static BepInEx.Logging.ManualLogSource Logger = null!;

    public override void Load()    // not Awake()!
    {
        Logger = Log;
        Harmony.CreateAndPatchAll(typeof(MyPatch));
    }
}
```

## Key Differences from Standard BepInEx

### Inheritance

- Use `BasePlugin` (not `BaseUnityPlugin`) from `BepInEx.Unity.IL2CPP`
- Entry point is `Load()` method (not `Awake()`)
- Static logger must be initialized: `Logger = Log;`

### IL2CPP Reflection

- Standard .NET reflection won't work for native game types — only wrapper fields (`isWrapped`, `pooledPtr`) will be visible
- Real fields/methods → look them up in `BepInEx/interop/` via dnSpy
- Private methods in interop assemblies are often `public` (IL2CPP wrapper makes everything accessible)
- `GetComponentInChildren<T>()` works normally ✅
- Use `Il2CppInterop.Runtime.Il2CppType.Of<T>()` instead of `typeof(T).GetFields()` for IL2CPP reflection

### Harmony Patches

```csharp
[HarmonyPatch(typeof(TargetClass))]
internal static class MyPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(TargetClass.TargetMethod))]
    private static void AfterMethod(TargetClass __instance)
    {
        // __instance = the object the method was called on
    }
}
```

### ANSI Colors in BepInEx Console

ANSI escape codes are natively supported in the BepInEx console window:

```csharp
private const string GREEN = "\u001b[32m";
private const string RESET = "\u001b[0m";

Logger.LogInfo($"{GREEN}Success!{RESET}");
```

## Debugging

1. **Log in game**: `UnityEngine.Debug.Log(...)` → appears as `[Message: Unity]` in BepInEx log
2. **Find fields**: RuntimeUnityEditor (F7) → Object Browser → click on component
3. **Find methods/types**: dnSpy → open `BepInEx/interop/` DLLs → search for class
4. **IL2CPP Reflection**: `Il2CppInterop.Runtime.Il2CppType.Of<T>()` instead of `typeof(T).GetFields()`

## Configuration

```csharp
// Normal setting
Config.Bind("Section", "Key", defaultValue, "Description");

// Advanced/Debug setting (only visible when Debug mode enabled)
Config.Bind("Debug", "Key", false,
    new ConfigDescription("Description", tags: new[] { "Advanced" })
);
```

## ScrollRect / LoopGridView

```csharp
// LoopGridView works with rows, not flat item indices
int row = selectedIndex / columns;

var scrollRect = win.GetComponentInChildren<UnityEngine.UI.ScrollRect>();
float scrollable = scrollRect.content.rect.height - scrollRect.viewport.rect.height;

// 1.0 = top, 0.0 = bottom → invert!
float normalized = 1f - Mathf.Clamp01((row * itemHeight) / scrollable);
scrollRect.verticalNormalizedPosition = normalized;
```

## Gotchas & Pitfalls

- **`BasePlugin`** not `BaseUnityPlugin` — plugin won't load otherwise
- **`Load()`** not `Awake()` — the entry point has a different name in BepInEx 6 IL2CPP
- **`Log` is an instance property** → store it statically so patch classes can access it
- **`\u001b` vs `\\u001b`** — double backslash means no ANSI escape, colors won't appear
- **`GetComponentIndex()`** on LoopGridView returns UI hierarchy depth, *not* item index → look for a game-specific method instead (e.g. `GetCurrentIndex()`)
- **`GenerateAssemblyInfo`** — set `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>` in the csproj when using `Properties/AssemblyInfo.cs`, otherwise you'll get duplicate attribute errors

## Building & Deployment

Build the project and the plugin DLL will automatically copy to `bin/BepInEx/plugins/AC_YourPluginName/`

For releases, use the included `release.ps1` script.
