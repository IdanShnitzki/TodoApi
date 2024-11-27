using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Todo.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion(2)]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
        }

        /// <summary>
        /// Get File by id
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("{fileId}")]
        public IActionResult GetFiles(string fileId)
        {
            // look up the actual file, depending on the fileId...
            // demo code
            var folderPath = "Files";
            var fileName = "getting-started-with-rest-slides.pdf";
            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), folderPath, fileName) ;


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

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveFile(IFormFile file)
        {
            // Validate the input. Put a limit on filesize to avoid large uploads attacks. 
            if (file.Length == 0 || file.Length > 20971520) 
            {
                return BadRequest("File size is not supported, please upload file of size up to 20mb");
            }

            if (file.ContentType != "application/pdf")
            {
                return BadRequest("File type is not supported");
            }

            // This is only for local environmental purposes
            // On production environment files should be stored on an external location than the application with no Execute privileges
            // Avoid using file.FileName, as an attacker can provide a
            // malicious one, including full paths or relative paths.  
            var folderPath = "Files";
            var path = Path.Combine(Directory.GetCurrentDirectory(), folderPath, $"uploaded_file_{Guid.NewGuid()}.pdf");


            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("Your file has been uploaded successfully.");
        }
    }
}
