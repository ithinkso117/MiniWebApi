using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiniWebApi.Utilities
{
    public class HttpContentParser
    {
        public IDictionary<string, string> Parameters = new Dictionary<string, string>();

        public bool Success
        {
            get;
            private set;
        }

        public HttpContentParser(Stream stream)
        {
            Parse(stream, Encoding.UTF8);
        }

        public HttpContentParser(Stream stream, Encoding encoding)
        {
            Parse(stream, encoding);
        }

        private void Parse(Stream stream, Encoding encoding)
        {
            Success = false;

            // Read the stream into a byte array
            byte[] data = HttpMisc.ToByteArray(stream);

            // Copy to a string for header parsing
            var content = encoding.GetString(data);

            var name = string.Empty;
            var value = string.Empty;
            var lookForValue = false;
            var charCount = 0;

            foreach (var c in content)
            {
                if (c == '=')
                {
                    lookForValue = true;
                }
                else if (c == '&')
                {
                    lookForValue = false;
                    AddParameter(name, value);
                    name = string.Empty;
                    value = string.Empty;
                }
                else if (!lookForValue)
                {
                    name += c;
                }
                else
                {
                    value += c;
                }

                if (++charCount == content.Length)
                {
                    AddParameter(name, value);
                    break;
                }
            }

            // Get the start & end indexes of the file contents
            //int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
            //Parameters.Add(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());

            // If some data has been successfully received, set success to true
            if (Parameters.Count != 0)
                Success = true;
        }

        private void AddParameter(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(value))
                Parameters.Add(name.Trim(), value.Trim());
        }

    }
}
