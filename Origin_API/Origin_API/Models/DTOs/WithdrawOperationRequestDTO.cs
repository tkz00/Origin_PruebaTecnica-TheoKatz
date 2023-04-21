using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Origin_API.Models.DTOs
{
    public class WithdrawOperationRequestDTO
    {
        public int withdrawOperationId { get; set; }
        
        public string securityToken { get; set; }
    }
}
