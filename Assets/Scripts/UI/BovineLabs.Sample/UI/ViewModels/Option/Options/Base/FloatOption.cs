// <copyright file="FloatOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Base
{
    using System.Globalization;
    using BovineLabs.Anchor.Services;
    using Unity.AppUI.UI;
    using Unity.Properties;
    using UnityEngine.UIElements;

    public abstract class FloatOption : OptionBaseView<float>
    {
        public const string FloatUss = OptionGroupUss + "__float";

        protected FloatOption(ILocalStorageService storageService)
            : base(storageService)
        {
        }

        /// <summary> Gets the low value of the slider. </summary>
        protected abstract float LowValue { get; }

        /// <summary> Gets the high value of the slider. </summary>
        protected abstract float HighValue { get; }

        /// <inheritdoc/>
        protected override VisualElement CreateField()
        {
            var field = new SliderFloat
            {
                dataSource = this,
                lowValue = this.LowValue,
                highValue = this.HighValue,
                track = TrackDisplayType.On,
            };

            field.AddToClassList(FloatUss);
            field.SetBinding(nameof(IntField.value), new DataBinding { dataSourcePath = new PropertyPath(nameof(this.Value)) });
            return field;
        }

        /// <inheritdoc/>
        protected override string ToString(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        protected override float FromString(string value, float defaultValue)
        {
            return float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var intValue)
                ? intValue
                : defaultValue;
        }
    }
}
