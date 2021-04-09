using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Contracts.DTO;
using Shaman.Contract.Bundle;
using Shaman.Contract.Common;
using Shaman.Contract.Common.Logging;
using Shaman.Messages.General.DTO.Events.RepositorySync;
using Shaman.Messages.General.DTO.Requests;
using Shaman.Messages.General.DTO.Responses;
using Shaman.Messages.Handling;
using Shaman.Serialization;
using Shaman.Serialization.Messages.Udp;

namespace Bundle
{
    public class RoomController : IRoomController
    {
        private readonly IShamanLogger _logger;
        private readonly IPlayerManager _playerManager;
        private readonly ISendManager _sendManager;
        private readonly IPositionsManager _positionsManager;
        private readonly ISerializer _serializer;
        public RoomController(IPlayerManager playerManager, ISendManager sendManager, IShamanLogger logger, IPositionsManager positionsManager, ISerializer serializer)
        {
            _playerManager = playerManager;
            _sendManager = sendManager;
            _logger = logger;
            _positionsManager = positionsManager;
            _serializer = serializer;
        }

        public void Dispose()
        {
            _logger.Error($"Room disposed...");
        }

        private void SendPlayersListUpdate()
        {
            _sendManager.SendToAll(new PlayersInGameEvent(_playerManager.GetAll().ToList()));
        }
        
#pragma warning disable 1998
        public async Task<bool> ProcessNewPlayer(Guid sessionId, Dictionary<byte, object> properties)
#pragma warning restore 1998
        {
            try
            {
                var nickName = "";
                if (properties.ContainsKey(PlayerProperties.NickName))
                    nickName = properties[PlayerProperties.NickName].ToString();
            
                _playerManager.AddPlayer(nickName, sessionId);
                _logger.Error($"Player {nickName} added");
                SendPlayersListUpdate();
                return true;
            }
            catch (Exception e)
            {
                _logger.Error($"ProcessNewPlayer error: {e}");
                return false;
            }

        }

        public void ProcessPlayerDisconnected(Guid sessionId, PeerDisconnectedReason reason, Payload reasonPayload)
        {
            _playerManager.RemovePlayer(sessionId);
            SendPlayersListUpdate();
        }

        public bool IsGameFinished()
        {
            return !_playerManager.GetAll().Any();
        }

        public void ProcessMessage(Payload message, DeliveryOptions deliveryOptions, Guid sessionId)
        {
            var operationCode = (byte)MessageBase.GetOperationCode(message.Buffer, message.Offset);

            try
            {
                //process room message
                switch (operationCode)
                {
                    case EventOperationCodes.UpdatePositionEvent:
                        var updatePositionEvent =
                            _serializer.DeserializeAs<UpdatePositionEvent>(message.Buffer, message.Offset,
                                message.Length);
                        var index = _playerManager.GetPlayerIndex(sessionId);
                        if (index == null)
                        {
                            _logger.Error($"ProcessMessage UpdatePositionEvent error: no player with session ID {sessionId}");
                            break;
                        }
                        _positionsManager.UpdatePosition(index.Value, updatePositionEvent.Position);
                        break;
                    default:
                        break;
                }
                    
            }
            catch (Exception ex)
            {
                throw new MessageProcessingException(
                    $"Error processing with code {operationCode}", ex);
            }
        }

        public TimeSpan ForceDestroyRoomAfter { get; }
        public int MaxMatchmakingWeight { get; }
    }
}