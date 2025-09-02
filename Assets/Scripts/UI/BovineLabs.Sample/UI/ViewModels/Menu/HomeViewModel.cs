// <copyright file="HomeViewModel.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Menu
{
    using BovineLabs.Anchor;
    using BovineLabs.Core.Utility;
    using Unity.Properties;
    using UnityEngine;

    public class HomeViewModel : SystemObservableObject<HomeViewModel.Data>
    {
        private bool hasSavedGame;

        [CreateProperty]
        public bool OnlineAvailable => Application.internetReachability != NetworkReachability.NotReachable;

        public struct Data
        {
            public ButtonEvent Private;
            public ButtonEvent Host;
            public ButtonEvent Join;
        }
    }
}
