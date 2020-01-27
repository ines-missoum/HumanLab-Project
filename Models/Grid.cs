using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Models
{
    public class Grid
    {
        public int GridId { get; set; }
        [Required]
        [MaxLength(40)]
        public string GridName { get; set; }
        [Required]
        public double ElementsWidth { get; set; }
        public double ElementsHeight{ get; set; }

        public ICollection<ActivityGrids> ActivityGrids { get; set; } = new List<ActivityGrids>();
        public ICollection<GridElements> GridElements { get; set; } = new List<GridElements>();
    }
}
