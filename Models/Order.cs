using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Order
    {
        [Key]
        public int IdOrder { get; set; }

        public int IdUser { get; set; }
        public int IdStatus { get; set; }

        public int IdBook { get; set; }

    }
}
