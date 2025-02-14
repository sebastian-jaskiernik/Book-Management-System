using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public Author? Author { get; set; } // ✅ Oznacz jako nullable, aby uniknąć walidacji

        [Required]
        public int PublisherId { get; set; }

        [ForeignKey("PublisherId")]
        public Publisher? Publisher { get; set; } // ✅ Oznacz jako nullable
    }

}
