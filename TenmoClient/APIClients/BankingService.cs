using RestSharp;
using RestSharp.Authenticators;
using System;
using TenmoClient.Data;
using TenmoServer.Models;

namespace TenmoClient.APIClients
{
    public class BankingService
    {
        private const string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public string GetBalance(int id)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}banking/{id}");

            IRestResponse<User> response = client.Get<User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                return "Could not communicate with the server";
                
            }

            if (!response.IsSuccessful)
            {
                return $"Could not get bugs. Error status code {(int)response.StatusCode}";
                
            }

            return $"Your current account balance is: {response.Data.Balance.ToString("C")}";
        }
    }
}
