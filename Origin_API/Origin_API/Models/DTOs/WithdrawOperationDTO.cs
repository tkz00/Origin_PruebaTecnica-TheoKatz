using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Origin_API.Models.DTOs
{
    public class WithdrawOperationDTO
    {
        public string CreditCardNumber { get; set; }

        public DateTime dateTime { get; set; }

        public decimal amount { get; set; }

        public decimal cardBalance { get; set; }
    }
}
