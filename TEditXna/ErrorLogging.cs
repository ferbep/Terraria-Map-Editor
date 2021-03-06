﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TEditXna
{
    public static class ErrorLogging
    {
        public static void ViewLog()
        {
            Process.Start(LogFilePath);
        }

        public static void Initialize()
        {
            lock (LogFilePath)
            {
                string fullPath = Path.GetFullPath(LogFilePath);
                if (Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                if (File.Exists(LogFilePath))
                {
                    string destFileName = LogFilePath + ".old";
                    if (File.Exists(destFileName))
                        File.Delete(destFileName);
                    File.Move(LogFilePath, destFileName);
                }
            }
        }

        public static string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria", "TEditLog.txt");

        #region ErrorLevel enum

        public enum ErrorLevel
        {
            Debug,
            Trace,
            Warn,
            Error,
            Fatal
        }

        #endregion

        public static void Log(string message)
        {
            lock (LogFilePath)
            {
                File.AppendAllText(LogFilePath,
                                   string.Format("{0}: {1} {2}",
                                                 DateTime.Now.ToString("MM-dd-yyyy HH:mm"),
                                                 message,
                                                 Environment.NewLine));
            }
        }

        public static void LogException(object ex)
        {
            if (ex is AggregateException)
            {
                var e = ex as AggregateException;
                foreach (var curE in e.Flatten().InnerExceptions) LogException(curE);
            }
            else if (ex is Exception)
            {
                var e = ex as Exception;
                // Log inner exceptions first
                if (e.InnerException != null)
                    LogException(e.InnerException);

                Log(String.Format("{0} - {1}\r\n{2}", ErrorLevel.Error, e.Message, e.StackTrace));
            }
        }

        public static string Version
        {
            get
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
                return fvi.FileVersion;
            }
        }
    }
}