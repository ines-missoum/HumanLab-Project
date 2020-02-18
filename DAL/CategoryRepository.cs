using humanlab.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace humanlab.DAL
{
    class CategoryRepository
    {
        /// <summary>
        /// Method in charge of the creation and update of a category. 
        /// It checks if the category name is allowed (ie: doesn't exists in the database) then if it's an existing category updates the name else creates it.
        /// </summary>
        /// <param name="category"> a category to update ou create </param>
        public bool SaveCategoryAsync(Category category)
        {
            using (var db = new ApplicationDbContext())
            {
                //we check if a category of this name doesn't already exist in the database
                if (!this.IsExistingCategory(category.CategoryName, db))
                {
                    //if it is an existing category, we update it
                    if (category.CategoryId > 0)
                    {
                        db.Attach(category);
                        db.Update(category);
                    }
                    else
                    {
                        //else it's a new category, we create it
                        db.Add(category);
                    }

                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        internal async void  CheckDefaultCategory()
        {
            var categories = await GetAllCategoriesAsync();
            if (categories.Count() < 1)
            {
                Category defaultCategory = new Category { CategoryName = "Autres"};
                using (var db = new ApplicationDbContext())
                {
                    try
                    {
                        db.Categories.Add(defaultCategory);
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Error With initialisation of default category");
                    };
                }
            }
        }

        /// <summary>
        /// Returns true if the category name already exist in database, false otherwise.
        /// </summary>
        /// <param name="name"> the name of the category </param>
        /// <param name="db"> db context </param>
        /// <returns></returns>
        private bool IsExistingCategory(string name, ApplicationDbContext db)
        {
            return db.Categories.Where(c => c.CategoryName.ToLower().Equals(name.ToLower()))
                                .Count() > 0;
        }

        /// <summary>
        /// Retrieves all the categories names of the database
        /// </summary>
        /// <returns>the list of the categories names of the database</returns>
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    return await db.Categories.ToListAsync();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
