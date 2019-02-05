using System;
using System.IO;

namespace MiniWebApi.Utilities
{
    public static class HttpMisc
    {
        public static int IndexOf(byte[] searchWithin, byte[] searchFor, int startIndex)
        {
            int index = 0;
            int startPos = Array.IndexOf(searchWithin, searchFor[0], startIndex);

            if (startPos != -1)
            {
                while ((startPos + index) < searchWithin.Length)
                {
                    if (searchWithin[startPos + index] == searchFor[index])
                    {
                        index++;
                        if (index == searchFor.Length)
                        {
                            return startPos;
                        }
                    }
                    else
                    {
                        startPos = Array.IndexOf(searchWithin, searchFor[0], startPos + index);
                        if (startPos == -1)
                        {
                            return -1;
                        }
                        index = 0;
                    }
                }
            }

            return -1;
        }

        public static byte[] ToByteArray(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}
