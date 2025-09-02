// <copyright file="GameAPI.ServiceState.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample
{
    using System.Runtime.CompilerServices;
    using BovineLabs.Core.Collections;
    using BovineLabs.Core.States;
    using BovineLabs.Nerve.Data.States;
    using Unity.Collections;
    using Unity.Entities;

    /// <inheritdoc cref="AppAPI"/>
    public static partial class GameAPI
    {
        /// <inheritdoc cref="AppAPI.StateIsEnabled{GameState, BitArray256, T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ServiceStateIsEnabled(ref SystemState systemState, FixedString32Bytes name)
        {
            return AppAPI.StateIsEnabled<GameState, BitArray256, ServiceStates>(ref systemState, name);
        }

        /// <inheritdoc cref="AppAPI.StateSet{GameState, BitArray256, T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ServiceStateSet(ref SystemState systemState, FixedString32Bytes name)
        {
            AppAPI.StateSet<GameState, BitArray256, ServiceStates>(ref systemState, name);
        }

        /// <inheritdoc cref="AppAPI.StateEnable{GameState, BitArray256, T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ServiceStatesStateEnable(ref SystemState systemState, FixedString32Bytes name)
        {
            AppAPI.StateEnable<GameState, BitArray256, ServiceStates>(ref systemState, name);
        }

        /// <inheritdoc cref="AppAPI.StateDisable{GameState, BitArray256, T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ServiceStatesStateDisable(ref SystemState systemState, FixedString32Bytes name)
        {
            AppAPI.StateDisable<GameState, BitArray256, ServiceStates>(ref systemState, name);
        }
    }
}
