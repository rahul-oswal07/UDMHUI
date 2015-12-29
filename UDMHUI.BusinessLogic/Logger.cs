using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace UDMHUI.BusinessLogic
{
    public class Logger
    {
        //   [ThreadStatic]
        //   private static bool _inLogging = false;
        private const string ArgumentErrorMessage = "Error in format string. Not all arguments are defined. Format: ";
        private static TraceSource _defaultSource = new TraceSource("UDHMLOG");
        private static TraceSource _logTraceSource = _defaultSource;

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogError(Exception ex, string format, params object[] args)
        {
            string message;
            try
            {
                message = string.Format(CultureInfo.InvariantCulture, format, args);
            }
            catch (Exception ex1)
            {
                message = format;
                LogWarning("string.Format error when formatting " + format + FormatArguments(" -- ", args), ex1);
            }

            LogMessage(TraceEventType.Error, message, ex);
        }
        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="traceEventType">Type of the trace event.</param>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        private static void LogMessage(TraceEventType traceEventType, string message, Exception ex)
        {
            //if (_inLogging)
            //{
            //    return; // prevent logging loops
            //}

            string formattedLogMessage = FormatLogMessage(message, ex);
            try
            {
                _logTraceSource.TraceEvent(traceEventType, 0, formattedLogMessage);
            }
            catch (Exception ex1)
            {
                // Any errors during logging should not fail application flow
                try
                {
                    using (var defaultTraceListener = new DefaultTraceListener())
                    {
                        defaultTraceListener.Write(ex1.ToString());
                        defaultTraceListener.Write(formattedLogMessage);
                    }
                }
                catch
                {
                    // Any errors during logging should not fail application flow
                }
            }
        }
        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public static void LogWarning(string message, Exception ex = null)
        {
            LogMessage(TraceEventType.Warning, message, ex);
        }
        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogWarning(Exception ex, string format, params object[] args)
        {
            string message;
            try
            {
                message = string.Format(CultureInfo.InvariantCulture, format, args);
                message += FormatUnspecifiedArgs(" -- ", format, args, ArgumentErrorMessage);
            }
            catch (Exception ex1)
            {
                message = format;
                LogWarning("string.Format error when formatting " + format + FormatArguments(" -- ", args), ex1);
            }

            LogMessage(TraceEventType.Warning, message, ex);
        }
        /// <summary>
        /// Logs the debug information.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogDebug(string format, params object[] args)
        {
            string message;
            try
            {
                message = string.Format(CultureInfo.InvariantCulture, format, args);
                message += FormatUnspecifiedArgs(" -- ", format, args, ArgumentErrorMessage);
            }
            catch (Exception ex)
            {
                message = format;
                LogWarning("string.Format error when formatting " + format + FormatArguments(" -- ", args), ex);
            }

            LogMessage(TraceEventType.Verbose, message, null);
        }

        /// <summary>
        /// Formats any unspecified arguments.
        /// This allows logging of any unspecified argument that were mistakenly omitted in the 'format' part of the message
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="warningToLog">The warning to log.</param>
        /// <returns></returns>
        public static string FormatUnspecifiedArgs(string prefix, string format, object[] args, string warningToLog)
        {
            if (args == null || args.Length == 0)
            {
                return string.Empty;

            }
            try
            {
                var items = System.Text.RegularExpressions.Regex.Matches(format, @"(?<!\{)\{([0-9]+).*?\}(?!})")
                    .Cast<System.Text.RegularExpressions.Match>()
                    .Select(m => Convert.ToInt32(m.Groups[1].Value.ToString(), CultureInfo.InvariantCulture))
                    .ToList();

                var argumentUsedList = new List<bool>();
                for (int i = 0; i < args.Length; i++)
                {
                    argumentUsedList.Add(false);
                }

                foreach (int itemNumber in items)
                {
                    if (itemNumber >= 0 && itemNumber < args.Length)
                    {
                        argumentUsedList[itemNumber] = true;
                    }
                }

                var text = new StringBuilder();

                for (int i = 0; i < args.Length; i++)
                {
                    if (i >= argumentUsedList.Count ||
                        !argumentUsedList[i])
                    {
                        // Missing arg in format
                        if (text.Length == 0)
                        {
                            text.Append(prefix);
                        }
                        else
                        {
                            text.Append(" ");
                            text.Append("{");
                            text.Append(i);
                            text.Append("}: ");
                            text.Append(args[i] == null ? "(null)" : args[i].ToString());
                        }
                    }
                }

                if (text.Length > 0)
                {
                    Logger.LogWarning(warningToLog + "\"" + format + "\" " + text);
                }

                return text.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failure during FormatUnspecifiedArgs");
                return string.Empty;
            }
        }
        /// <summary>
        /// Formats the arguments.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static string FormatArguments(string prefix, object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return string.Empty;
            }

            var text = new StringBuilder();
            text.Append(prefix);
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    text.Append("; ");
                }

                text.Append(i);
                text.Append(": ");
                text.Append(args[i] == null ? "(null)" : args[i].ToString());
            }

            return text.ToString();
        }
        /// <summary>
        /// Formats the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private static string FormatLogMessage(string message, Exception ex)
        {
            var msg = new StringBuilder();
            msg.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
            msg.Append("\t[");
            msg.Append(Thread.CurrentThread.ManagedThreadId);
            msg.Append("]\t");
            if (!string.IsNullOrEmpty(message))
            {
                msg.Append(message);
            }

            if (ex != null)
            {
                if (message != null && !message.EndsWith(".", StringComparison.Ordinal))
                {
                    msg.Append(". ");
                }

                msg.Append(ex);
            }

            return msg.ToString();
        }
    }
}
