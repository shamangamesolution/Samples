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
            
            return new RoomController(_logger);
        }
    }
}