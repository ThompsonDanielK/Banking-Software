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

        public int transferTypeId { get; set; }

        public string transferType
        {
            get
            {
                if (this.transferTypeId == 1001)
                {
                    return "from ";
                }
                else
                {
                    return "to ";
                }
            }
        }

        public string username { get; set; }

        public int transferId { get; set; }

        public Transfers()
        {

        }
    }

}
