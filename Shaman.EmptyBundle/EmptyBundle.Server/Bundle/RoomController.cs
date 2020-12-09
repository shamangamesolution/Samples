using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public RoomController(IShamanLogger logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            _logger.Error($"Room disposed");
        }

        
        public async Task<bool> ProcessNewPlayer(Guid sessionId, Dictionary<byte, object> properties)
        {
            _logger.Error($"Player joined to room");
            return true;
        }

        public void ProcessPlayerDisconnected(Guid sessionId, PeerDisconnectedReason reason, byte[] reasonPayload)
        {
            _logger.Error($"Player left room");
        }

        public bool IsGameFinished()
        {
            return false;
        }

        public void ProcessMessage(Payload message, DeliveryOptions deliveryOptions, Guid sessionId)
        {
        }

        public TimeSpan ForceDestroyRoomAfter { get; }
        public int MaxMatchmakingWeight { get; }
    }
}