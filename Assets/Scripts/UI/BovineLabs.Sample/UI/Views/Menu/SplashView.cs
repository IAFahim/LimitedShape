// <copyright file="SplashView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using Unity.AppUI.Navigation.Generated;

namespace BovineLabs.Sample.UI.Views.Menu
{
    using System;
    using System.ComponentModel;
    using BovineLabs.Anchor;
    using BovineLabs.Sample.UI.ViewModels.Menu;
    using Unity.AppUI.UI;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Utilities;
    using UnityEngine.UIElements;

    public class SplashView : MenuBaseView<SplashViewModel>
    {
        public const string UssClassName = "bl-splash-screen";
        public const string PressAnyButtonClassName = UssClassName + "__any-button";
        public const string PressAnyButtonTransitionClassName = PressAnyButtonClassName + "-flash";

        private readonly Text anyText;

        private IDisposable onAnyPress;

        /// <summary> Initializes a new instance of the <see cref="SplashView"/> class. </summary>
        /// <param name="viewModel"> The view model. </param>
        public SplashView(SplashViewModel viewModel)
            : base(viewModel)
        {
            this.AddToClassList(UssClassName);

            var preloader = new Preloader();
            preloader.StretchToParentSize();
            this.Add(preloader);

            this.anyText = new Text("@UI:pressAnyButton");
            this.anyText.AddToClassList(PressAnyButtonClassName);
            this.anyText.ToggleInClassList(PressAnyButtonTransitionClassName);
            this.anyText.style.display = DisplayStyle.None;
            this.anyText.size = TextSize.XL;
            this.Add(this.anyText);

            this.anyText.RegisterCallback<TransitionEndEvent, SplashView>((_, sv) => sv.anyText.ToggleInClassList(PressAnyButtonTransitionClassName), this);

            this.ViewModel.PropertyChanged += this.ViewModelOnPropertyChanged;
        }

        /// <summary> Finalizes an instance of the <see cref="SplashView"/> class. </summary>
        ~SplashView()
        {
            this.onAnyPress?.Dispose();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SplashViewModel.IsInitialized))
            {
                if (this.ViewModel.IsInitialized)
                {
                    this.anyText.style.display = DisplayStyle.Flex;
                    this.anyText.schedule.Execute(() => this.anyText.ToggleInClassList(PressAnyButtonTransitionClassName)).StartingIn(100);

                    this.onAnyPress = InputSystem.onAnyButtonPress.CallOnce(_ =>
                    {
                        this.Navigate(Actions.splash_to_home);
                        this.onAnyPress = null;
                    });
                }
                else
                {
                    this.onAnyPress?.Dispose();
                    this.onAnyPress = null;
                    this.anyText.style.display = DisplayStyle.None;
                }
            }
        }
    }
}
