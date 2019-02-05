using System;

namespace MiniWebApi.Log
{

    /// <summary>
    /// Write log to console.
    /// </summary>
    class ConsoleLogWriter :ILogWriter
    {
        /// <summary>
        /// Write the log to console.
        /// </summary>
        /// <param name="message">The log message to be written.</param>
        public void Write(string message)
        {
            lock (this)
            {
                var now = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss");
                var logMsg = $"[{now}] - {message}";
                Console.WriteLine(logMsg);
            }
        }
    }
}
