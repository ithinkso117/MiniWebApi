using System;

namespace MiniWebApi.Utilities
{
    /// <summary>
    /// WebApiUrlInfo contains the handler name, action name and the version.
    /// </summary>
    class WebApiUrlInfo
    {
        /// <summary>
        /// Gets the Handler name.
        /// </summary>
        public string HandlerName { get; }

        /// <summary>
        /// Gets the Action 's name.
        /// </summary>
        public string ActionName { get; }

        /// <summary>
        /// Gets the version from the url.
        /// </summary>
        public string Version { get; }


        public WebApiUrlInfo(string handlerName, string actionName, string version = "v1")
        {
            HandlerName = handlerName;
            ActionName = actionName;
            Version = version;
        }
    }

    class WebApiUrlInfoParser
    {
        /// <summary>
        /// Parser the request url, get the calling information.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public static WebApiUrlInfo Parser(string applicationName, Uri requestUrl)
        {
            var urlApplicationName = string.Empty;
            var urlVersion = "v1";
            string urlHandlerName;
            var urlActionName = string.Empty;

            var url = $"{requestUrl.AbsolutePath.Trim('/')}";
            var urlParts = url.Split('/');
            if (!string.IsNullOrEmpty(applicationName))
            {
                // AppName/Version/HandlerName/ActionName
                // AppName/HandlerName/ActionName
                if (urlParts.Length < 2 || urlParts.Length > 4) return null;
                if (urlParts.Length == 4)
                {
                    //Contains version.
                    urlApplicationName = urlParts[0].ToLower();
                    urlVersion = urlParts[1].ToLower();
                    urlHandlerName = urlParts[2].ToLower();
                    //Action name can not to lower
                    urlActionName = urlParts[3];
                }
                else if(urlParts.Length == 3)
                {
                    urlApplicationName = urlParts[0].ToLower();
                    urlHandlerName = urlParts[1].ToLower();
                    //Action name can not to lower
                    urlActionName = urlParts[2];
                }
                else
                {
                    urlApplicationName = urlParts[0].ToLower();
                    urlHandlerName = urlParts[1].ToLower();
                }
            }
            else
            {
                // Version/HandlerName/ActionName
                // HandlerName/ActionName
                if (urlParts.Length <1 || urlParts.Length >3) return null;
                if (urlParts.Length == 3)
                {
                    //Contains version.
                    urlVersion = urlParts[0].ToLower();
                    urlHandlerName = urlParts[1].ToLower();
                    //Action name can not to lower
                    urlActionName = urlParts[2];
                }
                else if(urlParts.Length == 2)
                {
                    urlHandlerName = urlParts[0].ToLower();
                    //Action name can not to lower
                    urlActionName = urlParts[1];
                }
                else
                {
                    urlHandlerName = urlParts[0].ToLower();
                }
            }

            //Check if the application is matched.
            return applicationName != urlApplicationName ? null : new WebApiUrlInfo(urlHandlerName, urlActionName, urlVersion);
        }
    }
}
