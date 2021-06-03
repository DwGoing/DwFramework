using System;
using System.Text;

namespace DwFramework.Core.Generator
{
    public static class StringGenerator
    {
        /// <summary>
        /// 创建表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="labels"></param>
        /// <param name="labelValues"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static string CreateTable<T>(string[] labels, Func<T, string>[] labelValues, T[] datas)
        {
            if (labels == null || labels.Length <= 0 || labelValues == null || labelValues.Length <= 0 || labels.Length != labelValues.Length)
                throw new Exception("参数错误");
            var labelWidths = new int[labels.Length];
            var rowCount = (datas == null ? 0 : datas.Length) + 1;
            var rows = new string[rowCount, labels.Length];
            // 初始化数据
            for (var i = 0; i < rowCount; i++)
            {
                for (var j = 0; j < labels.Length; j++)
                {
                    rows[i, j] = i == 0 ? labels[j] : labelValues[j](datas[i - 1]).ToString();
                    var dataWidth = rows[i, j].Length + 2;
                    labelWidths[j] = labelWidths[j] < dataWidth ? dataWidth : labelWidths[j];
                }
            }
            // 创建表结构
            var builder = new StringBuilder();
            builder.Append('+');
            for (var i = 0; i < labels.Length; i++)
            {
                builder.Append("".PadLeft(labelWidths[i], '-'));
                builder.Append('+');
            }
            builder.Append('\n');
            var split = builder.ToString();
            for (var i = 0; i < rowCount; i++)
            {
                builder.Append("|");
                for (var j = 0; j < labels.Length; j++)
                {
                    builder.Append(rows[i, j].PadLeft(labelWidths[j] - 1, ' '));
                    builder.Append(" |");
                }
                builder.Append('\n');
                builder.Append(split);
            }
            return builder.ToString(); ;
        }
    }
}
