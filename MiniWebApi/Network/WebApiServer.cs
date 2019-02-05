using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MiniWebApi.Log;

namespace MiniWebApi.Network
{
    public class WebApiServer:IDisposable
    {
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(true);
        private readonly WebApiRouter _router;
        private HttpListener _listener;
        private bool _disposed;

        public WebApiServer(string applicationName)
        {
            _router = new WebApiRouter(applicationName);
        }

        ~WebApiServer()
        {
            DoDispose();
        }


        public void Start(int port)
        {
            _stopEvent.Reset();
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{port}/");
            _listener.Start();
            Logger.Write($"WebApiServer started on port:{port}");
            while (!_stopEvent.WaitOne(1))
            {
                var context =  _listener.GetContext();
                Task.Run(() =>
                {
                    try
                    {
                        _router.DispatchCall(context);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
            }
        }

        public void Stop()
        {
            _stopEvent.Set();
            _listener.Close();
            Logger.Write($"WebApiServer stopped.");
        }


        private void DoDispose()
        {
            if (_disposed)
            {
                Stop();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            DoDispose();
            GC.SuppressFinalize(this);
        }
    }
}
