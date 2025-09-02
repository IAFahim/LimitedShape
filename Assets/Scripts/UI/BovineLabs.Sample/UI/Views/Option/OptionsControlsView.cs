// <copyright file="OptionsControlsView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Views.Option
{
    using System.ComponentModel;
    using BovineLabs.Sample.UI.Elements;
    using BovineLabs.Sample.UI.ViewModels.Option;
    using Unity.AppUI.UI;
    using UnityEngine.UIElements;

    public class OptionsControlsView : OptionTab<OptionsControlsViewModel>
    {
        public new const string UssClassName = OptionTab.UssClassName + "__controls";
        public const string SchemaClassName = UssClassName + "__schema";
        public const string SchemaHeadingClassName = SchemaClassName + "__header";
        public const string SchemaInputClassName = SchemaClassName + "__input";

        public OptionsControlsView(OptionsControlsViewModel viewModel)
            : base(viewModel)
        {
            this.AddToClassList(UssClassName);
            this.StretchToParentSize();

            this.ViewModel.PropertyChanged += this.ViewModelOnPropertyChanged;

            this.Rebuild();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OptionsControlsViewModel.CurrentSchema))
            {
                this.Rebuild();
            }
        }

        private void Rebuild()
        {
            this.Clear();

            if (this.ViewModel.CurrentSchema == null)
            {
                return;
            }

            foreach (var group in this.ViewModel.CurrentSchema.Groups)
            {
                var groupRoot = new VisualElement();
                groupRoot.AddToClassList(SchemaClassName);
                this.Add(groupRoot);

                var heading = new Heading(group.Name);
                heading.AddToClassList(SchemaHeadingClassName);
                groupRoot.Add(heading);

                foreach (var row in group.Bindings)
                {
                    var field = new InputActionBinding
                    {
                        text = row.Name,
                        inputAction = row.Binding,
                    };
                    field.AddToClassList(SchemaInputClassName);
                    groupRoot.Add(field);

                    this.ViewModel.ControlOption.RegisterBinding(field);
                }
            }
        }
    }
}
