//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Nuclei.Plugins.Core;

namespace Nuclei.Plugins.Composition.Mef
{
    /// <summary>
    /// Defines a <see cref="ComposablePartCatalog"/> that loads the available parts on demand only.
    /// </summary>
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "It's an MEF catalog in the first place, a collection in the second place.")]
    public sealed class LazyLoadCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static IDictionary<string, object> CreateExportMetadata(SerializableExportDefinition definition)
        {
            return new Dictionary<string, object>
                {
                    { MefConstants.ExportTypeIdentity, definition.ExportTypeIdentityForMef }
                };
        }

        private static ExportDefinition DeserializeExportDefinition(SerializableExportDefinition definition)
        {
            var memberInfo = (LazyMemberInfo)DeserializeExportMemberInfo((dynamic)definition);
            var metadata = CreateExportMetadata(definition);

            return ReflectionModelServices.CreateExportDefinition(
                memberInfo,
                definition.ContractName,
                new Lazy<IDictionary<string, object>>(() => metadata),
                null);
        }

        private static LazyMemberInfo DeserializeExportMemberInfo(MethodBasedExportDefinition definition)
        {
            return new LazyMemberInfo(
                MemberTypes.Method,
                () =>
                {
                    var type = TypeLoader.FromFullyQualifiedName(definition.DeclaringType.AssemblyQualifiedName);
                    var parameterTypes = definition.Method.Parameters.Select(p => TypeLoader.FromFullyQualifiedName(p.Identity.AssemblyQualifiedName)).ToArray();

                    return new[] { type.GetMethod(definition.Method.MethodName, DefaultBindingFlags, null, parameterTypes, new ParameterModifier[0]) };
                });
        }

        private static LazyMemberInfo DeserializeExportMemberInfo(PropertyBasedExportDefinition definition)
        {
            return new LazyMemberInfo(
                MemberTypes.Property,
                () =>
                {
                    var type = TypeLoader.FromFullyQualifiedName(definition.DeclaringType.AssemblyQualifiedName);
                    var property = type.GetProperty(definition.Property.PropertyName, DefaultBindingFlags);

                    // Note: MEF doesn't actually want the property, it wants the get and set methods (which sort of means there should
                    //       both be a get and set method. No way MEF is going to let you get away with having only a get method on an export ...
                    return new[] { property.GetGetMethod(true), property.GetSetMethod(true) };
                });
        }

        private static LazyMemberInfo DeserializeExportMemberInfo(TypeBasedExportDefinition definition)
        {
            return new LazyMemberInfo(
                MemberTypes.TypeInfo,
                () => new[] { TypeLoader.FromFullyQualifiedName(definition.DeclaringType.AssemblyQualifiedName) });
        }

        private static ImportDefinition DeserializeImportDefinition(SerializableImportDefinition definition)
        {
            var constructorDefinition = definition as ConstructorBasedImportDefinition;
            if (constructorDefinition != null)
            {
                var lazyParameter = DeserializeImportingConstructorInfo(constructorDefinition);
                return ReflectionModelServices.CreateImportDefinition(
                    lazyParameter,
                    definition.ContractName,
                    definition.RequiredTypeIdentityForMef,
                    new List<KeyValuePair<string, Type>>(), // We don't handle custom metadata at the moment.
                    definition.Cardinality,
                    definition.RequiredCreationPolicy,
                    MetadataServices.EmptyMetadata,
                    definition.IsExportFactory,
                    null);
            }
            else
            {
                var propertyDefinition = definition as PropertyBasedImportDefinition;
                if (propertyDefinition == null)
                {
                    throw new InvalidImportDefinitionException();
                }

                var memberInfo = new LazyMemberInfo(
                MemberTypes.Property,
                () =>
                    {
                        var type = TypeLoader.FromFullyQualifiedName(definition.DeclaringType.AssemblyQualifiedName);
                        var property = type.GetProperty(propertyDefinition.Property.PropertyName, DefaultBindingFlags);

                        // Note: MEF doesn't actually want the property, it wants the get and set methods (which sort of means there should
                        //       both be a get and set method. No way MEF is going to let you get away with having only a set method on an import ...
                        return new[] { property.GetGetMethod(true), property.GetSetMethod(true) };
                    });

                return ReflectionModelServices.CreateImportDefinition(
                    memberInfo,
                    definition.ContractName,
                    definition.RequiredTypeIdentityForMef,
                    new List<KeyValuePair<string, Type>>(), // We don't handle custom metadata at the moment.
                    definition.Cardinality,
                    definition.IsRecomposable,
                    definition.IsPrerequisite,
                    definition.RequiredCreationPolicy,
                    MetadataServices.EmptyMetadata,
                    definition.IsExportFactory,
                    null);
            }
        }

        private static Lazy<ParameterInfo> DeserializeImportingConstructorInfo(ConstructorBasedImportDefinition definition)
        {
            var lazyParameter = new Lazy<ParameterInfo>(() =>
            {
                var type = TypeLoader.FromFullyQualifiedName(definition.DeclaringType.AssemblyQualifiedName);
                return GetImportingConstructor(type, definition.Constructor.Parameters).GetParameters().Single(x => x.Name == definition.Parameter.Name);
            });

            return lazyParameter;
        }

        private static ConstructorInfo GetImportingConstructor(Type type, IEnumerable<ParameterDefinition> parameters)
        {
            var constructor = type.GetConstructors(DefaultBindingFlags)
                .Where(x => x.IsDefined(typeof(ImportingConstructorAttribute)))
                .Where(x => !x.GetParameters().Select(p => p.Name).Except(parameters.Select(p => p.Name)).Any())
                .FirstOrDefault();
            if (constructor != null)
            {
                return constructor;
            }

            throw new InvalidImportDefinitionException();
        }

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The repository that stores all the definitions of the types, imports and exports.
        /// </summary>
        private readonly ISatisfyPluginRequests _repository;

        /// <summary>
        /// The collection of part definitions.
        /// </summary>
        private volatile List<ComposablePartDefinition> _definitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyLoadCatalog"/> class.
        /// </summary>
        /// <param name="repository">The object that stores the references to the different parts.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        public LazyLoadCatalog(ISatisfyPluginRequests repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            _repository = repository;
        }

        /// <summary>
        /// Occurs when a <see cref="ComposablePartCatalog" /> has changed.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

        /// <summary>
        /// Occurs when a <see cref="ComposablePartCatalog" /> is changing.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        private void OnChanged(IEnumerable<ComposablePartDefinition> added, IEnumerable<ComposablePartDefinition> removed, AtomicComposition composition)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, new ComposablePartCatalogChangeEventArgs(added, removed, composition));
            }
        }

        private void OnChanging(IEnumerable<ComposablePartDefinition> added, IEnumerable<ComposablePartDefinition> removed, AtomicComposition composition)
        {
            var handler = Changing;
            if (handler != null)
            {
                handler(this, new ComposablePartCatalogChangeEventArgs(added, removed, composition));
            }
        }

        /// <summary>
        /// Gets the part definitions that are contained in the catalog.
        /// </summary>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                if (_definitions == null)
                {
                    lock (_lock)
                    {
                        if (_definitions == null)
                        {
                            var definitions = new List<ComposablePartDefinition>();
                            foreach (var serializedPart in _repository.Parts())
                            {
                                var definition = ReflectionModelServices.CreatePartDefinition(
                                    new Lazy<Type>(() => TypeLoader.FromFullyQualifiedName(serializedPart.Identity.AssemblyQualifiedName)),
                                    _repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(IDisposable)), serializedPart.Identity),
                                    new Lazy<IEnumerable<ImportDefinition>>(() => serializedPart.Imports.Select(DeserializeImportDefinition)),
                                    new Lazy<IEnumerable<ExportDefinition>>(() => serializedPart.Exports.Select(DeserializeExportDefinition)),
                                    new Lazy<IDictionary<string, object>>(() => MetadataServices.EmptyMetadata),
                                    null);
                                definitions.Add(definition);
                            }

                            Thread.MemoryBarrier(); // Make sure we don't set the collection prior to filling it up. Pointer assignments are atomic.
                            _definitions = definitions;
                        }
                    }
                }

                return _definitions.AsQueryable();
            }
        }
    }
}
