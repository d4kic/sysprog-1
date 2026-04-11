using System.Net;

namespace zip_server.src.Server
{
    internal class WebServer
    {
        private readonly HttpListener listener = new();
        private readonly RequestQueue queue = new();
        private readonly WorkerPool pool;

        public WebServer(int workerCount)
        {
            listener.Prefixes.Add("http://localhost:5050/");
            pool = new WorkerPool(queue, workerCount);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine($"[{DateTime.Now}] Server radi na http://localhost:5050/");
            pool.Start();

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                queue.EnqueueRequest(context);
            }
        }
    }
}
