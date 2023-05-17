using System.ComponentModel.DataAnnotations;

namespace API_Opdracht.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The Street field is required.")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "The HouseNumber field is required.")]
        public string HouseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Postcode field is required.")]
        public string Postcode { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Place field is required.")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Country field is required.")]
        public string Country { get; set; } = string.Empty;
        
    }
}