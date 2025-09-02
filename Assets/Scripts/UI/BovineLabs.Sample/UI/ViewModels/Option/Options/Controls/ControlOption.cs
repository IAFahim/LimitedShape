// <copyright file="ControlOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Controls
{
    using System.Collections.Generic;
    using BovineLabs.Anchor.Services;
    using BovineLabs.Sample.UI.Elements;
    using BovineLabs.Sample.UI.Services;
    using BovineLabs.Sample.UI.ViewModels.Option.Options.Base;
    using UnityEngine.InputSystem;

    public class ControlOption : OptionBase<string>
    {
        private readonly InputActionAsset inputActionAsset;
        private readonly List<InputActionBinding> bindings = new();

        public ControlOption(ILocalStorageService storageService, IControlService controlService)
            : base(storageService)
        {
            this.inputActionAsset = controlService.InputActions;
        }

        /// <inheritdoc/>
        protected override string DefaultValue => string.Empty;

        /// <inheritdoc/>
        protected override string Key => "controls";

        public void RegisterBinding(InputActionBinding binding)
        {
            binding.BindingChanged += () => this.Value = this.inputActionAsset.SaveBindingOverridesAsJson();
            this.bindings.Add(binding);
        }

        /// <inheritdoc/>
        public override void Load()
        {
            base.Load();
            this.UpdateBinding();
        }

        /// <inheritdoc/>
        public override void Revert()
        {
            base.Revert();
            this.UpdateBinding();
        }

        /// <inheritdoc/>
        public override void ResetToDefault()
        {
            base.ResetToDefault();
            this.UpdateBinding();
        }

        /// <inheritdoc/>
        protected override string ToString(string value)
        {
            return value;
        }

        /// <inheritdoc/>
        protected override string FromString(string value, string defaultValue)
        {
            return value;
        }

        /// <inheritdoc/>
        protected override void OnValueChanged(string newValue)
        {
        }

        private void UpdateBinding()
        {
            this.inputActionAsset.LoadBindingOverridesFromJson(this.Value);

            foreach (var b in this.bindings)
            {
                b.RefreshBindingText();
            }
        }
    }
}
