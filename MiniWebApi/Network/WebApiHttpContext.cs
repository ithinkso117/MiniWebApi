namespace MiniWebApi.Network
{
    public class WebApiHttpContext
    {
        /// <summary>
        /// Gets the request from the caller.
        /// </summary>
        public IWebApiHttpRequest Request { get; }

        /// <summary>
        /// Gets the response to write back.
        /// </summary>
        public IWebApiHttpResponse Response { get; }

        public WebApiHttpContext(IWebApiHttpRequest request, IWebApiHttpResponse response)
        {
            Request = request;
            Response = response;
        }
    }
}
