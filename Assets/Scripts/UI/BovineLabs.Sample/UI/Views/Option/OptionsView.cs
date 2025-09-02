// <copyright file="OptionsView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Views.Option
{
    using BovineLabs.Anchor;
    using BovineLabs.Sample.UI.ViewModels.Option;
    using BovineLabs.Sample.UI.Views.Menu;
    using JetBrains.Annotations;
    using Unity.AppUI.UI;
    using Unity.AppUI.Navigation;
    using UnityEngine.UIElements;
    using Button = Unity.AppUI.UI.Button;

    [UsedImplicitly]
    public class OptionsView : MenuBaseView<OptionViewModel>
    {
        public const string UssClassName = "bl-options-view";
        public const string AppBarClassName = UssClassName + "__app-bar";
        public const string BackClassName = AppBarClassName + "__back-button";
        public const string TabsClassName = AppBarClassName + "__tabs";

        private const string ResetDefaultText = "@UI:resetDefault";
        private const string RevertText = "@UI:revert";
        private const string CancelText = "@UI:cancel";
        private const string ResetText = "@UI:reset";
        private const string ResetDescriptionText = "@UI:resetDescription";

        private readonly OptionTab[] swipeItems;
        private readonly SwipeView swipeView;

        public OptionsView(OptionsGameplayView gameplayView, OptionsGraphicsView graphicsView, OptionsAudioView audioView, OptionsControlsView controlsView)
            : base(new OptionViewModel())
        {
            this.AddToClassList(UssClassName);

            var appBar = new VisualElement {name = AppBarClassName};
            appBar.AddToClassList(AppBarClassName);

            var backButton = new ActionButton()
            {
                name = BackClassName,
                quiet = true,
                icon = "caret-left",
            };
            backButton.clickable.clickedWithEventInfo += this.BackButtonTriggered;
            appBar.Add(backButton);

            var tabs = new Tabs
            {
                name = TabsClassName,
                direction = Direction.Horizontal,
                emphasized = true,
            };
            tabs.AddToClassList(TabsClassName);

            tabs.Add(new TabItem { label = "@UI:optionGameplay" });
            tabs.Add(new TabItem { label = "@UI:optionGraphics" });
            tabs.Add(new TabItem { label = "@UI:optionAudio" });
            tabs.Add(new TabItem { label = "@UI:optionControls" });

            appBar.Add(tabs);
            appBar.Add(new Spacer());

            this.swipeItems = new OptionTab[]
            {
                gameplayView,
                graphicsView,
                audioView,
                controlsView,
            };

            this.swipeView = new SwipeView
            {
                swipeable = false,
                sourceItems = this.swipeItems,
                bindItem = (item, i) => item.Add(this.swipeItems[i]),
                style =
                {
                    flexGrow = 1,
                },
            };

            tabs.RegisterValueChangedCallback(evt => this.swipeView.SetValueWithoutNotify(evt.newValue));

            this.Add(appBar);
            this.Add(this.swipeView);
        }

        /// <inheritdoc/>
        protected override void SetupBottomNavBar(BottomNavBar bottomBar, NavController controller)
        {
            base.SetupBottomNavBar(bottomNavBar, controller);
            {
                var revertButton = new Button { title = RevertText };
                revertButton.clicked += this.Revert;
                bottomNavBar.Add(revertButton);
            }

            {
                var defaultButton = new Button { title = ResetDefaultText };
                defaultButton.clickable.clickedWithEventInfo += this.ResetToDefaults;
                bottomNavBar.Add(defaultButton);
            }
        }

        private void BackButtonTriggered(EventBase evt)
        {
            this.SaveAll();
            this.PopBackStack();
        }

        private void ResetToDefaults(EventBase evt)
        {
            if (evt.target is not VisualElement btn)
            {
                return;
            }

            var dialog = new AlertDialog
            {
                title = ResetDefaultText,
                description = ResetDescriptionText,
                variant = AlertSemantic.Destructive,
            };

            dialog.SetPrimaryAction(ActionIds.Reset, ResetText, () =>
            {
                var item = this.swipeItems[this.swipeView.value];
                item.OptionsViewModel.ResetToDefault();
            });
            dialog.SetCancelAction(ActionIds.Cancel, CancelText);

            var modal = Modal.Build(btn, dialog);
            modal.Show();
        }

        private void Revert()
        {
            var item = this.swipeItems[this.swipeView.value];
            item.OptionsViewModel.RevertAll();
        }

        private void SaveAll()
        {
            foreach (var item in this.swipeItems)
            {
                item.OptionsViewModel.Save();
            }
        }
    }
}