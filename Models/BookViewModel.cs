using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class BookViewModel
    {
        [Key]
        public int BookID { get; set; }

        public string BookName { get; set; }
        public string Author { get; set; }
        public double Price { get; set; }
        public int PrintYear { get; set; }
        public int Amount { get; set; }
        public string? ImagePath { get; set; }

        public double Rating { get; set; }
        public IFormFile? Image { get; set; }

        public Book CreateBook()
        {
            return new Book
            {
                BookID = BookID,
                BookName = BookName,
                Author = Author,
                Price = Price,
                PrintYear = PrintYear,
                Amount = Amount,
                ImagePath = ImagePath,
                Rating = Rating
            };   

            
        }
    }
}
