// <copyright file="ControlSchema.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Services
{
    using System;
    using BovineLabs.Core.ObjectManagement;
    using BovineLabs.Sample.UI.Elements;
    using UnityEngine;

    [AutoRef(nameof(ControlSettings), nameof(ControlSettings.Schemas), nameof(ControlSchema), "Schemas/Control", false)]
    public class ControlSchema : ScriptableObject
    {
        public string Id;
        public Group[] Groups = Array.Empty<Group>();

        [Serializable]
        public class Group
        {
            public string Name;
            public Row[] Bindings = Array.Empty<Row>();
        }

        [Serializable]
        public class Row
        {
            public string Name = string.Empty;
            public ActionBinding Binding = new();
        }
    }
}