// Copyright (c) Baseflow. All rights reserved.
// Licensed under the Apache-2.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;

namespace Com.Airbnb.Lottie;

public partial class LottieComposition
{
    public partial class Factory
    {
        /// <summary>
        /// Asynchronously loads a composition from a file stored in /assets.
        /// </summary>
        /// <param name="context">Android context.</param>
        /// <param name="fileName">Asset file name.</param>
        /// <param name="onLoaded">Callback when composition is loaded.</param>
        /// <returns>Cancellable operation.</returns>
        public static ICancellable FromAssetFileName(Context context, string fileName, Action<LottieComposition?> onLoaded)
            => Factory.FromAssetFileName(context, fileName, new ActionCompositionLoaded(onLoaded));

        /// <summary>
        /// Asynchronously loads a composition from an arbitrary input stream.
        /// </summary>
        /// <param name="stream">Input stream containing JSON.</param>
        /// <param name="onLoaded">Callback when composition is loaded.</param>
        /// <returns>Cancellable operation.</returns>
        public static ICancellable FromInputStream(System.IO.Stream stream, Action<LottieComposition?> onLoaded)
            => Factory.FromInputStream(stream, new ActionCompositionLoaded(onLoaded));

        /// <summary>
        /// Asynchronously loads a composition from a JSON string. This is useful for animations loaded from the network.
        /// </summary>
        /// <param name="jsonString">JSON string containing animation data.</param>
        /// <param name="onLoaded">Callback when composition is loaded.</param>
        /// <returns>Cancellable operation.</returns>
        public static ICancellable FromJsonString(string jsonString, Action<LottieComposition?> onLoaded)
            => Factory.FromJsonString(jsonString, new ActionCompositionLoaded(onLoaded));

        /// <summary>
        /// Asynchronously loads a composition from a file stored in /assets.
        /// </summary>
        /// <param name="context">Android context.</param>
        /// <param name="fileName">Asset file name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that completes with the loaded composition.</returns>
        public static Task<LottieComposition?> FromAssetFileNameAsync(Context context, string fileName, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<LottieComposition?>(cancellationToken);

            var tcs = new TaskCompletionSource<LottieComposition?>();
            var cancelable = Factory.FromAssetFileName(context, fileName, composition =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                tcs.SetResult(composition);
            });

            cancellationToken.Register(() =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    cancelable.Cancel();
                    tcs.TrySetCanceled(cancellationToken);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Asynchronously loads a composition from an arbitrary input stream.
        /// </summary>
        /// <param name="stream">Input stream containing JSON.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that completes with the loaded composition.</returns>
        public static Task<LottieComposition?> FromInputStreamAsync(System.IO.Stream stream, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<LottieComposition?>(cancellationToken);

            var tcs = new TaskCompletionSource<LottieComposition?>();
            var cancelable = Factory.FromInputStream(stream, composition =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                tcs.SetResult(composition);
            });

            cancellationToken.Register(() =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    cancelable.Cancel();
                    tcs.TrySetCanceled(cancellationToken);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Asynchronously loads a composition from a raw JSON string. This is useful for animations loaded from the network.
        /// </summary>
        /// <param name="jsonString">JSON string containing animation data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that completes with the loaded composition.</returns>
        public static Task<LottieComposition?> FromJsonStringAsync(string jsonString, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<LottieComposition?>(cancellationToken);

            var tcs = new TaskCompletionSource<LottieComposition?>();
            var cancelable = Factory.FromJsonString(jsonString, composition =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                tcs.SetResult(composition);
            });

            cancellationToken.Register(() =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    cancelable.Cancel();
                    tcs.TrySetCanceled(cancellationToken);
                }
            });

            return tcs.Task;
        }

        private sealed class ActionCompositionLoaded(Action<LottieComposition?> onLoaded) : Java.Lang.Object, IOnCompositionLoadedListener
        {
            private readonly Action<LottieComposition?> _onLoaded = onLoaded;

            public void OnCompositionLoaded(LottieComposition? composition)
                => _onLoaded?.Invoke(composition);
        }
    }
}