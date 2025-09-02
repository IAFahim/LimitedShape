// <copyright file="OptionsAudioViewModel.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option
{
    using BovineLabs.Sample.UI.ViewModels.Option.Options.Audio;

    public class OptionsAudioViewModel : OptionTabViewModel
    {
        public readonly MasterVolumeOption MasterVolume;

        public OptionsAudioViewModel()
        {
            this.CreateOption(out this.MasterVolume);
        }
    }
}
