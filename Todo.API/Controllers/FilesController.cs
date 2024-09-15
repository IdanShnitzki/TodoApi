using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Todo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
        }

        [HttpGet("{fileId}")]
        public IActionResult GetFiles(string fileId)
        {
            // look up the actual file, depending on the fileId...
            // demo code
            const string folderName = "Files";
            const string fileName = "getting-started-with-rest-slides.pdf";
            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName) ;


            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound();
            }


            if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType)) 
            { 
                contentType = contentType = "application/octet-stream";
            }


            var bytes = System.IO.File.ReadAllBytes(pathToFile);
            return File(bytes, contentType, pathToFile);
        }
    }
}
