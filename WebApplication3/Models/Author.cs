using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Author
    {
        public int AuthorId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>(); // Dodaj inicjalizację
    }
}
