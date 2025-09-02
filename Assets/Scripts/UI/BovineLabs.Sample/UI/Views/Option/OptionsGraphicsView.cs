// <copyright file="OptionsGraphicsView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Views.Option
{
    using BovineLabs.Sample.UI.ViewModels.Option;

    public class OptionsGraphicsView : OptionTab<OptionsGraphicsViewModel>
    {
        public new const string UssClassName = OptionTab.UssClassName + "__graphics";

        public OptionsGraphicsView(OptionsGraphicsViewModel viewModel)
            : base(viewModel)
        {
            this.AddToClassList(UssClassName);

            this.Add(viewModel.Resolution.CreateElement());
#if UNITY_STANDALONE
            this.Add(viewModel.WindowMode.CreateElement());
#endif
        }
    }
}
