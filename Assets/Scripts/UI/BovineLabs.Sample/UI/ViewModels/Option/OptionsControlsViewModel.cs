// <copyright file="OptionsControlsViewModel.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option
{
    using System.Linq;
    using BovineLabs.Sample.UI.Services;
    using BovineLabs.Sample.UI.ViewModels.Option.Options.Controls;

    public class OptionsControlsViewModel : OptionTabViewModel
    {
        public readonly ControlOption ControlOption;
        private ControlSchema currentSchema;

        public OptionsControlsViewModel(IControlService controlService)
        {
            this.CurrentSchema = controlService.Schemas?.First();
            this.CreateOption(out this.ControlOption);
        }

        public ControlSchema CurrentSchema
        {
            get => this.currentSchema;
            set => this.SetProperty(ref this.currentSchema, value);
        }
    }
}
