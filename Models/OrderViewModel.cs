namespace Library.Models
{
    public class OrderViewModel
    {
        public int IdOrder { get; set; }
        public int IdStatus { get; set; }
        public int BookID { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public double Price { get; set; }
        public int PrintYear { get; set; }
        public int Amount { get; set; }
        public string? ImagePath { get; set; }

        public double Rating { get; set; }
    }
}
