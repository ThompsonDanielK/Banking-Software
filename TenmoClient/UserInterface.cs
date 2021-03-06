using System;
using System.Collections.Generic;
using TenmoClient.APIClients;
using TenmoClient.Data;
using TenmoServer.Models;

namespace TenmoClient
{
    /// <summary>
    /// Handles user input and display
    /// </summary>
    public class UserInterface
    {
        private readonly ConsoleService consoleService = new ConsoleService();
        private readonly AuthService authService = new AuthService();
        private readonly BankingService bankingService = new BankingService();
        
        private bool quitRequested = false;

        /// <summary>
        /// Displays Main Menu
        /// </summary>
        public void Start()
        {
            while (!quitRequested)
            {
                while (!UserService.IsLoggedIn)
                {
                    ShowLogInMenu();
                }

                // If we got here, then the user is logged in. Go ahead and show the main menu
                ShowMainMenu();
            }
        }

        /// <summary>
        /// Displays log in menu
        /// </summary>
        private void ShowLogInMenu()
        {
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.Write("Please choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int loginRegister))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
            }
            else if (loginRegister == 1)
            {
                HandleUserLogin();
            }
            else if (loginRegister == 2)
            {
                HandleUserRegister();
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        /// <summary>
        ///  Displays main menu
        /// </summary>
        private void ShowMainMenu()
        {
            int menuSelection;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else
                {
                    switch (menuSelection)
                    {
                        case 1: // View Balance
                            Console.WriteLine($"Your current account balance is: {bankingService.GetBalance(UserService.UserId).ToString("C")}");
                            break;

                        case 2: // View Past Transfers
                            List<Transfers> transferList = ListTransfers();
                            int transferId = GetTransferId();
                            ListTransfersDetails(bankingService.GetTransferDetails(transferList, transferId));
                            break;

                        case 3: // View Pending Requests
                            Console.WriteLine("NOT IMPLEMENTED!"); // TODO: Implement me
                            break;

                        case 4: // Send TE Bucks
                            ListUsers();
                            SendTransfer();
                            break;

                        case 5: // Request TE Bucks
                            Console.WriteLine("NOT IMPLEMENTED!"); // TODO: Implement me
                            break;

                        case 6: // Log in as someone else
                            Console.WriteLine();
                            UserService.ClearLoggedInUser(); //wipe out previous login info
                            return; // Leaves the menu and should return as someone else

                        case 0: // Quit
                            Console.WriteLine("Goodbye!");
                            quitRequested = true;
                            return;

                        default:
                            Console.WriteLine("That doesn't seem like a valid choice.");
                            break;
                    }
                }
            } while (menuSelection != 0);
        }

        /// <summary>
        /// Records user input and sends transfer
        /// </summary>
        private void SendTransfer()
        {
            int userId = UserInputId();
            if (userId != 0)
            {
                decimal transferAmount = UserInputAmount();                

                Transfers transfer = new Transfers();
                transfer.SenderID = UserService.UserId;
                transfer.RecipientID = userId;
                transfer.TransferAmount = transferAmount;

                bool success = bankingService.SendTransfer(transfer);

                if (!success)
                {
                    Console.WriteLine("We could not complete your request");
                }               
                else if (transferAmount == 0M)
                {
                    Console.WriteLine("Transfer terminated.");
                }
                else
                {
                    Console.WriteLine("Transfer Completed.");
                }
            }
        }

        /// <summary>
        /// Records and Validates user input for transfer Id
        /// </summary>
        /// <returns>Transfer Id</returns>
        private int GetTransferId()
        {
            int transferId = 0;
            bool transferIdLoop = true;
            while (transferIdLoop)
            {
                Console.Write("Please enter transfer ID to view details (0 to cancel): ");
                string inputTransferId = Console.ReadLine();

                bool isNumber = int.TryParse(inputTransferId, out transferId);
                if (!isNumber)
                {
                    Console.WriteLine("Please enter a Transfer ID number!");
                }
                else if (transferId == 0)
                {
                    return transferId;
                }
                else
                {
                    transferIdLoop = false;
                }
            }
            return transferId;
        }

        /// <summary>
        /// Takes user input for account registration
        /// </summary>
        private void HandleUserRegister()
        {
            bool isRegistered = false;

            while (!isRegistered) //will keep looping until user is registered
            {
                LoginUser registerUser = consoleService.PromptForLogin();
                isRegistered = authService.Register(registerUser);
            }

            Console.WriteLine("");
            Console.WriteLine("Registration successful. You can now log in.");
        }

        /// <summary>
        /// Handles user login
        /// </summary>
        private void HandleUserLogin()
        {
            while (!UserService.IsLoggedIn) //will keep looping until user is logged in
            {
                LoginUser loginUser = consoleService.PromptForLogin();
                authService.Login(loginUser);
            }
        }

        /// <summary>
        /// Validates and displays a list of users for transfers
        /// </summary>
        private void ListUsers()
        {
            List<User> userList = bankingService.GetUserList(UserService.UserId);

            if (userList.Count < 1)
            {
                Console.WriteLine("We could not complete your request");
                return;
            }

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Users");
            Console.WriteLine($"{"ID",-10}{"Name",-20}");
            Console.WriteLine("-------------------------------------------");

            foreach (User user in userList)
            {
                Console.WriteLine($"{user.UserId,-10}{user.Username,-20}");
            }

            Console.WriteLine("---------");
        }
        
        /// <summary>
        /// Records user input for transfer amount
        /// </summary>
        /// <returns>Transfer Amount</returns>
        private decimal UserInputAmount()
        {
            decimal transferAmount;
            bool transferAmountLoop = true;
            transferAmount = 0M;

            while (transferAmountLoop)
            {

                Console.Write("Please enter amount of transfer: ");
                string inputTransferAmount = Console.ReadLine();

                bool isDecimal = decimal.TryParse(inputTransferAmount, out transferAmount);
                if (!isDecimal)
                {
                    Console.WriteLine("Please enter a decimal amount!");

                }
                else if (bankingService.GetBalance(UserService.UserId) < transferAmount || (bankingService.GetBalance(UserService.UserId) - transferAmount) <= 0)
                {
                    Console.WriteLine("You do not have sufficient funds to make this transfer.");
                    transferAmountLoop = false;
                    transferAmount = 0M;

                }
                else if (transferAmount <= 0M)
                {
                    Console.WriteLine("Please enter a valid transfer amount");

                }
                else
                {
                    transferAmountLoop = false;
                }
            }

            return transferAmount;
        }

        /// <summary>
        /// Records user input user ID for transfer
        /// </summary>
        /// <returns>User ID</returns>
        private int UserInputId()
        {
            int userId = 0;
            bool transferIdLoop = true;
            while (transferIdLoop)
            {
                Console.Write("Please enter User ID for transfer (0 to cancel): ");
                string inputUserId = Console.ReadLine();

                bool isNumber = int.TryParse(inputUserId, out userId);
                if (!isNumber)
                {
                    Console.WriteLine("Please enter a User ID number!");
                }
                else if (userId == 0)
                {
                    return userId;
                }
                else
                {
                    foreach (User user in bankingService.GetUserList(UserService.UserId))
                    {
                        if (user.UserId == userId)
                        {
                            return userId;
                        }
                    }
                }               
            }

            return userId;
        }

        /// <summary>
        /// Displays a list of past transfers to user
        /// </summary>
        /// <returns></returns>
        private List<Transfers> ListTransfers()
        {
            List<Transfers> transferList = bankingService.GetTransferList(UserService.UserId);

            if (transferList.Count < 1)
            {
                Console.WriteLine("We could not complete your request");
                return transferList;
            }

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine($"{"ID",-10}{"From/To",-20}{"Amount", -10}");
            Console.WriteLine("-------------------------------------------");

            foreach (Transfers transfers in transferList)
            {
                Console.WriteLine($"{transfers.TransferId,-10}{transfers.TransferType + transfers.Username,-20}{transfers.TransferAmount.ToString("C"),-10}");
            }

            Console.WriteLine("---------");

            return transferList;
        }

        /// <summary>
        /// Displays Transfer Details
        /// </summary>
        /// <param name="transfer"></param>
        private void ListTransfersDetails(Transfers transfer)
        {
            if(transfer.TransferId == 0)
            {
                return;
            }

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Transfer Details");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Id: {transfer.TransferId}");
            Console.WriteLine($"From: {transfer.SendersUsername}");
            Console.WriteLine($"To: {transfer.RecipientsUsername}");
            Console.WriteLine($"Type: {transfer.TransferTypeDetails}");
            Console.WriteLine($"Status: {transfer.TransferStatus}");
            Console.WriteLine($"Amount: {transfer.TransferAmount.ToString("C")}");
        }
    }
}
