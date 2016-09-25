//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Nuclei.Plugins.Core;

namespace Nuclei.Plugins.Composition.Mef
{
    /// <summary>
    /// Defines a <see cref="ComposablePartCatalog"/> that loads the available parts on demand only.
    /// </summary>
    public sealed class LazyLoadCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static ExportDefinition DeserializeExportDefinition(SerializableExportDefinition definition)
        {
            var memberInfo = (LazyMemberInfo)DeserializeMemberInfo((dynamic)definition);

            return ReflectionModelServices.CreateExportDefinition(
                memberInfo,
                definition.ContractName,
                new Lazy<IDictionary<string, object>>(() => MetadataServices.EmptyMetadata),
                null);
        }

        private static ImportDefinition DeserializeImportDefinition(SerializableImportDefinition definition)
        {
            var memberInfo = (LazyMemberInfo)DeserializeMemberInfo((dynamic)definition);

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
                false, // We don't handle ExportFactory<T> at the moment.
                null);
        }

        private static LazyMemberInfo DeserializeMemberInfo(ConstructorBasedImportDefinition definition)
        {
            return new LazyMemberInfo(
                MemberTypes.Constructor,
                () =>
                {
                    var type = TypeLoader.LoadType(definition.DeclaringType);
                    var parameterTypes = definition.Constructor.Parameters.Select(p => TypeLoader.LoadType(p.Identity)).ToArray();

                    return new[] { type.GetConstructor(DefaultBindingFlags, null, CallingConventions.Any, parameterTypes, new ParameterModifier[0]) };
                });
        }

        private static LazyMemberInfo DeserializeMemberInfo(MethodBasedExportDefinition definition)
        {
            return new LazyMemberInfo(
                MemberTypes.Method,
                () =>
                {
                    var type = TypeLoader.LoadType(definition.DeclaringType);
                    var parameterTypes = definition.Method.Parameters.Select(p => TypeLoader.LoadType(p.Identity)).ToArray();

                    return new[] { type.GetMethod(definition.Method.MethodName, DefaultBindingFlags, null, parameterTypes, new ParameterModifier[0]) };
                });
        }

        private static LazyMemberInfo DeserializeMemberInfo(PropertyBasedExportDefinition definition)
        {
            return new LazyMemberInfo(
                MemberTypes.Property,
                () =>
                {
                    var type = TypeLoader.LoadType(definition.DeclaringType);
                    return new[] { type.GetProperty(definition.Property.PropertyName, DefaultBindingFlags) };
                });
        }

        private static LazyMemberInfo DeserializeMemberInfo(PropertyBasedImportDefinition definition)
        {
            return new LazyMemberInfo(
                MemberTypes.Property,
                () =>
                {
                    var type = TypeLoader.LoadType(definition.DeclaringType);
                    return new[] { type.GetProperty(definition.Property.PropertyName, DefaultBindingFlags) };
                });
        }

        private static LazyMemberInfo DeserializeMemberInfo(TypeBasedExportDefinition definition)
        {
            return new LazyMemberInfo(
                MemberTypes.TypeInfo,
                () => new[] { TypeLoader.LoadType(definition.DeclaringType) });
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
                                    new Lazy<Type>(() => TypeLoader.LoadType(serializedPart.Identity)),
                                    _repository.IsSubTypeOf(TypeIdentity.CreateDefinition(typeof(IDisposable)), serializedPart.Identity),
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
