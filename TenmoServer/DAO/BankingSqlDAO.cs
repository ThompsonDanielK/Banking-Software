using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.DAO
{
    public class BankingSqlDAO : IBankingDAO
    {
        private readonly string connStr;
        public BankingSqlDAO(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            this.connStr = connectionString;
        }

        public decimal GetBalanceSQL(int id)
        {
            decimal balance;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                const string sql = "SELECT a.balance " +
                    "FROM users u INNER JOIN accounts a ON a.user_id = u.user_id " +
                    "WHERE a.user_id = @userId";
                
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@userId", id);

                    balance = Convert.ToDecimal(command.ExecuteScalar());
                }
            }

            return balance;
        }

    }
}
