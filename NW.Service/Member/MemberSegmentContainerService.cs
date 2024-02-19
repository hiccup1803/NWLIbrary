using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NW.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service.Member
{
    public class MemberSegmentContainerService : IMemberSegmentContainerService
    {
        public int[] GetUsernameListByQuery(string query, int? interval, int? memberId = null)
        {
            JObject queryObj = (JObject)JsonConvert.DeserializeObject(query);

            string condition = queryObj["condition"]?.Value<string>();
            var intervalObj = queryObj["rules"]?.FirstOrDefault(r => r["id"]?.Value<string>() == "interval");
            int[] idList = new int[0];
            if (queryObj["rules"] != null)
            {
                foreach (var rule in queryObj["rules"])
                {
                    var memberIdList = GetUsernameListByQuery(JsonConvert.SerializeObject(rule), intervalObj != null ? intervalObj["value"].Value<int>() : new Nullable<int>());
                    if (memberIdList != null)
                    {
                        if (idList.Length == 0)
                            idList = memberIdList;
                        else
                        {
                            if (condition == "AND")
                                idList = idList.Intersect(memberIdList).ToArray();
                            else if (condition == "OR")
                                idList = idList.Union(memberIdList).ToArray();
                        }
                    }
                }
            }
            else
            {
                List<int> resultUserIdList = new List<int>();
                string operatorName = queryObj["operator"].Value<string>();
                switch (queryObj["id"].Value<string>())
                {
                    case "username":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["BackOfficeReadOnlySQL"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.CommandText = "SELECT Id FROM Member WHERE (" + (memberId.HasValue ? ("Id = " + memberId.Value) : "1=1") + ") AND Username " + (operatorName == "in" ? "IN" : "NOT IN") + "(" + string.Join(",", queryObj["value"].Value<string>().Split(',').Select(u => "'" + u + "'")) + ")";
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["Id"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();
                    case "deposit":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["BackOfficeReadOnlySQL"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.CommandText = "SELECT MemberId FROM MemberProviderActivity WHERE (" + (memberId.HasValue ? ("MemberId = " + memberId.Value) : "1=1") + ") AND (TransactionTypeId IN(4) OR TransactionTypeId IN(SELECT Id FROM VoltronTransactionType WHERE VoltronTransactionAdjustmentTypeId = 1)) AND " + (interval == null || interval == 99 ? "1=1" : GetFilterByIntervalValue(interval)) + " GROUP BY MemberId HAVING " + GetFilterByOperatorAndValue("SUM(Amount)", operatorName, queryObj["value"].Values().Count() > 0 ? queryObj["value"].Values().Select(t => t.Value<string>()).ToArray() : new string[] { queryObj["value"].Value<string>() }, 100);
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["MemberId"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();
                    case "withdraw":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["BackOfficeReadOnlySQL"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.CommandText = "SELECT MemberId FROM MemberProviderActivity WHERE (" + (memberId.HasValue ? ("MemberId = " + memberId.Value) : "1=1") + ") AND TransactionTypeId IN(5,15,16,17) AND " + (interval == null || interval == 99 ? "1=1" : GetFilterByIntervalValue(interval)) + " GROUP BY MemberId HAVING " + GetFilterByOperatorAndValue("SUM(Amount)", operatorName, queryObj["value"].Values().Count() > 0 ? queryObj["value"].Values().Select(t => t.Value<string>()).ToArray() : new string[] { queryObj["value"].Value<string>() }, 100);
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["MemberId"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();

                    case "depositCount":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["BackOfficeReadOnlySQL"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            if (operatorName == "equal" && queryObj["value"].Value<string>() == "0")
                                sqlCommand.CommandText = "SELECT Id AS MemberId FROM Member WHERE Id NOT IN(SELECT MemberId FROM MemberProviderActivity WHERE (" + (memberId.HasValue ? ("MemberId = " + memberId.Value) : "1=1") + ") AND (TransactionTypeId IN(4) OR TransactionTypeId IN(SELECT Id FROM VoltronTransactionType WHERE VoltronTransactionAdjustmentTypeId = 1)) AND " + (interval == null || interval == 99 ? "1=1" : GetFilterByIntervalValue(interval)) + " GROUP BY MemberId HAVING " + GetFilterByOperatorAndValue("SUM(TransactionCount)", "greater_or_equal", new string[] { "1" }, 1) + ")";
                            else
                                sqlCommand.CommandText = "SELECT MemberId FROM MemberProviderActivity WHERE (" + (memberId.HasValue ? ("MemberId = " + memberId.Value) : "1=1") + ") AND (TransactionTypeId IN(4) OR TransactionTypeId IN(SELECT Id FROM VoltronTransactionType WHERE VoltronTransactionAdjustmentTypeId = 1)) AND " + (interval == null || interval == 99 ? "1=1" : GetFilterByIntervalValue(interval)) + " GROUP BY MemberId HAVING " + GetFilterByOperatorAndValue("SUM(TransactionCount)", operatorName, queryObj["value"].Values().Count() > 0 ? queryObj["value"].Values().Select(t => t.Value<string>()).ToArray() : new string[] { queryObj["value"].Value<string>() }, 1);
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["MemberId"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();
                    case "withdrawCount":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["BackOfficeReadOnlySQL"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.CommandText = "SELECT MemberId FROM MemberProviderActivity WHERE (" + (memberId.HasValue ? ("MemberId = " + memberId.Value) : "1=1") + ") AND TransactionTypeId IN(5,15,16,17) AND " + (interval == null || interval == 99 ? "1=1" : GetFilterByIntervalValue(interval)) + " GROUP BY MemberId HAVING " + GetFilterByOperatorAndValue("SUM(CASE WHEN TransactionTypeId IN(5) THEN (TransactionCount * 1) ELSE (TransactionCount * -1) END)", operatorName, queryObj["value"].Values().Count() > 0 ? queryObj["value"].Values().Select(t => t.Value<string>()).ToArray() : new string[] { queryObj["value"].Value<string>() }, 1);
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["MemberId"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();
                    case "d-w":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["BackOfficeReadOnlySQL"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.CommandText = "SELECT MemberId FROM MemberProviderActivity WHERE (" + (memberId.HasValue ? ("MemberId = " + memberId.Value) : "1=1") + ") AND (TransactionTypeId IN(4,5,15,16,17) OR TransactionTypeId IN(SELECT Id FROM VoltronTransactionType WHERE VoltronTransactionAdjustmentTypeId = 1)) AND " + (interval == null || interval == 99 ? "1=1" : GetFilterByIntervalValue(interval)) + " GROUP BY MemberId HAVING " + GetFilterByOperatorAndValue("SUM(Amount)", operatorName, queryObj["value"].Values().Count() > 0 ? queryObj["value"].Values().Select(t => t.Value<string>()).ToArray() : new string[] { queryObj["value"].Value<string>() }, 100);
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["MemberId"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();
                    case "bet":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["BackOfficeReadOnlySQL"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.CommandText = "SELECT MemberId FROM MemberProviderActivity WHERE (" + (memberId.HasValue ? ("MemberId = " + memberId.Value) : "1=1") + ") AND TransactionTypeId IN(2) AND " + (interval == null || interval == 99 ? "1=1" : GetFilterByIntervalValue(interval)) + " GROUP BY MemberId HAVING " + GetFilterByOperatorAndValue("SUM(Amount)", operatorName, queryObj["value"].Values().Count() > 0 ? queryObj["value"].Values().Select(t => t.Value<string>()).ToArray() : new string[] { queryObj["value"].Value<string>() }, 100);
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["MemberId"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();
                    case "win":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["BackOfficeReadOnlySQL"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.CommandText = "SELECT MemberId FROM MemberProviderActivity WHERE (" + (memberId.HasValue ? ("MemberId = " + memberId.Value) : "1=1") + ") AND TransactionTypeId IN(1) AND " + (interval == null || interval == 99 ? "1=1" : GetFilterByIntervalValue(interval)) + " GROUP BY MemberId HAVING " + GetFilterByOperatorAndValue("SUM(Amount)", operatorName, queryObj["value"].Values().Count() > 0 ? queryObj["value"].Values().Select(t => t.Value<string>()).ToArray() : new string[] { queryObj["value"].Value<string>() }, 100);
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["MemberId"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();
                    case "age":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DBt"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.CommandText = "SELECT [Value] FROM MemberDetail WHERE [Key] = 'Integration.Voltron' AND MemberId IN(SELECT [MemberId] FROM MemberDetail WHERE [Key] = 'Register.Birthday' AND (" + (memberId.HasValue ? ("MemberId IN (SELECT MemberId FROM MemberDetail WHERE [Key] = 'Integration.Voltron' AND [Value] = '" + memberId.Value + "')") : "1=1") + ") AND " + GetFilterByOperatorAndValue("DATEDIFF(YEAR,  [Value], GETUTCDATE())", operatorName, queryObj["value"].Values().Count() > 0 ? queryObj["value"].Values().Select(t => t.Value<string>()).ToArray() : new string[] { queryObj["value"].Value<string>() }, 1) + ")";
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["Value"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();
                    case "ggr":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["BackOfficeReadOnlySQL"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.CommandText = "SELECT MemberId FROM MemberProviderActivity WHERE (" + (memberId.HasValue ? ("MemberId = " + memberId.Value) : "1=1") + ") AND TransactionTypeId IN(1,2,3) AND " + (interval == null || interval == 99 ? "1=1" : GetFilterByIntervalValue(interval)) + " GROUP BY MemberId HAVING " + GetFilterByOperatorAndValue("SUM(Amount)", operatorName, queryObj["value"].Values().Count() > 0 ? queryObj["value"].Values().Select(t => t.Value<string>()).ToArray() : new string[] { queryObj["value"].Value<string>() }, 100);
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["MemberId"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();
                    case "interval":
                        resultUserIdList = null;
                        break;
                    case "demography":
                        using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["BackOfficeReadOnlySQL"].ConnectionString))
                        {
                            sql.Open();
                            SqlCommand sqlCommand = sql.CreateCommand();
                            sqlCommand.CommandTimeout = 0;
                            sqlCommand.CommandText = "SELECT MPA.MemberId FROM MemberProviderActivity MPA JOIN Provider P ON P.Id = MPA.ProviderId WHERE (" + (memberId.HasValue ? ("MPA.MemberId = " + memberId.Value) : "1=1") + ") AND MPA.TransactionTypeId IN(2) AND " + (interval == null || interval == 99 ? "1=1" : GetFilterByIntervalValue(interval)) + " GROUP BY MPA.MemberId HAVING " + (queryObj["value"].Value<string>() == "1" ? "SUM(CASE WHEN P.ProviderTypeId = 1 THEN (Amount * -1) ELSE 0 END) > SUM(CASE WHEN P.ProviderTypeId = 3 THEN (Amount * -1) ELSE 0 END)" : "SUM(CASE WHEN P.ProviderTypeId = 1 THEN (Amount * -1) ELSE 0 END) < SUM(CASE WHEN P.ProviderTypeId = 3 THEN (Amount * -1) ELSE 0 END)");
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    resultUserIdList.Add(Convert.ToInt32(dr["MemberId"]));
                                }
                            }
                        }
                        return resultUserIdList.ToArray();
                    default:
                        break;
                }
            }

            return idList;
        }

        string GetFilterByIntervalValue(int? value)
        {
            switch (value)
            {
                case 1:
                    return "[Date] >= DATEADD(DAY, -1, GETUTCDATE()) AND [Date] <= GETUTCDATE()";
                case 2:
                    return "[Date] >= DATEADD(DAY, -30, GETUTCDATE()) AND [Date] <= GETUTCDATE()";
                case 3:
                    return "[Date] >= DATEADD(DAY, -90, GETUTCDATE()) AND [Date] <= GETUTCDATE()";
                default:
                    return "1=1";
            }
        }
        string GetFilterByOperatorAndValue(string query, string operatorName, string[] values, int multiplyBy)
        {
            switch (operatorName)
            {
                case "equal":
                    return query + " = " + Convert.ToDecimal(values[0]) * multiplyBy;
                case "less":
                    return query + " < " + Convert.ToDecimal(values[0]) * multiplyBy;
                case "less_or_equal":
                    return query + " <= " + Convert.ToDecimal(values[0]) * multiplyBy;
                case "greater":
                    return query + " > " + Convert.ToDecimal(values[0]) * multiplyBy;
                case "greater_or_equal":
                    return query + " >= " + Convert.ToDecimal(values[0]) * multiplyBy;
                case "between":
                    return query + " >= " + Convert.ToDecimal(values[0]) * multiplyBy + " AND " + query + " <" + Convert.ToDecimal(values[1]) * multiplyBy;
                default:
                    return "1=1";
            }
        }
    }
}
