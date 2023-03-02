using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Diagnostics.CodeAnalysis;

namespace Onibara.Logger
{
    public class DbLogger : ILogger
    {
        private readonly DbLoggerProvider _dbLoggerProvider;
        public DbLogger([NotNull] DbLoggerProvider dbLoggerProvider)
        {
            _dbLoggerProvider = dbLoggerProvider;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                // Don't log the entry if it's not enabled.
                return;
            }            

            using (var connection = new NpgsqlConnection(_dbLoggerProvider.Options.ConnectionString))
            {               

                var values = new JObject();

                if (_dbLoggerProvider?.Options?.LogFields?.Any() ?? false)
                {
                    values["LogLevel"] = logLevel.ToString();

                    values["Message"] = state?.ToString();

                    if(exception?.Message!= null)
                        values["ExceptionMessage"] = exception?.Message;                     
                }                             

                var query = string.Format("INSERT INTO error (errorvalue, created) VALUES ('{0}', '{1}')",
                    JsonConvert.SerializeObject(values, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        Formatting = Formatting.None
                    }).ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                try
                {
                    connection.Open();
                    connection.Query(query);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }               
            }            
        }
    }
}
