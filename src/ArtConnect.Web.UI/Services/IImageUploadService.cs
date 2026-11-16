namespace ArtConnect.Web.UI.Services
{
    public interface IImageUploadService
    {
        /// <summary>
        /// Saves a JPG or PNG file (max 5 MB) under wwwroot/uploads/{folder}.
        /// Returns the public URL, or an error message.
        /// </summary>
        Task<(string? Url, string? Error)> SaveAsync(IFormFile file, string folder);
    }
}
