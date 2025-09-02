// <copyright file="ControlSettingsEditor.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.Editor.Services
{
    using BovineLabs.Core.Editor.Inspectors;
    using BovineLabs.Core.Editor.ObjectManagement;
    using BovineLabs.Sample.UI.Services;
    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(ControlSettings))]
    public class ControlSettingsEditor : ElementEditor
    {
        protected override VisualElement CreateElement(SerializedProperty property)
        {
            return property.name switch
            {
                nameof(ControlSettings.Schemas) => new AssetCreator<ControlSchema>(this.serializedObject, property).Element,
                _ => CreatePropertyField(property),
            };
        }
    }
}