﻿using DevelopersChallenge2.Application.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Application.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        void Add(Transaction transaction);
        void Save(List<Transaction> transactions);
        void DeleteAll();
        Task<List<Transaction>> GetAllTransactions();
    }
}
