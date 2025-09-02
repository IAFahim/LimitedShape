// <copyright file="ControlServiceBehaviour.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Services
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Object = UnityEngine.Object;

    public class ControlServiceBehaviour : MonoBehaviour
    {
        public ControlSettings Settings;
    }

    public interface IControlService
    {
        InputActionAsset InputActions { get; }

        IReadOnlyList<ControlSchema> Schemas { get; }
    }

    public class ControlService : IControlService
    {
        private readonly ControlServiceBehaviour behaviour = Object.FindAnyObjectByType<ControlServiceBehaviour>();

        public InputActionAsset InputActions => this.Settings?.Asset;

        public IReadOnlyList<ControlSchema> Schemas => this.Settings?.Schemas;

        private ControlSettings Settings => this.behaviour == null || this.behaviour.Settings == null ? null : this.behaviour.Settings;
    }
}
