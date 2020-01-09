﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Models
{
    public class Category
    {
        
        public int CategoryId { get; set; }
        [Required]

        [MaxLength(40)]
        public string CategoryName { get; set; }

        public ICollection<Element> Elements { get; set; }
    }
}
