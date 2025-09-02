// <copyright file="OptionBaseView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Base
{
    using BovineLabs.Anchor.Services;
    using Unity.AppUI.UI;
    using UnityEngine.UIElements;

    public abstract class OptionBaseView<T> : OptionBase<T>
    {
        public const string OptionGroupUss = "bl-option-group";

        /// <summary> Initializes a new instance of the <see cref="OptionBaseView{T}"/> class. </summary>
        /// <param name="storageService"> Injected storage service. </param>
        protected OptionBaseView(ILocalStorageService storageService)
            : base(storageService)
        {
        }

        /// <summary> Gets the title for the option. A value of null or empty is valid and will. </summary>
        protected abstract string Title { get; }

        /// <summary> Gets a value indicating whether should the default header be created. </summary>
        protected virtual bool CreateHeader => true;

        /// <summary> Gets the per option group style class. </summary>
        private string GroupUss => $"{OptionGroupUss}__{this.Key}";

        public VisualElement CreateElement()
        {
            var root = new VisualElement();
            root.AddToClassList(OptionGroupUss);
            root.AddToClassList(this.GroupUss);

            if (this.CreateHeader)
            {
                root.Add(new Heading(this.Title));
            }

            root.Add(this.CreateField());
            return root;
        }

        /// <summary> Create the field to modify the option. This needs to be bound to <see cref="Value"/>. </summary>
        /// <returns> The field to edit the <see cref="Value"/>. </returns>
        protected abstract VisualElement CreateField();
    }
}
