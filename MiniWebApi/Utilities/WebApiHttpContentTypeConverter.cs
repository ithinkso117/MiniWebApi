namespace MiniWebApi.Utilities
{
    enum WebApiHttpContentType
    {
        NotSupported,
        Json,
        XWwwFormUrlencoded,
    }

    class WebApiHttpContentTypeConverter
    {
        /// <summary>
        /// Get the WebApiHttpContentType Only support Json and XWwwFormUrlencoded.
        /// </summary>
        /// <param name="contentTypeStr">The ContentType to return.</param>
        /// <returns></returns>
        public static WebApiHttpContentType ToContentType(string contentTypeStr)
        {
            if (string.IsNullOrEmpty(contentTypeStr)) return WebApiHttpContentType.NotSupported;
            var parts = contentTypeStr.ToLower().Split(';');
            if (parts.Length > 0)
            {
                switch (parts[0].Trim())
                {
                    case "application/json":
                    case "text/plain":
                        return WebApiHttpContentType.Json;
                    case "application/x-www-form-urlencoded":
                        return WebApiHttpContentType.XWwwFormUrlencoded;
                }
            }
            return WebApiHttpContentType.NotSupported;
        }
    }
}
