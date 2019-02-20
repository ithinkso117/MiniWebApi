using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using MiniWebApi.Handler;
using MiniWebApi.Network;

namespace MiniWebApi.Utilities
{
    /// <summary>
    /// A Parser for getting the arguments from the http request.
    /// </summary>
    internal interface IWebApiArgumentParser
    {
        /// <summary>
        /// Parse the arguments from the http request.
        /// </summary>
        /// <param name="context">The http context from HttpListener</param>
        /// <param name="callingParameters">The calling parameters from the found method.</param>
        /// <returns>All args parsed from the request.</returns>
        WebApiArgument[] Parse(HttpListenerContext context, CallingParameter[] callingParameters);
    }

    /// <summary>
    /// Parser for Get request.
    /// </summary>
    internal class GetArgumentParser : IWebApiArgumentParser
    {
        /// <summary>
        /// Parse the arguments from the http request.
        /// </summary>
        /// <param name="context">The http context from HttpListener</param>
        /// <param name="callingParameters">The calling parameters from the found method.</param>
        /// <returns>All args parsed from the request.</returns>
        public WebApiArgument[] Parse(HttpListenerContext context, CallingParameter[] callingParameters)
        {
            var queryData = context.Request.QueryString;
            var webApiHttpContext = new WebApiHttpContext(new WebApiHttpRequest(context), new WebApiHttpResponse(context));
            var args = new List<WebApiArgument> {new WebApiArgument("context", webApiHttpContext)};
            
            foreach (var parameter in callingParameters)
            {
                var paramType = parameter.ParameterType;
                object paramValue = null;
                if (parameter.FromType == FromType.FromUrl)
                {
                    //Do not get the propertyInfo here, the new method recorded all propertyInfos in cache, no need reflection.
                    paramValue = paramType.New(queryData);
                }
                else
                {
                    if (queryData.AllKeys.Contains(parameter.Name))
                    {
                        paramValue = WebApiConverter.StringToObject(queryData[parameter.Name], paramType);
                        if (paramValue != null && paramType == typeof(string))
                        {
                            paramValue = Uri.UnescapeDataString((string)paramValue);
                        }
                    }
                }
                if (paramValue == null)
                {
                    paramValue = paramType.Default();
                }
                args.Add(new WebApiArgument(parameter.Name, paramValue));
            }

            return args.ToArray();
        }
    }


    /// <summary>
    /// Parser for Post, Put, Delete request.
    /// </summary>
    internal class PostArgumentParser : IWebApiArgumentParser
    {
        /// <inheritdoc />
        /// <summary>
        /// Parse the arguments from the http request.
        /// </summary>
        /// <param name="context">The http context from HttpListener</param>
        /// <param name="callingParameters">The calling parameters from the found method.</param>
        /// <returns>All args parsed from the request.</returns>
        public WebApiArgument[] Parse(HttpListenerContext context, CallingParameter[] callingParameters)
        {
            var contentType = WebApiHttpContentTypeConverter.ToContentType(context.Request.ContentType);
            var queryData = context.Request.QueryString;
            var webApiHttpContext = new WebApiHttpContext(new WebApiHttpRequest(context), new WebApiHttpResponse(context));
            var args = new List<WebApiArgument> { new WebApiArgument("context", webApiHttpContext) };

            foreach (var parameter in callingParameters)
            {
                var paramType = parameter.ParameterType;
                object paramValue = null;
                //FromBody
                if (parameter.FromType == FromType.FromBody)
                {
                    if (contentType == WebApiHttpContentType.Json)
                    {
                        var streamLength = (int) context.Request.ContentLength64;
                        var bodyData = new byte[streamLength];
                        var offset = 0;
                        while (streamLength > 0)
                        {
                            var readLength = context.Request.InputStream.Read(bodyData, offset, streamLength);
                            streamLength -= readLength;
                            offset += readLength;
                        }

                        paramValue = WebApiConverter.JsonToObject(Encoding.UTF8.GetString(bodyData), paramType);
                    }
                    else if (contentType == WebApiHttpContentType.XWwwFormUrlencoded)
                    {
                        var parser = new HttpContentParser(context.Request.InputStream);
                        if (parser.Success)
                        {
                            var formData = new NameValueCollection();
                            foreach (var parserParameter in parser.Parameters)
                            {
                                formData.Add(parserParameter.Key, parserParameter.Value);
                            }

                            paramValue = paramType.New(formData);
                        }
                    }
                }
                else if (parameter.FromType == FromType.FromUrl)
                {
                    paramValue = paramType.New(queryData);
                }
                else
                {
                    if (queryData.AllKeys.Contains(parameter.Name))
                    {
                        paramValue = WebApiConverter.StringToObject(queryData[parameter.Name], paramType);
                        if (paramValue != null && paramType == typeof(string))
                        {
                            paramValue = Uri.UnescapeDataString((string) paramValue);
                        }
                    }
                }
                if (paramValue == null)
                {
                    paramValue = paramType.Default();
                }
                args.Add(new WebApiArgument(parameter.Name, paramValue));
            }
            return args.ToArray();
        }
    }
}
