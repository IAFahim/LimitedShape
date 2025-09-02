// <copyright file="OptionTabViewModel.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option
{
    using System.Collections.Generic;
    using BovineLabs.Anchor;
    using BovineLabs.Sample.UI.ViewModels.Option.Options.Base;
    using Unity.AppUI.MVVM;

    [IsService]
    public abstract class OptionTabViewModel : ObservableObject
    {
        public List<IOption> Options { get; } = new();

        public void Load()
        {
            foreach (var o in this.Options)
            {
                o.Load();
            }
        }

        public void ResetToDefault()
        {
            foreach (var o in this.Options)
            {
                o.ResetToDefault();
            }
        }

        public void RevertAll()
        {
            foreach (var o in this.Options)
            {
                o.Revert();
            }
        }

        public void Save()
        {
            foreach (var o in this.Options)
            {
                o.Save();
            }
        }

        protected void CreateOption<TO>(out TO option)
            where TO : IOption
        {
            option = (TO)AnchorApp.current.services.GetService(typeof(TO));
            this.Options.Add(option);
        }
    }
}
