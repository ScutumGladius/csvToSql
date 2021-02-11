using CsvToSql.logging;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace CsvToSql.SqlServer
{
    public class SqlServerService
    {
        private readonly Logging _logger;
        private readonly string connectionString;

        public SqlServerService(Logging logger, string connectionString)
        {
            _logger = logger;
            this.connectionString = connectionString;
        }


        public int simpleExecQuery(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(connectionString)) return -1;
                //logQuery(query);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception)
            {
                _logger.Error($"SqlServerService : catch exception in query : {query}.");
                throw;
            }
        }

        private void logQuery(string query) { 
            if(query.Length < 60)
                _logger.Trace($"SqlServerService : execute query {query}.");
            else
                _logger.Trace($"SqlServerService : execute query {query.Substring(0, 60)} ...");
        }

        internal void simpleExecQuery(string insertStatement, int retryPolicyNumRetries, int retryPolicyDelayRetries)
        {
            for (int i = 0; i < retryPolicyNumRetries; i++)
            {
                try
                {
                    simpleExecQuery(insertStatement);
                    return;
                }
                catch (Exception)
                {
                    _logger.Error($"Try execute failed for {i + 1}-th time. ");
                    Thread.Sleep(retryPolicyDelayRetries);
                    if (i >= retryPolicyNumRetries - 1)
                        throw;
                }
            }


        }
    }
}
