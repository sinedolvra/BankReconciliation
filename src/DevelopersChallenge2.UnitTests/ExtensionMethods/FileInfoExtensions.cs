using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;

namespace DevelopersChallenge2.UnitTests.ExtensionMethods
{
    public static class FileInfoExtensions
    {
        public static IFormFile AsMockIFormFile(this FileInfo physicalFile)
        {
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(physicalFile.OpenRead());
            writer.Flush();
            ms.Position = 0;
            var fileName = physicalFile.Name;
            //Setup mock file using info from physical file
            fileMock.Setup(m => m.FileName).Returns(fileName);
            fileMock.Setup(m => m.Length).Returns(ms.Length);
            fileMock.Setup(m => m.OpenReadStream()).Returns(ms);
            fileMock.Setup(m => m.ContentDisposition).Returns(string.Format("inline; filename={0}", fileName));

            return fileMock.Object;
        }
    }
}
