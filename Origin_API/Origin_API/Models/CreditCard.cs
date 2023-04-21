using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Origin_API.Models
{
    public partial class CreditCard
    {
        public CreditCard()
        {
            BalanceOperation = new HashSet<BalanceOperation>();
            WithdrawOperation = new HashSet<WithdrawOperation>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string CardNumber { get; set; }
        public string Pin { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal Balance { get; set; }
        public bool Enabled { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastFailedLoginTime { get; set; }

        public virtual ICollection<BalanceOperation> BalanceOperation { get; set; }
        public virtual ICollection<WithdrawOperation> WithdrawOperation { get; set; }
    }
}
