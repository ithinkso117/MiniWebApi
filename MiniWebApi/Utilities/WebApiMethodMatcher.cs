using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using MiniWebApi.Handler;

namespace MiniWebApi.Utilities
{
    /// <summary>
    /// A matcher for check if the method is match for request.
    /// </summary>
    internal interface IWebApiMethodMatcher
    {
        /// <summary>
        /// Check if current request is match the given parameters of the method.
        /// </summary>
        /// <param name="context">The HttpContext</param>
        /// <param name="callingMethod">The method to compare.</param>
        /// <returns>True if matched otherwise false.</returns>
        bool IsMatch(HttpListenerContext context, CallingMethod callingMethod);
    }


    /// <summary>
    /// Matcher for Get
    /// </summary>
    internal class GetMethodMatcher : IWebApiMethodMatcher
    {
        private readonly Dictionary<Type,PropertyInfo[]> _properties = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Check if current request is match the given parameters of the method.
        /// </summary>
        /// <param name="context">The HttpContext</param>
        /// <param name="callingMethod">The method to compare.</param>
        /// <returns>True if matched otherwise false.</returns>
        public bool IsMatch(HttpListenerContext context, CallingMethod callingMethod)
        {
            if (callingMethod.WebApiType != WebApiType.Get) return false;

            var queryData = context.Request.QueryString;

            var argCount = 0;
            foreach (var parameter in callingMethod.Parameters)
            {
                if (parameter.FromType == FromType.FromUrl)
                {
                    var paramType = parameter.ParameterType;
                    if (!_properties.ContainsKey(paramType))
                    {
                        _properties.Add(paramType,
                            paramType.GetProperties().Where(x => x.CanRead && x.CanWrite).ToArray());
                    }

                    var properties = _properties[paramType];
                    foreach (var property in properties)
                    {
                        if (!queryData.AllKeys.Contains(property.Name))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (!queryData.AllKeys.Contains(parameter.Name))
                    {
                        return false;
                    }
                }

                argCount++;
            }

            return argCount == callingMethod.Parameters.Count;
        }
    }

    /// <summary>
    /// Matcher for Post
    /// </summary>
    internal class PostMethodMatcher : IWebApiMethodMatcher
    {
        private readonly Dictionary<Type, PropertyInfo[]> _properties = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Check if match the WebApiType
        /// </summary>
        /// <param name="callingMethod">The method to check</param>
        /// <returns>True if matched otherwise false.</returns>
        protected virtual bool WebApiTypeMatched(CallingMethod callingMethod)
        {
            return callingMethod.WebApiType == WebApiType.Post;
        }

        /// <summary>
        /// Check if current request is match the given parameters of the method.
        /// </summary>
        /// <param name="context">The HttpContext</param>
        /// <param name="callingMethod">The method to compare.</param>
        /// <returns>True if matched otherwise false.</returns>
        public bool IsMatch(HttpListenerContext context, CallingMethod callingMethod)
        {
            if (!WebApiTypeMatched(callingMethod)) return false;
            var queryData = context.Request.QueryString;

            var argCount = 0;
            foreach (var parameter in callingMethod.Parameters)
            {
                var paramType = parameter.ParameterType;
                //FromBody
                if (parameter.FromType == FromType.FromBody)
                {
                    if (context.Request.ContentLength64 == 0)
                    {
                        //We do not actually check the body data, just check if there is body data.
                        return false;
                    }
                }
                else if (parameter.FromType == FromType.FromUrl)
                {
                    if (!_properties.ContainsKey(paramType))
                    {
                        _properties.Add(paramType, paramType.GetProperties().Where(x => x.CanRead && x.CanWrite).ToArray());
                    }

                    var properties = _properties[paramType];
                    foreach (var property in properties)
                    {
                        if (!queryData.AllKeys.Contains(property.Name))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (!queryData.AllKeys.Contains(parameter.Name))
                    {
                        return false;
                    }
                }

                argCount++;
            }

            return argCount == callingMethod.Parameters.Count;
        }
    }

    /// <summary>
    /// Matcher for Put
    /// </summary>
    internal class PutMethodMatcher : PostMethodMatcher
    {
        /// <summary>
        /// Check if match the WebApiType
        /// </summary>
        /// <param name="callingMethod">The method to check</param>
        /// <returns>True if matched otherwise false.</returns>
        protected override bool WebApiTypeMatched(CallingMethod callingMethod)
        {
            return callingMethod.WebApiType == WebApiType.Put;
        }
    }

    /// <summary>
    /// Matcher for Delete
    /// </summary>
    internal class DeleteMethodMatcher : PostMethodMatcher
    {
        /// <summary>
        /// Check if match the WebApiType
        /// </summary>
        /// <param name="callingMethod">The method to check</param>
        /// <returns>True if matched otherwise false.</returns>
        protected override bool WebApiTypeMatched(CallingMethod callingMethod)
        {
            return callingMethod.WebApiType == WebApiType.Delete;
        }
    }
}
