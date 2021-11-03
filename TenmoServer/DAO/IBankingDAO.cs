using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.DAO
{
    public interface IBankingDAO
    {
        decimal GetBalanceSQL(int id);
    }
}
