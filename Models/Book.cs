using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }
        [Required]
        public string BookName { get; set; }
        [Required]
        public string Author { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Should be greater or equal to 1")]
        [Required]
        public double Price { get; set; }
        [Required]
        public int PrintYear { get; set; }
        [Required]
        public int Amount { get; set; }

        [Required]
        public double Rating { get; set; }
        public string? ImagePath { get; set; }
    }
}
