using System;
using Npgsql;

namespace ZenOh_ActiveRecord.Factories
{
    public static class FactoryConnection
    {
        public static String ConnectionString { private get; set; }

        public static NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(ConnectionString);
    
            return connection;
        }
    }
}
