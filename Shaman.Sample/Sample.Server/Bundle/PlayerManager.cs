using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Contracts;
using Shaman.Contract.Common.Logging;

namespace Bundle
{
    public interface IPlayerManager
    {
        void AddPlayer(string name, Guid sessionId);
        void RemovePlayer(Guid sessionId);
        IEnumerable<PlayerData> GetAll();
        byte? GetPlayerIndex(Guid sessionId);
    }
    
    public class PlayerManager : IPlayerManager
    {
        private readonly IShamanLogger _logger;
        private readonly ConcurrentDictionary<byte, PlayerData> _players = new ConcurrentDictionary<byte, PlayerData>();
        private readonly ConcurrentDictionary<Guid, byte> _sessionIds = new ConcurrentDictionary<Guid, byte>(); 
        private byte _index = 0;

        public PlayerManager(IShamanLogger logger)
        {
            _logger = logger;
        }

        private byte GetNextIndex()
        {
            return _index++;
        }
        
        public void AddPlayer(string name, Guid sessionId)
        {
            var index = GetNextIndex();
            _players.TryAdd(index, new PlayerData(index, name));
            _sessionIds.TryAdd(sessionId, index);
        }

        public void RemovePlayer(Guid sessionId)
        {
            if (_sessionIds.TryGetValue(sessionId, out var index))
            {
                _players.TryRemove(index, out _);
                _sessionIds.TryRemove(sessionId, out _);
            }
            else
                _logger.Error($"RemovePlayer error: no player with session ID {sessionId}");

        }

        public IEnumerable<PlayerData> GetAll()
        {
            return _players.Values;
        }

        public byte? GetPlayerIndex(Guid sessionId)
        {
            if (!_sessionIds.TryGetValue(sessionId, out var index))
                return null;

            return index;
        }
    }
}