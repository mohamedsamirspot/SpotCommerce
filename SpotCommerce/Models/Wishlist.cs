using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpotCommerce.Models
{
    public class Wishlist
    {



        public int Id { get; set; }



        public string ApplicationUserId { get; set; } 

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int MenuItemId { get; set; }

        [NotMapped]
        [ForeignKey("MenuItemId")]
        public virtual Item Item { get; set; }


    }
}
