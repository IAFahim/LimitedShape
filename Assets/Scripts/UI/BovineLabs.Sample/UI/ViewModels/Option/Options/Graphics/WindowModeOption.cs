// <copyright file="WindowModeOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

#if UNITY_STANDALONE
namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Graphics
{
    using System;
    using System.Collections.Generic;
    using BovineLabs.Anchor.Services;
    using BovineLabs.Sample.UI.ViewModels.Option.Options.Base;
    using UnityEngine;

    public sealed class WindowModeOption : ListOption
    {
        private readonly FullScreenMode[] modes;

        /// <summary> Initializes a new instance of the <see cref="WindowModeOption"/> class. </summary>
        /// <param name="storageService"> Injected storage service. </param>
        public WindowModeOption(ILocalStorageService storageService)
            : base(storageService)
        {
            this.modes = new[]
            {
#if UNITY_STANDALONE_WIN
                FullScreenMode.ExclusiveFullScreen,
#endif
                FullScreenMode.FullScreenWindow,
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                FullScreenMode.MaximizedWindow,
#endif
                FullScreenMode.Windowed,
            };

            this.ItemSource = new List<string>
            {
#if UNITY_STANDALONE_WIN
                "@UI:windowModeExclusiveFullScreen",
#endif
                "@UI:windowModeFullScreenWindow",
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                "@UI:windowModeMaximizedWindow",
#endif
                "@UI:windowModeWindow",
            };

            this.DefaultValue = Array.IndexOf(this.modes, Screen.fullScreenMode);
        }

        /// <inheritdoc/>
        protected override string Title => "@UI:windowMode";

        /// <inheritdoc/>
        protected override string Key => "window-mode";

        /// <inheritdoc/>
        protected override int DefaultValue { get; }

        /// <inheritdoc/>
        protected override List<string> ItemSource { get; }

        /// <inheritdoc/>
        protected override void OnValueChanged(int newValue)
        {
            if (newValue < 0 || newValue >= this.modes.Length)
            {
                return;
            }

            var c = Screen.currentResolution;
            Screen.SetResolution(c.width, c.height, this.modes[newValue]);
        }
    }
}
#endif
