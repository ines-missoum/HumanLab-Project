using humanlab.Models;
using humanlab.ViewModels;
using Prism.Commands;
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
        public Element Element { get; set; }
        public bool IsSelected { get; set; }

        public ElementChecked(Element element, bool isSelected)
        {
            Element = element;
            IsSelected = isSelected;
        }

    }
 
    
}
