using System.Collections.Specialized;
using System.Net;

namespace MiniWebApi.Network
{
    class WebApiHttpRequest:IWebApiHttpRequest
    {
        /// <summary>
        /// Gets the query string of this request
        /// </summary>
        public NameValueCollection QueryString { get; }

        public WebApiHttpRequest(HttpListenerContext context)
        {
            QueryString = context.Request.QueryString;
        }
    }
}
