using System;

namespace MiniWebApi.Handler
{
    /// <summary>
    /// Attribute for the handler.
    /// </summary>
    public class WebApiHandlerAttribute : Attribute
    { 
        /// <summary>
        /// Gets the Url string
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Gets the version of the handler.
        /// </summary>
        public int Version { get; }


        /// <summary>
        /// Router to api handler with url string
        /// </summary>
        /// <param name="name">The url string</param>
        public WebApiHandlerAttribute(string name)
        {
            Version = 1;
            Name = name;
        }

        /// <summary>
        /// Router to api handler with url string
        /// </summary>
        /// <param name="name">The url string</param>
        /// <param name="version">The api version</param>
        public WebApiHandlerAttribute(string name, int version = 1)
        {
            Version = version;
            Name = name;
        }
    }


    /// <summary>
    /// The WebApi Attribute, which could be Get,Put,Post,Delete...
    /// </summary>
    public abstract class WebApiAttribute : Attribute
    {
        /// <summary>
        /// Get the WebApiType from this Attribute.
        /// </summary>
        /// <returns>The WebApiType</returns>
        public abstract WebApiType ToWebApiType();
    }

    /// <inheritdoc />
    /// <summary>
    /// Get attribute
    /// </summary>
    public class GetAttribute : WebApiAttribute
    {
        public override WebApiType ToWebApiType()
        {
            return WebApiType.Get;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Put attribute
    /// </summary>
    public class PutAttribute : WebApiAttribute
    {
        public override WebApiType ToWebApiType()
        {
            return WebApiType.Put;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Post attribute
    /// </summary>
    public class PostAttribute : WebApiAttribute
    {
        public override WebApiType ToWebApiType()
        {
            return WebApiType.Post;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Delete attribute
    /// </summary>
    public class DeleteAttribute : WebApiAttribute
    {
        public override WebApiType ToWebApiType()
        {
            return WebApiType.Delete;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// FromUrl attribute
    /// </summary>
    public class FromUrlAttribute : WebApiAttribute
    {
        public override WebApiType ToWebApiType()
        {
            return WebApiType.FromUrl;
        }
    }

    /// <summary>
    /// FromBody attribute
    /// </summary>
    public class FromBodyAttribute : WebApiAttribute
    {
        public override WebApiType ToWebApiType()
        {
            return WebApiType.FromBody;
        }
    }
}
