using humanlab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Helpers.Models
{
    class CategoryOrdered
    {
        public Category Category { get; set; }
        public string Key { get; set; }

        public CategoryOrdered(Category category, string key)
        {
            Category = category;
            Key = key;
        }
    }
}
