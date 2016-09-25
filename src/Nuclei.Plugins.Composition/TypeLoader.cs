//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using Nuclei.Plugins.Composition.Properties;
using Nuclei.Plugins.Core;

namespace Nuclei.Plugins.Composition
{
    /// <summary>
    /// Defines static methods for loading the <see cref="Type"/> associated with a <see cref="TypeIdentity"/>.
    /// </summary>
    public static class TypeLoader
    {
        /// <summary>
        /// Loads the <see cref="Type"/> for the given identity.
        /// </summary>
        /// <param name="typeIdentity">The identity of the type that should be loaded.</param>
        /// <returns>The requested type.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="typeIdentity"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnableToLoadPluginTypeException">
        ///     Thrown when something goes wrong either while loading the assembly containing the type or while
        ///     loading the type itself.
        /// </exception>
        public static Type LoadType(TypeIdentity typeIdentity)
        {
            if (typeIdentity == null)
            {
                throw new ArgumentNullException("typeIdentity");
            }

            try
            {
                return Type.GetType(typeIdentity.AssemblyQualifiedName, true, false);
            }
            catch (TargetInvocationException e)
            {
                // Type initializer throw an exception
                throw new UnableToLoadPluginTypeException(Resources.Exceptions_Messages_UnableToLoadPluginType, e);
            }
            catch (TypeLoadException e)
            {
                // Type is not found, typeName contains invalid characters, typeName represents an array type with an invalid size,
                // typeName represents and array of TypedReference
                throw new UnableToLoadPluginTypeException(Resources.Exceptions_Messages_UnableToLoadPluginType, e);
            }
            catch (ArgumentException e)
            {
                // typeName contains invalid syntax, typeName represents a generic type that has a pointer, a ByRef type or Void as one of
                // its type arguments, typeName represents a generic type that has an incorrect number of type arguments, typeName
                // represents a generic type, and one of its arguments does not satisfy the constraints for the corresponding type parameter
                throw new UnableToLoadPluginTypeException(Resources.Exceptions_Messages_UnableToLoadPluginType, e);
            }
            catch (FileNotFoundException e)
            {
                // The assembly or one of its dependencies was not found
                throw new UnableToLoadPluginTypeException(Resources.Exceptions_Messages_UnableToLoadPluginType, e);
            }
            catch (BadImageFormatException e)
            {
                // The assembly or one of its dependencies was not valid
                throw new UnableToLoadPluginTypeException(Resources.Exceptions_Messages_UnableToLoadPluginType, e);
            }
        }
    }
}
