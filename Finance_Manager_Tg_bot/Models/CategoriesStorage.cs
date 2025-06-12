using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_Tg_bot.Models;

public static class CategoriesStorage
{
    public static List<CategoryDTO> AllCategories { get; set; } = new();

    public static CategoryDTO? GetCategoryById(int id)
    {
        return AllCategories.FirstOrDefault(c => c.Id == id);
    }    
}
