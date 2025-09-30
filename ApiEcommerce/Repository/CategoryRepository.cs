using System;
using ApiEcommerce.Repository.IRepository;

namespace ApiEcommerce.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly AplicationDbContext _db;

    public CategoryRepository(AplicationDbContext db)
    {
        _db = db;
    }

    public bool CategoryExists(int id)
    {
        return _db.Categories.Any(c => c.Id == id);
    }

    public bool CategoryExists(string name)
    {
        return _db.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool CreateCategory(Category category)
    {
        category.CreateonDate = DateTime.Now;
        _db.Categories.Add(category);
        return Save();
    }

    public bool DeteleCategory(Category category)
    {
        _db.Categories.Remove(category);
        return Save();
    }

    public ICollection<Category> GetCategories()
    {
        return _db.Categories.OrderBy(c => c.Name).ToList();
    }

    public Category? GetCategory(int id)
    {
        return _db.Categories.FirstOrDefault(c => c.Id == id);
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0 ? true : false;
    }

    public bool UpdateCategory(Category category)
    {
        category.CreateonDate = DateTime.Now;
        _db.Categories.Update(category);
        return Save();
    }
}
