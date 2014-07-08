using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Configuration;
using System.Configuration;

namespace CleverApiWrapper
{
    /// <summary>
    /// This logger class will perform logging using 3 different levels which can be set (LogLevel:) by user in the WebConfigurationManager.AppSettings or  
    ///   ConfigurationManager.AppSettings.  If there is no log path found in AppSettings, it is assumed the user does not want any logging.  This class will  
    ///   also perform log file cleanup.  Based on the max time to keep log files setting (MaxDaysLogFile:) in AppSettings, this class will upon instantiation, 
    ///   delete any log files older than the setting. Instead of the std list of log levels (debug, info, warning, error and critical), here we implemented a 
    ///   smaller range of values (1 - 3). Log level 1 is (highest priority) for errors and critical logging, 3 log level is routine (lowest) info.
    /// </summary>
    internal class Logger
    {
        int mMaxDays = 7;
        int mLogLevel = 1;  // Log level 1 is (highest priority) for errors and critical logging, 3 log level is routine (lowest) info.
        string mLogPath = string.Empty;
        internal string mLogFile = string.Empty;
        DateTime mLastCleanLogTime;
        HelperClass mHelper;

        internal Logger()
        {
            Init();
        }

        void Init()
        {
            mHelper = new HelperClass();

            mLogPath = GetAppSetting("LogPath", "C:\\ProgramData\\Rick\\CleverLogs");  // NEED TO SET DEFAULT TO EMPTY STRING BEFORE PUTTING THIS IN PRODUCTION
            mLogLevel = mHelper.ConvertStringToInt(GetAppSetting("LogLevel", "3"));
            mMaxDays = mHelper.ConvertStringToInt(GetAppSetting("MaxDaysLogFile", "7"));

            // if there is no setting in ini file for Log Path, the user doesn't want to do any logging.
            if (string.IsNullOrEmpty(mLogPath)) return;
            if (!Directory.Exists(mLogPath)) Directory.CreateDirectory(mLogPath);
            mLastCleanLogTime = DateTime.Now;
            string logTimestamp = DateTime.Now.ToString(@"yyMMdd_HHmmss");
            string logName = string.Format("CleverApiLog_{0}.txt", logTimestamp);
            mLogFile = System.IO.Path.Combine(mLogPath, logName);
            Log("Init", "Starting CleverApiWrapper - Logging level set to: " + mLogLevel, 1);

            CleanLogFiles();
        }

        string GetAppSetting(string settingName, string defaultValue = "")
        {
            // variable to store the return value
            string value = String.Empty;

            // Web.config settings          
            if (WebConfigurationManager.AppSettings != null)
            {
                value = WebConfigurationManager.AppSettings.Get(settingName);
            }
            else if (ConfigurationManager.AppSettings != null) // App.config settings
            {
                value = ConfigurationManager.AppSettings.Get(settingName);
            }

            // Return the setting if found
            if (String.IsNullOrEmpty(value)) value = defaultValue;
            return value;
        }

        /// <summary>
        /// When we instantiate this class or every nMaxDays, clean out the older log files
        /// </summary>
        /// <param name="elapsedTime"></param>
        void CleanLogFiles(double elapsedTime=0)
        {
            mLastCleanLogTime = DateTime.Now;
            Log("CleanLogFiles", string.Format("Removing log files older than {0} days old. Days since last clean: {1}", mMaxDays, elapsedTime), 2);
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
        /// Writes a line in mLogFile.  Line begins with current timestamp, then calling method, finally the passed in msg to log.
        /// If we don't have a log path specified (it is empty string) we don't log anything.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="msg"></param>
        internal void Log(string methodName, string msg, int levelThisMsg, bool lineSpaceBeforeAndAfter=false)
        {
            try
            {
                if (string.IsNullOrEmpty(mLogPath)) return;
                if (mLogLevel < levelThisMsg) return;

                TimeSpan difference = DateTime.Now - mLastCleanLogTime;
                // in case this instantiation exits for extended periods of time, we will check each time we log to see if we should do a clean
                if(difference.TotalHours > 24)
                {
                    CleanLogFiles(difference.TotalHours);
                }

                StreamWriter sw = new StreamWriter(mLogFile, true);
                DateTime now = DateTime.Now;
                string time = now.ToString("HH:mm:ss");
                if (lineSpaceBeforeAndAfter) sw.WriteLine("");
                sw.WriteLine("{0} {1}:{2} - {3}", time, methodName, levelThisMsg, msg);
                if (lineSpaceBeforeAndAfter) sw.WriteLine("");
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
