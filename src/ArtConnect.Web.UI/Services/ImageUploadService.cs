namespace ArtConnect.Web.UI.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private const long MaxBytes = 5 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        private readonly IWebHostEnvironment _environment;

        public ImageUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<(string? Url, string? Error)> SaveAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return (null, "Please choose an image.");

            if (file.Length > MaxBytes)
                return (null, "The image is larger than 5 MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return (null, "Only JPG and PNG images are allowed.");

            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var targetFolder = Path.Combine(webRoot, "uploads", folder);
            Directory.CreateDirectory(targetFolder);

            // A new unique name avoids name conflicts and unsafe file names.
            var fileName = Guid.NewGuid().ToString("N") + extension;
            var fullPath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return ($"/uploads/{folder}/{fileName}", null);
        }
    }
}
