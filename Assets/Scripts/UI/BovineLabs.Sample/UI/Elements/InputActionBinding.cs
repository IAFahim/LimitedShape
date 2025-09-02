// <copyright file="InputActionBinding.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Elements
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Unity.AppUI.Core;
    using Unity.AppUI.UI;
    using Unity.Properties;
    using UnityEngine.InputSystem;
    using UnityEngine.UIElements;
    using Button = Unity.AppUI.UI.Button;

    [UxmlElement]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "UITK standard")]
    public partial class InputActionBinding : BaseVisualElement
    {
        public const string UssClassName = "bl-input-action";
        public const string TextClassName = UssClassName + "__text";
        public const string ButtonClassName = UssClassName + "__button";

        private const string Title = "@UI:rebindTitle";
        private const string Description = "@UI:rebindDescription";

        private static readonly BindingId LabelProperty = nameof(LabelProperty);
        private static readonly BindingId InputActionProperty = nameof(inputAction);

        private readonly Text textField;
        private readonly Button buttonField;

        private ActionBinding actionBinding = new();
        private InputActionRebindingExtensions.RebindingOperation rebindOperation;

        public InputActionBinding()
        {
            this.AddToClassList(UssClassName);

            this.textField = new Text();
            this.textField.AddToClassList(TextClassName);
            this.buttonField = new Button();
            this.buttonField.AddToClassList(ButtonClassName);

            this.Add(this.textField);
            this.Add(this.buttonField);

            this.buttonField.clicked += this.PerformInteractiveRebind;
        }

        public event Action BindingChanged;

#pragma warning disable SA1300
        [CreateProperty]
        [UxmlAttribute]
        public string text
        {
            get => this.textField.text;
            set
            {
                if (this.textField.text != value)
                {
                    this.textField.text = value;
                    this.NotifyPropertyChanged(in LabelProperty);
                }
            }
        }

        [CreateProperty]
        [UxmlAttribute]
        public ActionBinding inputAction
        {
            get => this.actionBinding;
            set
            {
                if (!this.actionBinding.Equals(value))
                {
                    this.actionBinding = value;
                    this.RefreshBindingText();
                    this.NotifyPropertyChanged(in InputActionProperty);
                }
            }
        }
#pragma warning restore SA1300

        public void RefreshBindingText()
        {
            if (!this.inputAction.ResolveActionAndBinding(out var action, out var bindingIndex))
            {
                this.buttonField.title = string.Empty;
                return;
            }

            InputBinding.DisplayStringOptions displayOptions = default;
            var displayText = action.GetBindingDisplayString(bindingIndex, out _, out _, displayOptions);
            this.buttonField.title = displayText;
        }

        private void PerformInteractiveRebind()
        {
            if (!this.inputAction.ResolveActionAndBinding(out var action, out var bindingIndex))
            {
                return;
            }

            var popup = ShowPopup(this.buttonField);

            this.rebindOperation?.Cancel(); // Will null out rebindOperation.

            // Fixes the "InvalidOperationException: Cannot rebind action x while it is enabled" error
            action.Disable();

            // Configure the rebind.
            this.rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .OnCancel(_ =>
                {
                    // this.UpdateBindingDisplay();
                    CleanUp();
                })
                .OnComplete(_ =>
                {
                    this.RefreshBindingText();
                    this.BindingChanged?.Invoke();
                    CleanUp();
                });

            // Just don't allow escape to be bound ever (non button already set this)
            if (this.rebindOperation.expectedControlType == "Button")
            {
                this.rebindOperation.WithCancelingThrough("<Keyboard>/escape");
            }

            this.rebindOperation.Start();
            return;

            void CleanUp()
            {
                this.rebindOperation?.Dispose();
                this.rebindOperation = null;
                action.Enable();

                popup.Dismiss(DismissType.Cancel);
            }

            static Modal ShowPopup(VisualElement ve)
            {
                var dialog = new AlertDialog
                {
                    title = Title,
                    description = Description,
                };

                var modal = Modal.Build(ve, dialog);
                modal.Show();
                return modal;
            }
        }
    }
}
