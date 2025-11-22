using System;
using System.IO;

namespace RyzenAdjService
{
    public static class Logger
    {
        private static readonly string LogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "RyzenAdjService.log");
        private static readonly object LockObject = new object();

        /// <summary>
        /// Writes a message to the console with a timestamp prefix.
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void WriteLine(string message)
        {
            string formattedMessage = $"{GetTimestampPrefix()} {message}";
            Console.WriteLine(formattedMessage);
            WriteToFile(formattedMessage);
        }

        /// <summary>
        /// Writes an error message to the error stream with a timestamp prefix.
        /// </summary>
        /// <param name="message">The error message to log</param>
        public static void WriteError(string message)
        {
            string formattedMessage = $"{GetTimestampPrefix()} {message}";
            Console.Error.WriteLine(formattedMessage);
            WriteToFile(formattedMessage);
        }

        /// <summary>
        /// Writes a message to the log file.
        /// </summary>
        /// <param name="message">The message to write to the file</param>
        private static void WriteToFile(string message)
        {
            try
            {
                lock (LockObject)
                {
                    File.AppendAllText(LogFilePath, message + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to write to log file: {ex.Message}");
            }
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