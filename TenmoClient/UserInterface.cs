using System;
using System.Collections.Generic;
using TenmoClient.APIClients;
using TenmoClient.Data;
using TenmoServer.Models;

namespace TenmoClient
{
    public class UserInterface
    {
        private readonly ConsoleService consoleService = new ConsoleService();
        private readonly AuthService authService = new AuthService();
        private readonly BankingService bankingService = new BankingService();
        
        private bool quitRequested = false;

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
                            //Console.WriteLine("NOT IMPLEMENTED!"); // TODO: Implement me
                            break;

                        case 2: // View Past Transfers
                            ListTransfers();
                            break;

                        case 3: // View Pending Requests
                            Console.WriteLine("NOT IMPLEMENTED!"); // TODO: Implement me
                            break;

                        case 4: // Send TE Bucks
                            ListUsers();
                            int userId = UserInputId();
                            if (userId != 0)
                            {
                                decimal transferAmount = UserInputAmount();

                                Transfers transfer = new Transfers();
                                transfer.senderId = UserService.UserId;
                                transfer.recipientID = userId;
                                transfer.transferAmount = transferAmount;

                                bool success = bankingService.SendTransfer(transfer);

                                if(!success)
                                {
                                    Console.WriteLine("We could not complete your request");
                                }
                                else
                                {
                                    Console.WriteLine("Transfer Completed.");
                                }
                            }
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

        private void HandleUserLogin()
        {
            while (!UserService.IsLoggedIn) //will keep looping until user is logged in
            {
                LoginUser loginUser = consoleService.PromptForLogin();
                authService.Login(loginUser);
            }
        }

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
                else if (bankingService.GetBalance(UserService.UserId) < transferAmount)
                {
                    Console.WriteLine("You do not have sufficient funds to make this transfer.");

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

        private void ListTransfers()
        {
            List<Transfers> transferList = bankingService.GetTransferList(UserService.UserId);

            if (transferList.Count < 1)
            {
                Console.WriteLine("We could not complete your request");
                return;
            }

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine($"{"ID",-10}{"From/To",-20}{"Amount", -10}");
            Console.WriteLine("-------------------------------------------");

            foreach (Transfers transfers in transferList)
            {
                Console.WriteLine($"{transfers.transferId,-10}{transfers.transferType + transfers.username,-20}{transfers.transferAmount.ToString("C"),-10}");
            }

            Console.WriteLine("---------");
        }
    }
}
