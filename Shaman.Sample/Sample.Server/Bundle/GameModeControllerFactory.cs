using Shaman.Contract.Bundle;
using Shaman.Contract.Common;
using Shaman.Contract.Common.Logging;
using Shaman.Serialization;

namespace Bundle
{
    public class GameModeControllerFactory :IRoomControllerFactory
    {
        private readonly IShamanLogger _logger;
        private readonly ISerializer _serializer;
        private readonly IBundleConfig _bundleConfig;
        
        public GameModeControllerFactory(IShamanLogger logger, ISerializer serializer, IBundleConfig bundleConfig)
        {
            _logger = logger;
            _serializer = serializer;
            _bundleConfig = bundleConfig;
        }


        public IRoomController GetGameModeController(IRoomContext room, ITaskScheduler taskScheduler,
            IRoomPropertiesContainer roomPropertiesContainer)
        {
            
            var playerManager = new PlayerManager(_logger);
            var sendManager = new SendManager(room, _serializer);
            var positionsManager = new PositionsManager(taskScheduler, sendManager, _bundleConfig, _logger);
            return new RoomController(playerManager, sendManager, _logger, positionsManager,_serializer);
        }
    }
}