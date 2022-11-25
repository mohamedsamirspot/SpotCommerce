using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotCommerce.Models.ViewModels
{
    public class ItemViewModel
    {
        public Item Item { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<SubCategory> SubCategory { get; set; }
    }
}
