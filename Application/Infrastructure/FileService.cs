using Application.Extensions;
using Domain.Common;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;



namespace Application.Infrastructure;

public class FileService(IWebHostEnvironment env,IConfiguration configuration) : IFileService
{
    // Magic bytes for common image types
    private static readonly Dictionary<FileType, List<byte[]>> _fileSignatures = new()
    {
        {
            FileType.Image, new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, // JPEG
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }, // JPG
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } // PNG
            }
        },
        // Add other file types like PDF, etc. here
    };
    
    public string GetServerPath(string? relativePath)
    {
        return Path.Combine(env.WebRootPath, relativePath.TrimStart('/', '\\'));
    }

    public string GetFullUrl(string? relativePath)
    {
        var hostAddress = configuration["HOST_ADDRESS"];
        return string.IsNullOrEmpty(relativePath) 
            ? string.Empty
            : $"{hostAddress?.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }

    public async Task<UploadResult> SaveFileAsync(IFormFile file, string? relativePath, FileType[] allowedTypes)
    {
        if (file == null || file.Length == 0)
        {
            return new UploadResult { Succeeded = false, ErrorMessage = "File is empty." };
        }

        // --- Security Step 1: Validate file content with Magic Bytes ---
        if (!IsFileValid(file, allowedTypes))
        {
            return new UploadResult { Succeeded = false, ErrorMessage = "Invalid file type or content." };
        }

        // --- Security Step 2: Sanitize filename to prevent Path Traversal ---
        var fileName = Path.GetFileName(file.FileName); // Strips any directory info from the filename
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

        var serverPath = GetServerPath(relativePath);

        if (!Directory.Exists(serverPath))
        {
            Directory.CreateDirectory(serverPath);
        }

        var fullPath = Path.Combine(serverPath, uniqueFileName);

        try
        {
            // Use async stream copy for performance
            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }
        catch (Exception ex)
        {
            // Log the exception
            return new UploadResult { Succeeded = false, ErrorMessage = $"An error occurred: {ex.Message}" };
        }

        var fileRelativePath = Path.Combine(relativePath, uniqueFileName).Replace('\\', '/');
        return new UploadResult { Succeeded = true, FilePath = fileRelativePath };
    }

    public async Task CreateThumbnailAsync(string originServerPath, string thumbServerPath, int width, int height)
    {
        if (!File.Exists(originServerPath)) return;

        var thumbDirectory = Path.GetDirectoryName(thumbServerPath);
        if (!Directory.Exists(thumbDirectory))
        {
            Directory.CreateDirectory(thumbDirectory!);
        }

        using var image = await Image.LoadAsync(originServerPath);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Crop // Or another mode like Max, Pad, etc.
        }));
        await image.SaveAsync(thumbServerPath); // Saves in the original format
    }

    public void DeleteFile(string? relativePath, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || fileName == PathExtensions.DefaultAvatar) return;

        var serverPath = GetServerPath(relativePath);
        var fullPath = Path.Combine(serverPath, fileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private static bool IsFileValid(IFormFile file, FileType[] allowedTypes)
    {
        var allowedSignatures = allowedTypes.SelectMany(type => _fileSignatures[type]).ToList();

        using var reader = new BinaryReader(file.OpenReadStream());
        // Read the first few bytes of the file
        var headerBytes = reader.ReadBytes(allowedSignatures.Max(m => m.Length));

        // Reset stream position for next operations
        file.OpenReadStream().Position = 0;

        // Check if the file's header matches any of the allowed signatures
        return allowedSignatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
    }
}