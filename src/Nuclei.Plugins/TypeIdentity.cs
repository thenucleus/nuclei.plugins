//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Nuclei.Plugins
{
    /// <summary>
    /// Stores information about the identity of a type in a serializable form, i.e. without requiring the
    /// type in question to be loaded.
    /// </summary>
    [Serializable]
    public sealed class TypeIdentity : IEquatable<TypeIdentity>, IEquatable<Type>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TypeIdentity first, TypeIdentity second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(TypeIdentity first, TypeIdentity second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TypeIdentity"/> class based on the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type for which a serialized definition needs to be created.</param>
        /// <param name="identityStorage">
        /// The function that stores <see cref="TypeIdentity"/> objects that are generated while creating the current
        /// <see cref="TypeIdentity"/>.
        /// </param>
        /// <returns>The serialized definition for the given type.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        public static TypeIdentity CreateDefinition(Type type, Func<Type, TypeIdentity> identityStorage)
        {
            {
                Lokad.Enforce.Argument(() => type);
            }

            // It turns out that if the type is a generic parameter all kinds of crazy stuff happens
            // For instance generic parameters are nested, which means that we'll try to get
            // the identity of the declaring type, which then means we have to get the 
            // generic parameters, which ... que infinite loop. Hence if we are a generic 
            // parameter, then we bail early.
            var isGenericParameter = type.IsGenericParameter;
            return new TypeIdentity(
                type.Name,
                type.Namespace,
                AssemblyDefinition.CreateDefinition(type.Assembly),
                isGenericParameter,
                !isGenericParameter && type.IsNested,
                !isGenericParameter && type.IsNested ? identityStorage(type.DeclaringType) : null,
                !isGenericParameter && type.ContainsGenericParameters,
                !isGenericParameter && type.IsGenericType,
                !isGenericParameter && type.IsGenericType
                    ? type.GetGenericArguments().Select(t => identityStorage(t)).ToArray()
                    : new TypeIdentity[0]);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TypeIdentity"/> class based on the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given type.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        public static TypeIdentity CreateDefinition(Type type)
        {
            return CreateDefinition(type, t => CreateDefinition(t));
        }

        /// <summary>
        /// The name of the type.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// The namespace of the type.
        /// </summary>
        private readonly string m_Namespace;

        /// <summary>
        /// The assembly information for the assembly that contains the type.
        /// </summary>
        private readonly AssemblyDefinition m_Assembly;

        /// <summary>
        /// The type definition for the type that declares the current nested type or
        /// generic parameter.
        /// </summary>
        private readonly TypeIdentity m_DeclaringType;

        /// <summary>
        /// The collection that defines all the generic type arguments.
        /// </summary>
        private readonly TypeIdentity[] m_TypeArguments
            = new TypeIdentity[0];

        /// <summary>
        /// A flag indicating if the current type is a generic type.
        /// </summary>
        private readonly bool m_IsGenericType;

        /// <summary>
        /// A flag indicating that the current type has generic parameter, some of which 
        /// have not been replaced by real types.
        /// </summary>
        private readonly bool m_IsOpenGeneric;

        /// <summary>
        /// A flag indicating that the current type is actually a generic parameter (e.g. T)
        /// for an generic type.
        /// </summary>
        private readonly bool m_IsGenericParameter;

        /// <summary>
        /// A flag indicating if the current type is a nested type or not.
        /// </summary>
        private readonly bool m_IsNested;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeIdentity"/> class.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="typeNamespace">The namespace of the type.</param>
        /// <param name="assembly">The assembly which contains the type.</param>
        /// <param name="isGenericParameter">
        /// A flag indicating that the current type is actually a generic parameter (e.g. T) for an generic type.
        /// </param>
        /// <param name="isNested">A flag indicating if the current type is a nested type or not.</param>
        /// <param name="declaringType">
        /// The type definition for the type that declares the current nested type or generic parameter.
        /// </param>
        /// <param name="isOpenGeneric">
        /// A flag indicating that the current type has generic parameter, some of which have not been replaced by real types.
        /// </param>
        /// <param name="isGenericType">A flag indicating if the current type is a generic type.</param>
        /// <param name="typeParameters">The collection that defines all the generic type arguments.</param>
        private TypeIdentity(
            string typeName,
            string typeNamespace,
            AssemblyDefinition assembly,
            bool isGenericParameter,
            bool isNested,
            TypeIdentity declaringType,
            bool isOpenGeneric,
            bool isGenericType,
            TypeIdentity[] typeParameters)
        {
            m_Name = typeName;
            m_Namespace = typeNamespace;
            m_Assembly = assembly;
            m_IsGenericParameter = isGenericParameter;
            m_IsNested = isNested;
            m_DeclaringType = declaringType;
            m_IsOpenGeneric = isOpenGeneric;
            m_IsGenericType = isGenericType;
            m_TypeArguments = typeParameters;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the namespace of the type.
        /// </summary>
        public string Namespace
        {
            get
            {
                return m_Namespace;
            }
        }

        /// <summary>
        /// Gets the assembly qualified name which includes the full name of the assembly that contains the type.
        /// </summary>
        public string AssemblyQualifiedName
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", FullName, Assembly);
            }
        }

        /// <summary>
        /// Gets the fully qualified name of the type.
        /// </summary>
        public string FullName
        {
            get
            {
                if (m_IsOpenGeneric || m_IsGenericParameter)
                {
                    return FormatWithoutTypeParameters();
                }

                return FormatWithTypeParameters(true);
            }
        }

        private string FormatWithoutTypeParameters()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Namespace, FormatNestedName());
        }

        private string FormatWithTypeParameters(bool areTypeParametersFullyQualified)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}{2}",
                Namespace,
                FormatNestedName(),
                FormatTypeParameters(areTypeParametersFullyQualified));
        }

        private string FormatNestedName()
        {
            return m_IsNested
                ? string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}+{1}",
                    m_DeclaringType.FormatNestedName(),
                    Name)
                : Name;
        }

        private string FormatTypeParameters(bool isFullyQualified)
        {
            var builder = new StringBuilder();
            if (m_TypeArguments.Length > 0)
            {
                builder.Append("[");
                for (int i = 0; i < m_TypeArguments.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(",");
                    }

                    var arg = m_TypeArguments[i];
                    if (arg.m_IsGenericParameter)
                    {
                        builder.Append(arg.Name);
                    }
                    else
                    {
                        builder.Append(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "[{0}]",
                                isFullyQualified ? arg.AssemblyQualifiedName : arg.FullName));
                    }
                }

                builder.Append("]");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the assembly information for the type.
        /// </summary>
        public AssemblyDefinition Assembly
        {
            get
            {
                return m_Assembly;
            }
        }

        /// <summary>
        /// Gets the identity for the type that declares the current nested type or generic parameter type.
        /// </summary>
        public TypeIdentity DeclaringType
        {
            get
            {
                return m_DeclaringType;
            }
        }

        /// <summary>
        /// Gets the collection that contains all the generic type arguments for the current type.
        /// </summary>
        public IEnumerable<TypeIdentity> TypeArguments
        {
            get
            {
                return m_TypeArguments;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current type is a generic type.
        /// </summary>
        public bool IsGenericType
        {
            get
            {
                return m_IsGenericType;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current type has one or more generic parameters
        /// which have not been replaced by real types.
        /// </summary>
        public bool IsOpenGeneric
        {
            get
            {
                return m_IsOpenGeneric;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current type is a generic parameter (e.g. T) for a 
        /// generic type.
        /// </summary>
        public bool IsGenericParameter
        {
            get
            {
                return m_IsGenericParameter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current type is a nested type or not.
        /// </summary>
        public bool IsNested
        {
            get
            {
                return m_IsNested;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="TypeIdentity"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="TypeIdentity"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="TypeIdentity"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(TypeIdentity other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase)
                && Assembly.Equals(other.Assembly)
                && m_TypeArguments.SequenceEqual(other.m_TypeArguments);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Type"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Type"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="Type"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "There is no need to validate the parameter because it is implicitly verified.")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(Type other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            //
            // Note that generic parameters (e.g. T in IEnumerable<T>) are weird. They have 
            // a name, namespace and assembly but not a FullName or an AssemblyQualifiedName (both are null)
            // so we'll do the comparison manually.
            var areEqual = !ReferenceEquals(other, null)
                && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Assembly.FullName, other.Assembly.FullName, StringComparison.OrdinalIgnoreCase);

            if (areEqual)
            {
                var typeArguments = other.GetGenericArguments();
                areEqual = areEqual && (typeArguments.Length == m_TypeArguments.Length);
                if (areEqual)
                {
                    for (int i = 0; i < m_TypeArguments.Length; i++)
                    {
                        areEqual = areEqual && m_TypeArguments[i].Equals(typeArguments[i]);
                        if (!areEqual)
                        {
                            break;
                        }
                    }
                }
            }

            return areEqual;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as TypeIdentity;
            return Equals(id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // As obtained from the Jon Skeet answer to:
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ Name.GetHashCode();
                hash = (hash * 23) ^ Namespace.GetHashCode();
                foreach (var arg in m_TypeArguments)
                {
                    hash = (hash * 23) ^ arg.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return FormatWithTypeParameters(false);
        }
    }
}
