using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

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

        public IEnumerable<User> GetUserListSQL(int id)
        {
            List<User> users = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    const string sql = "SELECT user_id, username " +
                        "FROM users " +
                        "WHERE user_id != @userId";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.Parameters.AddWithValue("@userId", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                User user = new User();

                                user.UserId = Convert.ToInt32(reader["user_id"]);
                                user.Username = Convert.ToString(reader["username"]);

                                users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (SqlException err)
            {
                string getReservationException = "Problem querying database " + err.Message;
            }
            return users;
        }
    }
}
