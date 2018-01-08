using System;
using DemoServer.Service;
using GGM.Application;
using System.IO;

namespace DemoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            GGMApplication.Run(typeof(Program), "./application.cfg", args, typeof(MyWebService));
        }
    }
}
