using System;

namespace ApiEcommerce.Repository.IRepository;

public interface ICategoryRepository
{
    ICollection<Category> GetCategories();

    Category? GetCategory(int id);

    bool CategoryExists(int id);
    
    bool CategoryExists(String name);

    bool CreateCategory(Category category);

    bool DeteleCategory(Category category);

    bool UpdateCategory(Category category);

    bool Save();
}
