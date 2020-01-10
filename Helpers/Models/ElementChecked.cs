using humanlab.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace humanlab.Helpers.Models
{
    public class ElementChecked
    {
        public String Name { get; set; }
        public bool IsSelected { get; set; }

        public ElementChecked(string name, bool isSelected)
        {
            Name = name;
            IsSelected = isSelected;
        }
    }
 
    
}
