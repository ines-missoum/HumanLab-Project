using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Models
{
    public class Activity
    {
       
        public int ActivityId { get; set; }
        [Required]
        [MaxLength(40)]
        public string ActivityName { get; set; }
        [Required]
        public int FixingTime { get; set; }

        public ICollection<ActivityGrids> ActivityGrids { get; } = new List<ActivityGrids>();
    }
}
