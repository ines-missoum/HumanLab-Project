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
                        Xposition = Convert.ToInt32(ep.PositionX),
                        Yposition = Convert.ToInt32(ep.PositionY)
                    };
                    db.GridElements.Add(newGridElement);
                    db.SaveChanges();
                }
            }
        }

        public async Task<List<Grid>> GetGridsAsync()
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    return await db.Grids.Select(g => new Grid { 
                        GridName = g.GridName, 
                        GridId = g.GridId, 
                        GridElements = g.GridElements
                    })
                        .ToListAsync();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }


    }
}
