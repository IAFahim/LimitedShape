// <copyright file="BoolOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Base
{
    using BovineLabs.Anchor.Services;
    using Unity.Properties;
    using UnityEngine.UIElements;
    using Toggle = Unity.AppUI.UI.Toggle;

    public abstract class BoolOption : OptionBaseView<bool>
    {
        public const string BoolUss = OptionGroupUss + "__bool";

        /// <summary> Initializes a new instance of the <see cref="BoolOption"/> class. </summary>
        /// <param name="storageService"> Injected storage service. </param>
        protected BoolOption(ILocalStorageService storageService)
            : base(storageService)
        {
        }

        /// <inheritdoc/>
        protected override VisualElement CreateField()
        {
            var field = new Toggle { dataSource = this };
            field.AddToClassList(BoolUss);
            field.SetBinding(nameof(Toggle.value), new DataBinding { dataSourcePath = new PropertyPath(nameof(this.Value)) });
            return field;
        }

        /// <inheritdoc/>
        protected override string ToString(bool value)
        {
            return value.ToString();
        }

        /// <inheritdoc/>
        protected override bool FromString(string value, bool defaultValue)
        {
            return bool.TryParse(value, out var boolValue) ? boolValue : defaultValue;
        }
    }
}
