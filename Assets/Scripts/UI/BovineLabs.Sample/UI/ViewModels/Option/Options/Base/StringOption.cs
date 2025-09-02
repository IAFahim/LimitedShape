// <copyright file="StringOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Base
{
    using BovineLabs.Anchor.Services;
    using Unity.Properties;
    using UnityEngine.UIElements;
    using TextField = Unity.AppUI.UI.TextField;

    public abstract class StringOption : OptionBaseView<string>
    {
        public const string StringUss = OptionGroupUss + "__string";

        /// <summary> Initializes a new instance of the <see cref="StringOption"/> class. </summary>
        /// <param name="storageService"> Injected storage service. </param>
        protected StringOption(ILocalStorageService storageService)
            : base(storageService)
        {
        }

        /// <inheritdoc/>
        protected override VisualElement CreateField()
        {
            var field = new TextField { dataSource = this };
            field.AddToClassList(StringUss);
            field.SetBinding(nameof(TextField.value), new DataBinding { dataSourcePath = new PropertyPath(nameof(this.Value)) });
            return field;
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
    }
}
