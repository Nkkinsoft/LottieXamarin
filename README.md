# Com.Nkkinsoft.Android.Lottie

<p align="center">
  <img src="assets/icon.svg" alt="Lottie Android Binding" width="128" height="128">
</p>

<p align="center">
  <strong>Lottie Android binding for .NET (nkkinsoft fork)</strong> — Render After Effects animations natively on Android via Xamarin/.NET for Android. Based on <a href="https://github.com/airbnb/lottie-android/releases/tag/6.7.1">lottie-android 6.7.1</a>.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/Com.Nkkinsoft.Android.Lottie/"><img src="https://img.shields.io/nuget/v/Com.Nkkinsoft.Android.Lottie?label=NuGet&color=cc3333&logo=nuget" alt="NuGet version"></a>
  <a href="https://www.nuget.org/packages/Com.Nkkinsoft.Android.Lottie/"><img src="https://img.shields.io/nuget/dt/Com.Nkkinsoft.Android.Lottie?color=0d9488&logo=nuget" alt="NuGet downloads"></a>
  <a href="https://github.com/airbnb/lottie-android/releases"><img src="https://img.shields.io/badge/Upstream-6.7.1-14b8a6?logo=airbnb" alt="Upstream version"></a>
  <a href="https://github.com/Nkkinsoft/LottieXamarin"><img src="https://img.shields.io/badge/Binding-LottieXamarin-661818?logo=xamarin" alt="Binding project"></a>
  <img src="https://img.shields.io/badge/TFM-net10.0--android%20%7C%20net11.0--android-0f1011?logo=dotnet" alt="Target frameworks">
  <img src="https://img.shields.io/badge/C%23-15-0f1011?logo=csharp" alt="C# 15">
  <img src="https://img.shields.io/badge/Nullable-enabled-14b8a6" alt="Nullable enabled">
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-Apache--2.0-cc3333" alt="License"></a>
</p>

> **Note:** This is a community fork by **nkkinsoft**. The official Airbnb package is `Com.Airbnb.Android.Lottie`. This fork provides .NET 10/11, C# 15 support, modernized bindings, and trimmed-AOT compatibility.

---

## Why this binding?

| Concern | This Package |
|---------|--------------|
| **Upstream parity** | Tracks [lottie-android](https://github.com/airbnb/lottie-android) releases (currently **6.7.1**) |
| **Modern .NET** | `net10.0-android` + `net11.0-android`, C# 15, primary constructors, `Nullable`/`ImplicitUsings` enabled |
| **Trim-safe** | Annotated for .NET trimming / R8; no reflection surprises in Release AOT |
| **Async-first** | `Task<T>` overloads for every load operation (`FromAssetFileNameAsync`, `FromJsonStringAsync`, …) |
| **Zero-conflict** | Single `Metadata.xml` — no duplicate-type conflicts with other AndroidX bindings |
| **Symbols included** | `.snupkg` published alongside every release for debugging |

---

## Install

```xml
<!-- PackageReference -->
<PackageReference Include="Com.Nkkinsoft.Android.Lottie" Version="6.7.2" />

<!-- Or via CLI -->
dotnet add package Com.Nkkinsoft.Android.Lottie --version 6.7.2
```

**Requirements**
- .NET 10+ (`net10.0-android`) or .NET 11 (`net11.0-android`)
- Android API 24+ (`minSdkVersion 24`)
- `AndroidX.AppCompat` 1.8.0+ (transitive)
- `OkIO` 3.9.0+ (transitive)

---

## Quick Start

```xml
<!-- Resources/layout/activity_main.axml -->
<com.airbnb.lottie.LottieAnimationView
    android:id="@+id/animation_view"
    android:layout_width="200dp"
    android:layout_height="200dp"
    app:lottie_fileName="animation.json"
    app:lottie_autoPlay="true"
    app:lottie_loop="true" />
```

```csharp
// Activity.cs
using Com.Airbnb.Lottie;

protected override void OnCreate(Bundle? savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    SetContentView(Resource.Layout.activity_main);

    var animationView = FindViewById<LottieAnimationView>(Resource.Id.animation_view);
    
    // Or load programmatically
    animationView.SetAnimation("loading.json");
    animationView.PlayAnimation();
}
```

---

## Async API (Recommended)

```csharp
using Com.Airbnb.Lottie;
using System.Threading.Tasks;

public async Task LoadAnimationAsync(string assetName)
{
    var composition = await LottieComposition.Factory.FromAssetFileNameAsync(this, assetName);
    if (composition != null)
    {
        animationView.Composition = composition;
        animationView.PlayAnimation();
    }
}

// From JSON string (network-loaded animations)
var json = await httpClient.GetStringAsync("https://cdn.example.com/anim.json");
var composition = await LottieComposition.Factory.FromJsonStringAsync(json);

// From stream
using var stream = await httpClient.GetStreamAsync("https://cdn.example.com/anim.json");
var composition = await LottieComposition.Factory.FromInputStreamAsync(stream);
```

---

## Key Types

| Type | Purpose |
|------|---------|
| `LottieAnimationView` | Main view — XML or programmatic |
| `LottieComposition` | Immutable animation data model |
| `LottieComposition.Factory` | Async/sync loaders (`FromAssetFileNameAsync`, `FromJsonStringAsync`, …) |
| `LottieDrawable` | Drawable variant for `ImageView`/custom views |
| `LottieProperty` | Animated properties (color, position, opacity, …) for dynamic overrides |
| `ICancellable` | Cancel in-flight loads |
| `LottieListener` / `IOnCompositionLoadedListener` | Completion callbacks |

---

## Migration from 4.x (LottieXamarin 4.2.2)

| 4.x | 6.x |
|-----|-----|
| `LottieAnimationView.SetAnimation(string)` | `LottieAnimationView.SetAnimation(string)` ✓ |
| `LottieAnimationView.Progress` | `LottieAnimationView.Progress` ✓ |
| `LottieCompositionFactory.FromAssetFileName` | `LottieComposition.Factory.FromAssetFileNameAsync` |
| `LottieCompositionFactory.FromJsonString` | `LottieComposition.Factory.FromJsonStringAsync` |
| `app:lottie_rawRes` | **Removed** — use `FromAssetFileNameAsync` |
| `LottieDrawable` | Still available, prefer `LottieAnimationView` |

**Breaking:** Sync factory methods return `ICancellable`; prefer `*Async` overloads returning `Task<LottieComposition?>`.

---

## ProGuard / R8 / Trimming

No extra rules needed — the binding includes `[DynamicallyAccessedMembers]` annotations. For full AOT (`ReleaseFullAot`):

```xml
<PropertyGroup Condition="'$(Configuration)' == 'ReleaseFullAot'">
  <PublishReadyToRun>true</PublishReadyToRun>
  <EnableLLVM>true</EnableLLVM>
  <AndroidLinkMode>SdkOnly</AndroidLinkMode>
  <TrimmerRootAssembly Include="Com.Nkkinsoft.Android.Lottie" RootMode="library" />
</PropertyGroup>
```

---

## Building from Source

```bash
# Prerequisites
dotnet workload install android

# Build Release (produces .nupkg + .snupkg in artifacts/)
dotnet build Lottie.Android/Lottie.Android.csproj -c Release

# Pack
dotnet pack Lottie.Android/Lottie.Android.csproj -c Release -o ./artifacts
```

---

## Versioning

| This Package | Upstream (lottie-android) | Notes |
|--------------|---------------------------|-------|
| 6.7.2 | 6.7.1 | .NET 10/11, C# 15, async API |
| 4.2.2 | 4.2.2 | Legacy Xamarin binding |

We follow upstream **minor.patch**; binding improvements increment **build metadata** (e.g., `6.7.2.1`).

---

## License

Apache-2.0 — same as [lottie-android](https://github.com/airbnb/lottie-android/blob/master/LICENSE).

```
Copyright 2024 Baseflow
Copyright 2024 Airbnb, Inc.
Copyright 2024 nkkinsoft (fork modifications)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
```

---

## Contributing

1. Fork the [binding repo](https://github.com/Nkkinsoft/LottieXamarin)
2. Update `Directory.Build.props` version
3. Modernize any generated `.cs` in `Additions/`
4. Run `dotnet build -c Release` — zero warnings expected
5. Open PR with upstream changelog link

---

## Links

- **Upstream:** https://github.com/airbnb/lottie-android
- **Binding source (this fork):** https://github.com/Nkkinsoft/LottieXamarin
- **NuGet (this fork):** https://www.nuget.org/packages/Com.Nkkinsoft.Android.Lottie
- **Issues:** https://github.com/Nkkinsoft/LottieXamarin/issues
- **Sample animations:** https://lottiefiles.com