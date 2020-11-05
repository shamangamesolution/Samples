using System.Collections.Generic;
using Bundle;
using Microsoft.Extensions.DependencyInjection;
using Shaman.Common.Server.Configuration;
using Shaman.Contract.Bundle;
using Shaman.Contract.Common.Logging;
using Shaman.Launchers.Game.DebugServer;

namespace DebugServer
{
    
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ApplicationConfig()
            {
                ServerName = "TestGame",
                Region = "SomeRegion",
                PublicDomainNameOrAddress = "localhost",
                ListenPorts = new List<ushort> {23452},
                BindToPortHttp = 7005,
                SocketTickTimeMs = 100,
                ReceiveTickTimeMs = 33,
                SendTickTimeMs = 50,
                MaxPacketSize = 300,
                BasePacketBufferSize = 64,
                IsAuthOn = false,
                SocketType = SocketType.BareSocket
            };
            
            var result = StandaloneServerLauncher.Launch(new MyGameResolver(), args, config, "0.0.0.0", "Error");
            result.ServerTask.Wait();

        }
    }
}