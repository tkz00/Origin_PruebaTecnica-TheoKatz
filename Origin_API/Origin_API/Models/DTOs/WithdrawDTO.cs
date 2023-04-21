using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Origin_API.Models.DTOs
{
    public class WithdrawDTO
    {
        public string SecurityToken { get; set; }

        public decimal Amount { get; set; }
    }
}
