using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MiniWebApi.Utilities
{
    public class HttpMultipartParser
    {
        public IDictionary<string, string> Parameters = new Dictionary<string, string>();

        public string FilePartName
        {
            get;
        }

        public bool Success
        {
            get;
            private set;
        }

        public string ContentType
        {
            get;
            private set;
        }

        public string Filename
        {
            get;
            private set;
        }

        public byte[] FileContents
        {
            get;
            private set;
        }

        public HttpMultipartParser(Stream stream, string filePartName)
        {
            FilePartName = filePartName;
            Parse(stream, Encoding.UTF8);
        }

        public HttpMultipartParser(Stream stream, Encoding encoding, string filePartName)
        {
            FilePartName = filePartName;
            Parse(stream, encoding);
        }

        private void Parse(Stream stream, Encoding encoding)
        {
            Success = false;

            // Read the stream into a byte array
            var data = HttpMisc.ToByteArray(stream);

            // Copy to a string for header parsing
            var content = encoding.GetString(data);

            // The first line should contain the delimiter
            var delimiterEndIndex = content.IndexOf("\r\n", StringComparison.InvariantCultureIgnoreCase);

            if (delimiterEndIndex > -1)
            {
                var delimiter = content.Substring(0, content.IndexOf("\r\n", StringComparison.InvariantCultureIgnoreCase));

                var sections = content.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in sections)
                {
                    if (s.Contains("Content-Disposition"))
                    {
                        // If we find "Content-Disposition", this is a valid multi-part section
                        // Now, look for the "name" parameter
                        Match nameMatch = new Regex(@"(?<=name\=\"")(.*?)(?=\"")").Match(s);
                        string name = nameMatch.Value.Trim().ToLower();

                        if (name == FilePartName)
                        {
                            // Look for Content-Type
                            var re = new Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
                            var contentTypeMatch = re.Match(content);

                            // Look for filename
                            re = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                            var filenameMatch = re.Match(content);

                            // Did we find the required values?
                            if (contentTypeMatch.Success && filenameMatch.Success)
                            {
                                // Set properties
                                ContentType = contentTypeMatch.Value.Trim();
                                Filename = filenameMatch.Value.Trim();

                                // Get the start & end indexes of the file contents
                                var startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;

                                var delimiterBytes = encoding.GetBytes("\r\n" + delimiter);
                                var endIndex = HttpMisc.IndexOf(data, delimiterBytes, startIndex);

                                var contentLength = endIndex - startIndex;

                                // Extract the file contents from the byte array
                                var fileData = new byte[contentLength];

                                Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength);

                                FileContents = fileData;
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(name))
                        {
                            // Get the start & end indexes of the file contents
                            int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
                            Parameters.Add(name, s.Substring(startIndex).TrimEnd('\r', '\n').Trim());
                        }
                    }
                }

                // If some data has been successfully received, set success to true
                if (FileContents != null || Parameters.Count != 0)
                    Success = true;
            }
        }

    }
}
