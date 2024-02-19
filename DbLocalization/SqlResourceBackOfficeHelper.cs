using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLocalization
{
    public class SqlResourceBackOfficePagingModel
    {
        public IList<SqlResourceBackOfficeModel> ResourceList { get; set; }
        public int ResourceCount { get; set; }
    }
    public class SqlResourceBackOfficeModel
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string Culture { get; set; }
        public string ResourceName { get; set; }
        public string ResourceValue { get; set; }
    }
    public class SqlResourceProcessResultModel
    {
        public bool Result { get; set; }
        public string CultureName { get; set; }
    }
    public class SqlResourceBackOfficeHelper
    {
        public static SqlResourceBackOfficePagingModel ResourcesList(string culture, string className, string resourceName, string resourceValue, int pageSize, int pageIndex)
        {
            SqlResourceBackOfficePagingModel model = new SqlResourceBackOfficePagingModel();
            using (SqlConnection conn = SqlResourceDataAccess.CreateConnection(false, null))
            {
                string selectPhrase = "RowId=ROW_NUMBER() OVER (ORDER BY ResourceId DESC), [ResourceId],[DomainId],[VirtualPath],[ClassName],[LanguageId],[Culture],[ResourceName],[ResourceValue]";
                string queryWithWhereClause = "SELECT {0} FROM [CMS_Resource] WHERE 1=1 ";
                if (!string.IsNullOrEmpty(className))
                    queryWithWhereClause += "AND [ClassName] LIKE '%"+ className +"%'";
                if (!string.IsNullOrEmpty(resourceName))
                    queryWithWhereClause += "AND [ResourceName] LIKE '%" + resourceName + "%'";
                if (!string.IsNullOrEmpty(resourceValue))
                    queryWithWhereClause += "AND [ResourceValue] LIKE '%" + resourceValue + "%'";
                if (!string.IsNullOrEmpty(culture))
                    queryWithWhereClause += "AND [Culture] LIKE '%" + culture + "%'";


                string query = string.Format("SELECT TOP(" + pageSize + ") * FROM ({0}) A WHERE A.RowId > ((" + pageIndex + " - 1) * " + pageSize + ")", string.Format(queryWithWhereClause, selectPhrase));
                string countQuery = string.Format(queryWithWhereClause, "COUNT(ResourceId)");


                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = query;

                SqlCommand countCommand = conn.CreateCommand();
                countCommand.CommandText = countQuery;

                List<SqlResourceBackOfficeModel> list = new List<SqlResourceBackOfficeModel>();

                conn.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    list.Add(new SqlResourceBackOfficeModel() {
                        Id = Convert.ToInt32(sqlDataReader["ResourceId"]),
                        Culture = sqlDataReader["Culture"].ToString(),
                        ClassName = sqlDataReader["ClassName"].ToString(),
                        ResourceName = sqlDataReader["ResourceName"].ToString(),
                        ResourceValue = sqlDataReader["ResourceValue"].ToString(),
                    });
                }
                sqlDataReader.Close();
                sqlDataReader.Dispose();


                int count = (int)countCommand.ExecuteScalar();

                conn.Close();
                conn.Dispose();




                model.ResourceList = list;
                model.ResourceCount = count;
            }

            return model;
        }

        public static IList<string> ClassNameList()
        {
            SqlResourceBackOfficePagingModel model = new SqlResourceBackOfficePagingModel();
            List<string> list = new List<string>();
            using (SqlConnection conn = SqlResourceDataAccess.CreateConnection(false, null))
            {
                string query = "SELECT ClassName FROM CMS_Resource GROUP BY ClassName ORDER BY COUNT(ResourceId) DESC";

                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = query;


                conn.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    list.Add(sqlDataReader["ClassName"].ToString());
                }
                sqlDataReader.Close();
                sqlDataReader.Dispose();

                conn.Close();
                conn.Dispose();
            }

            return list;
        }
        public static SqlResourceProcessResultModel UpdateResourceList(int id, string resourceValue)
        {
            int effectedCount = 0;
            string culture = string.Empty;
            using (SqlConnection conn = SqlResourceDataAccess.CreateConnection(false, null))
            {
                string query = "UPDATE CMS_Resource SET ResourceValue = @resourceValue WHERE ResourceId = @id";

                string cultureQuery = "SELECT TOP 1 Culture FROM CMS_Resource WHERE ResourceId = @id";

                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.AddWithValue("resourceValue", resourceValue);
                sqlCommand.Parameters.AddWithValue("id", id);


                SqlCommand cultureCommand = conn.CreateCommand();
                cultureCommand.CommandText = cultureQuery;
                cultureCommand.Parameters.AddWithValue("id", id);



                conn.Open();

                effectedCount = sqlCommand.ExecuteNonQuery();
                culture = (string)cultureCommand.ExecuteScalar();

                conn.Close();
                conn.Dispose();
            }

            SqlResourceProcessResultModel result = new SqlResourceProcessResultModel();
            result.Result = effectedCount > 0;
            result.CultureName = culture;

            return result;
        }
        public static SqlResourceProcessResultModel InsertResourceList(string culture, string className, string resourceName, string resourceValue)
        {
            int effectedCount = 0;
            using (SqlConnection conn = SqlResourceDataAccess.CreateConnection(false, null))
            {
                string languageIdQuery = "SELECT TOP 1 LanguageId FROM Language WHERE Culture = @culture";
                string query = "INSERT INTO CMS_Resource(DomainId, VirtualPath, ClassName, LanguageId, Culture, ResourceName, ResourceValue) VALUES(1, NULL, @className, @languageId, @culture, @resourceName, @resourceValue)";

                SqlCommand sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.AddWithValue("className", className);
                sqlCommand.Parameters.AddWithValue("culture", culture);
                sqlCommand.Parameters.AddWithValue("resourceName", resourceName);
                sqlCommand.Parameters.AddWithValue("resourceValue", resourceValue);



                SqlCommand languageIdCommand = conn.CreateCommand();
                languageIdCommand.CommandText = languageIdQuery;
                languageIdCommand.Parameters.AddWithValue("culture", culture);


                conn.Open();

                int languageId = (int)languageIdCommand.ExecuteScalar();

                sqlCommand.Parameters.AddWithValue("languageId", languageId);
                effectedCount = sqlCommand.ExecuteNonQuery();


                conn.Close();
                conn.Dispose();
            }


            SqlResourceProcessResultModel result = new SqlResourceProcessResultModel();
            result.Result = effectedCount > 0;
            result.CultureName = culture;
            return result;
        }
    }
}
