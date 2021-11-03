﻿using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BankingController : ControllerBase
    {
        private readonly IBankingDAO banking;

        public BankingController(IBankingDAO banking)
        {
            this.banking = banking;
        }

        [HttpGet("{id}")]
        public ActionResult GetBalance(int id)
        {
            decimal balance = banking.GetBalanceSQL(id);

            //string balanceString = $"Your current account balance is: {balance.ToString("C")}";

            User user = new User();
            user.Balance = balance;

            return Ok(user);
        }

        [HttpGet("{id}/UserList")]
        public ActionResult GetUserList(int id)
        {
            IEnumerable<User> users = banking.GetUserListSQL(id);

            return Ok(users);
        }
    }
}
