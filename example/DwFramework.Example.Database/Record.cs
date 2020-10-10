using System;
using SqlSugar;

namespace DwFramework.Example.Database
{
    [SugarTable("record")]
    public class Record
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        [SugarColumn(ColumnName = "create_time")]
        public DateTime CreateTime { get; set; }
    }
}
