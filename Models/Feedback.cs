using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required]
        public string MemberName { get; set; }

        [Required]
        public string BookTitle { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }
    }
}