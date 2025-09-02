// <copyright file="LoadingView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Views.Menu
{
    using BovineLabs.Sample.UI.ViewModels.Menu;
    using Unity.AppUI.UI;
    using UnityEngine.UIElements;

    public class LoadingView : MenuBaseView<HomeViewModel>
    {
        public const string UssClassName = "bl-load-view";

        private string next;

        public LoadingView(HomeViewModel viewModel)
            : base(viewModel)
        {
            this.AddToClassList(UssClassName);

            var content = new Preloader();
            content.StretchToParentSize();
            this.Add(content);
        }
    }
}
