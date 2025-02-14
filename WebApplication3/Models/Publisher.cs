using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Publisher
    {
        public int PublisherId { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>(); // Dodano inicjalizację
    }
}
