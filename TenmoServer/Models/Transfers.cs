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

        public string transferType { get; set; } 
        
        public string transferTypeDetails
        {
            get
            {
                if (transferTypeId == 1001)
                {
                    return "Send";
                }
                else
                {
                    return "Request";
                }
            }
        }

        public string username { get; set; }

        public string sendersUsername { get; set; }

        public string recipientsUsername { get; set; }

        public int transferStatusId { get; set; }

        public string transferStatus
        {
            get
            {
                switch (transferStatusId)
                {
                    case 2001:
                        return "Pending";

                    case 2002:
                        return "Approved";

                    case 2003:
                        return "Rejected";

                    default:
                        return "";

                }
            }
        }

        public int transferId { get; set; }

        public Transfers()
        {

        }
    }
}
