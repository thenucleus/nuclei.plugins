//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Extensions.Plugins
{
    /// <summary>
    /// Defines the interface for objects that want to register one or more component groups.
    /// </summary>
    public interface IExportGroupDefinitions
    {
        /// <summary>
        /// Provides an object that allows the registration of one or more component groups.
        /// </summary>
        /// <param name="builder">The group registration object.</param>
        void RegisterGroups(IRegisterGroupDefinitions builder);
    }
}
