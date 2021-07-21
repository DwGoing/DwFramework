using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using SqlSugar;

namespace DwFramework.SqlSugar
{
    public sealed class Config
    {
        public Dictionary<string, DbConnectionConfig> ConnectionConfigs { get; init; }
    }

    public sealed class DbConnectionConfig
    {
        public string ConnectionString { get; init; }
        [JsonConverter(typeof(DbType))]
        public DbType DbType { get; init; }
        public SlaveDbConnectionConfig[] SlaveConnections { get; init; }
        public bool UseMemoryCache { get; init; } = false;
    }

    public sealed class SlaveDbConnectionConfig
    {
        public string ConnectionString { get; init; }
        public int HitRate { get; init; }
    }
}