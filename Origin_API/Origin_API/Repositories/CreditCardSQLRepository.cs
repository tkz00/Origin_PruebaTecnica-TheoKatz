using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Origin_API.Models;
using Origin_API.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Origin_API.Repositories
{
    public class CreditCardSQLRepository : ICreditCardRepository
    {
        private readonly CreditCardContext _context;

        public CreditCardSQLRepository(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("OriginDb");
            var optionsBuilder = new DbContextOptionsBuilder<CreditCardContext>();
            optionsBuilder.UseSqlServer(connectionString);
            _context = new CreditCardContext(optionsBuilder.Options);
        }

        public async Task<IEnumerable<CreditCard>> GetCreditCards()
        {
            return _context.CreditCard
                    .Include(cc => cc.WithdrawOperation)
                    .Include(cc => cc.BalanceOperation)
                    .ToList();
        }

        public async Task<CreditCard> GetById(int cardId)
        {
            return _context.CreditCard
                    .Single(cc => cc.Id == cardId);
        }

        public async Task<CreditCard> GetByNumber(string cardNumber)
        {
            return _context.CreditCard
                    .SingleOrDefault(cc => cc.CardNumber == cardNumber);
        }

        public async Task<WithdrawResult> Withdraw(int cardId, decimal amount)
        {
            var card = _context.CreditCard.Find(cardId);
            if (card != null && card.Enabled)
            {
                if(card.Balance < amount)
                {
                    return WithdrawResult.Non_sufficient_funds;
                }
                
                card.Balance -= amount;
                _context.SaveChanges();
                
                return WithdrawResult.Successful;
            }

            return WithdrawResult.Failed;
        }

        public async Task<bool> CreditCardExists(string cardNumber)
        {
            return _context.CreditCard.Any(cc => cc.CardNumber == cardNumber && cc.Enabled);
        }

        public async Task<bool> AddFailedLoginAttempt(int cardId)
        {
            var card = _context.CreditCard.Find(cardId);

            if(card != null)
            {
                if(card.LastFailedLoginTime != null && DateTime.UtcNow.Subtract(card.LastFailedLoginTime.Value).TotalMinutes > 59)
                {
                    card.FailedLoginAttempts = 0;
                }

                card.FailedLoginAttempts++;
                card.LastFailedLoginTime = DateTime.UtcNow;

                if(card.FailedLoginAttempts > 3)
                {
                    card.Enabled = false;
                }

                _context.SaveChanges();

                return !card.Enabled;
            }

            throw new Exception("Error: Credit card not found.");
        }

        public async Task<bool> RegisterBalance(int cardId, string operationCode)
        {
            try
            {
                var newBalanceOperation = new BalanceOperation
                {
                    Code = operationCode,
                    CardId = cardId,
                    Created = DateTime.Now
                };

                _context.BalanceOperation.Add(newBalanceOperation);
                _context.SaveChanges();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<int> RegisterWithdraw(int cardId, string operationCode, decimal amount)
        {
            try
            {
                var newWithdrawOperation = new WithdrawOperation
                {
                    CardId = cardId,
                    Code = operationCode,
                    Amount = amount,
                    Created = DateTime.Now
                };

                _context.WithdrawOperation.Add(newWithdrawOperation);
                _context.SaveChanges();

                return newWithdrawOperation.Id;
            }
            catch(Exception ex)
            {
                return -1;
            }
        }

        public async Task<WithdrawOperationDTO> GetWithdrawOperation(int withdrawOperationId)
        {
            WithdrawOperation withdrawOperation = _context.WithdrawOperation.Find(withdrawOperationId);
            CreditCard creditCard = _context.CreditCard.Find(withdrawOperation.CardId);

            WithdrawOperationDTO withdrawOperationDTO = new WithdrawOperationDTO
            {
                CreditCardNumber = creditCard.CardNumber,
                cardBalance = creditCard.Balance,
                amount = withdrawOperation.Amount,
                dateTime = withdrawOperation.Created
            };

            return withdrawOperationDTO;
        }
    }
}
