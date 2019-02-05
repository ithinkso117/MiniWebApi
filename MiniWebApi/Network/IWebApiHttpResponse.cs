namespace MiniWebApi.Network
{
    public interface IWebApiHttpResponse
    {
        /// <summary>
        /// Gets or sets the result status code.
        /// </summary>
        int StatusCode { get; set; }

        /// <summary>
        /// Write an object back to the requester.
        /// </summary>
        void Write(object obj);

        /// <summary>
        /// Write an OK to client without content.
        /// </summary>
        void WriteOk();
    }
}
