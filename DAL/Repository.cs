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
        internal async static Task CreateCategories()
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
        internal static void SaveElement(Element model)
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
                    db.Add(model);
                }

                db.SaveChanges();
            }
        }

        public async static Task<List<string>> GetCategoriesNamesAsync()
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

        public async static Task<List<string>> GetElementsNamesAsync()
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


        internal static Category GetCategoryByName(string name)
        {
            using (var db = new ApplicationDbContext())
            {
                return (from p in db.Categories
                        where p.CategoryName.Equals(name)
                        select p).FirstOrDefault();
            }
        }


    }
}
