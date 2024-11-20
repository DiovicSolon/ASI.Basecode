using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRespository _categoryRespository;

        public CategoryService(ICategoryRespository categoryRespository)
        {
            _categoryRespository = categoryRespository;
        }
        public IEnumerable<Category> GetAllCategory()
        {
            return _categoryRespository.GetAllCategory();
        }
        public Category GetCategoryById(int Id)
        {
            return _categoryRespository.GetCategoryById(Id);
        }

        public void AddCategory(Category category)
        {
            _categoryRespository.AddCategory(category);
        }

        public void UpdateCategory(Category category)
        {
            _categoryRespository.UpdateCategory(category);
        }
        public void DeleteCategory(int Id)
        {
            _categoryRespository.DeleteCategory(Id);
        }

        public IEnumerable<object> GetAllCategories()
        {
            throw new NotImplementedException();
        }
    }
}
