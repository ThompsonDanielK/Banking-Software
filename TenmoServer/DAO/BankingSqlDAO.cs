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

        public bool PostTransferSQL(int recipientId, int senderId, decimal transferAmount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    const string transferSql = "INSERT INTO transfers " +
                        "(transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                        "VALUES (@transferTypeId, @transferStatusId, @accountFrom, @accountTo, @amount); " +
                        "UPDATE accounts SET balance = balance - @amount " +
                        "WHERE user_id = @senderId; " +
                        "UPDATE accounts SET balance = balance + @amount " +
                        "WHERE user_id = @recipientId;";

                    using (SqlCommand command = new SqlCommand(transferSql, conn))
                    {
                        command.Parameters.AddWithValue("@transferTypeId", 1001);
                        command.Parameters.AddWithValue("@transferStatusId", 2002);
                        command.Parameters.AddWithValue("@accountFrom", GetAccountId(senderId));
                        command.Parameters.AddWithValue("@accountTo", GetAccountId(recipientId));
                        command.Parameters.AddWithValue("@amount", transferAmount);
                        command.Parameters.AddWithValue("@senderId", senderId);
                        command.Parameters.AddWithValue("@recipientId", recipientId);
                        int returnedRows = command.ExecuteNonQuery();

                        if (returnedRows == 0)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (SqlException err)
            {
                Console.WriteLine("Problem querying database " + err.Message);
            }
            return true;
        }

        private int GetAccountId(int id)
        {
            int accountId = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    const string retrieveAccountIdSql = "SELECT account_id " +
                        "FROM accounts " +
                        "WHERE user_id = @userId";

                    using (SqlCommand command = new SqlCommand(retrieveAccountIdSql, conn))
                    {
                        command.Parameters.AddWithValue("@userId", id);

                        accountId = Convert.ToInt32(command.ExecuteScalar());


                    }
                }
            }
            catch (SqlException err)
            {
                Console.WriteLine("Problem querying database " + err.Message);
            }
            return accountId;
        }

        public List<Transfers> GetTransferList(int userId)
        {
            List<Transfers> transferList = new List<Transfers>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    const string retrieveAccountIdSql = "SELECT t.transfer_id, t.transfer_type_id, t.account_from, t.account_to, t.amount, u.username " +
                        "FROM transfers t INNER JOIN accounts a ON a.account_id = t.account_from OR a.account_id = t.account_to " +
                        "INNER JOIN users u ON u.user_id = a.user_id " +
                        "WHERE a.user_id = @userId;";

                    using (SqlCommand command = new SqlCommand(retrieveAccountIdSql, conn))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Transfers transfers = new Transfers
                                {
                                    username = Convert.ToString(reader["username"]),
                                    transferId = Convert.ToInt32(reader["transfer_id"]),
                                    transferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                                    recipientID = Convert.ToInt32(reader["account_to"]),
                                    senderId = Convert.ToInt32(reader["account_from"]),
                                    transferAmount = Convert.ToDecimal(reader["amount"])
                                };

                                transferList.Add(transfers);

                            }
                        }
                    }
                }
            }
            catch (SqlException err)
            {
                Console.WriteLine("Problem querying database " + err.Message);
            }
            return transferList;
        }

    }
}
