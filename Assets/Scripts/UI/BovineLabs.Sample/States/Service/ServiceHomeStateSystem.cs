// <copyright file="ServiceHomeStateSystem.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.States.Service
{
    using BovineLabs.Anchor;
    using BovineLabs.Core.States;
    using BovineLabs.Nerve.Data.States;
    using BovineLabs.Nerve.States.Service;
    using BovineLabs.Sample.UI.ViewModels.Menu;
    using Unity.AppUI.MVVM;
    using Unity.Burst;
    using Unity.Entities;

    [UpdateInGroup(typeof(ServiceStateSystemGroup))]
    public partial struct ServiceHomeStateSystem : ISystem, ISystemStartStop
    {
        private UIHelper<HomeViewModel, HomeViewModel.Data> helper;

        /// <inheritdoc/>
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            StateAPI.Register<GameState, StateHome, ServiceStates>(ref state, "home");
        }

        /// <inheritdoc/>
        public void OnStartRunning(ref SystemState state)
        {
            // Allow the splash screen to close if it's still visible
            App.current.services.GetRequiredService<SplashViewModel>().IsInitialized = true;

            this.helper.Bind();
        }

        /// <inheritdoc/>
        public void OnStopRunning(ref SystemState state)
        {
            this.helper.Unbind();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ref var binding = ref this.helper.Binding;

            if (binding.Private.TryConsume())
            {
                GameAPI.ServiceStateSet(ref state, ServiceStates.Game);
            }
            else if (binding.Host.TryConsume())
            {
                GameAPI.ServiceStateSet(ref state, ServiceStates.HostGame);
            }
            else if (binding.Join.TryConsume())
            {
                GameAPI.ServiceStateSet(ref state, ServiceStates.JoinGame);
            }
        }

        private struct StateHome : IComponentData
        {
        }
    }
}
