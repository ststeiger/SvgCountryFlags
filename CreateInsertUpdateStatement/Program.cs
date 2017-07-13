using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CreateInsertUpdateStatement
{
    static class Program
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



        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if false
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Form1());
#endif

            // https://wiki.postgresql.org/wiki/What%27s_new_in_PostgreSQL_9.5
            // Postgres 9.5 (released since 2016-01-07) offers an "upsert" command:
            // INSERT ... ON CONFLICT DO NOTHING/UPDATE
            // INSERT INTO countries (country) VALUES ('France'),('Japan') ON CONFLICT DO NOTHING;

            // Wrong 
            // INSERT INTO TestTable(some_field) VALUES('37F582A1-7D28-4052-837B-1AB065FDA88A') WHERE 0 = (SELECT COUNT(*) FROM TestTable WHERE some_field = '37F582A1-7D28-4052-837B-1AB065FDA88A') 
            
            // Correct: 
            // INSERT INTO TestTable(some_field) SELECT '37F582A1-7D28-4052-837B-1AB065FDA88A' WHERE 0 = (SELECT COUNT(*) FROM TestTable WHERE some_field = '37F582A1-7D28-4052-837B-1AB065FDA88A') 



            string sql = @"
INSERT INTO TABLE (fields)
SELECT fields 
WHERE 0 = (SELECT COUNT(*) FROM table WHERE PK = @pk) ";


            sql = @"
UPDATE table 
    SET  field1 = value1 
        ,field2 = value2 
WHERE PK = @pk";


            // Get non-computed columns for table 
            sql = @"
SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
WHERE (1=1)
AND TABLE_SCHEMA = 'dbo' 
AND TABLE_NAME = 'T_Benutzer' 
AND COLUMNPROPERTY(OBJECT_ID(QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME)), COLUMN_NAME,'IsComputed') = 0 
";


            // Get PK Info 
            sql = @"
SELECT 
	 tc.TABLE_CATALOG 
	,tc.TABLE_SCHEMA 
	,tc.TABLE_NAME 
	,tc.CONSTRAINT_NAME 
	,kcu.COLUMN_NAME 
	,kcu.ORDINAL_POSITION 
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc

LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu  
	ON kcu.TABLE_SCHEMA = tc.TABLE_SCHEMA 
	AND kcu.TABLE_NAME = tc.TABLE_NAME 
	AND kcu.TABLE_CATALOG = tc.TABLE_CATALOG 
	AND kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME 
	

WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY' 
-- AND tc.TABLE_CATALOG = '...'
AND tc.TABLE_SCHEMA = 'dbo' 
AND tc.TABLE_NAME = 'T_ZO_Inventory_Dwg'

ORDER BY TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CONSTRAINT_NAME, ORDINAL_POSITION 
";


            using (System.Data.Common.DbConnection con = GetConnection())
            {
                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();

                using (System.Data.Common.DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SQL";

                    // dimMin
                    System.Data.Common.DbParameter width = cmd.CreateParameter();
                    width.ParameterName = "@dimMin";
                    width.Value = 123;
                    width.DbType = System.Data.DbType.Double;
                    cmd.Parameters.Add(width);

                    // cmd.ExecuteReader()


                    // using (System.Data.Common.DbDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)cmd)) { da.Fill(dt); }

                }

                if (con.State != System.Data.ConnectionState.Closed)
                    con.Close();
            }


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }
    }
}
