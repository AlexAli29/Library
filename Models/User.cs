using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string UserPassword {get; set; }

        public int IdRole { get; set; }
    }
}
