// <copyright file="OptionsGameplayViewModel.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option
{
    using BovineLabs.Sample.UI.ViewModels.Option.Options.Gameplay;

    public class OptionsGameplayViewModel : OptionTabViewModel
    {
        public readonly LanguageOption Language;

        public OptionsGameplayViewModel()
        {
            this.CreateOption(out this.Language);
        }
    }
}
