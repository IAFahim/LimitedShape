// <copyright file="InputActionBindingDrawer.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.Editor.Elements
{
    using System.Collections.Generic;
    using System.Linq;
    using BovineLabs.Core.Editor.Inspectors;
    using BovineLabs.Sample.UI.Elements;
    using UnityEditor;
    using UnityEngine.InputSystem;
    using UnityEngine.UIElements;

    [CustomPropertyDrawer(typeof(ActionBinding))]
    public class InputActionBindingDrawer : ElementProperty
    {
        protected override VisualElement CreateElement(SerializedProperty property)
        {
            var cache = this.Cache<Cache>();

            switch (property.name)
            {
                case nameof(ActionBinding.Asset):
                    cache.AssetProperty = property;
                    var field = CreatePropertyField(property);
                    field.RegisterValueChangeCallback(_ => UpdateBindingOptions(cache));
                    return field;

                case nameof(ActionBinding.BindingId):
                    cache.BindingProperty = property;
                    cache.Dropdown = new DropdownField(property.displayName, cache.BindingOptions, 0);
                    cache.Dropdown.RegisterValueChangedCallback(_ =>
                    {
                        cache.BindingProperty.stringValue = cache.Dropdown.index >= 0 && cache.Dropdown.index < cache.BindingOptionValues.Count
                            ? cache.BindingOptionValues[cache.Dropdown.index]
                            : string.Empty;

                        cache.BindingProperty.serializedObject.ApplyModifiedProperties();
                    });
                    return cache.Dropdown;
            }

            return base.CreateElement(property);
        }

        protected override void PostElementCreation(VisualElement root, bool createdElements)
        {
            UpdateBindingOptions(this.Cache<Cache>());
        }

        private static void UpdateBindingOptions(Cache cache)
        {
            cache.Dropdown.index = RefreshBindingOptions(cache);
            cache.BindingProperty.stringValue = cache.Dropdown.index >= 0 && cache.Dropdown.index < cache.BindingOptionValues.Count
                ? cache.BindingOptionValues[cache.Dropdown.index]
                : string.Empty;

            cache.BindingProperty.serializedObject.ApplyModifiedProperties();
        }

        // Taken from InputSystem samples
        private static int RefreshBindingOptions(Cache cache)
        {
            var actionReference = (InputActionReference)cache.AssetProperty.objectReferenceValue;
            var action = actionReference?.action;

            cache.BindingOptions.Clear();
            cache.BindingOptionValues.Clear();

            if (action == null)
            {
                return -1;
            }

            var bindings = action.bindings;
            var bindingCount = bindings.Count;

            var selectedBindingOption = -1;

            var currentBindingId = cache.BindingProperty.stringValue;
            for (var i = 0; i < bindingCount; ++i)
            {
                var binding = bindings[i];
                if (binding.isComposite)
                {
                    continue;
                }

                var bindingId = binding.id.ToString();
                var haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

                // If we don't have a binding groups (control schemes), show the device that if there are, for example,
                // there are two bindings with the display string "A", the user can see that one is for the keyboard
                // and the other for the gamepad.
                var displayOptions = InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;

                if (!haveBindingGroups)
                {
                    displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;
                }

                // Create display string.
                var displayString = action.GetBindingDisplayString(i, displayOptions);

                // If binding is part of a composite, include the part name.
                if (binding.isPartOfComposite)
                {
                    displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";
                }

                // Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
                // by instead using a backlash.
                displayString = displayString.Replace('/', '\\');

                // If the binding is part of control schemes, mention them.
                if (haveBindingGroups)
                {
                    var asset = action.actionMap?.asset;
                    if (asset != null)
                    {
                        var controlSchemes = string.Join(", ", binding.groups.Split(InputBinding.Separator).Where(s => !string.IsNullOrWhiteSpace(s)).
                            Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

                        displayString = $"{displayString} ({controlSchemes})";
                    }
                }

                cache.BindingOptions.Add(displayString);
                cache.BindingOptionValues.Add(bindingId);

                if (currentBindingId == bindingId)
                {
                    selectedBindingOption = cache.BindingOptions.Count - 1;
                }
            }

            return selectedBindingOption;
        }

        private class Cache
        {
            public readonly List<string> BindingOptions = new();
            public readonly List<string> BindingOptionValues = new();
            public SerializedProperty AssetProperty;
            public SerializedProperty BindingProperty;
            public DropdownField Dropdown;
        }
    }
}
