using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CleverApiWrapper
{
    internal class Logger
    {
        int mMaxDays = 7;
        int mLogLevel = 1;
        string mLogPath = string.Empty;
        string mLogFile = string.Empty;

        internal Logger()
        {
            GetLogSettings();
            // if there is no setting in ini file for Log Path, the user doesn't want to do any logging.
            if (string.IsNullOrEmpty(mLogPath)) return;
            if (!Directory.Exists(mLogPath)) Directory.CreateDirectory(mLogPath);
            CleanLogFiles();
            string logTimestamp = DateTime.Now.ToString(@"yyMMdd_HHmmss");
            string logName = string.Format("CleverApiLog_{0}.txt", logTimestamp);
            mLogFile = System.IO.Path.Combine(mLogPath, logName);
        }

        void GetLogSettings()
        {
            // read ini file and override log location and log level if set in ini file
            try
            {
                string iniFilePath = @"C:\ProgramData\Rick\CleverApiWrapper.ini";
                if (!File.Exists(iniFilePath))
                {
                    return;
                }
                else
                {
                    string line;
                    using (StreamReader reader = new StreamReader(iniFilePath))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.StartsWith("#")) continue;
                            else if (line.StartsWith("LogPath:"))
                            {
                                mLogPath = line.Substring(8, line.Length - 8);
                            }
                            else if (line.StartsWith("LogLevel:"))
                            {
                                string value = line.Substring(9, line.Length - 9);
                                int testValue;
                                int.TryParse(value, out testValue);
                                if (testValue <= 3 && testValue >= 1)
                                {
                                    mLogLevel = testValue;
                                }
                            }
                            else if (line.StartsWith("MaxDaysLogFile:"))
                            {
                                string value = line.Substring(15, line.Length - 15);
                                int testValue;
                                int.TryParse(value, out testValue);
                                if (testValue <= (365 * 2) && testValue >= 0)
                                {
                                    mMaxDays = testValue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                mLogPath = string.Empty;
                return;
            }
        }

        void CleanLogFiles()
        {
            List<string> files = new List<string>(Directory.EnumerateFiles(mLogPath));
            foreach (string logFile in files)
            {
                FileInfo fileinfo = new FileInfo(logFile);
                double daysAged = (DateTime.Now - fileinfo.LastWriteTime).TotalDays;
                if (daysAged > mMaxDays)
                {
                    File.Delete(logFile);
                }
            }
        }

        /// <summary>
        /// Writes a line in mLogFile.  Line begins with current timestamp, then calling method, finally the passed in msg to log
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="msg"></param>
        internal void Log(string methodName, string msg, int level)
        {
            try
            {
                if (string.IsNullOrEmpty(mLogPath)) return;

                StreamWriter sw = new StreamWriter(mLogFile, true);
                DateTime now = DateTime.Now;
                string time = now.ToString("HH:mm:ss");
                sw.WriteLine("{0} {1} - {2}", time, methodName, msg);
                sw.Close();
                sw.Dispose();
            }
            catch (Exception e)
            {
                //
            }
        }
    }
}
