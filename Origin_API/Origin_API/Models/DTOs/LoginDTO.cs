using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Origin_API.Models.DTOs
{
    public class LoginDTO
    {
        public string CreditCardNumber { get; set; }
        public string Pin { get; set; }
    }
}
