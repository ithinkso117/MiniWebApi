using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniWebApi.Log
{
    /// <summary>
    /// Write log to target place, replace the Writer to redirect the write place.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// The writer implementation.
        /// </summary>
        public static ILogWriter Writer { get; set; }


        static Logger()
        {
            Writer = new ConsoleLogWriter();
        }

        /// <summary>
        /// Write the log into target place.
        /// </summary>
        /// <param name="logMessage">The log message to be written.</param>
        public static void Write(string logMessage)
        {
            Writer?.Write(logMessage);
        }
    }
}
