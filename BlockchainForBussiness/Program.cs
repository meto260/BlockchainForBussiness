using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BlockchainForBussiness {
    class Program {

        HttpListener http = new HttpListener();
        ServiceHeplers _helper = new ServiceHeplers();
        static void Main(string[] args) {
            Program p = new Program();
            p.Prepare();
            p.Run();
        }

        void Run() {
            http.Start();
            while (true) {
                HttpListenerContext context = http.GetContext();
                Worker(ref context);
            }
        }

        void Prepare() {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true);

            var appconfig = builder.Build();
            appconfig.GetSection("http_prefixes")
                .AsEnumerable()
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => x.Value)
                .ToList()
                .ForEach(p => {
                    http.Prefixes.Add(p);
                    Console.WriteLine("--- Listening : " + p);
                });

            string ChainName = appconfig.GetSection("blockchain_name").Value;
            string ChainDescription = appconfig.GetSection("blockchain_description").Value;

            Console.WriteLine();
            Console.WriteLine($"-------------{ChainName} Engine Started----------------");
            Console.WriteLine();
            Console.WriteLine("   " + ChainDescription);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine();
        }

        void Worker(ref HttpListenerContext context) {
            HttpListenerRequest Request = context.Request;
            HttpListenerResponse Response = context.Response;
            string requestText = "";
            string responseText = "";
            if (Request.HttpMethod.Equals("GET")) {
                requestText = Request.UrlReferrer?.AbsolutePath ?? Request.RawUrl;
                Console.WriteLine(DateTimeOffset.UtcNow + " - " + requestText);
                responseText = _helper.PushToWork(Utils.ClearUrlParams(requestText));
            } else if (Request.HttpMethod.Equals("POST")) {
                StreamReader stream = new StreamReader(Request.InputStream);
                requestText = stream.ReadToEnd();
                Console.WriteLine(DateTimeOffset.UtcNow + " - " + requestText);
                responseText = _helper.PushToWork(requestText);
            } else {
                responseText = DateTimeOffset.UtcNow.ToString() + " - " + requestText;
            }
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);
            Response.AddHeader("Content-Type", "application/json ; charset=utf-8");
            Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.Close();
        }
    }
}
