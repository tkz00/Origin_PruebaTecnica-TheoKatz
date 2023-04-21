using Origin_API.Models;
using Origin_API.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Origin_API.Repositories
{
    public interface ICreditCardRepository
    {
        Task<IEnumerable<CreditCard>> GetCreditCards();

        Task<CreditCard> GetById(int cardId);

        Task<CreditCard> GetByNumber(string cardNumber);

        Task<bool> AddFailedLoginAttempt(int cardId);

        Task<WithdrawResult> Withdraw(int cardId, decimal amount);

        Task<bool> CreditCardExists(string cardNumber);

        Task<bool> RegisterBalance(int cardId, string operationCode);
     
        Task<int> RegisterWithdraw(int cardId, string operationCode, decimal amount);

        Task<WithdrawOperationDTO> GetWithdrawOperation(int withdrawOperationId);
    }
}
