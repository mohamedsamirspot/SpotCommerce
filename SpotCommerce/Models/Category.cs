using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpotCommerce.Models
{
    public class Category
    {

        [Key] 

        public int Id { get; set; }



        [Display(Name="Category Name")] 
        [Required] 
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
