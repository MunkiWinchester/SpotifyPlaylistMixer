using System;
using NLog;

namespace SpotifyPlaylistMixer.Business
{
    public static class Logger
    {

        public static string System { get; set; }
        public static DateTime StartDateTime { get; set; }

        /// <summary>
        /// Writes the diagnostic message at the Trace level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Trace(string message, Exception exception = null)
        {
            Log(LogLevel.Trace, message, exception);
        }

        /// <summary>
        /// Writes the diagnostic message at the Debug level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Debug(string message, Exception exception = null)
        {
            Log(LogLevel.Debug, message, exception);
        }

        /// <summary>
        /// Writes the diagnostic message at the Info level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Info(string message, Exception exception = null)
        {
            Log(LogLevel.Info, message, exception);
        }

        /// <summary>
        /// Writes the diagnostic message at the Warn level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Warn(string message, Exception exception = null)
        {
            Log(LogLevel.Warn, message, exception);
        }

        /// <summary>
        /// Writes the diagnostic message at the Error level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(string message, Exception exception = null)
        {
            Log(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Writes the diagnostic message at the Fatal level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Fatal(string message, Exception exception = null)
        {            
            Log(LogLevel.Fatal, message, exception);
        }

        /// <summary>
        /// Writes the specified diagnostic message.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="callerPath"></param>
        /// <param name="callerMember"></param>
        /// <param name="callerLine"></param>
        private static void Log(LogLevel level, string message, Exception exception = null, string callerPath = "",
            string callerMember = "", int callerLine = 0)
        {
            // get the source-file-specific logger
            var logger = LogManager.GetLogger(callerPath);

            // quit processing any further if not enabled for the requested logging level
            if (!logger.IsEnabled(level)) return;

            // log the event with caller information bound to it
            var logEvent = new LogEventInfo(level, callerPath, message) {Exception = exception};
            logEvent.Properties.Add("callerpath", callerPath);
            logEvent.Properties.Add("callermember", callerMember);
            logEvent.Properties.Add("callerline", callerLine);
            logEvent.Properties.Add("system", System);
            logEvent.Properties.Add("startDateTime", StartDateTime.ToString("yyyy-MM-dd.HH-mm-ss"));
            logger.Log(logEvent);
        }
    }
}
