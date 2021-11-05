using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    /// <summary>
    /// Handles all external banking requests
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class BankingController : ControllerBase
    {
        private readonly IBankingDAO banking;

        /// <summary>
        /// Class constructor that assists with dependency injection
        /// </summary>
        /// <param name="banking">Banking Interface Object</param>
        public BankingController(IBankingDAO banking)
        {
            this.banking = banking;
        }

        /// <summary>
        /// Handles external requests to get balance of logged in user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>JSON object containing balance</returns>
        [HttpGet("{id}")]
        public ActionResult GetBalance(int id)
        {
            decimal balance = banking.GetBalanceSQL(id);
            int userId = int.Parse(this.User.FindFirst("sub").Value);

            if (id != userId)
            {
                return Forbid();
            }

            User user = new User();
            user.Balance = balance;

            return Ok(user);
        }

        /// <summary>
        /// Handles external requests to get list of users aside from logged in user
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>JSON objects containing other users</returns>
        [HttpGet("{id}/UserList")]
        public ActionResult GetUserList(int id)
        {
            IEnumerable<User> users = banking.GetUserListSQL(id);

            int userId = int.Parse(this.User.FindFirst("sub").Value);

            if (id != userId)
            {
                return Forbid();
            }

            return Ok(users);
        }

        /// <summary>
        /// Handles external requests to transfer money to other users
        /// </summary>
        /// <param name="transfer">Transfer Object containing details of transfer</param>
        /// <returns>Status code and original JSON object request</returns>
        [HttpPost("transfers")]
        public ActionResult PostTransfer(Transfers transfer)
        {
            int userId = int.Parse(this.User.FindFirst("sub").Value);

            if (transfer.SenderId != userId)
            {
                return Forbid();
            }

            bool success = banking.PostTransferSQL(transfer.RecipientID, transfer.SenderId, transfer.TransferAmount);

            if(success)
            {
                return Ok(transfer);
            }

            return BadRequest(new { message = "This request could not be completed." });
        }

        /// <summary>
        /// Handles external requests to get list of transfers related to logged in user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Status code and lists JSON objects containing transfer information</returns>
        [HttpGet("{id}/TransferList")]
        public ActionResult GetTransferList(int id)
        {
            List<Transfers> transfers = banking.GetTransferList(id);

            int userId = int.Parse(this.User.FindFirst("sub").Value);

            if (id != userId)
            {
                return Forbid();
            }

            return Ok(transfers);
        }
    }
}
