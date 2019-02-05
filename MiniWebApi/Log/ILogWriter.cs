namespace MiniWebApi.Log
{
    /// <summary>
    /// Write interface for writing the log.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Write the log into target place.
        /// </summary>
        /// <param name="message">The log message to be written.</param>
        void Write(string message);
    }
}
