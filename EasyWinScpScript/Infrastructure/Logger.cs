using System;
using System.Collections.Generic;
using System.IO;

namespace EasyWinScpScript.Infrastructure
{
    public class Logger
    {
        private StreamWriter sw;
        private bool isIgnoreLogging;
        public Logger(string outputFile, bool isIgnoreLogging)
        {
            bool isAppend = false;
            this.isIgnoreLogging = isIgnoreLogging;

            if (File.Exists(outputFile))
            {
                isAppend = true;
            }
            sw = new StreamWriter(outputFile, isAppend);

            LogHeader();
        }

        /// <summary>
        /// Logs a string
        /// </summary>
        /// <param name="str"></param>
        public void Log(string str)
        {
            WriteLine(str);
        }

        /// <summary>
        /// Logs an IEnumarable<T> object on to several lines
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void LogEnumarable<T>(IEnumerable<T> obj)
        {
            string s = "";
            foreach (Object o in obj)
            {
                s += o + Environment.NewLine;
            }

            WriteLine(s);
        }

        /// <summary>
        /// Adds a line seperator to the log
        /// </summary>
        /// <param name="c"></param>
        /// <param name="count"></param>
        public void LogSeperator(char c = '*', int count = 20)
        {
            string s = "";

            for (int i = 0; i < count; i++)
            {
                s += "*";
            }

            WriteLine(s);
        }

        /// <summary>
        /// Logs an Exception
        /// </summary>
        /// <param name="exception"></param>
        public void LogError(Exception exception)
        {
            string s = "Message: ";
            s += exception.Message + Environment.NewLine;
            s += "Stack Trace: " + exception.StackTrace;

            WriteLine(s);
        }

        /// <summary>
        /// Closes the log
        /// </summary>
        public void Close()
        {
            DateTime dt = DateTime.Now;
            Log("Log Ended: " + dt.ToString("MM/dd/yy H:mm:ss"));
            LogSeperator();
            sw.Close();
            sw.Dispose();
        }

        /// <summary>
        /// Write a string to the log
        /// </summary>
        /// <param name="str"></param>
        private void WriteLine(string str)
        {
            if (!isIgnoreLogging)
            {
                sw.WriteLine(str);
            }
        }

        /// <summary>
        /// Adds the header to the log
        /// </summary>
        private void LogHeader()
        {
            LogSeperator();
            DateTime dt = DateTime.Now;
            Log("Log Start: " + dt.ToString("MM/dd/yy H:mm:ss"));
            Log("Machine: " + Environment.MachineName);
            Log("User: " + Environment.UserName);
            Log("OS Version: " + Environment.OSVersion);
            Log("Is 64 bit system: " + Environment.Is64BitOperatingSystem);
            Log("Is 64 bit program: " + Environment.Is64BitProcess);
            LogSeperator('#');
        }
    }
}
