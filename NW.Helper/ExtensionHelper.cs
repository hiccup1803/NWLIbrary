using RestSharp.Extensions.MonoHttp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Helper
{
    public static class ExtensionHelper
    {
        public static DataTable ToDataTable(this int[] array)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            if (array != null)
            {
                DataRow dr;
                foreach (int selectedTransactionTypeId in array)
                {
                    dr = dt.NewRow();
                    dr["Id"] = selectedTransactionTypeId;

                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        public static string ToCsv(this DataTable dataTable)
        {
            StringBuilder sbData = new StringBuilder();

            // Only return Null if there is no structure.
            if (dataTable.Columns.Count == 0)
                return null;

            foreach (var col in dataTable.Columns)
            {
                if (col == null)
                    sbData.Append(",");
                else
                    sbData.Append("\"" + col.ToString().Replace("\"", "\"\"") + "\",");
            }

            sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    if (column == null)
                        sbData.Append(",");
                    else
                        sbData.Append("\"" + column.ToString().Replace("\"", "\"\"") + "\",");
                }
                sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);
            }
            
            return sbData.ToString();
        }
        public static string TimeStamp(this DateTime date)
        {
            return ((long)(date - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
        }
        public static string UrlEncode(this string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : HttpUtility.UrlEncode(text);
        }
    }
}
