using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfers
    {
        public int recipientID { get; set; }

        public int senderId { get; set; }

        public decimal transferAmount { get; set; }

        public Transfers()
        {

        }
    }

}
