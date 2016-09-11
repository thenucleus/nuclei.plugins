//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Provides helper methods used to match part imports with part exports.
    /// </summary>
    internal sealed class PartImportEngine : IConnectParts
    {
        private static TypeIdentity ExportedType(SerializableExportDefinition exportDefinition)
        {
            var typeExport = exportDefinition as TypeBasedExportDefinition;
            if (typeExport != null)
            {
                return typeExport.DeclaringType;
            }

            var propertyExport = exportDefinition as PropertyBasedExportDefinition;
            if (propertyExport != null)
            {
                return propertyExport.Property.PropertyType;
            }

            var methodExport = exportDefinition as MethodBasedExportDefinition;
            if (methodExport != null)
            {
                return methodExport.Method.ReturnType;
            }

            return null;
        }

        /// <summary>
        /// The object that stores information about all the available parts and part groups.
        /// </summary>
        private readonly ISatisfyPluginRequests _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartImportEngine"/> class.
        /// </summary>
        /// <param name="repository">The object that stores information about all the available parts and part groups.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        public PartImportEngine(ISatisfyPluginRequests repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            _repository = repository;
        }

        /// <summary>
        /// Returns a value indicating if the given import would accept the given export.
        /// </summary>
        /// <param name="importDefinition">The import definition.</param>
        /// <param name="exportDefinition">The export definition.</param>
        /// <returns>
        ///     <see langword="true" /> if the given import would accept the given export; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Accepts(SerializableImportDefinition importDefinition, SerializableExportDefinition exportDefinition)
        {
            if (!string.Equals(importDefinition.ContractName, exportDefinition.ContractName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var importRequiredType = importDefinition.RequiredTypeIdentity;
            var importRequiredTypeDef = _repository.TypeByIdentity(importRequiredType);

            var exportType = ExportedType(exportDefinition);
            if (AvailableTypeMatchesRequiredType(importRequiredType, exportType))
            {
                return true;
            }

            Func<TypeIdentity, TypeDefinition> toDefinition = t => _repository.TypeByIdentity(t);
            if (importRequiredTypeDef.IsCollection(toDefinition)
                && ExportMatchesCollectionImport(importRequiredType, exportType, toDefinition))
            {
                return true;
            }

            if (importRequiredTypeDef.IsLazy(toDefinition) && ExportMatchesLazyImport(importRequiredType, exportType))
            {
                return true;
            }

            if (importRequiredTypeDef.IsFunc(toDefinition) && ExportMatchesFuncImport(importRequiredType, exportType, exportDefinition))
            {
                return true;
            }

            if (importRequiredTypeDef.IsAction(toDefinition) && ExportMatchesActionImport(importRequiredType, exportDefinition))
            {
                return true;
            }

            return false;
        }

        private bool AvailableTypeMatchesRequiredType(TypeIdentity requiredType, TypeIdentity availableType)
        {
            return (availableType != null) && (requiredType.Equals(availableType) || _repository.IsSubTypeOf(requiredType, availableType));
        }

        private bool ExportMatchesActionImport(TypeIdentity importType, SerializableExportDefinition exportDefinition)
        {
            Debug.Assert(importType.TypeArguments.Count() > 0, "Action<T> should have at least 1 generic type argument.");
            var typeArguments = importType.TypeArguments.ToList();

            // Export is a method that matches the signature of the Action<T>
            var methodExport = exportDefinition as MethodBasedExportDefinition;
            if (methodExport != null)
            {
                if (methodExport.Method.ReturnType != null)
                {
                    return false;
                }

                var parameters = methodExport.Method.Parameters.ToList();
                if (parameters.Count != typeArguments.Count)
                {
                    return false;
                }

                for (int i = 0; i < typeArguments.Count; i++)
                {
                    if (!AvailableTypeMatchesRequiredType(typeArguments[i], parameters[i].Identity))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private bool ExportMatchesCollectionImport(TypeIdentity importType, TypeIdentity exportType, Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            if (importType.TypeArguments.Count() != 1)
            {
                return false;
            }

            var genericType = importType.TypeArguments.First();
            if (AvailableTypeMatchesRequiredType(genericType, exportType))
            {
                return true;
            }

            // Handle IEnumerable<Lazy<T, TMeta>>
            if (toDefinition(genericType).IsLazy(toDefinition))
            {
                return ExportMatchesLazyImport(genericType, exportType);
            }

            return false;
        }

        private bool ExportMatchesFuncImport(TypeIdentity importType, TypeIdentity exportType, SerializableExportDefinition exportDefinition)
        {
            Debug.Assert(importType.TypeArguments.Count() > 0, "Func<T> should have at least 1 generic type argument.");
            var typeArguments = importType.TypeArguments.ToList();
            if (typeArguments.Count == 1)
            {
                // The exported type matches the T of Func<T>or the export is a property that returns T (in Func<T>)
                var genericType = typeArguments[0];
                if (AvailableTypeMatchesRequiredType(genericType, exportType))
                {
                    return true;
                }
            }

            // Export is a method that matches the signature of the Func<T>
            var methodExport = exportDefinition as MethodBasedExportDefinition;
            if (methodExport != null)
            {
                var returnType = methodExport.Method.ReturnType;
                if (!AvailableTypeMatchesRequiredType(typeArguments.Last(), returnType))
                {
                    return false;
                }

                var parameters = methodExport.Method.Parameters.ToList();
                if (parameters.Count != (typeArguments.Count - 1))
                {
                    return false;
                }

                for (int i = 0; i < typeArguments.Count - 1; i++)
                {
                    if (!AvailableTypeMatchesRequiredType(typeArguments[i], parameters[i].Identity))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private bool ExportMatchesLazyImport(TypeIdentity importType, TypeIdentity exportType)
        {
            Debug.Assert(importType.TypeArguments.Count() >= 1, "Lazy<T> / Lazy<T, TMeta> should have at least 1 generic type argument");
            var genericType = importType.TypeArguments.First();
            return AvailableTypeMatchesRequiredType(genericType, exportType);
        }
    }
}
