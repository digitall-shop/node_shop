using Domain.Common;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Infrastructure;

public interface IFileService
{
    /// <summary>
    /// Saves a file to a specified path securely and asynchronously.
    /// It validates the file type based on its content (magic bytes).
    /// </summary>
    /// <param name="file">The IFormFile to save.</param>
    /// <param name="relativePath">The relative directory path to save the file in (e.g., "/images/UserAvatar/origin/").</param>
    /// <param name="allowedTypes">An array of allowed file types (e.g., FileType.Image).</param>
    /// <returns>An UploadResult object indicating success or failure.</returns>
    Task<UploadResult> SaveFileAsync(IFormFile file, string? relativePath, FileType[] allowedTypes);

    /// <summary>
    /// Creates a thumbnail for an image using ImageSharp.
    /// </summary>
    Task CreateThumbnailAsync(string originServerPath, string thumbServerPath, int width, int height);

    /// <summary>
    /// Deletes a file and its corresponding thumbnail if it exists.
    /// </summary>
    void DeleteFile(string? relativePath, string fileName);

    /// <summary>
    /// Gets the full server path from a relative web path.
    /// </summary>
    string GetServerPath(string? relativePath);
    
    string GetFullUrl(string? relativePath);
}