using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using MiniWebApi.Handler;
using MiniWebApi.Log;
using MiniWebApi.Utilities;

namespace MiniWebApi.Network
{
    /// <summary>
    /// WebApiRouter will find all implemented handler which inherit from the BaseHandler, and register into itself.
    /// </summary>
    internal class WebApiRouter
    {
        private readonly Dictionary<string,IWebApiArgumentParser> _parsers = new Dictionary<string, IWebApiArgumentParser>();
        private readonly Dictionary<string,IWebApiMethodMatcher> _matchers = new Dictionary<string, IWebApiMethodMatcher>();
        private readonly Dictionary<string, BaseHandler> _handlers = new Dictionary<string, BaseHandler>();

        private readonly string _applicationName;

        public WebApiRouter(string applicationName)
        {
            _applicationName = applicationName;
            _applicationName = _applicationName == null ? string.Empty : _applicationName.ToLower();
            RegisterWebApiMethodMatchers();
            RegisterWebApiArgumentParsers();
            RegisterHandlers();
            Logger.Write($"WebApi Router created with application name:{applicationName}");
        }

        private void RegisterWebApiMethodMatchers()
        {
            var getMatcher = new GetMethodMatcher();
            var postMatcher = new PostMethodMatcher();
            var putMatcher = new PutMethodMatcher();
            var deleteMatcher = new DeleteMethodMatcher();
            _matchers.Add("get", getMatcher);
            _matchers.Add("post", postMatcher);
            _matchers.Add("put", putMatcher);
            _matchers.Add("delete", deleteMatcher);
        }

        /// <summary>
        /// Register all argument parser.
        /// </summary>
        private void RegisterWebApiArgumentParsers()
        {
            var getParser = new GetArgumentParser();
            var postParser = new PostArgumentParser();
            _parsers.Add("get", getParser);
            _parsers.Add("post", postParser);
            _parsers.Add("put", postParser);
            _parsers.Add("delete", postParser);
        }

        /// <summary>
        /// Register all handlers which implemented by users
        /// </summary>
        private void RegisterHandlers()
        {
            var handlerTypes = Assembly.GetEntryAssembly().GetTypes().Where(x =>
                typeof(BaseHandler).IsAssignableFrom(x) && x != typeof(BaseHandler)).ToArray();
            foreach (var handlerType in handlerTypes)
            {
                var handlerAttrs = handlerType.GetCustomAttributes(typeof(WebApiHandlerAttribute), false);
                if (handlerAttrs.Length > 0)
                {
                    var handlerAttr = (WebApiHandlerAttribute)handlerAttrs[0];
                    if (string.IsNullOrEmpty(handlerAttr.Name)) continue;
                    var key = $"v{handlerAttr.Version}/{handlerAttr.Name.ToLower()}";
                    _handlers.Add(key, (BaseHandler)handlerType.New());
                    Logger.Write($"Register handler:{key}");
                }
            }
        }


        /// <summary>
        /// Dispatch the http context to the router.
        /// </summary>
        /// <param name="context">The context to be dispatched.</param>
        public void DispatchCall(HttpListenerContext context)
        {
            var handlerExist = false;
            var httpMethod = context.Request.HttpMethod.ToLower();
            Logger.Write($"Handle request [{httpMethod}]: {context.Request.Url}");
            try
            {
                var urlInfo = WebApiUrlInfoParser.Parser(_applicationName, context.Request.Url);
                if (urlInfo != null)
                {
                    var key = $"{urlInfo.Version}/{urlInfo.HandlerName}";
                    if (_handlers.ContainsKey(key))
                    {
                        var matcher = _matchers[httpMethod];
                        CallingMethod callingMethod = null;
                        if (!string.IsNullOrEmpty(urlInfo.ActionName))
                        {
                            callingMethod = _handlers[key].GetCallingMethod(urlInfo.ActionName);
                            if (callingMethod != null)
                            {
                                if (!matcher.IsMatch(context, callingMethod))
                                {
                                    callingMethod = null;
                                }
                            }
                        }
                        else
                        {
                            var callingMethods = _handlers[key].GetCallingMethods();
                            foreach (var method in callingMethods)
                            {
                                if (matcher.IsMatch(context, method))
                                {
                                    callingMethod = method;
                                    break;
                                }
                            }
                        }
                        if (callingMethod != null)
                        {
                            if (_parsers.ContainsKey(httpMethod))
                            {
                                var parser = _parsers[httpMethod];
                                var args = parser.Parse(context, callingMethod.Parameters.ToArray());
                                //The parser will add context into the args, so the final count is parameter count + 1.
                                if (args != null && args.Length == callingMethod.Parameters.Count + 1)
                                {
                                    handlerExist = true;
                                    try
                                    {
                                        callingMethod.Call(args);
                                    }
                                    catch (Exception ex)
                                    {
                                        var argStr = new StringBuilder();
                                        foreach (var arg in args)
                                        {
                                            argStr.AppendLine($"{arg.Name} = {arg.Value}");
                                        }

                                        Logger.Write(
                                            $"Call method {callingMethod.Name} with args:{Environment.NewLine + argStr + Environment.NewLine} error :{ex}");
                                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                                        context.Response.Close();

                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write($"Handle request error: {ex}");
            }

            if (!handlerExist)
            {
                Logger.Write($"Handler for request: {context.Request.Url} not found.");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
            }
        }
    }
}
