using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.Core.Interfaces
{
    public interface IUserConnectionManager
    {
        public void AddConnection(int userId, string connectionId);
        public void RemoveConnection(int userId, string connectionId);
        public IReadOnlyCollection<string> GetConnections(int userId);
    }
}
