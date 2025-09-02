// <copyright file="ListOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Base
{
    using System.Collections.Generic;
    using BovineLabs.Anchor.Services;
    using Unity.AppUI.UI;
    using Unity.Properties;
    using UnityEngine.UIElements;

    public abstract class ListOption : IntOption
    {
        public const string ListUss = OptionGroupUss + "__list";

        /// <summary> Initializes a new instance of the <see cref="ListOption"/> class. </summary>
        /// <param name="storageService"> Injected storage service. </param>
        protected ListOption(ILocalStorageService storageService)
            : base(storageService)
        {
        }

        /// <inheritdoc/>
        protected override bool CreateHeader => true;

        /// <inheritdoc/>
        protected override int LowValue => 0;

        /// <inheritdoc/>
        protected override int HighValue => this.ItemSource.Count - 1;

        /// <summary> Gets the list of values for the option. </summary>
        protected abstract List<string> ItemSource { get; }

        /// <inheritdoc/>
        protected override VisualElement CreateField()
        {
            var field = new Dropdown
            {
                dataSource = this,
                defaultMessage = string.Empty,
                bindItem = (item, i) => item.label = this.ItemSource[i],
                sourceItems = this.ItemSource,
            };

            field.AddToClassList(ListUss);

            field.SetBinding(nameof(Dropdown.selectedIndex), new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(this.Value)),
            });

            return field;
        }
    }
}
