using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ApiEcommerce.Repository.IRepository;
public class FileService : IFileService
{

    private const string defaultPlaceholder = "https://placehold.co/300x300";

    public async Task<(string ImgUrl, string? ImgUrlLocal)> SaveProductImageAsync(IFormFile? image, 
        string productId, 
        HttpRequest request, 
        string imagesFolder = "ProductsImages"
    )
    {
         if (image == null) return (defaultPlaceholder, (string?)null);

        string fileName = productId + "-" + Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);
        var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagesFolder);

        if (!Directory.Exists(imagesPath))
            Directory.CreateDirectory(imagesPath);

        var filePath = Path.Combine(imagesPath, fileName);

        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Exists)
            fileInfo.Delete();

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var baseUrl = $"{request.Scheme}://{request.Host.Value}{request.PathBase.Value}";
        var publicUrl = $"{baseUrl}/{imagesFolder}/{fileName}";

        return (publicUrl, filePath);
    }
    
}
