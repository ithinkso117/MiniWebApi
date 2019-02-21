using System;
using System.Net;
using System.Text;
using MiniWebApi.Log;
using MiniWebApi.Utilities;

namespace MiniWebApi.Network
{
    class WebApiHttpResponse:IWebApiHttpResponse
    {
        private readonly HttpListenerContext _context;

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public int StatusCode { get; set; }

        public WebApiHttpResponse(HttpListenerContext context)
        {
            _context = context;
            StatusCode = _context.Response.StatusCode;
        }

        /// <summary>
        /// Write an object back to the caller.
        /// </summary>
        /// <param name="obj"></param>
        public void Write(object obj)
        {
            try
            {
                if (obj == null)
                {
                    _context.Response.StatusCode = StatusCode;
                    _context.Response.OutputStream.Close();
                }
                else
                {
                    _context.Response.StatusCode = StatusCode;
                    _context.Response.ContentEncoding = Encoding.UTF8;
                    _context.Response.ContentType = "application/json";
                    var result = Encoding.UTF8.GetBytes(WebApiConverter.ObjectToJson(obj));
                    _context.Response.ContentLength64 = result.Length;
                    _context.Response.OutputStream.Write(result, 0, result.Length);
                    _context.Response.OutputStream.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Write($"Write object back to client error:{ex}");
            }
        }

        /// <summary>
        /// Write binary back to the requester.
        /// </summary>
        /// <param name="data">The data to write</param>
        public void Write(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                _context.Response.StatusCode = StatusCode;
                _context.Response.OutputStream.Close();
            }
            else
            {
                _context.Response.StatusCode = StatusCode;
                _context.Response.OutputStream.Write(data, 0, data.Length);
                _context.Response.OutputStream.Flush();
                _context.Response.OutputStream.Close();
            }
        }

        /// <summary>
        /// Write an OK to client without content.
        /// </summary>
        public void WriteOk()
        {
            _context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            _context.Response.OutputStream.Close();
        }
    }
}
