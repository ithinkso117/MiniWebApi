using System;

namespace MiniWebApi.Handler
{
    /// <summary>
    /// Default implementation for the IWebApiMethod
    /// </summary>
    internal class WebApiMethod : IWebApiMethod
    {
        /// <summary>
        /// The action which to call the WebApi.
        /// </summary>
        private readonly Action<object, object[]> _action;

        /// <summary>
        /// Create a WebApiMethod which implemented the IWebApiMethod
        /// </summary>
        /// <param name="action">The delegate to call the WebApi</param>
        public WebApiMethod(Action<object, object[]> action)
        {
            _action = action;
        }

        /// <summary>
        /// Call the WebApi implementation
        /// </summary>
        /// <param name="thisObject">The this object which will pass to the method for the first argument</param>
        /// <param name="args">The arguments for the WebApi method.</param>
        public void Call(object thisObject, object[] args)
        {
            _action(thisObject, args);
        }
    }
}
