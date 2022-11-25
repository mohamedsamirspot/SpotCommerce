using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotCommerce.Models.ViewModels
{

    public class OrderDetailsViewModel
    {
        public List<OrderDetails> OrderDetails { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
