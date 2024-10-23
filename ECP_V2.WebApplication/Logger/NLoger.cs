using NLog;


namespace ECP_V2.WebApplication.Logger
{
    public static class NLoger
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        #region PRINTER METHOD TO FILE
        /// <summary>
        /// Print inputstring s to current active log file
        /// </summary>
        /// <param name="logMessage"></param>
        public static void Info(string logMessage)
        {
            //Logger.Info(logMessage);
        }

        public static void Trace(string logMessage)
        {
            //Logger.Trace(logMessage);
        }
        /// <summary>
        /// Print line
        /// </summary>
        public static void Pln()
        {
            //Info(string.Empty);
        }
        /// <summary>
        ///  Log to file
        /// </summary>
        /// <param name="specialFileName">file name</param>
        /// <param name="logMessage">log info</param>
        public static void Info(string specialFileName, string logMessage)
        {
            //var logger = GetLogger(specialFileName);
            //var logger = LogManager.GetLogger(specialFileName);
            //logger.Info(logMessage);
        }
        public static void Trace(string specialFileName, string logMessage)
        {
            //var logger = LogManager.GetLogger(specialFileName);
            //logger.Trace(logMessage);
        }
        #region Warn
        public static void Warn(string logMessage)
        {
            //Logger.Warn(logMessage);
        }
        public static void Warn(string specialFileName, string logMessage)
        {
            //var logger = LogManager.GetLogger(specialFileName);
            //logger.Warn(logMessage);
        }
        #endregion
        #region Warn
        public static void Error(string logMessage)
        {
            //Logger.Error(logMessage);
        }
        public static void Error(string specialFileName, string logMessage)
        {
            //var logger = LogManager.GetLogger(specialFileName);
            //logger.Error(logMessage);
        }
        #endregion
        #endregion
    }
}
