using humanlab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void SaveCategoryAsync(Category category)
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
            return db.Categories.Where(c => c.CategoryName.Equals(name))
                                .Count() > 0;
        }

    }
}
