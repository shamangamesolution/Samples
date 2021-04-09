using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Shaman.Client;
using Shaman.Client.Peers;
using Shaman.Client.Providers;
using Shaman.Common.Utils.Logging;
using Shaman.Common.Utils.TaskScheduling;
using Shaman.Contract.Common.Logging;
using Shaman.Messages;
using Shaman.Messages.General.DTO.Requests.Auth;
using Shaman.Messages.General.DTO.Responses.Auth;
using Shaman.Messages.RoomFlow;
using Shaman.Serialization.Messages.Http;
using Shaman.Serialization.Messages.Udp;
using UnityEngine;


namespace Code.Network
{
    public enum ConnectType
    {
        DebugServer,
        Router,
        StandAloneGame,
        Pair
    }
    
    public class UnityClientPeer : MonoBehaviour, IShamanClientPeerListener
    {
        public IShamanLogger Logger { get; set; }
        public IShamanClientPeer ClientPeer { get; set; }
        public IClientServerInfoProvider ServerProvider { get; set; }
        private Route _route;
        private INetworkConfiguration _networkConfiguration;
        [HideInInspector] public Action OnDisconnected;

        public bool IsConnectedToRoom => (ConnectionStatus != null && ConnectionStatus.Status == ShamanClientStatus.InRoom);

        //private for using
        private string _clientVersion = "0.0.1";
        private float _receiveRatePerSec;
        private int _syncersProcessQueuesIntervalMs;
        private object _queueSync = new object();
        private Queue<EventBase> _sendQueue = new Queue<EventBase>();    
        private float _lastSentOn = 0, _lastReceivedOn = 0, _lastQueuesProcessedOn = 0;
        private Guid _sessionId;
        
        private bool _isSelfDisconnect;
    
        //important flow variables
        private ShamanConnectionStatus ConnectionStatus;
    
        public void Initialize(IShamanLogger logger, IShamanClientPeer clientPeer, IClientServerInfoProvider serverInfoProvider, INetworkConfiguration networkConfiguration, float receiveRate = 50, int syncersQueueProcessIntervalMs = 33)
        {
            Logger = logger;
            ClientPeer = clientPeer;
            ServerProvider = serverInfoProvider;
            _networkConfiguration = networkConfiguration;
            _syncersProcessQueuesIntervalMs = syncersQueueProcessIntervalMs;
            _receiveRatePerSec = receiveRate;
            _lastReceivedOn = Time.time;
            _lastQueuesProcessedOn = Time.time;
            ClientPeer.OnDisconnected += (reason) =>
            {
                if (!_isSelfDisconnect)
                    this.Logger.Error($"Disconnected on {ClientPeer.GetStatus()} status. Reason: {reason}");
                OnDisconnected?.Invoke();
            };
        }

        public void OnStatusChanged(ShamanClientStatus prevStatus, ShamanClientStatus newStatus)
        {
            Logger.Info($"Connect status: prev: {prevStatus} new {newStatus}");    
            ConnectionStatus = new ShamanConnectionStatus(newStatus);
        }
        
        public Guid RegisterEventHandler<T>(Action<T> handler, bool callOnce = false)
            where T:MessageBase, new()
        {
            return ClientPeer.RegisterOperationHandler<T>(handler, callOnce);
        }

        public void UnregisterOperationHandler(Guid handlerId)
        {
            ClientPeer.UnregisterOperationHandler(handlerId);
        }
    
        public void SendEvent(EventBase eve)
        {
            lock (_queueSync)
            {
                if (ConnectionStatus != null && ConnectionStatus.Status == ShamanClientStatus.InRoom)
                {
                    ClientPeer.SendEvent(eve);
                }
                else
                {
                    Logger.Error($"SendEvent error: Wrong connection status");
                }
            }
        }

        
        public async Task JoinGameThroughRouter()
        {

#if !UNITY_STANDALONE && !UNITY_EDITOR
        _clientVersion = Application.version;
#endif

            var routes = await ServerProvider.GetRoutes(_networkConfiguration.RouterUrl, _clientVersion);
            if (routes == null || routes.Count == 0)
            {
                Logger.Error($"No matchmakers received");
                return;
            }
            else
            {
                //TODO - choose route based on ping here
                _route = routes.FirstOrDefault();
                await JoinGame(_route.MatchMakerAddress, _route.MatchMakerPort);
            }
        }
        
        public void Disconnect()
        {
            _isSelfDisconnect = true;
            ClientPeer.Disconnect();
        }
        
        public async Task SendRequest<T>(RequestBase request, Action<T> callback) where T:ResponseBase, new()
        {
            request.SessionId = _sessionId;
            var response = await ClientPeer.SendRequest<T>(request);
            callback(response);
        }

        public async Task<T> SendRequest<T>(HttpRequestBase request, string url) where T:HttpResponseBase, new()
        {
            request.SessionId = _sessionId;
            return await ClientPeer.SendWebRequest<T>(url, request);
        }
        
        public async Task<T> SendRequest<T>(RequestBase request) where T:ResponseBase, new()
        {
            request.SessionId = _sessionId;
            return await ClientPeer.SendRequest<T>(request);
        }
    
        public async Task<JoinInfo> JoinGame(string matchMakerAddress, ushort matchMakerPort)
        {

            var matchMakingProperties = new Dictionary<byte, object>
            {
            };
            var joinGameProperties = new Dictionary<byte, object>
            {
                {PlayerProperties.NickName, "Player_123"}
            };

            //reset manual disconnect flag
            _isSelfDisconnect = false;

            return await ClientPeer.JoinGame(matchMakerAddress, matchMakerPort,  _sessionId, 
                matchMakingProperties, joinGameProperties);
        }
        
        public async Task<JoinInfo> JoinRandomRoom(string gameServerAddress, ushort gameServerPort)
        {
            //reset manual disconnect flag
            _isSelfDisconnect = false;

            var roomProperties = new Dictionary<byte, object>();
            var joinGameProperties = new Dictionary<byte, object> {{PlayerProperties.NickName, "Player_123"}};

            return await ClientPeer.DirectConnectToGameServerToRandomRoom(gameServerAddress, gameServerPort, _sessionId, roomProperties, joinGameProperties);
        }

        public int GetPing()
        {
            return ClientPeer.GetPing();
        }

        private void Start()
        {
            _sessionId = Guid.NewGuid();
        }
        

        private void FixedUpdate()
        {
            
            if (Time.time - _lastReceivedOn > 1 / _receiveRatePerSec)
            {
                try
                {
                    if (ClientPeer.GetStatus() == ShamanClientStatus.InRoom)
                        Logger.Debug($"Start processing messages. (Current queue size {ClientPeer.GetMessagesCountInQueue()}, CurrentPing {ClientPeer.GetPing()})");
                    ClientPeer.ProcessMessages();
                }
                catch (Exception e)
                {
                    Logger.Error($"Process messages error: {e}");
                }
                finally
                {
                    _lastReceivedOn = Time.time;
                }
            }
        }
    
        private void OnDestroy()
        {
            if (ClientPeer.GetStatus() != ShamanClientStatus.Offline)
                ClientPeer?.Disconnect();
        }

    }
}
