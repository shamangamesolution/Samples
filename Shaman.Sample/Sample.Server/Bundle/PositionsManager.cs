using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Contracts;
using Contracts.DTO;
using Shaman.Contract.Bundle;
using Shaman.Contract.Common;
using Shaman.Contract.Common.Logging;

namespace Bundle
{
    public interface IPositionsManager
    {
        void UpdatePosition(byte playerIndex, Position position);
        void Start(int positionsSyncIntervalMs = 20);
        void Stop();
    }
    
    public class PositionsManager : IPositionsManager
    {
        private readonly ITaskScheduler _taskScheduler;
        private readonly ISendManager _sendManager;
        private readonly IBundleConfig _bundleConfig;
        private readonly IShamanLogger _logger;
        private readonly ConcurrentDictionary<byte, Position> _positions = new ConcurrentDictionary<byte, Position>();
        
        private IPendingTask _syncTask;
        private object _mutex = new object();
        
        public PositionsManager(ITaskScheduler taskScheduler, ISendManager sendManager, IBundleConfig bundleConfig, IShamanLogger logger)
        {
            _taskScheduler = taskScheduler;
            _sendManager = sendManager;
            _bundleConfig = bundleConfig;
            _logger = logger;
        }

        public void UpdatePosition(byte playerIndex, Position position)
        {
            lock (_mutex)
            {
                _positions.AddOrUpdate(playerIndex, position, (b, position1) => position);
            }
        }

        private void Tick()
        {
            lock (_mutex)
            {
                _sendManager.SendToAll(new PositionSyncEvent(new Dictionary<byte, Position>(_positions)));
            }
        }
        
        public void Start(int positionsSyncIntervalMs = 20)
        {
            var parameter = _bundleConfig.GetValueOrNull("PositionsSyncIntervalMs");
            if (string.IsNullOrWhiteSpace(parameter))
            {
                _logger.Error($"No parameter PositionsSyncIntervalMs in bundle config. Using {positionsSyncIntervalMs} ms as default");
            }
            else
                positionsSyncIntervalMs = Convert.ToInt32(parameter);
            _syncTask = _taskScheduler.ScheduleOnInterval(Tick, 0, positionsSyncIntervalMs);
        }

        public void Stop()
        {
            lock (_mutex)
            {
                _taskScheduler.Remove(_syncTask);
            }
        }
    }
}