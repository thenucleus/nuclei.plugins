using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nuclei.Plugins.Core
{
    public abstract class PluginOrigin : Id<PluginOrigin, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginOrigin"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "Unfortunately we cannot validate this before using it because it's being passed to the base constructor.")]
        public PluginOrigin(PackageName value)
            : base(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Package: {0}.{1}",
                    value.Id,
                    value.Version.ToString()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginOrigin"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "Unfortunately we cannot validate this before using it because it's being passed to the base constructor.")]
        public PluginOrigin(FileInfo value)
            : base(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "File: {0}",
                    value.FullName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginOrigin"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private PluginOrigin(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override PluginOrigin Clone(string value)
        {
            return new PluginOrigin(value);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InternalValue;
        }
    }
}