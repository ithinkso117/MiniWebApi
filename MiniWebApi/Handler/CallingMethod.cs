using System.Collections.Generic;
using System.Linq;

namespace MiniWebApi.Handler
{
    /// <summary>
    /// CallingMethod contains the WebApi method, Call the Call method to call the WebApi.
    /// </summary>
    public class CallingMethod
    {
        /// <summary>
        /// The this object for call, the class method need the this object for the first argument.
        /// </summary>
        private readonly object _thisObject;

        /// <summary>
        /// The WebApi to call.
        /// </summary>
        private readonly IWebApiMethod _method;

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the information for the parameters. 
        /// </summary>
        public IReadOnlyList<CallingParameter> Parameters { get; }

        /// <summary>
        /// Gets the WebApiType of the method.
        /// </summary>
        public WebApiType WebApiType { get; }

        /// <summary>
        /// Create the CallingMethod.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="webApiType">The WebApi Type of the method.</param>
        /// <param name="parameters">The parameters information of the method.</param>
        /// <param name="thisObject">The object which contains the WebApi Method.</param>
        /// <param name="method">The method to call the WebApi.</param>
        public CallingMethod(string name, WebApiType webApiType, IEnumerable<CallingParameter> parameters, object thisObject, IWebApiMethod method)
        {
            Name = name;
            WebApiType = webApiType;
            Parameters = parameters.ToList();
            _thisObject = thisObject;
            _method = method;
        }

        /// <summary>
        /// Call the WebApi method.
        /// </summary>
        /// <param name="webApiArguments"></param>
        public void Call(WebApiArgument[] webApiArguments)
        {
            var args = webApiArguments.Select(x => x.Value).ToArray();
            _method.Call(_thisObject, args);
        }
    }
}
