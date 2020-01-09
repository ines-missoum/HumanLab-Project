using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Models
{
    public class Element
    {
       
        public int ElementId { get; set; }
        [Required]
        [MaxLength(40)]
        public string ElementName { get; set; }
        [Required]
        public string Image { get; set; }
      
        public string Audio { get; set; }
        public string SpeachText { get; set; }

        public Category Category { get; set; }
        public ICollection<Grid> Grids { get; } = new List<Grid>();
    }
}
