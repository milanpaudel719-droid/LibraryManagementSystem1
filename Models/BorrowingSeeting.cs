namespace LibraryManagementSystem.Models
{
    public class BorrowingSetting
    {
        public int Id { get; set; }

        public int LoanDurationDays { get; set; }

        public int RenewalLimit { get; set; }

        public decimal OverduePenalty { get; set; }

        public int MaxBorrowableItems { get; set; }
    }
}