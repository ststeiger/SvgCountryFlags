
namespace SvgCountryFlags
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


        public static void SaveFlagImageInDb(string iso, int w, int h, string base64)
        {

            string sql = @"
UPDATE [flags]
   SET [width] = @width 
      ,[height] = @height 
      ,[b64] = @b64 
WHERE [flag] = @iso_country2
";

            using (System.Data.Common.DbConnection con = GetConnection())
            {
                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();

                using (System.Data.Common.DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;

                    //cmd.Parameters.Add("@width", System.Data.SqlDbType.Int).Value = w;
                    //cmd.Parameters.Add("@height", System.Data.SqlDbType.Int).Value = h;
                    //cmd.Parameters.Add("@b64", System.Data.SqlDbType.VarChar).Value = base64;

                    //cmd.Parameters.Add("@iso_country2", System.Data.SqlDbType.NVarChar).Value = iso;

                    System.Data.Common.DbParameter width = cmd.CreateParameter();
                    width.ParameterName = "width";
                    width.Value = w;
                    width.DbType = System.Data.DbType.Int32;
                    cmd.Parameters.Add(width);

                    System.Data.Common.DbParameter height = cmd.CreateParameter();
                    height.ParameterName = "height";
                    height.Value = h;
                    height.DbType = System.Data.DbType.Int32;
                    cmd.Parameters.Add(height);

                    System.Data.Common.DbParameter b64 = cmd.CreateParameter();
                    b64.ParameterName = "b64";
                    b64.Value = base64;
                    b64.DbType = System.Data.DbType.AnsiString;
                    cmd.Parameters.Add(b64);

                    System.Data.Common.DbParameter country = cmd.CreateParameter();
                    country.ParameterName = "iso_country2";
                    country.Value = iso;
                    country.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(country);


                    cmd.ExecuteNonQuery();
                }

                if (con.State != System.Data.ConnectionState.Closed)
                    con.Close();
            }

        }




        public static System.Data.DataTable GetFlags(double dimMin)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            string sql = @"

-- DECLARE @dimMin float
-- SET @dimMin = 30.0 


;WITH CTE AS 
(
	SELECT
		 flag
		,country
		,country_id
		,width
		,height
		,b64
		
		,@dimMin/width AS rW
		,@dimMin/height AS rH
		
		,
		CASE
			WHEN @dimMin/width > @dimMin/height THEN @dimMin/width 
			ELSE @dimMin/height
		END AS rMAX
		
		,
		CASE
			WHEN @dimMin/width > @dimMin/height THEN @dimMin/height
			ELSE @dimMin/width
		END AS rMIN
		
        --,geoip_locations_temp.continent_name
	    --,geoip_locations_temp.country_iso_code
	    ,geoip_locations_temp.country_name 
	FROM flags 
    
    LEFT JOIN geoip.geoip_locations_temp
	    ON geoip_locations_temp.country_iso_code = FLAGS.flag  
)
SELECT 
	 flag
	,country
	,country_id
	,width
	,height
	--,CEILING(width * rMIN) AS wRedim
	--,CEILING(height * rMIN) AS hRedim 
	
	,CAST(ROUND(width * rMIN, 0) AS integer) AS wRedim
	,CAST(ROUND(height * rMIN, 0) AS integer) AS hRedim
	
	,b64
	--,ABS(CHECKSUM(NEWID())) % (10+1 -0) + 0 AS rn 
	,ABS(CHECKSUM(NEWID())) % (150+1 -2) + 2 AS flagCount 
	,country_name 
FROM CTE 
WHERE flag IN ('NL', 'CH', 'AD', 'FR', 'IT', 'NP')
ORDER BY flag 
";

            using (System.Data.Common.DbConnection con = GetConnection())
            {
                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();

                using (System.Data.Common.DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;

                    // dimMin
                    System.Data.Common.DbParameter width = cmd.CreateParameter();
                    width.ParameterName = "@dimMin";
                    width.Value = dimMin;
                    width.DbType = System.Data.DbType.Double;
                    cmd.Parameters.Add(width);


                    using (System.Data.Common.DbDataAdapter da = new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)cmd))
                    {
                        da.Fill(dt);
                    }

                }

                if (con.State != System.Data.ConnectionState.Closed)
                    con.Close();
            }

            return dt;
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

            if (System.StringComparer.OrdinalIgnoreCase.Equals(System.Environment.UserDomainName, "COR"))
            {
                flag256 = @"D:\username\Documents\Visual Studio 2013\Projects\flags\png\256";
                flag512 = @"D:\username\Documents\Visual Studio 2013\Projects\flags\png\512";
                flagSVG = @"D:\username\Documents\Visual Studio 2013\Projects\flags\svg";
            }



            string flag = flag512;
            // InsertFlags(flag);



            string svgCountryTemplate = @"
  <g>
    <title>GroupBar</title>
    <image y=""@image_y"" x=""@image_x"" width=""@width"" height=""@height"" class=""a"" preserveAspectRatio=""none"" 
    xlink:href=""@data"" >
      <title>@CountryName</title>
    </image>
    
    <text y=""@text_y"" x=""@text_x"" class=""b"">
      <title>@CountryName</title>
      <tspan alignment-baseline=""middle"" dy=""@dy"" ><!--
        --><title>@CountryName</title><!--
        -->@CountryShort<!--
      --></tspan><!--
        --><tspan x=""110"" alignment-baseline=""middle"" text-anchor=""end"" ><!--
            --><title>@FlagCountToolTip</title><!--
            -->@FlagCount<!--
        --></tspan>
    </text>

  </g>
";

            int x = 10;
            int y = 10;


            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int fontSize = 12;

            string svgPre = @"<svg viewBox=""0 0 1000 1000"" height=""1000"" width=""1000""
xmlns=""http://www.w3.org/2000/svg"" xmlns:svg=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"">
  <style>
    .a{ image-rendering:optimizeSpeed; }
    .b{ font-family: arial, sans-serif; font-size: " + fontSize.ToString(System.Globalization.CultureInfo.InvariantCulture) + @"px; }
  </style>
";
            sb.Append(svgPre);

            foreach (System.Data.DataRow dr in GetFlags(30.0).Rows)
            { 
                string iso_name = System.Convert.ToString(dr["flag"]);
                int w = System.Convert.ToInt32(dr["wRedim"]);
                int h = System.Convert.ToInt32(dr["hRedim"]);
                int flagCount = System.Convert.ToInt32(dr["flagCount"]);
                string b64 = System.Convert.ToString(dr["b64"]);
                string country_name = System.Convert.ToString(dr["country_name"]);
                



                string thisEntry = svgCountryTemplate;
                thisEntry = thisEntry

                    .Replace("@image_x", x.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("@image_y", y.ToString(System.Globalization.CultureInfo.InvariantCulture))

                    .Replace("@width", w.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("@height", h.ToString(System.Globalization.CultureInfo.InvariantCulture))

                    .Replace("@text_x", (x + 35).ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("@text_y", y.ToString(System.Globalization.CultureInfo.InvariantCulture))

                    //.Replace("@dy", (h / 2 + fontSize - fontSize/2 - fontSize/8).ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("@dy", (h / 2 + fontSize / 8).ToString(System.Globalization.CultureInfo.InvariantCulture))



                    .Replace("@CountryName", country_name)

                    .Replace("@FlagCountToolTip", flagCount.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("@FlagCount", flagCount.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    //.Replace("@FlagCount", flagCount.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(3, ' ')  )
                    .Replace("@CountryShort", iso_name)
                    .Replace("@data", b64);

                y += h + 10;
                sb.Append(thisEntry);

            }

            sb.Append(@"
</svg>");
            string str = sb.ToString();

            if(System.StringComparer.OrdinalIgnoreCase.Equals(System.Environment.UserDomainName, "COR"))
                System.IO.File.WriteAllText(@"d:\experimental_img.svg", str, System.Text.Encoding.UTF8);
            else
                System.IO.File.WriteAllText(@"experimental_img.svg", str, System.Text.Encoding.UTF8);


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }

        public static void InsertFlags(string flagDirPath)
        {
            double dimMin = 30.0;


            string[] flags = System.IO.Directory.GetFiles(flagDirPath, "*.png");
            foreach (string thisFlag in flags)
            {
                string country_iso2 = System.IO.Path.GetFileNameWithoutExtension(thisFlag);
                System.Console.WriteLine(country_iso2);
                int w = -1;
                int h = -1;
                int wNew = -1;
                int hNew = -1;
                string b64 = null;

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {

                    using (System.Drawing.Image origImage = System.Drawing.Image.FromFile(thisFlag))
                    {
                        w = origImage.Width;
                        h = origImage.Height;

                        double f = System.Math.Min(dimMin / w, dimMin / h);
                        wNew = (int)System.Math.Round(w * f, 0, System.MidpointRounding.AwayFromZero);
                        hNew = (int)System.Math.Round(h * f, 0, System.MidpointRounding.AwayFromZero);

                        using (System.Drawing.Image img = ResizeImage(origImage, wNew, hNew))
                        {
                            // img.Save("path", System.Drawing.Imaging.ImageFormat.Png);
                            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    } // End Using img 

                    byte[] ba = ms.ToArray();
                    b64 = System.Convert.ToBase64String(ba);
                } // End Using ms 

                System.Console.WriteLine(b64);
                SaveFlagImageInDb(country_iso2, wNew, hNew, "data:image/png;base64," + b64);
            } // Next thisFlag 

        }


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(0, 0, width, height);
            System.Drawing.Bitmap destImage = new System.Drawing.Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (System.Drawing.Imaging.ImageAttributes wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
                } // End Using wrapMode 
            } // End Using graphics 

            return destImage;
        }

    }


}
