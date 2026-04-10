using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace zip_server.src.Server
{
    internal class WorkerPool
    {
        private readonly int workerCount;
        private readonly RequestQueue? queue;

        public WorkerPool(RequestQueue queue, int workerCount)
        {
            this.queue = queue;
            this.workerCount = workerCount;
        }

        public void Start()
        {
            for (int i = 0; i < workerCount; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    HttpListenerContext? context = queue?.DequeueRequest();
                    RequestHandler.HandleRequest(context);
                });
            }
        }
    }
}
