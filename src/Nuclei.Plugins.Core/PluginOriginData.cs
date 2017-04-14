//--------------------------------------------;---------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines the data storage for a <see cref="PluginOrigin"/>.
    /// </summary>
    [Serializable]
    public abstract class PluginOriginData : IComparable<PluginOriginData>, IEquatable<PluginOriginData>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(PluginOriginData first, PluginOriginData second)
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
        public static bool operator !=(PluginOriginData first, PluginOriginData second)
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
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(PluginOriginData first, PluginOriginData second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null))
            {
                return false;
            }

            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.CompareTo(second) > 0;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(PluginOriginData first, PluginOriginData second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null))
            {
                return true;
            }

            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.CompareTo(second) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(PluginOriginData first, PluginOriginData second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null))
            {
                return true;
            }

            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.CompareTo(second) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(PluginOriginData first, PluginOriginData second)
        {
            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null))
            {
                return false;
            }

            // Check if first and second are null references by using ReferenceEquals because
            // we overload the == operator. If either isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(first, null) && first.CompareTo(second) >= 0;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has
        /// the following meanings:
        ///     Value Meaning
        ///     Less than zero This object is less than the other parameter.
        ///     Zero This object is equal to other.
        ///     Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(PluginOriginData other)
        {
            // We don't strictly need to use the ReferenceEquals method but
            // it seems more consistent to use it.
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (GetType().Equals(other.GetType()))
            {
                return CompareValues(other);
            }
            else
            {
                return string.Compare(GetType().AssemblyQualifiedName, other.GetType().AssemblyQualifiedName, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that
        /// indicates whether the current instance precedes, follows, or occurs in the same position in the
        /// sort order as the other object.
        /// </summary>
        /// <param name="other">The value of the object with which the current object is being compared.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// The current instance is less than <paramref name="other"/>.
        /// Zero
        /// The current instance is equal to <paramref name="other"/>.
        /// Greater than zero
        /// The current instance is greater than <paramref name="other"/>.
        /// </returns>
        protected abstract int CompareValues(PluginOriginData other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public abstract override bool Equals(object obj);

        /// <summary>
        /// Determines whether the specified <see cref="PluginOriginData"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="PluginOriginData"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="PluginOriginData"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public abstract bool Equals(PluginOriginData other);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public abstract override int GetHashCode();

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public abstract override string ToString();
    }
}
