using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Network
{
    public interface INetworkConfiguration
    {
        string RouterUrl { get; }
    }

    public class NetworkConfiguration : INetworkConfiguration
    {
        public string RouterUrl { get; }

        public NetworkConfiguration(string routerUrl)
        {
            RouterUrl = routerUrl;
        }
    }
}