using humanlab.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using humanlab.Helpers.Models;
using System.Diagnostics;

namespace humanlab.DAL
{
    class ElementRepository
    {
        
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


        public async void UpdateElementAsync(Element elementToUpdate, string categoryName)
        {
            using (var db = new ApplicationDbContext())
            {
                // Recupere l'element déja en db pour pouvoir le supprimer de la liste d'element de la categorie 
                Element oldElement = db.Elements.Select(e => e).Where(e => e.ElementId == elementToUpdate.ElementId).FirstOrDefault();
                Category oldCategory = GetCategories(db).Select(c => c).Where(c => c.Elements.Contains(oldElement)).FirstOrDefault();

                // Supprime l'ancien élément 
                oldCategory.Elements.Remove(oldElement);
                db.SaveChanges();
                Category selectedCategory = GetCategoryByName(categoryName, db);
                selectedCategory.Elements.Add(elementToUpdate);
                //Update l'element ( faut passer l'id) 
                db.SaveChanges();
            }
        }

        public async Task<List<Element>> GetElementsAsync()
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    return await db.Elements.Select(e => new Element
                    {
                        ElementId = e.ElementId,

                        ElementName = e.ElementName,

                        Image = e.Image,

                        Audio = e.Audio,

                        SpeachText = e.SpeachText,

                        Category = e.Category
                    }).ToListAsync();

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



        public async Task<List<string>> GetGridsNamesAsync()
        {

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    return await db.Grids.Select(g => g.GridName.ToString()).ToListAsync();
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
        public List<Category> GetCategories(ApplicationDbContext db)
        {
            return db.Categories.Include(c => c.Elements).ToList();
        }
    }
}
