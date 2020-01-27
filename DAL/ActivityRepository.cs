using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    }
}
