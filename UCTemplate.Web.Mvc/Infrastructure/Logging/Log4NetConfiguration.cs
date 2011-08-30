namespace UCTemplate.Web.Mvc.Infrastructure.Logging
{
    #region using

    using System.Data;
    using System.Data.SqlClient;

    using log4net.Appender;
    using log4net.Core;
    using log4net.Layout;

    #endregion

    /// <summary>
    /// Log4Net appender configurations.
    /// </summary>
    public static class Log4NetConfiguration
    {
        /// <summary>
        /// Returns a configured AdoNetAppender.
        /// </summary>
        /// <param name="connectionString">SqlConnectionString for the logging database.</param>
        /// <param name="tableName">Table to save logs to.</param>
        /// <param name="level">The level to log at.</param>
        /// <returns>A configured AdoNetAppender.</returns>
        public static AdoNetAppender GetSqlLogAppender(string connectionString, string tableName, Level level)
        {
            var appender = new AdoNetAppender
            {
                BufferSize = 1,
                ConnectionType = typeof(SqlConnection).AssemblyQualifiedName,
                ConnectionString = connectionString,
                CommandText = "INSERT INTO " + tableName + " (Date, Level, Logger, Message, Exception) VALUES (@date, @level, @logger, @message, @exception)",
                Threshold = level
            };

            appender.AddParameter(new AdoNetAppenderParameter
                                      {
                                          ParameterName = "@date",
                                          DbType = DbType.DateTime,
                                          Layout = new RawTimeStampLayout()

                                      });
            appender.AddParameter(new AdoNetAppenderParameter
                                      {
                                          ParameterName = "@level",
                                          DbType = DbType.String,
                                          Size = -1,
                                          Layout = new Layout2RawLayoutAdapter(new PatternLayout("%level"))
                                      });

            appender.AddParameter(new AdoNetAppenderParameter
                                      {
                                          ParameterName = "@logger",
                                          DbType = DbType.String,
                                          Size = -1,
                                          Layout = new Layout2RawLayoutAdapter(new PatternLayout("%logger"))
                                      });

            appender.AddParameter(new AdoNetAppenderParameter
                                      {
                                          ParameterName = "@message",
                                          DbType = DbType.String,
                                          Size = -1,
                                          Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message"))
                                      });

            appender.AddParameter(new AdoNetAppenderParameter
                                      {
                                          ParameterName = "@exception",
                                          DbType = DbType.String,
                                          Size = -1,
                                          Layout = new Layout2RawLayoutAdapter(new PatternLayout("%exception"))
                                      });

            appender.ActivateOptions();

            return appender;
        }
    }
}