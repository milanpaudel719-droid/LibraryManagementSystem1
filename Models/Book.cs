using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        public string ISBN { get; set; }

        public string AvailabilityStatus { get; set; }

        public string CoverImageUrl { get; set; }
    }
}