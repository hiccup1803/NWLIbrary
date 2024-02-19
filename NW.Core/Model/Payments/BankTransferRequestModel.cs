using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model.Payments
{
    public class BankTransferRequestV2Model
    {
        public string Id { get; set; }
        public int MemberId { get; set; }
        public string Username { get; set; }
        public string LevelName { get; set; }
        public string Status { get; set; }

        public string TransferDate { get; set; }
        public DateTime CreateDateDate { get; set; }
        public string CreateDate { get; set; }
        public decimal AmountDecimal { get; set; }
        public string Amount { get; set; }
        public string UsersAmount { get; set; }

        public string ReceiverIBAN { get; set; }
        public string ReceiverBankId { get; set; }
        public string ReceiverBankName { get; set; }
        public string ReceiverBranchCode { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public string ReceiverFullname { get; set; }
        public string ReceiverReference { get; set; }
        public int CompanyId { get; set; }

        public string SenderIdentityNumber { get; set; }
        public string SenderFirstname { get; set; }
        public string SenderLastname { get; set; }
        public int RequestBankId { get; set; }
        public string RequestBankName { get; set; }
        public string FastEnabled { get; set; }
    }
}
