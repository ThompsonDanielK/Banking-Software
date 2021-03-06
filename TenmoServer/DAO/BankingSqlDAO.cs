using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    /// <summary>
    /// Handles all database queries for banking
    /// </summary>
    public class BankingSqlDAO : IBankingDAO
    {
        private readonly string connStr;

        /// <summary>
        /// Class constructor that validates connection string
        /// </summary>
        /// <param name="connectionString">connection string</param>
        public BankingSqlDAO(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            this.connStr = connectionString;
        }

        /// <summary>
        /// Retrieves balance from database
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User Account Balance</returns>
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

        /// <summary>
        /// Queries Database for users(aside from logged in user) and adds them to a list
        /// </summary>
        /// <param name="id">Logged in user ID</param>
        /// <returns>List of users</returns>
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

        /// <summary>
        /// Inserting transfers into database
        /// </summary>
        /// <param name="recipientId">ID of transfer recipient</param>
        /// <param name="senderId">ID of transfer sender</param>
        /// <param name="transferAmount">amount of transfer</param>
        /// <returns>Success state</returns>
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

        /// <summary>
        /// Queries database to retrieve account ID based on user ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Account ID</returns>
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

        /// <summary>
        /// Queries database for all transfers related to logged in user and adds them to a list
        /// </summary>
        /// <param name="userId">ID of logged in user</param>
        /// <returns>List of transfers</returns>
        public List<Transfers> GetTransferList(int userId)
        {
            List<Transfers> transferList = new List<Transfers>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    const string retrieveAccountIdSql = "SELECT t.transfer_id, t.transfer_status_id, t.transfer_type_id, t.account_from, t.account_to, " +
                        "t.amount, u.username AS senderUsername, uto.username AS receiverUsername " +
                        "FROM transfers t " +
                        "INNER JOIN accounts a ON a.account_id = t.account_from " +
                        "INNER JOIN users u ON u.user_id = a.user_id " +
                        "INNER JOIN accounts ato ON ato.account_id = t.account_to " +
                        "INNER JOIN users uto ON uto.user_id = ato.user_id " +
                        "WHERE a.user_id = @userId OR ato.user_id = @userId";

                    using (SqlCommand command = new SqlCommand(retrieveAccountIdSql, conn))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Transfers transfers = new Transfers
                                {                                    
                                    TransferId = Convert.ToInt32(reader["transfer_id"]),
                                    TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                                    TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                                    RecipientID = Convert.ToInt32(reader["account_to"]),
                                    SenderId = Convert.ToInt32(reader["account_from"]),
                                    TransferAmount = Convert.ToDecimal(reader["amount"]),
                                    RecipientsUsername = Convert.ToString(reader["receiverUsername"]),
                                    SendersUsername = Convert.ToString(reader["senderUsername"])
                                };

                                if (transfers.SenderId == GetAccountId(userId))
                                {
                                    transfers.TransferType = "To: ";
                                    transfers.Username = Convert.ToString(reader["receiverUsername"]);
                                }
                                else
                                {
                                    transfers.TransferType = "From: ";
                                    transfers.Username = Convert.ToString(reader["senderUsername"]);
                                }

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
