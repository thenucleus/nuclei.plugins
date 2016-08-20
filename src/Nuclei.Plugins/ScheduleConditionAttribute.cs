//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Extensions.Plugins
{
    /// <summary>
    /// Defines the attribute that indicates that a method or property is a condition that can be placed
    /// in a schedule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ScheduleConditionAttribute : Attribute
    {
        /// <summary>
        /// The name of the action.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleConditionAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the condition.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="name"/> is an empty string.
        /// </exception>
        public ScheduleConditionAttribute(string name)
        {
            {
                Lokad.Enforce.Argument(() => name);
                Lokad.Enforce.Argument(() => name, Lokad.Rules.StringIs.NotEmpty);
            }

            m_Name = name;
        }

        /// <summary>
        /// Gets the name of the condition.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }
    }
}
