using System;

namespace MiniWebApi.Handler
{
    public enum FromType
    {
        /// <summary>
        /// SimpleType
        /// </summary>
        None,
        /// <summary>
        /// The object is from Url, can not to large.
        /// </summary>
        FromUrl,
        /// <summary>
        /// The object is from body.
        /// </summary>
        FromBody
    }

    /// <summary>
    /// The parameter information for the calling method.
    /// </summary>
    public class CallingParameter
    {
        /// <summary>
        /// Gets the parameter's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the parameter's type.
        /// </summary>
        public Type ParameterType { get; }

        /// <summary>
        /// Gets the From type of this parameter.
        /// </summary>
        public FromType FromType { get; }

        /// <summary>
        /// Create the CallingParameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="parameterType">The type of the parameter.</param>
        /// <param name="fromType">The From type of this parameter.</param>
        public CallingParameter(string name, Type parameterType, FromType fromType)
        {
            Name = name;
            ParameterType = parameterType;
            FromType = fromType;
        }
    }
}
