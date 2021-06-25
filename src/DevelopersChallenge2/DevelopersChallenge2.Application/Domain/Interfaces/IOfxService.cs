using DevelopersChallenge2.Application.Domain.Entity;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Application.Domain.Interfaces
{
    public interface IOfxService
    {
        Task<List<Transaction>> ProcessOfxFiles(List<IFormFile> formFiles);
    }
}
