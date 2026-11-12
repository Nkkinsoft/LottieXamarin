// Copyright (c) Baseflow. All rights reserved.
// Licensed under the Apache-2.0 license. See LICENSE file in the project root for full license information.

namespace Com.Airbnb.Lottie;

public partial class LottieAnimationView
{
    /// <summary>
    /// Delegate to handle the loading of bitmaps that are not packaged in the assets of your app.
    /// </summary>
    /// <param name="funcAssetLoad">Function to load bitmap from a Lottie image asset.</param>
    public void SetImageAssetDelegate(Func<LottieImageAsset, Android.Graphics.Bitmap?> funcAssetLoad)
    {
        SetImageAssetDelegate(new ImageAssetDelegateImpl(funcAssetLoad));
    }

    private sealed class ImageAssetDelegateImpl(Func<LottieImageAsset, Android.Graphics.Bitmap?> funcAssetLoad)
        : Java.Lang.Object, IImageAssetDelegate
    {
        private readonly Func<LottieImageAsset, Android.Graphics.Bitmap?> _funcAssetLoad = funcAssetLoad;

        public Android.Graphics.Bitmap? FetchBitmap(LottieImageAsset asset)
            => _funcAssetLoad(asset);
    }
}