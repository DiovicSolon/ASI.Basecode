using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ICategoryRespository
    {
        IEnumerable<Category> GetAllCategory();
        Category GetCategoryById(int Id);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int Id);
    }
}
