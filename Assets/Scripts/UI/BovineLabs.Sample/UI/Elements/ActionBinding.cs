// <copyright file="ActionBinding.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Elements
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [Serializable]
    public class ActionBinding : IEquatable<ActionBinding>
    {
        public InputActionReference Asset;
        public string BindingId;
        public bool Bindable = true;

        /// <inheritdoc/>
        public bool Equals(ActionBinding other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(this.Asset, other.Asset) && this.BindingId == other.BindingId;
        }

        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;

            action = this.Asset?.action;
            if (action == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(this.BindingId))
            {
                return false;
            }

            // Look up binding index.
            var bindingId = new Guid(this.BindingId);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
            if (bindingIndex == -1)
            {
                Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'");
                return false;
            }

            return true;
        }
    }
}
