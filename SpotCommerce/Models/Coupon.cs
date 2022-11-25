using Newtonsoft.Json.Linq;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpotCommerce.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }






        [Required]
        [Display(Name = "Coupon Type")]
        public string CouponType { get; set; }

        public enum ECouponType { Percent = 0, Dollar = 1 }







        [Required]
        [Range(1, 1000000)]
        public double Discount { get; set; }

        [Required]
        [Display(Name = "Minimum Amount")]
        [Range(1, 1000000)]
        public double MinimumAmount { get; set; }






        public byte[] Picture { get; set; }

        [Display(Name="Is Active")]
        public bool IsActive { get; set; }
    }
}
