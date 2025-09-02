// <copyright file="GameAPI.Misc.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample
{
    using System.Runtime.CompilerServices;
    using BovineLabs.Core.Pause;
    using Unity.Entities;

    public static partial class GameAPI
    {
        /// <inheritdoc cref="PauseGame.Pause"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pause(ref SystemState systemState, bool pausePresentation = false)
        {
            PauseGame.Pause(ref systemState, pausePresentation);
        }

        /// <inheritdoc cref="PauseGame.Unpause"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unpause(ref SystemState systemState)
        {
            PauseGame.Unpause(ref systemState);
        }
    }
}
