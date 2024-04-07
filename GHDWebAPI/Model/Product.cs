using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GHDWebAPI.Model
{
    public class Product
    {
        /// <summary>
        /// Product Id
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Name of the product
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name field should be a minimum of 2 characters and a maximum of 50")]
        public string? Name { get; set; }

        /// <summary>
        /// Product brand
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Brand field should be a minimum of 2 characters and a maximum of 50")]
        public string? Brand { get; set; }

        /// <summary>
        /// Price of the product
        /// </summary>
        [Required]
        public double Price { get; set; }
    }
}
    