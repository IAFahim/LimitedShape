// <copyright file="NavVisualController.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI
{
    using JetBrains.Annotations;
    using Unity.AppUI.Navigation;
    using Unity.AppUI.UI;

    [UsedImplicitly]
    public class NavVisualController : INavVisualController
    {
        public void SetupBottomNavBar(BottomNavBar bottomNavBar, NavDestination destination, NavController navController)
        {

        }

        public void SetupAppBar(AppBar appBar, NavDestination destination, NavController navController)
        {
            appBar.expandedHeight = 0;
            appBar.stretch = true;
        }

        public void SetupDrawer(Drawer drawer, NavDestination destination, NavController navController)
        {
        }

        public void SetupNavigationRail(NavigationRail navigationRail, NavDestination destination, NavController navController)
        {
        }
    }
}