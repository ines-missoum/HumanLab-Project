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
        public int ElementsSize { get; set; }

        public ICollection<Activity> Activities { get; } = new List<Activity>();
        public ICollection<Element> Elements { get; } = new List<Element>();
    }
}
