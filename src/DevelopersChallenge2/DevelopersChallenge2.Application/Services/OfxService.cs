using DevelopersChallenge2.Application.Domain.Entity;
using DevelopersChallenge2.Application.Domain.ExtensionMethods;
using DevelopersChallenge2.Application.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Application.Services
{
    public class OfxService : IOfxService
    {
        private readonly ILogger _logger;
        private readonly ITransactionRepository _transactionRepository;

        public OfxService(ILogger<OfxService> logger, ITransactionRepository transactionRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public async Task ProcessOfxFiles(List<IFormFile> formFiles)
        {
            _logger.LogInformation($"Start processing {formFiles.Count} Ofx files");

            var transactionsGroupedByFile = new List<List<Transaction>>();
            foreach (var formFile in formFiles)
            {
                if (formFile.Length <= 0) continue;
                
                var fileName = Path.GetFileName(formFile.FileName);
                var filePath = Path.Combine(Path.GetTempPath() + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + fileName);

                await using (var stream = File.Create(filePath))
                {
                    _logger.LogInformation($"Persisting the {fileName} file");
                    await formFile.CopyToAsync(stream);
                }

                transactionsGroupedByFile.Add(GetTransactionsByOfxFile(filePath));
            }

            PersistsOfxTransactions(transactionsGroupedByFile);
            _logger.LogInformation($"End processing Ofx files");
        }
        
        private List<Transaction> GetTransactionsByOfxFile(string filePath)
        {
            var ofxFile = filePath.ToOfx();
            return ofxFile.Transactions;
        }

        private void PersistsOfxTransactions(List<List<Transaction>> transactionsGroupedByFile)
        {
            _logger.LogInformation($"Start of persistence of transactions file.");
            if (transactionsGroupedByFile.Count > 1)
            {
                var transactions = RemoveDuplicatedTransactions(transactionsGroupedByFile);
                _transactionRepository.Save(transactions);
                return;
            }
            _transactionRepository.Save(transactionsGroupedByFile[0]);
            _logger.LogInformation($"End of persistence of transactions file.");
        }

        private List<Transaction> RemoveDuplicatedTransactions(List<List<Transaction>> transactionsGroupedByFile)
        {
            _logger.LogInformation("Start of duplicated transaction removal");
            var transactions = new List<Transaction>();
            for (var i = 0; i < transactionsGroupedByFile.Count-1; i++)
            {
                var currentTransactions = transactionsGroupedByFile[i];
                var nextTransactions = transactionsGroupedByFile[i+1];

                foreach (var transaction in currentTransactions)
                {
                    nextTransactions.RemoveAll(x => x.UniqueKey == transaction.UniqueKey);
                }

                transactions.AddRange(transactionsGroupedByFile[i].Union(transactionsGroupedByFile[i+1]));
            }

            _logger.LogInformation("End of duplicated transaction removal");
            return transactions;
        }

        /*private void PersistsOfxTransactions(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            _logger.LogInformation($"Start of persistence of transactions {fileName} file.");

            var ofxFile = filePath.ToOfx();
            if(ofxFile.Transactions != null)
            {
                var transactions = ofxFile.Transactions
                    .Select(x => { x.OfxFileReference = fileName; return x; })
                    .ToList();
                
                RemoveDuplicatedTransactions(transactions);
                _transactionRepository.Save(transactions);
            }            
            
            
        }*/
    }
}
