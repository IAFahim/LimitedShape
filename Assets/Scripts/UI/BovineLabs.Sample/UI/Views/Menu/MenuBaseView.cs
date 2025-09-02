// <copyright file="MenuBaseView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Views.Menu
{
    public abstract class MenuBaseView<T> : BaseScreen<T>
    {
        public const string MenuClassName = "bl-menu-view";

        protected MenuBaseView(T viewModel)
            : base(viewModel)
        {
            this.AddToClassList(MenuClassName);
        }
    }
}
