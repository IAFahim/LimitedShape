// <copyright file="SplashViewModel.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Menu
{
    using BovineLabs.Anchor;
    using Unity.AppUI.MVVM;

    [IsService]
    public class SplashViewModel : ObservableObject
    {
        private bool isInitialized;

        public bool IsInitialized
        {
            get => this.isInitialized;
            set => this.SetProperty(ref this.isInitialized, value);
        }
    }
}
