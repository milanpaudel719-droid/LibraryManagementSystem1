using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BorrowTransaction
    {
        public int Id { get; set; }

        [Required]
        public string MemberName { get; set; }

        [Required]
        public string BookTitle { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public string Status { get; set; }

        public decimal FineAmount { get; set; }
    }
}