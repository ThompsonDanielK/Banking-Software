namespace TenmoServer.Models
{
    public class Transfers
    {
        public int RecipientID { get; set; }

        public int SenderID { get; set; }

        public decimal TransferAmount { get; set; }

        public int TransferTypeId { get; set; }

        public string TransferType { get; set; }

        public string TransferTypeDetails
        {
            get
            {
                if (TransferTypeId == 1001)
                {
                    return "Request";
                }
                else
                {
                    return "Send";
                }
            }
        }

        public string Username { get; set; }

        public string SendersUsername { get; set; }

        public string RecipientsUsername { get; set; }

        public int TransferStatusId { get; set; }

        public string TransferStatus
        {
            get
            {
                switch (TransferStatusId)
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

        public int TransferId { get; set; }

        public Transfers()
        {

        }
    }

}
