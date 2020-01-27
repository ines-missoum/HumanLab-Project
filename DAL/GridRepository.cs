using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using humanlab.Helpers.Models;
using humanlab.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace humanlab.DAL
{
    public class GridRepository
    {

        public async void SaveGridAsync(Grid grid, List<ElementPlaced> elements)
        {
            using (var db = new ApplicationDbContext())
            {

                // 1) Enregistrer la grille
                db.Grids.Add(grid);


                // 2) Créer un gridElements: 
                foreach (ElementPlaced ep in elements)
                {

                    var dbElement = db.Elements.Select(e => e).Where(e => e.ElementName.Equals(ep.Element.ElementName)).FirstOrDefault();

                    var newGridElement = new GridElements
                    {
                        Element = dbElement,
                        Grid = grid,
                        Xposition = Convert.ToInt32(ep.XPosition),
                        Yposition = Convert.ToInt32(ep.YPosition)
                    };
                    db.GridElements.Add(newGridElement);
                    db.SaveChanges();
                }
            }
        }

        public async Task<List<Element>> GetAllGridElements(int gridId)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    //we retrieve the grid of the corresponding id
                    var grid = await db.Grids.Include(g => g.GridElements)
                        .ThenInclude(g => g.Element)
                        .Where(g => g.GridId == gridId)
                        .FirstAsync();

                    //we retrieve all its elements.
                    List<Element> allGridElements = new List<Element>();
                    foreach (GridElements gridElement in grid.GridElements)
                    {
                        allGridElements.Add(gridElement.Element);
                    }

                    return allGridElements;

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error");
                    return null;
                }
            }
        }
    }

}
