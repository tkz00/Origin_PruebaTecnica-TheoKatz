using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Origin_API.Models.DTOs
{
    public class CreditCardDTO
    {
        public string CardNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal Balance { get; set; }

        public static explicit operator CreditCardDTO(CreditCard creditCard)
        {
            return new CreditCardDTO
            {
                CardNumber = creditCard.CardNumber,
                ExpirationDate = creditCard.ExpirationDate,
                Balance = creditCard.Balance
            };
        }
    }
}
