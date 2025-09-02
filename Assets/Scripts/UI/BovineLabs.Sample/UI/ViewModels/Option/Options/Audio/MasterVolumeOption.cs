// <copyright file="MasterVolumeOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Audio
{
    using BovineLabs.Anchor.Services;
    using BovineLabs.Sample.UI.ViewModels.Option.Options.Base;
    using Unity.Mathematics;
    using UnityEngine;

    public sealed class MasterVolumeOption : IntOption
    {
        /// <summary> Initializes a new instance of the <see cref="MasterVolumeOption"/> class. </summary>
        /// <param name="storageService"> Injected storage service. </param>
        public MasterVolumeOption(ILocalStorageService storageService)
            : base(storageService)
        {
            this.DefaultValue = (int)math.round(AudioListener.volume * this.HighValue);
        }

        /// <inheritdoc/>
        protected override string Title => "@UI:masterVolume";

        /// <inheritdoc/>
        protected override string Key => "master-volume";

        /// <inheritdoc/>
        protected override int DefaultValue { get; }

        /// <inheritdoc/>
        protected override int LowValue => 0;

        /// <inheritdoc/>
        protected override int HighValue => 100;

        /// <inheritdoc/>
        protected override void OnValueChanged(int newValue)
        {
            AudioListener.volume = newValue / (float)this.HighValue;
        }
    }
}
