// <copyright file="OptionTab.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Views.Option
{
    using BovineLabs.Anchor;
    using BovineLabs.Sample.UI.ViewModels.Option;
    using Unity.AppUI.UI;
    using UnityEngine.UIElements;

    [IsService]
    public abstract class OptionTab : BaseVisualElement
    {
        public const string UssClassName = OptionsView.UssClassName + "__tab";

        protected OptionTab()
        {
            this.AddToClassList(UssClassName);
            this.StretchToParentSize();
        }

        public abstract OptionTabViewModel OptionsViewModel { get; }
    }

    public abstract class OptionTab<T> : OptionTab
        where T : OptionTabViewModel
    {
        protected OptionTab(T viewModel)
        {
            this.ViewModel = viewModel;
        }

        public T ViewModel { get; }

        public override OptionTabViewModel OptionsViewModel => this.ViewModel;
    }
}
