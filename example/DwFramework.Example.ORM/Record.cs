using SqlSugar;

namespace DwFramework.Example.ORM
{
    [SugarTable("record")]
    public class Record
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public long ID { get; set; }
        [SugarColumn(ColumnName = "name")]
        public string Name { get; set; }
    }
}
