// <copyright file="DistortAppBuilder.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI
{
    using System;
    using BovineLabs.Anchor;
    using BovineLabs.Sample.UI.Services;
    using BovineLabs.Sample.UI.ViewModels.Option;
    using Unity.AppUI.MVVM;

    public class BovineLabsAppBuilder : AnchorAppBuilder
    {
        protected override Type NavVisualController => typeof(NavVisualController);

        /// <inheritdoc/>
        protected override void OnConfiguringApp(AppBuilder builder)
        {
            base.OnConfiguringApp(builder);

            builder.services.AddSingleton<IControlService, ControlService>();
        }

        /// <inheritdoc/>
        protected override void OnAppInitialized(AnchorApp anchor)
        {
            base.OnAppInitialized(anchor);

            // Load all our options
            anchor.services.GetRequiredService<OptionsAudioViewModel>().Load();
            anchor.services.GetRequiredService<OptionsControlsViewModel>().Load();
            anchor.services.GetRequiredService<OptionsGameplayViewModel>().Load();
            anchor.services.GetRequiredService<OptionsGraphicsViewModel>().Load();
        }
    }
}