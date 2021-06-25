using AutoFixture;
using DevelopersChallenge2.Application.Domain.Entity;
using DevelopersChallenge2.Application.Domain.Interfaces;
using DevelopersChallenge2.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.IO;
using Xunit;
using DevelopersChallenge2.UnitTests.ExtensionMethods;
using FluentAssertions;
using System.Threading.Tasks;

namespace DevelopersChallenge2.UnitTests
{
    public class OfxServiceTests
    {
        public readonly OfxService _ofxService;
        public readonly Fixture _fixture;
        public readonly Mock<ILogger<OfxService>> _logger;
        public readonly Mock<ITransactionRepository> _repository;
        public List<IFormFile> formFiles;

        public OfxServiceTests()
        {
            _logger = new Mock<ILogger<OfxService>>();
            _repository = new Mock<ITransactionRepository>();
            _ofxService = new OfxService(_logger.Object, _repository.Object);
        }

        [Fact]
        public async Task ProcessOfxFiles_GivenATwoValidFormFiles_ShouldSaveOnceAsync()
        {
            formFiles = BuildValidFormFiles();
            var transactions = await _ofxService.ProcessOfxFiles(formFiles);
            _repository.Verify(x => x.Save(It.IsAny<List<Transaction>>()), Times.Once);
        }

        [Theory]
        [InlineData("extrato1.ofx")]
        [InlineData("extrato2.ofx")]
        public void GetTransactionsByOfxFile_GivenAValidOfxFilePath_ShouldReturnAListOfTransactions(string filename)
        {
            var transactions = _ofxService.GetTransactionsByOfxFile(filename);
            transactions.Count.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("extrato1.ofx", 31)]
        [InlineData("extrato2.ofx", 22)]
        public void RemoveDuplicatedTransactions_GivenAnOneOfxFile_ShouldReturnItsTransactions(string filename, int transactionsCount)
        {
            var transactionsFromFile = _ofxService.GetTransactionsByOfxFile(filename);
            var transactionsList = new List<List<Transaction>>()
            {
                transactionsFromFile
            };

            var transactionsWithoutDuplicates = _ofxService.RemoveDuplicatedTransactions(transactionsList);
            transactionsWithoutDuplicates.Count.Should().Be(transactionsCount);
        }

        [Fact]
        public void RemoveDuplicatedTransactions_GivenATwoOfxFiles_ShouldRemoveDuplicatedTransactions()
        {
            var transactionsFromFile1 = _ofxService.GetTransactionsByOfxFile("extrato1.ofx"); //The file has 31 transactions
            var transactionsFromFile2 = _ofxService.GetTransactionsByOfxFile("extrato2.ofx"); //The file has 22 transactions
            var transactionsList = new List<List<Transaction>>()
            {
                transactionsFromFile1,
                transactionsFromFile2
            };

            var transactionsWithoutDuplicates = _ofxService.RemoveDuplicatedTransactions(transactionsList);
            transactionsWithoutDuplicates.Count.Should().Be(39);
        }

        private IFormFile BuildFormFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            return fileInfo.AsMockIFormFile();            
        }

        private List<IFormFile> BuildValidFormFiles()
        {
            IFormFile formFile1 = BuildFormFile("extrato1.ofx");
            IFormFile formFile2 = BuildFormFile("extrato2.ofx");

            return new List<IFormFile>()
            {
                formFile1,
                formFile2,
            };
        }
    }
}
