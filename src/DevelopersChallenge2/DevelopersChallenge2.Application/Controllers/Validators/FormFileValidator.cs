using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace DevelopersChallenge2.Application.Controllers.Validators
{
    public static class FormFileValidator
    {
        public static bool Validate(List<IFormFile> formFiles)
        {
            return formFiles.Count > 0 && formFiles.All(file => file.ContentType == "application/octet-stream");
        }
    }
}