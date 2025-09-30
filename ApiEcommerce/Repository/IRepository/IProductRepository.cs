using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public interface IProductRepository
{
    ICollection<Product> GetProducts();

    ICollection<Product> GetProductsForCategory(int categoryId);

    ICollection<Product> SearchProduct(string name);
    Product? GetProduct(int id);

    bool BuyProduct(string name, int quantity);

    bool ProductExists(int id);

    bool ProductExists(string name);

    bool CreateProduct(Product product);

    bool UpdateProductDto(Product product);

    bool DeleteProductDto(Product product);

    bool Save();
}
