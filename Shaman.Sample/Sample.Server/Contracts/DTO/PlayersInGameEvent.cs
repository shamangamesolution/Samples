using System.Collections.Generic;
using Shaman.Serialization;
using Shaman.Serialization.Extensions;
using Shaman.Serialization.Messages.Udp;

namespace Contracts.DTO
{
    public class PlayersInGameEvent : EventBase
    {
        public List<PlayerData> PlayersInGame { get; set; }
        
        public PlayersInGameEvent() : base(EventOperationCodes.PlayersInGameEvent)
        {
        }

        public PlayersInGameEvent(List<PlayerData> playersInGame) : this()
        {
            PlayersInGame = playersInGame;
        }

        protected override void SerializeBody(ITypeWriter typeWriter)
        {
            typeWriter.WriteList(PlayersInGame);
        }

        protected override void DeserializeBody(ITypeReader typeReader)
        {
            PlayersInGame = typeReader.ReadList<PlayerData>();
        }
    }
}