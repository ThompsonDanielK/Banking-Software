using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System;
using TenmoClient.Data;
using TenmoServer.Models;

namespace TenmoClient.APIClients
{
    /// <summary>
    /// Handles all banking related requests to API
    /// </summary>
    public class BankingService
    {
        private const string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public BankingService()
        {
            
        }

        /// <summary>
        /// Sends Request to API to retrieve User Balance
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User Balance</returns>
        public decimal GetBalance(int id)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}banking/{id}");
            request.AddHeader("Authorization", $"bearer {UserService.Token}");

            IRestResponse<User> response = client.Get<User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not communicate with the server");
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Could not get balance. Error status code {(int)response.StatusCode}");
                
            }

            return response.Data.Balance;
        }

        /// <summary>
        /// Sends Request to API to retrieve list of users
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User list data</returns>
        public List<User> GetUserList(int id)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}banking/{id}/UserList");
            request.AddHeader("Authorization", $"bearer {UserService.Token}");

            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not communicate with the server");
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Could not get users. Error status code {(int)response.StatusCode}");
            }

            return response.Data;
        }

        /// <summary>
        /// Sends Request to API to send money transfer
        /// </summary>
        /// <param name="transfer">Transfer ID</param>
        /// <returns>Success State</returns>
        public bool SendTransfer(Transfers transfer)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}banking/transfers");
            request.AddHeader("Authorization", $"bearer {UserService.Token}");
            request.AddJsonBody(transfer);

            IRestResponse<Transfers> response = client.Post<Transfers>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not communicate with the server");
                return false;
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Could not transfer. Error status code {(int)response.StatusCode}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sends Request to API to retrieve list of transfers
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>List of transfers</returns>
        public List<Transfers> GetTransferList(int id)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}banking/{id}/TransferList");
            request.AddHeader("Authorization", $"bearer {UserService.Token}");

            IRestResponse<List<Transfers>> response = client.Get<List<Transfers>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not communicate with the server");
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Could not get transfers. Error status code {(int)response.StatusCode}");
            }

            return response.Data;
        }

        /// <summary>
        /// Goes through List of transfers and retrieves user requested transfer details
        /// </summary>
        /// <param name="transferList">List of transfers</param>
        /// <param name="transferId">User requested transfer</param>
        /// <returns></returns>
        public Transfers GetTransferDetails(List<Transfers> transferList, int transferId)
        {
            foreach (Transfers transfer in transferList)
            {
                if(transfer.transferId == transferId)
                {
                    return transfer;
                }
            }
            return new Transfers();
        }
    }
}
