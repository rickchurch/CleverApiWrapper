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
    /// This logger class will perform logging using 3 different levels which can be set (LogLevel:) by user in the .ini file.  If there is no log path found in 
    ///   the .ini file (LogPath:), it is assumed the user does not wantany logging.  This class will also perform log file cleanup.  Based on the max time to 
    ///   keep log files setting (MaxDaysLogFile:) in the .ini file, this class will upon instantiation, delete any log files older than the setting.
    ///   Instead of the std list of log levels (debug, info, warning, error and critical), here we are implemented a smaller range of values (debug, info, and critical)
    /// </summary>
    internal class Logger
    {
        int mMaxDays = 7;
        int mLogLevel = 1;  // Log level 1 is highest priority log, 3 log level is routine info
        string mLogPath = string.Empty;
        string mLogFile = string.Empty;
        DateTime mLastCleanLogTime;
        HelperClass mHelper;

        internal Logger()
        {
            Init();
        }

        void Init()
        {
            mHelper = new HelperClass();
            //GetLogSettings();

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


        /// <summary>
        /// read ini file and override log location, max time to keep logs and log level if any of these are set in ini file
        /// </summary>
        //void GetLogSettings()
        //{
        //    try
        //    {
        //        string iniFilePath = @"C:\ProgramData\IDLA\CleverApiWrapper.ini";
        //        if (!File.Exists(iniFilePath))
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            string line;
        //            using (StreamReader reader = new StreamReader(iniFilePath))
        //            {
        //                while ((line = reader.ReadLine()) != null)
        //                {
        //                    if (line.StartsWith("#")) continue;
        //                    else if (line.StartsWith("LogPath:"))
        //                    {
        //                        mLogPath = line.Substring(8, line.Length - 8);
        //                    }
        //                    else if (line.StartsWith("LogLevel:"))
        //                    {
        //                        string value = line.Substring(9, line.Length - 9);
        //                        int testValue;
        //                        int.TryParse(value, out testValue);
        //                        if (testValue <= 3 && testValue >= 1)
        //                        {
        //                            mLogLevel = testValue;
        //                        }
        //                    }
        //                    else if (line.StartsWith("MaxDaysLogFile:"))
        //                    {
        //                        string value = line.Substring(15, line.Length - 15);
        //                        int testValue;
        //                        int.TryParse(value, out testValue);
        //                        if (testValue <= (365 * 2) && testValue >= 0)
        //                        {
        //                            mMaxDays = testValue;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        mLogPath = string.Empty;
        //        return;
        //    }
        //}

        // Get web.config or app.config appsetting value 
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
        /// When we instantiate this class or every 
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
        /// Writes a line in mLogFile.  Line begins with current timestamp, then calling method, finally the passed in msg to log
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
                //if(difference.TotalSeconds > 45)
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
