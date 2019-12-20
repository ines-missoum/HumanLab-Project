using humanlab.Models;
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
