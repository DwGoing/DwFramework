using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using SqlSugar;

namespace DwFramework.ORM
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

        // private DbType ParseDbType()
        // {
        //     if (string.IsNullOrEmpty(DbType)) throw new Exception("缺少DbType配置");
        //     foreach (var item in Enum.GetValues(typeof(DbType)))
        //     {
        //         if (string.Compare(item.ToString().ToLower(), DbType.ToLower(), true) == 0)
        //             return (DbType)item;
        //     }
        //     throw new Exception("无法找到匹配的DbType");
        // }
    }

    public sealed class SlaveDbConnectionConfig
    {
        public string ConnectionString { get; init; }
        public int HitRate { get; init; }
    }
}