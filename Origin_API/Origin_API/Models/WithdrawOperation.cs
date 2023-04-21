using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Origin_API.Models
{
    public partial class WithdrawOperation
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public DateTime Created { get; set; }
        public string Code { get; set; }
        public decimal Amount { get; set; }

        public virtual CreditCard Card { get; set; }
    }
}
