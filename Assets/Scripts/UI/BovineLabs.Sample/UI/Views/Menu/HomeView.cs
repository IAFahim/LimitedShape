// <copyright file="HomeView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using Unity.AppUI.Navigation.Generated;

namespace BovineLabs.Sample.UI.Views.Menu
{
    using BovineLabs.Anchor;
    using BovineLabs.Sample.UI.ViewModels.Menu;
    using Unity.AppUI.Navigation;
    using Unity.AppUI.UI;
    using UnityEngine.UIElements;

    public class HomeView : MenuBaseView<HomeViewModel>
    {
        public const string UssHomeClassName = "bl-home-view";
        public const string LeftClassName = UssHomeClassName + "__left";
        public const string RightClassName = UssHomeClassName + "__right";

        public const string ButtonClassName = UssHomeClassName + "__button";

        private const string ContinueText = "@UI:continue";
        private const string PlayText = "@UI:play";
        private const string LoadText = "@UI:loadGame";
        private const string OptionsText = "@UI:options";
        private const string QuitText = "@UI:quit";
        private const string QuitTitleText = "@UI:quitTitle";
        private const string QuitDescriptionText = "@UI:quitDescription";
        private const string QuitCancelText = "@UI:quitCancel";

        private readonly ActionButton continueButton;
        private readonly ActionButton loadButton;

        public HomeView(HomeViewModel viewModel)
            : base(viewModel)
        {
            this.AddToClassList(UssHomeClassName);

            var left = new ActionGroup { direction = Direction.Vertical };
            left.AddToClassList(LeftClassName);
            this.Add(left);

            var right = new VisualElement();
            right.AddToClassList(RightClassName);
            this.Add(right);

            this.continueButton = new ActionButton(this.Continue) { label = ContinueText };
            this.continueButton.AddToClassList(ButtonClassName);
            left.Add(this.continueButton);

            var playButton = new ActionButton(this.Play)
            {
                label = PlayText,
            };

            playButton.AddToClassList(ButtonClassName);
            left.Add(playButton);

            this.loadButton = new ActionButton(this.Load) { label = LoadText };
            this.loadButton.AddToClassList(ButtonClassName);
            left.Add(this.loadButton);

            var optionButton = new ActionButton(this.Options) { label = OptionsText };
            optionButton.AddToClassList(ButtonClassName);
            left.Add(optionButton);

#if UNITY_STANDALONE
            var quitButton = new ActionButton
            {
                label = QuitText,
            };

            quitButton.AddToClassList(ButtonClassName);
            quitButton.clickable.clickedWithEventInfo += Quit;
            left.Add(quitButton);
#endif
            PrivateGame();
        }

        public override void OnEnter(NavController controller, NavDestination destination, Argument[] args)
        {
            base.OnEnter(controller, destination, args);

            var hasSaveDisplay = false ? DisplayStyle.Flex : DisplayStyle.None;
            this.continueButton.style.display = hasSaveDisplay;
            this.loadButton.style.display = hasSaveDisplay;
        }

#if UNITY_STANDALONE
        private static void Quit(EventBase evt)
        {
            if (evt.target is ExVisualElement btn)
            {
                var dialog = new AlertDialog
                {
                    description = QuitDescriptionText,
                    variant = AlertSemantic.Destructive,
                    title = QuitTitleText,
                };

                dialog.SetPrimaryAction(99, QuitText, () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    UnityEngine.Application.Quit();
#endif
                });

                dialog.SetCancelAction(1, QuitCancelText);

                var modal = Modal.Build(btn, dialog);
                modal.Show();
            }
        }

#endif

        private void Continue()
        {
        }

        private void Play()
        {
            this.Navigate(Actions.home_to_play);
        }
        
        private void PrivateGame()
        {
            Play();
            this.ViewModel.Value.Private.TryProduce();
        }

        private void Load()
        {
        }

        private void Options()
        {
            this.Navigate(Actions.go_to_options);
        }
    }
}