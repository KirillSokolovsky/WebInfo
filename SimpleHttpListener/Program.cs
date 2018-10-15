namespace SimpleHttpListener
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            var ts = new CancellationTokenSource();
            var task = Listen(ts.Token);

            Console.WriteLine("Press Enter to start quiting");
            Console.ReadLine();

            ts.Cancel();
        }

        private static async Task Listen(CancellationToken cancellationToken)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8899/");
            listener.Start();
            Console.WriteLine("Waiting for connections...");

            while (!cancellationToken.IsCancellationRequested)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                Console.WriteLine("===================");
                Console.WriteLine("==== Connected ====");
                Console.WriteLine("===================");
                HttpListenerRequest request = context.Request;
                Console.WriteLine($"{request.HttpMethod} {request.Url}");
                Console.WriteLine("===== HEADERS =====");

                foreach (var k in request.Headers.AllKeys)
                {
                    Console.WriteLine($"{k}: {request.Headers[k]}");
                }

                var origins = request.Headers["Oriring"];

                if (request.HttpMethod == "POST")
                {
                    using (var sr = new StreamReader(request.InputStream))
                    {
                        var content = await sr.ReadToEndAsync();
                        Console.WriteLine("Content:");
                        Console.WriteLine(content);
                        Console.WriteLine("===================");
                    }
                }

                HttpListenerResponse response = context.Response;

                string responseString = "OK";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;

                response.AddHeader("Access-Control-Allow-Credentials", "true");
                response.AddHeader("Access-Control-Allow-Headers", "X-PINGOTHER, Content-Type, Authorization, Content-Length, X-Requested-With");
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
                response.AddHeader("Access-Control-Allow-Origin", origins ?? "null");

                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }
    }
}
