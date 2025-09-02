// <copyright file="ResolutionOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BovineLabs.Anchor.Services;
    using BovineLabs.Sample.UI.ViewModels.Option.Options.Base;
    using UnityEngine;

    public sealed class ResolutionOption : ListOption
    {
        private readonly Vector2Int[] resolutions;

        /// <summary> Initializes a new instance of the <see cref="ResolutionOption"/> class. </summary>
        /// <param name="storageService"> Injected storage service. </param>
        public ResolutionOption(ILocalStorageService storageService)
            : base(storageService)
        {
            this.resolutions = Screen.resolutions.Select(r => new Vector2Int(r.width, r.height)).Distinct().ToArray();
            this.ItemSource = new List<string>(this.resolutions.Select(r => $"{r.x} x {r.y}"));
            this.DefaultValue = Array.IndexOf(this.resolutions, new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height));
        }

        /// <inheritdoc/>
        protected override string Title => "@UI:resolution";

        /// <inheritdoc/>
        protected override string Key => "resolution";

        /// <inheritdoc/>
        protected override int DefaultValue { get; }

        /// <inheritdoc/>
        protected override List<string> ItemSource { get; }

        /// <inheritdoc/>
        protected override void OnValueChanged(int newValue)
        {
            if (newValue < 0 || newValue >= this.resolutions.Length)
            {
                return;
            }

            var r = this.resolutions[newValue];
            Screen.SetResolution(r.x, r.y, Screen.fullScreenMode);
        }
    }
}
