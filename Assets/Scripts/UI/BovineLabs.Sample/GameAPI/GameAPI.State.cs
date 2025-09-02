// <copyright file="GameAPI.State.cs" company="BovineLabs">
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
        /// <inheritdoc cref="AppAPI.StateCurrent{GameState, BitArray256}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArray256 StateCurrent(ref SystemState systemState)
        {
            return AppAPI.StateCurrent<GameState, BitArray256>(ref systemState);
        }

        /// <inheritdoc cref="AppAPI.StateIsEnabled{GameState, BitArray256, T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StateIsEnabled(ref SystemState systemState, FixedString32Bytes name)
        {
            return AppAPI.StateIsEnabled<GameState, BitArray256, ClientStates>(ref systemState, name);
        }

        /// <inheritdoc cref="AppAPI.StateSet{GameState, BitArray256, T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StateSet(ref SystemState systemState, FixedString32Bytes name)
        {
            AppAPI.StateSet<GameState, BitArray256, ClientStates>(ref systemState, name);
        }

        /// <inheritdoc cref="AppAPI.StateSet{GameState, BitArray256}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StateSet(ref SystemState systemState, byte state)
        {
            AppAPI.StateSet<GameState, BitArray256>(ref systemState, state);
        }

        /// <inheritdoc cref="AppAPI.StateEnable{GameState, BitArray256, T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StateEnable(ref SystemState systemState, FixedString32Bytes name)
        {
            AppAPI.StateEnable<GameState, BitArray256, ClientStates>(ref systemState, name);
        }

        /// <inheritdoc cref="AppAPI.StateEnable{GameState, BitArray256}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StateEnable(ref SystemState systemState, byte state)
        {
            AppAPI.StateEnable<GameState, BitArray256>(ref systemState, state);
        }

        /// <inheritdoc cref="AppAPI.StateDisable{GameState, BitArray256, T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StateDisable(ref SystemState systemState, FixedString32Bytes name)
        {
            AppAPI.StateDisable<GameState, BitArray256, ClientStates>(ref systemState, name);
        }

        /// <inheritdoc cref="AppAPI.StateDisable{GameState, BitArray256}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StateDisable(ref SystemState systemState, byte state)
        {
            AppAPI.StateDisable<GameState, BitArray256>(ref systemState, state);
        }
    }
}
