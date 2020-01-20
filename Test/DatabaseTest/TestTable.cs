using System;

using SqlSugar;

namespace Test
{
    [SugarTable("testtable")]
    public class TestTable
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        [SugarColumn(ColumnName = "name")]
        public string Name { get; set; }
    }
}
