// <copyright file="IOption.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option.Options.Base
{
    public interface IOption
    {
        /// <summary> Load the option. Should be called once at start of app. </summary>
        public void Load();

        /// <summary> Saves the options. </summary>
        public void Save();

        /// <summary> Revert to the value of the last <see cref="Save"/>. </summary>
        public void Revert();

        /// <summary> Reset the option to its default value. </summary>
        public void ResetToDefault();
    }
}
