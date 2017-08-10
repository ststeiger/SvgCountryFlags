using System;
using System.Collections.Generic;
using System.Web;

namespace FlagCounter
{


    /// <summary>
    /// Zusammenfassungsbeschreibung für image
    /// </summary>
    public class image : IHttpHandler
    {


        public static System.Data.Common.DbConnection GetConnection()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();

            csb.DataSource = "DESKTOP-4P9UFE8";
            csb.InitialCatalog = "GeoData";

            csb.IntegratedSecurity = true;
            if (!csb.IntegratedSecurity)
            {
                csb.UserID = "GeoDataWebServices";
                csb.Password = "TOP_SECRET";
            }


            if (System.StringComparer.OrdinalIgnoreCase.Equals(System.Environment.UserDomainName, "COR"))
                csb.DataSource = "COR-W81-101";

            return new System.Data.SqlClient.SqlConnection(csb.ConnectionString);
        }




        public static byte[] GetFlagFromDb(string iso)
        {
            byte[] baResult = null;

            using (System.Data.Common.DbConnection con = GetConnection())
            {
                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();

                using (System.Data.Common.DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT SUBSTRING(b64, 23, 40000000000000000) AS b64 FROM flags WHERE flag = @iso";

                    System.Data.Common.DbParameter country = cmd.CreateParameter();
                    country.ParameterName = "@iso";
                    country.Value = iso;
                    country.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(country);

                    object obj = cmd.ExecuteScalar();
                    string b64 = System.Convert.ToString(obj);
                    baResult = System.Convert.FromBase64String(b64);
                } // End Using cmd 

                if (con.State != System.Data.ConnectionState.Closed)
                    con.Close();
            } // End Using con 

            return baResult;
        }


        public void ProcessRequest(HttpContext context)
        {
            // save IP/country with img_id
            // id, img_id, ip_num, country_id
            // query img_id - group by country
            // count country, country_short, count(distinct ip_num)
            // by usr_language

            context.Response.ContentType = "image/png";

            string ctry = context.Request.Params["country"];
            if(string.IsNullOrEmpty(ctry))
                ctry = "CH";

            byte[] file = GetFlagFromDb(ctry);
            if(file.Length != 0)
                context.Response.BinaryWrite(file);
        }


        protected string GetIPAddress(System.Web.HttpContext context)
        {
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


    }


}
