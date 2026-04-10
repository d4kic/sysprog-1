using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace zip_server.Server
{
    internal class WebServer
    {
        readonly HttpListener listener = new HttpListener();

        public WebServer()
        {
            listener.Prefixes.Add("http://localhost:5050/");
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Server radi na http://localhost:5050/");
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                ThreadPool.QueueUserWorkItem(RequestHandler.HandleRequest, context);
            }
        }
    }
}
