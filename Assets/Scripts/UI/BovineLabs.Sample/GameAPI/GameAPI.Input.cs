// <copyright file="GameAPI.Input.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample
{
    using System.Runtime.CompilerServices;
    using BovineLabs.Core.Input;
    using Unity.Collections;
    using Unity.Entities;

    public static partial class GameAPI
    {
        /// <inheritdoc cref="InputAPI.InputEnable"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InputEnable(ref SystemState systemState, FixedString32Bytes input)
        {
            InputAPI.InputEnable(ref systemState, input);
        }

        /// <inheritdoc cref="InputAPI.InputDisable"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InputDisable(ref SystemState systemState, FixedString32Bytes input)
        {
            InputAPI.InputDisable(ref systemState, input);
        }
    }
}
