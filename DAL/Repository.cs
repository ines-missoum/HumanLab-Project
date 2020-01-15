using humanlab.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.DAL
{
    class Repository
    {
        //Add Initial Categories 
        public async Task CreateCategories()
        {
            var categorie1 = new Category();
            categorie1.CategoryName = "Alimentation";

            var categorie2 = new Category();
            categorie2.CategoryName = "Loisir";

            using (var db = new ApplicationDbContext())
            {
               
                db.Add(categorie1);
                db.Add(categorie2);
                db.SaveChanges();
            }

            }
        public async void SaveElementAsync(Element model, string categoryName)
        {
            using (var db = new ApplicationDbContext())
            {
                if (model.ElementId > 0)
                {
                    db.Attach(model);
                    db.Update(model);
                }
                else
                {
                    Category selectedCategory = GetCategoryByName(categoryName, db);
                    selectedCategory.Elements.Add(model);
                }

                db.SaveChanges();
            }
        }

        public async Task<List<Element>> GetElementsAsync()
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    return await db.Elements.ToListAsync();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public async Task<List<string>> GetCategoriesNamesAsync()
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    return await db.Categories.Select(c => c.CategoryName.ToString()).ToListAsync();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public async Task<List<string>> GetElementsNamesAsync()
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    return await db.Elements.Select(e => e.ElementName.ToString()).ToListAsync();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }




        public Category GetCategoryByName(string name, ApplicationDbContext db)
        {

                return db.Categories.Include(c => c.Elements)
                                    .Where(c => c.CategoryName.Equals(name))
                                    .First();
    
        }


    }
}
