// <copyright file="LanguageOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Gameplay
{
    using System.Collections.Generic;
    using System.Linq;
    using BovineLabs.Anchor.Services;
    using BovineLabs.Sample.UI.ViewModels.Option.Options.Base;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Settings;

    /// <summary> Option for selecting the applications language from a dropdown. </summary>
    public sealed class LanguageOption : ListOption
    {
        /// <summary> Initializes a new instance of the <see cref="LanguageOption"/> class. </summary>
        /// <param name="storageService"> Injected storage service. </param>
        public LanguageOption(ILocalStorageService storageService)
            : base(storageService)
        {
            this.ItemSource = new List<string>(LocalizationSettings.AvailableLocales.Locales.Select(s => s.ToString()));
            this.DefaultValue = LocalizationSettings.SelectedLocale != null
                ? LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale)
                : -1;

            LocalizationSettings.SelectedLocaleChanged += this.OnSelectedLocaleChanged;
        }

        /// <inheritdoc/>
        protected override string Title => "@UI:language";

        /// <inheritdoc/>
        protected override string Key => "locale";

        /// <inheritdoc/>
        protected override int DefaultValue { get; }

        /// <inheritdoc/>
        protected override List<string> ItemSource { get; }

        /// <inheritdoc/>
        protected override void OnValueChanged(int newValue)
        {
            LocalizationSettings.SelectedLocale = newValue != -1 ? LocalizationSettings.AvailableLocales.Locales[newValue] : default;
        }

        private void OnSelectedLocaleChanged(Locale obj)
        {
            this.Value = this.ItemSource.IndexOf(obj.ToString());
        }
    }
}
