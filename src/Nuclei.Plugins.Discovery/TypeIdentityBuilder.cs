//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Nuclei.Plugins.Core;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines a method for the creation of a function that creates <see cref="TypeIdentity"/> objects.
    /// </summary>
    internal static class TypeIdentityBuilder
    {
        /// <summary>
        /// Returns a function that creates <see cref="TypeIdentity"/> objects and stores the type reference in a <see cref="IPluginRepository"/>.
        /// </summary>
        /// <param name="typeStorage">The object that stores the type definitions.</param>
        /// <param name="currentlyBuilding">
        /// The dictionary that keeps track of the types that are currently being constructed. This is necessary because of self-referencing generics,
        /// e.g. System.Boolean : IComparable{System.Boolean} etc.
        /// </param>
        /// <returns>The function that creates type identity objects.</returns>
        public static Func<Type, TypeIdentity> IdentityFactory(IPluginRepository typeStorage, IDictionary<Type, TypeIdentity> currentlyBuilding)
        {
            // Fake out the compiler because we need the function inside the function itself
            Func<Type, TypeIdentity> createTypeIdentity = null;
            createTypeIdentity =
                t =>
                {
                    // First make sure we're not already creating a definition for this type. If so then we just
                    // return the identity because at some point we'll get the definition being added.
                    // This is necessary because if we don't check this there is a good possibility that
                    // we end-up in an infinite loop. e.g. trying to handle
                    // System.Boolean means we have to process System.IComparable<System.Boolean> which means ....
                    if (currentlyBuilding.ContainsKey(t))
                    {
                        return currentlyBuilding[t];
                    }

                    // Create the type full name ourselves because generic type parameters don't have one (see
                    // http://blogs.msdn.com/b/haibo_luo/archive/2006/02/17/534480.aspx).
                    var name =
                        t.AssemblyQualifiedName
                        ?? string.Format(CultureInfo.InvariantCulture, "{0}.{1}, {2}", t.Namespace, t.Name, t.Assembly.FullName);
                    if (!typeStorage.ContainsDefinitionForType(name))
                    {
                        try
                        {
                            // Create a local version of the TypeIdentity and store that so that we can use that if we
                            // come across this type before we're completely finished storing the definition of it
                            var typeIdentity = TypeIdentity.CreateDefinition(t);
                            currentlyBuilding.Add(t, typeIdentity);

                            var typeDefinition = TypeDefinition.CreateDefinition(t, createTypeIdentity);
                            typeStorage.AddType(typeDefinition);
                        }
                        finally
                        {
                            // Once we add the real definition then we can just remove the local copy
                            // from the stack.
                            currentlyBuilding.Remove(t);
                        }
                    }

                    return typeStorage.IdentityByName(name);
                };

            return createTypeIdentity;
        }
    }
}
