// <copyright file="ControlSettings.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Services
{
    using System;
    using BovineLabs.Core.Settings;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class ControlSettings : ScriptableObject, ISettings
    {
        public InputActionAsset Asset;

        [Tooltip("The list of rebindable controls that will appear in the options control window.")]
        public ControlSchema[] Schemas = Array.Empty<ControlSchema>();
    }
}
