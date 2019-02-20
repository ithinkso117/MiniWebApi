using System.Collections.Specialized;

namespace MiniWebApi.Network
{
    public interface IWebApiHttpRequest
    {
        /// <summary>
        /// Gets the query string of this request.
        /// </summary>
        NameValueCollection QueryString { get; }
    }
}
