using System.Collections.Generic;
using Contracts.Extensions;
using Shaman.Serialization;
using Shaman.Serialization.Extensions;
using Shaman.Serialization.Messages.Udp;

namespace Contracts.DTO
{
    public class PositionSyncEvent : EventBase
    {
        public Dictionary<byte, Position> PlayersPositions { get; set; }
        
        public PositionSyncEvent() : base(EventOperationCodes.PositionSyncEvent)
        {
        }

        public PositionSyncEvent(Dictionary<byte, Position> playersPositions) : this()
        {
            PlayersPositions = playersPositions;
        }

        protected override void SerializeBody(ITypeWriter typeWriter)
        {
            typeWriter.Write(PlayersPositions);
        }

        protected override void DeserializeBody(ITypeReader typeReader)
        {
            PlayersPositions = typeReader.ReadByteEntityDictionary();
        }
    }
}