// <copyright file="GameBaseView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.Views.Game
{
    public abstract class GameBaseView<T> : BaseScreen<T>
    {
        public const string GameClassName = "bl-game-view";

        protected GameBaseView(T viewModel)
            : base(viewModel)
        {
            this.AddToClassList(GameClassName);
        }
    }
}
