using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public interface IProductRepository
{
    ICollection<Product> GetProducts();

    ICollection<Product> GetProductsInPage(int pageNumber, int pageSize);

    int GetTotalProducts();

    ICollection<Product> GetProductsForCategory(int categoryId);

    ICollection<Product> SearchProducts(string name);
    Product? GetProduct(int id);

    bool BuyProduct(string name, int quantity);

    bool ProductExists(int id);

    bool ProductExists(string name);

    bool CreateProduct(Product product);

    bool UpdateProductDto(Product product);

    bool DeleteProductDto(Product product);

    bool Save();
}
