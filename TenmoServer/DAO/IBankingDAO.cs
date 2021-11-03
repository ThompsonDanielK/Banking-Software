using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IBankingDAO
    {
        decimal GetBalanceSQL(int id);
        public IEnumerable<User> GetUserListSQL(int id);
    }

}
