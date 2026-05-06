using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class LibraryProfile
    {
        public int Id { get; set; }

        [Required]
        public string LibraryName { get; set; }

        public string Location { get; set; }

        public string OperatingHours { get; set; }

        public string ContactDetails { get; set; }
    }
}