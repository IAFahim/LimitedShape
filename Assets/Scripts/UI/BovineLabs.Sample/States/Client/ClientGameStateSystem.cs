// <copyright file="ClientGameStateSystem.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using Unity.AppUI.Navigation.Generated;

namespace BovineLabs.Sample.States.Client
{
    using BovineLabs.Anchor;
    using BovineLabs.Core.States;
    using BovineLabs.Nerve.Data.States;
    using BovineLabs.Nerve.States.Client;
    using BovineLabs.Sample.UI.Views;
    using Unity.Burst;
    using Unity.Entities;
    using Unity.NetCode;

    [UpdateInGroup(typeof(ClientStateSystemGroup))]
    public partial struct ClientGameStateSystem : ISystem, ISystemStartStop
    {
        /// <inheritdoc/>
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            StateAPI.Register<GameState, StateGame, ClientStates>(ref state, "game");

            state.RequireForUpdate<NetworkStreamInGame>();
        }

        /// <inheritdoc/>
        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            AnchorApp.Navigate(Actions.go_to_game);
        }

        public void OnStopRunning(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }

        private struct StateGame : IComponentData
        {
        }
    }
}