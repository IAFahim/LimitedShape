// <copyright file="GameView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Views.Game
{
    using BovineLabs.Sample.UI.ViewModels.Game;

    public class GameView : GameBaseView<GameViewModel>
    {
        public GameView(/*AbilityToolbarView abilityToolbarView*/)
            : base(new GameViewModel())
        {
            /*this.Add(abilityToolbarView);*/
        }
    }
}
