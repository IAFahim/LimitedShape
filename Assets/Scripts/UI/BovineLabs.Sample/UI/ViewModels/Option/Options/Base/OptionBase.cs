// <copyright file="OptionBase.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Base
{
    using BovineLabs.Anchor;
    using BovineLabs.Anchor.Services;
    using Unity.AppUI.MVVM;
    using Unity.Properties;

    [IsService]
    public abstract class OptionBase<T> : ObservableObject, IOption
    {
        private const string KeyBase = "bl.options.";

        private readonly ILocalStorageService storageService;

        private T savedValue;
        private T liveValue;
        private bool changed;

        /// <summary> Initializes a new instance of the <see cref="OptionBase{T}"/> class. </summary>
        /// <param name="storageService"> Injected storage service. </param>
        protected OptionBase(ILocalStorageService storageService)
        {
            this.storageService = storageService;
        }

        /// <summary> Gets or sets the value of the option. </summary>
        [CreateProperty]
        public T Value
        {
            get => this.liveValue;
            set
            {
                if (this.SetProperty(ref this.liveValue, value))
                {
                    this.OnValueChanged(this.liveValue);
                    this.changed = true;
                }
            }
        }

        /// <summary> Gets the default value of the option if it's the first time loading or options are reset to default. </summary>
        protected abstract T DefaultValue { get; }

        /// <summary> Gets the unique key to save the option. This will be prefixed with <see cref="KeyBase"/>. </summary>
        protected abstract string Key { get; }

        protected string FullKey => KeyBase + this.Key;

        /// <inheritdoc/>
        public virtual void Load()
        {
            // Load in the value
            if (this.storageService.HasKey(this.FullKey))
            {
                var stringValue = this.storageService.GetValue(this.FullKey);
                this.liveValue = this.savedValue = this.FromString(stringValue, this.DefaultValue);
            }
            else
            {
                this.liveValue = this.savedValue = this.DefaultValue;
            }
        }

        /// <inheritdoc/>
        public void Save()
        {
            if (!this.changed)
            {
                return;
            }

            this.changed = false;

            this.savedValue = this.liveValue;
            var valueAsString = this.ToString(this.savedValue);
            this.storageService.SetValue(this.FullKey, valueAsString);
        }

        /// <inheritdoc/>
        public virtual void Revert()
        {
            if (!this.changed)
            {
                return;
            }

            this.Value = this.savedValue;
            this.changed = false;
        }

        /// <inheritdoc/>
        public virtual void ResetToDefault()
        {
            this.Value = this.DefaultValue;
        }

        /// <summary> Serialize the value to a string. </summary>
        /// <param name="value"> The value to serialize. </param>
        /// <returns> The value serialized as a string. </returns>
        protected abstract string ToString(T value);

        /// <summary> Deserialize the value from a string. </summary>
        /// <param name="value"> The value serialized as a string. </param>
        /// <param name="defaultValue"> A default value to use if deserialization fails. </param>
        /// <returns> The value deserialized. </returns>
        protected abstract T FromString(string value, T defaultValue);

        /// <summary> Called when <see cref="Value"/> changes. </summary>
        /// <param name="newValue"> The new value. </param>
        protected abstract void OnValueChanged(T newValue);
    }
}
