using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System;
using TenmoClient.Data;
using TenmoServer.Models;

namespace TenmoClient.APIClients
{
    public class BankingService
    {
        private const string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public decimal GetBalance(int id)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}banking/{id}");

            IRestResponse<User> response = client.Get<User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Could not communicate with the server");
                
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Could not get bugs. Error status code {(int)response.StatusCode}");
                
            }

            return response.Data.Balance;
        }
        public List<User> GetUserList(int id)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}banking/{id}/UserList");

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

        public bool SendTransfer(Transfers transfer)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}banking/transfers");
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

        public List<Transfers> GetTransferList(int id)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}banking/{id}/TransferList");

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
    }
}
