// <copyright file="PlayView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using Unity.AppUI.Navigation.Generated;

namespace BovineLabs.Sample.UI.Views.Menu
{
    using BovineLabs.Anchor;
    using BovineLabs.Sample.UI.ViewModels.Menu;
    using Unity.Properties;
    using UnityEngine.UIElements;
    using Button = Unity.AppUI.UI.Button;

    public class PlayView : MenuBaseView<HomeViewModel>
    {
        public const string UssClassName = "bl-play-view";

        private const string PrivateText = "@UI:privateGame";
        private const string PrivateSubText = "@UI:privateGameSub";
        private const string HostText = "@UI:hostGame";
        private const string HostSubText = "@UI:hostGameSub";
        private const string JoinText = "@UI:joinGame";
        private const string JoinSubText = "@UI:joinGameSub";

        public PlayView(HomeViewModel viewModel)
            : base(viewModel)
        {
            this.AddToClassList(UssClassName);

            var privateButton = new Button(this.PrivateGame)
            {
                title = PrivateText,
                subtitle = PrivateSubText,
            };

            this.Add(privateButton);

            var onlineDataBinding = new DataBinding
            {
                bindingMode = BindingMode.ToTarget,
                updateTrigger = BindingUpdateTrigger.EveryUpdate,
                dataSourcePath = new PropertyPath(nameof(HomeViewModel.OnlineAvailable)),
            };

            var hostButton = new Button(this.HostGame)
            {
                title = HostText,
                subtitle = HostSubText,
                dataSource = this.ViewModel,
            };

            hostButton.SetBinding(nameof(this.enabledSelf), onlineDataBinding);

            this.Add(hostButton);

            var joinButton = new Button(this.JoinGame)
            {
                title = JoinText,
                subtitle = JoinSubText,
                dataSource = this.ViewModel,
            };

            joinButton.SetBinding(nameof(this.enabledSelf), onlineDataBinding);

            this.Add(joinButton);
        }

        private void PrivateGame()
        {
            this.ViewModel.Value.Private.TryProduce();
        }

        private void HostGame()
        {
            this.ViewModel.Value.Host.TryProduce();
            this.ToGoLoading();
        }

        private void JoinGame()
        {
            this.ViewModel.Value.Join.TryProduce();
            this.ToGoLoading();
        }

        private void ToGoLoading()
        {
            this.Navigate(Actions.go_to_loading);
        }
    }
}
