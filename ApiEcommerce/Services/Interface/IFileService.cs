using System;

namespace ApiEcommerce.Repository.IRepository;

public interface IFileService
{
     /// <summary>
    /// Guarda una imagen en wwwroot/{imagesFolder} y devuelve (urlPublica, rutaLocal).
    /// Si image es null devuelve placeholder por defecto.
    /// </summary>
    Task<(string ImgUrl, string? ImgUrlLocal)> SaveProductImageAsync(
        IFormFile? image,
        string productId,
        HttpRequest request,
        string imagesFolder = "ProductsImages"
    );
}
