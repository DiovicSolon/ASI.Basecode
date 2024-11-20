using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class CategoryRepository : ICategoryRespository
    {
        private readonly AsiBasecodeDBContext _context;

        public CategoryRepository(AsiBasecodeDBContext context)
        {
            _context = context;
        }
        public IEnumerable<Category> GetAllCategory()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategoryById(int Id)
        {
            return _context.Categories.Find(Id);
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }


        public void UpdateCategory(Category category)
        {
            var exist = _context.Categories.Find(category.CategoryId);
            if (exist != null)
            {
                exist.CategoryName = category.CategoryName;
                exist.DateCreated = category.DateCreated;

                _context.SaveChanges();
            }
        }
        public void DeleteCategory(int Id)
        {
            var exist = _context.Categories.Find(Id);
            if (exist != null)
            {
                _context.Categories.Remove(exist);
                _context.SaveChanges();
            }
        }
    }
}
