using System;
using MiniWebApi.Network;

namespace TestWebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new WebApiServer("api");
            server.Start(8090);
            Console.ReadLine();
            server.Dispose();
        }
    }
}
