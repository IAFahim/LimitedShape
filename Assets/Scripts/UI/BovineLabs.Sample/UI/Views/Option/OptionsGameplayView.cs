// <copyright file="OptionsGameplayView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Views.Option
{
    using BovineLabs.Sample.UI.ViewModels.Option;

    public class OptionsGameplayView : OptionTab<OptionsGameplayViewModel>
    {
        public new const string UssClassName = OptionTab.UssClassName + "__gameplay";

        public OptionsGameplayView(OptionsGameplayViewModel viewModel)
            : base(viewModel)
        {
            this.AddToClassList(UssClassName);

            this.Add(viewModel.Language.CreateElement());
        }
    }
}
