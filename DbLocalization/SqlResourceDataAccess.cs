using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.UI.Design;
using System.Windows.Forms;
using System.Configuration;
using System.Web.Configuration;

namespace DbLocalization
{
    public static class SqlResourceDataAccess
    {
        private static string localizationDomain;

        public static string GetLocalizationDomain()
        {
            if (localizationDomain == null) 
            {
                localizationDomain = WebConfigurationManager.AppSettings[SqlResourceHelper.LocalizationDomainStringKey];
                if (localizationDomain == null)
                {
                    throw new Exception("Unable to find <LocalizationDomain> application setting. - 1");
                }
            }

            return localizationDomain;
        }

        public static string GetLocalizationDomain(IServiceProvider serviceProvider)
        {
            if (localizationDomain == null)
            {
                IWebApplication webApp = serviceProvider.GetService(typeof (IWebApplication)) as IWebApplication;
                if (webApp == null)
                {
                    throw new Exception("Unable to get WebApplication.");
                }

                Configuration webConfig = webApp.OpenWebConfiguration(true);
                if (webConfig == null)
                {
                    throw new Exception("Unable to open configuration file.");
                }

                KeyValueConfigurationElement element = webConfig.AppSettings.Settings[SqlResourceHelper.LocalizationDomainStringKey];
                if (element == null)
                {
                    throw new Exception("Unable to find <LocalizationDomain> application setting. - 2");
                }

                localizationDomain = element.Value;
            }

            return localizationDomain;
        }

        public static SqlConnection CreateConnection(bool designMode, IServiceProvider serviceProvider)
        {
            string connectionString;
            if (serviceProvider == null)
            {
                connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[SqlResourceHelper.ConnectionStringKey].ToString();
            }
            else
            {
                IWebApplication webApp = (IWebApplication)serviceProvider.GetService(typeof(IWebApplication));
                connectionString = webApp.OpenWebConfiguration(true).ConnectionStrings.ConnectionStrings[SqlResourceHelper.ConnectionStringKey].ToString();
            }
            return new SqlConnection(connectionString);
        }

        public static string GetSingleResource(string virtualPath, string className, string cultureName, string resourceName)
        {
            string domain = GetLocalizationDomain();

            SqlConnection conn = CreateConnection(false, null);
            SqlCommand cmd;

            string culture = cultureName;

            if (string.IsNullOrEmpty(cultureName))
                culture = SqlResourceHelper.DefaultCulture;
            else
                culture += "%";

            if (String.IsNullOrEmpty(className))
            {
                cmd = new SqlCommand("usp_CMS_Resource_GetSingleVirtualPathV2", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Domain", domain));
                cmd.Parameters.Add(new SqlParameter("VirtualPath", virtualPath));
                cmd.Parameters.Add(new SqlParameter("Culture", culture));
                cmd.Parameters.Add(new SqlParameter("ResourceName", resourceName));
            }
            else if (String.IsNullOrEmpty(virtualPath))
            {
                cmd = new SqlCommand("usp_CMS_Resource_GetSingleClassNameV2", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Domain", domain));
                cmd.Parameters.Add(new SqlParameter("ClassName", className));
                cmd.Parameters.Add(new SqlParameter("Culture", culture));
                cmd.Parameters.Add(new SqlParameter("ResourceName", resourceName));
            }
            else
            {
                throw new Exception("SqlResourceDataAccess GetSingleResource - No className or virtualPath");
            }

            try
            {
                conn.Open();
                return (string)cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("SqlResourceDataAccess GetSingleResource - An error occured while reading resources from the database.", e);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public static SqlResourceListDictionary GetResources(string virtualPath, string className, string cultureName, bool designMode, IServiceProvider serviceProvider)
        {
            string domain;

            if (serviceProvider != null)
                domain = GetLocalizationDomain(serviceProvider);
            else
                domain = GetLocalizationDomain();

            SqlConnection conn = CreateConnection(designMode, serviceProvider);
            SqlCommand cmd;

            string culture = cultureName;

            if (string.IsNullOrEmpty(cultureName))
                culture = SqlResourceHelper.DefaultCulture;
            else
                culture += "%";

            if (String.IsNullOrEmpty(className))
            {
                cmd = new SqlCommand("usp_CMS_Resource_GetVirtualPathV2", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Domain", domain));
                cmd.Parameters.Add(new SqlParameter("VirtualPath", virtualPath));
                cmd.Parameters.Add(new SqlParameter("Culture", culture));
            }
            else if (String.IsNullOrEmpty(virtualPath))
            {
                cmd = new SqlCommand("usp_CMS_Resource_GetClassNameV2", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Domain", domain));
                cmd.Parameters.Add(new SqlParameter("ClassName", className));
                cmd.Parameters.Add(new SqlParameter("Culture", culture));
            }
            else
            {
                throw new Exception("SqlResourceDataAccess GetResources - No className or virtualPath");
            }

            SqlResourceListDictionary resources = new SqlResourceListDictionary();
            resources.DateCreated = DateTime.Now;

            try
            {
                conn.Open();         
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    object resourceValue = reader["ResourceValue"];
                    string resourceName = (string)reader["ResourceName"];
                    resources[resourceName] = resourceValue;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("SqlResourceDataAccess GetResources - An error occured while reading resources from the database.", e);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return resources;
        }

        public static void AddResource(string key, object value, string virtualPath, IServiceProvider serviceProvider)
        {
            string domain = GetLocalizationDomain(serviceProvider);

            string cultureName = SqlResourceHelper.DefaultCulture;

            SqlConnection conn = SqlResourceDataAccess.CreateConnection(true, serviceProvider);

            try
            {
                conn.Open();
                // Check if resource already exists
                SqlCommand cmd = new SqlCommand("usp_CMS_Resource_VirtualPathExistsV2", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Domain", domain));
                cmd.Parameters.Add(new SqlParameter("VirtualPath", (virtualPath == null ? (object)DBNull.Value : (object)virtualPath)));
                cmd.Parameters.Add(new SqlParameter("Culture", cultureName));
                cmd.Parameters.Add(new SqlParameter("ResourceName", key));
                int count = (int)cmd.ExecuteScalar();

                if (count == 0)
                {
                    // New resource, use INSERT
                    cmd = new SqlCommand("usp_CMS_Resource_CreateVirtualPathV2", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("Domain", domain));
                    cmd.Parameters.Add(new SqlParameter("VirtualPath", virtualPath));
                    cmd.Parameters.Add(new SqlParameter("Culture", cultureName));
                    cmd.Parameters.Add(new SqlParameter("ResourceName", key));
                    cmd.Parameters.Add(new SqlParameter("ResourceValue", ((value == null) ? (object)DBNull.Value : (object)value.ToString())));
                    int rowsAffected = cmd.ExecuteNonQuery();
                    //Debug.Assert(rowsAffected == 1, "Expected exactly 1 row to be affected when inserting new resource: " + key + " = " + value);
                }
                else
                {
                    // Old resource with this key exists, use UPDATE
                    cmd = new SqlCommand("usp_CMS_Resource_UpdateVirtualPathV2", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("Domain", domain));
                    cmd.Parameters.Add(new SqlParameter("VirtualPath", virtualPath));
                    cmd.Parameters.Add(new SqlParameter("Culture", cultureName));
                    cmd.Parameters.Add(new SqlParameter("ResourceName", key));
                    cmd.Parameters.Add(new SqlParameter("ResourceValue", ((value == null) ? (object)DBNull.Value : (object)value.ToString())));
                    int rowsAffected = cmd.ExecuteNonQuery();
                    //Debug.Assert(rowsAffected == 1, "Expected exactly 1 row to be affected when updating existing resource: " + key + " = " + value);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("SqlResourceHelper AddResource - An error occured while reading resources from the database.", e);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        public static DataTable GetResourcesByCulture(string culture)
        {
            SqlConnection conn = CreateConnection(false, null);

            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                // Check if resource already exists
                SqlCommand cmd = new SqlCommand("CMS_Resource_GetResources", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("Culture", culture));

                dt.Columns.Add("ClassName");
                dt.Columns.Add("ResourceName");
                dt.Columns.Add("ResourceValue");
                SqlDataReader sqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (sqlDataReader.Read())
                {
                    DataRow dr = dt.NewRow();
                    dr["ClassName"] = sqlDataReader["ClassName"].ToString();
                    dr["ResourceName"] = sqlDataReader["ResourceName"].ToString();
                    dr["ResourceValue"] = sqlDataReader["ResourceValue"].ToString();

                    dt.Rows.Add(dr);
                }
                sqlDataReader.Close();
                sqlDataReader.Dispose();

                return dt;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("SqlResourceHelper AddResource - An error occured while reading resources from the database.", e);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}
