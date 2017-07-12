
namespace SvgCountryFlags
{


    static class Program
    {


        public static string getcon()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.InitialCatalog = "GeoData";
            csb.DataSource = "DESKTOP-4P9UFE8";
            csb.IntegratedSecurity = true;

            return csb.ConnectionString;
        }


        public static void ex(string sql, string iso, int w, int h, string base64)
        {
            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(getcon()))
            {
                if(con.State != System.Data.ConnectionState.Open)
                    con.Open();

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@width", System.Data.SqlDbType.Int).Value=w;
                    cmd.Parameters.Add("@height", System.Data.SqlDbType.Int).Value =h;
                    cmd.Parameters.Add("@b64", System.Data.SqlDbType.VarChar).Value = base64;

                    cmd.Parameters.Add("@iso_country2", System.Data.SqlDbType.NVarChar).Value = iso;

                    cmd.ExecuteNonQuery();
                }

                if (con.State != System.Data.ConnectionState.Closed)
                    con.Close();
            }
        }



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [System.STAThread]
        static void Main(string[] args)
        {
#if false
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Form1());
#endif
            string flag256 = @"C:\Users\anonymous\Documents\Visual Studio 2017\Projects\flags\png\256";
            string flag512 = @"C:\Users\anonymous\Documents\Visual Studio 2017\Projects\flags\png\512";
            string flagSVG = @"C:\Users\anonymous\Documents\Visual Studio 2017\Projects\flags\svg";
            string flag = flag512;


            string[] flags = System.IO.Directory.GetFiles(flag, "*.png");

            string sql = @"
UPDATE [flags]
   SET [width] = @width 
      ,[height] = @height 
      ,[b64] = @b64 
WHERE [flag] = @iso_country2
";


            foreach (string thisFlag in flags)
            {
                string country_iso2 = System.IO.Path.GetFileNameWithoutExtension(thisFlag);
                System.Console.WriteLine(country_iso2);
                int w = -1;
                int h = -1;
                string b64 = null;

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {

                    using (System.Drawing.Image img = System.Drawing.Image.FromFile(thisFlag))
                    {
                        w = img.Width;
                        h = img.Height;

                        // img.Save("path", System.Drawing.Imaging.ImageFormat.Png);
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    byte[] ba = ms.ToArray();
                    b64 = System.Convert.ToBase64String(ba);
                }

                System.Console.WriteLine(b64);
                ex(sql, country_iso2, w,h, "data:image/png;base64," + b64);
            } // Next thisFlag 
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }
    }
}
