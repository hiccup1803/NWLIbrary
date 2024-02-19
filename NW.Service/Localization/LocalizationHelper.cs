using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Service.Localization
{
    public static class LocalizationHelper
    {
        public static string Value(string culture, string className, string resourceName)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbLocalization"].ToString()))
            {
                SqlCommand sqlCommand = new SqlCommand("CMS_Resource_GetResourceValue", connection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("className", className));
                sqlCommand.Parameters.Add(new SqlParameter("culture", culture));
                sqlCommand.Parameters.Add(new SqlParameter("resourceName", resourceName));
                connection.Open();
                object value = sqlCommand.ExecuteScalar();
                connection.Close();
                return value != null ? value.ToString() : string.Empty;
            }
        }

        public static string Update(string culture, string className, string resourceName, string resourceValue)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbLocalization"].ToString()))
            {
                SqlCommand sqlCommand = new SqlCommand("CMS_Resource_UpdateResourceValue", connection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("className", className));
                sqlCommand.Parameters.Add(new SqlParameter("culture", culture));
                sqlCommand.Parameters.Add(new SqlParameter("resourceName", resourceName));
                sqlCommand.Parameters.Add(new SqlParameter("resourceValue", resourceValue));
                connection.Open();
                object value = sqlCommand.ExecuteScalar();
                connection.Close();
                return value != null ? value.ToString() : string.Empty;
            }
        }
    }
}
