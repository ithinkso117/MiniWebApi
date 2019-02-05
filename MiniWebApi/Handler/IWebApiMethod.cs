namespace MiniWebApi.Handler
{
    public interface IWebApiMethod
    {
        /// <summary>
        /// Call the WebApi implementation
        /// </summary>
        /// <param name="thisObject">The this object which will pass to the method for the first argument</param>
        /// <param name="args">The arguments for the WebApi method.</param>
        void Call(object thisObject, object[] args);
    }
}
