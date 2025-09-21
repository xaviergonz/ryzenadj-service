using System;

namespace RyzenAdjService
{
    public static class Logger
    {
        /// <summary>
        /// Writes a message to the console with a timestamp prefix.
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void WriteLine(string message)
        {
            Console.WriteLine($"{GetTimestampPrefix()} {message}");
        }

        /// <summary>
        /// Writes an error message to the error stream with a timestamp prefix.
        /// </summary>
        /// <param name="message">The error message to log</param>
        public static void WriteError(string message)
        {
            Console.Error.WriteLine($"{GetTimestampPrefix()} {message}");
        }

        /// <summary>
        /// Generates a timestamp prefix in the format [dd/MM/yyyy HH:mm]
        /// </summary>
        /// <returns>Formatted timestamp string</returns>
        private static string GetTimestampPrefix()
        {
            return DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]");
        }
    }
}