// <copyright file="IntOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Base
{
    using BovineLabs.Anchor.Services;
    using Unity.AppUI.UI;
    using Unity.Properties;
    using UnityEngine.UIElements;
    using SliderInt = Unity.AppUI.UI.SliderInt;

    public abstract class IntOption : OptionBaseView<int>
    {
        public const string IntUss = OptionGroupUss + "__int";

        /// <summary> Initializes a new instance of the <see cref="IntOption"/> class. </summary>
        /// <param name="storageService"> Injected storage service. </param>
        protected IntOption(ILocalStorageService storageService)
            : base(storageService)
        {
        }

        /// <summary> Gets the minimum value inclusive. </summary>
        protected abstract int LowValue { get; } // TODO use this for validation

        /// <summary> Gets the maximum value inclusive. </summary>
        protected abstract int HighValue { get; }

        /// <inheritdoc/>
        protected override VisualElement CreateField()
        {
            var field = new SliderInt
            {
                dataSource = this,
                lowValue = this.LowValue,
                highValue = this.HighValue,
                track = TrackDisplayType.On,
            };

            field.AddToClassList(IntUss);
            field.SetBinding(nameof(SliderInt.value), new DataBinding { dataSourcePath = new PropertyPath(nameof(this.Value)) });

            return field;
        }

        /// <inheritdoc/>
        protected override string ToString(int value)
        {
            return value.ToString();
        }

        /// <inheritdoc/>
        protected override int FromString(string value, int defaultValue)
        {
            return int.TryParse(value, out var intValue) ? intValue : defaultValue;
        }
    }
}
