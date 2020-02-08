using humanlab.Helpers.Models;
using humanlab.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.DAL
{
    class ActivityRepository
    {
        public async Task<List<string>> GetActivityNamesAsync()
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    return await db.Activities.Select(g => g.ActivityName.ToString()).ToListAsync();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        internal void SaveActivityAsync(Activity newActivity, ObservableCollection<GridChecked> selectedGridsSource)
        {
            using (var db = new ApplicationDbContext())
            {

                // 1) Save activity object
                db.Activities.Add(newActivity);


                // 2) Créer un gridElements: 
                foreach (GridChecked gc in selectedGridsSource)
                {
                    // Retrieve grid efcore object from db
                    var dbGrid = db.Grids.Select(g => g).Where(g => g.GridName.Equals(gc.Grid.GridName)).FirstOrDefault();

                    // Get index of the grid among all grids
                    var gridIndex = gc.IndexInListView;

                    // Create new related object activityGrids
                    var newActivityGrid = new ActivityGrids
                    {
                        Grid = dbGrid,
                        Activity = newActivity,
                        Order = gridIndex
                    };

                    db.ActivityGrids.Add(newActivityGrid);
                    db.SaveChanges();
                }
            }
        }
        internal void UpdateActivityAsync(Activity updatedActivity, ObservableCollection<GridChecked> selectedGridsSource)
        {
            using (var db = new ApplicationDbContext())
            {

                // 1) Update activity object
                db.Activities.Update(updatedActivity);

                // 2) Delete Related entities
                // Retrieve  activitygrid efcore object from db
                List<ActivityGrids> dbActivityGrids = db.ActivityGrids.Select(ag => ag).Where(ag => ag.ActivityId.Equals(updatedActivity.ActivityId)).ToList();
                
                dbActivityGrids.ForEach(dbAg => db.ActivityGrids.Remove(dbAg));
                db.SaveChanges();
                //3 Recréer les tables intermediaires
                // 2) Créer un gridElements: 
                foreach (GridChecked gc in selectedGridsSource)
                {
                    // Retrieve grid efcore object from db
                    var dbGrid = db.Grids.Select(g => g).Where(g => g.GridName.Equals(gc.Grid.GridName)).FirstOrDefault();

                    // Get index of the grid among all grids
                    var gridIndex = gc.IndexInListView;


                    // Create new related object activityGrids
                    var newActivityGrid = new ActivityGrids
                    {
                        Grid = dbGrid,
                        Activity = updatedActivity,
                        Order = gridIndex
                    };

                    db.ActivityGrids.Add(newActivityGrid);
                }
                db.SaveChanges();
            }
        }
        internal void DeleteActivity(Activity oldActivity)
        {
            using (var db = new ApplicationDbContext())
            {

                // Delete Related entities
                List<ActivityGrids> dbActivityGrids = db.ActivityGrids.Select(ag => ag).Where(ag => ag.ActivityId.Equals(oldActivity.ActivityId)).ToList();
                dbActivityGrids.ForEach(dbAg => { db.ActivityGrids.Remove(dbAg); });
                db.SaveChanges();
                // Delete Activity
                db.Activities.Remove(oldActivity);
                db.SaveChanges();
            }
        }
        public async Task<List<Activity>> GetActivitiesAsync()
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    return await db.Activities.Select(a => new Activity { ActivityId = a.ActivityId, ActivityName = a.ActivityName, FixingTime = a.FixingTime }).ToListAsync();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public async Task<List<ActivityGrids>> GetGridsOfActivity(int idActivity)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    return await db.ActivityGrids.Select(ag => ag).Where(ag=> ag.ActivityId == idActivity).ToListAsync();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

    }
}
