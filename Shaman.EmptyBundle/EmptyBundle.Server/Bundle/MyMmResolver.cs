using System.Collections.Generic;
using Shaman.Contract.MM;
using Shaman.Messages;

namespace Bundle
{
    public class RoomPropertiesProvider : IRoomPropertiesProvider
    {
        public int GetMatchMakingTick(Dictionary<byte, object> playerMatchMakingProperties)
        {
            return 250;
        }

        public int GetMaximumPlayers(Dictionary<byte, object> playerMatchMakingProperties)
        {
            return 20;
        }

        public int GetMaximumMatchMakingWeight(Dictionary<byte, object> playerMatchMakingProperties)
        {
            return 1;
        }

        public int GetMaximumMatchMakingTime(Dictionary<byte, object> playerMatchMakingProperties)
        {
            return 0;
        }

        public Dictionary<byte, object> GetAdditionalRoomProperties(Dictionary<byte, object> playerMatchMakingProperties)
        {
            var result = new Dictionary<byte, object>();
            return result;
        }
    }
    
    public class MyMmResolver : IMmResolver
    {
        public void Configure(IMatchMakingConfigurator matchMaker)
        {
        }

        public IRoomPropertiesProvider GetRoomPropertiesProvider()
        {
            return new RoomPropertiesProvider();
        }
    }
}