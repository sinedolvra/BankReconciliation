using System;
using DevelopersChallenge2.Application.Domain.Entity;
using DevelopersChallenge2.Application.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Application.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly MyContext dbContext;
        public TransactionRepository(MyContext context)
        {
            dbContext = context;
        }

        public void Add(Transaction transaction)
        {
            dbContext.Add(transaction);
            dbContext.SaveChanges();
        }

        public void Save(List<Transaction> transactions)
        {
            var newTransactionsWithId = transactions.Select(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                return x;
            });
            dbContext.AddRange(newTransactionsWithId);
            dbContext.SaveChanges();
        }

        public async Task<List<Transaction>> GetAllTransactions()
        {
            return await dbContext.Transactions
                .OrderBy(x => x.PostedDate)
                .ToListAsync();
        }

        public void DeleteAll()
        {
            dbContext.RemoveRange(dbContext.Transactions);
            dbContext.SaveChanges();
        }
    }
}
