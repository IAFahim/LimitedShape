// <copyright file="OptionsAudioView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Views.Option
{
    using BovineLabs.Sample.UI.ViewModels.Option;

    public class OptionsAudioView : OptionTab<OptionsAudioViewModel>
    {
        public new const string UssClassName = OptionTab.UssClassName + "__audio";

        public OptionsAudioView(OptionsAudioViewModel viewModel)
            : base(viewModel)
        {
            this.AddToClassList(UssClassName);

            this.Add(viewModel.MasterVolume.CreateElement());
        }
    }
}
