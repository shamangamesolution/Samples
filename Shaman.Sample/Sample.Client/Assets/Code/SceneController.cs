using System;
using System.Threading.Tasks;
using Code.Network;
using Shaman.Client.Peers;
using Shaman.Client.Providers;
using Shaman.Common.Utils.TaskScheduling;
using Shaman.Contract.Common.Logging;
using Shaman.Contract.Routing.Balancing;
using Shaman.Serialization;
using UnityEngine;

namespace Code
{
    public class SceneController : MonoBehaviour
    {
    
        //setup
        public ConnectType ConnectType;

        [Header("Setup in case of Router connect type")]
        public string RouterAddress;
        public ushort RouterHttpPort;
        [Header("Setup in case of StandAloneGame or Debug Server connect type")]
        public string GameServerServerAddress;
        public ushort GameServerUdpPort;
        [Header("Setup in case of Pair connect type")]
        public string PairMatchMakerAddress;
        public ushort PairMatchMakerUdpPort;
    
        //inspector based monobehs
        [Header("MonoBehaviour components")]
        public UnityClientPeer Network;
        public UnityRequestSender RequestSender;

        private IShamanLogger _logger;
        private ISerializer _serializer;
        private IShamanClientPeer _clientPeer;
        private ITaskSchedulerFactory _taskSchedulerFactory;
        private IShamanClientPeerConfig _clientPeerConfig;
        // private IWebRequester _webRequester;
        // private IWebRequesterConfigProvider _webRequesterConfigProvider;
        private IClientServerInfoProvider _serverInfoProvider;
        private IRouterClient _routerClient;
        private INetworkConfiguration _networkConfiguration;
    
        private Guid _inputsSyncHandlerId;
        private float _lastInputRegistered = 0;
    
        // Start is called before the first frame update
        async Task Start()
        {
            //DI initialization(strongly recommended)
            _clientPeerConfig = new UnityClientPeerConfig(20, false, 300, 20);
            _logger = new UnityConsoleLogger();
            _taskSchedulerFactory = new TaskSchedulerFactory(_logger);
            _serializer = new BinarySerializer();
            _clientPeer = new ShamanClientPeer(_logger, _taskSchedulerFactory,_serializer, RequestSender, Network, _clientPeerConfig);
            // _webRequesterConfigProvider = new WebRequesterConfigProviderHardcoded();
            // _webRequester = new WebRequesterSystem(_webRequesterConfigProvider);
            _networkConfiguration = new NetworkConfiguration($"http://{RouterAddress}:{RouterHttpPort}");
            _routerClient = new RouterClient(RequestSender, _logger, _networkConfiguration);
            _serverInfoProvider = new ClientServerInfoProvider(_logger, _routerClient);
        
            //monobehs initialization
            RequestSender.Initialize(_logger, _serializer);
            Network.Initialize(_logger, _clientPeer, _serverInfoProvider, _networkConfiguration);
        
            //register on inputs sync
            // _inputsSyncHandlerId = Network.RegisterEventHandler<SyncInputEvent>(OnInputsIncoming);
            _lastInputRegistered = Time.time;
        
            //join room
            switch (ConnectType)
            {
                case ConnectType.StandAloneGame:
                case ConnectType.DebugServer:
                    await Network.JoinRandomRoom(GameServerServerAddress, GameServerUdpPort); 
                    break;
                case ConnectType.Pair:
                    await Network.JoinGame(PairMatchMakerAddress, PairMatchMakerUdpPort);
                    break;
                case ConnectType.Router:
                    await Network.JoinGameThroughRouter();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // private void OnInputsIncoming(SyncInputEvent eve)
        // {
        //     foreach(var input in eve.Inputs)
        //         _logger.Info($"Input incoming: player {input.Index} input ({input.X}, {input.Y})");
        // }

        private void Update()
        {
            //send inputs 
            if (Time.time - _lastInputRegistered > 0.1 && Network.IsConnectedToRoom)
            {
                // Network.SendEvent(new RegisterInputEvent(1, Input.mousePosition.x, Input.mousePosition.y));
                _lastInputRegistered = Time.time;
            }
        }

        private void OnDestroy()
        {
            Network.UnregisterOperationHandler(_inputsSyncHandlerId);
        }
    }
}
