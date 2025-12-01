using AtConnect.Core.Interfaces;
using System.Collections.Concurrent;

namespace AtConnect.DAL.Data
{
    public class InMemoryUserConnectionManager : IUserConnectionManager
    {
        /// <summary>
        /// Implements thread-safe storage for user connections using a nested ConcurrentDictionary.
        /// 
        /// Using a standard Dictionary of HashSet or even a ConcurrentDictionary of HashSet would require 
        /// full locks to ensure thread safety when adding or removing connections, which can reduce performance.
        /// 
        /// To avoid explicit locking, this implementation uses a ConcurrentDictionary<int, ConcurrentDictionary<string, bool>>,
        /// where the outer dictionary maps user IDs to their connections, and the inner dictionary stores connection IDs as keys 
        /// (the boolean value is ignored). This approach ensures thread-safe add/remove operations without additional locks.
        /// </summary>

        private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, bool>> Connections = new ();
        public void AddConnection(int userId, string connectionId)
        {
            Connections.AddOrUpdate(
             userId,
             _ => new ConcurrentDictionary<string, bool>(new[] { new KeyValuePair<string, bool>(connectionId, true) }),
             (_, existingConnections) => {
                 existingConnections.TryAdd(connectionId, true);
                 return existingConnections;
             });
        }

        public IReadOnlyCollection<string> GetConnections(int userId)
        {
                Connections.TryGetValue(userId, out ConcurrentDictionary<string, bool>? result);
                if (result == null) return new List<string>();

                return result.Keys.ToList();
        }

        public void RemoveConnection(int userId, string connectionId)
        {
                if(Connections.ContainsKey(userId))
                {
                    Connections[userId].TryRemove(connectionId, out _);
                    if(Connections[userId].Count <=1)
                        Connections.TryRemove(userId, out _);
                }
        }
    }
}
