namespace MiniWebApi.Handler
{
    public class WebApiArgument
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Create the WebApiParameter
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public WebApiArgument(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
