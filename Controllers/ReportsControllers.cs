using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalBorrowed = _context.BorrowTransactions.Count(x => x.Status == "Borrowed");
            ViewBag.TotalReserved = _context.BorrowTransactions.Count(x => x.Status == "Reserved");
            ViewBag.TotalReturned = _context.BorrowTransactions.Count(x => x.Status == "Returned");
            ViewBag.TotalOverdue = _context.BorrowTransactions.Count(x => x.Status == "Overdue");

            ViewBag.PopularBooks = _context.BorrowTransactions
                .GroupBy(x => x.BookTitle)
                .Select(g => new
                {
                    BookTitle = g.Key,
                    TotalBorrowed = g.Count()
                })
                .OrderByDescending(x => x.TotalBorrowed)
                .Take(5)
                .ToList();

            ViewBag.ActiveMembers = _context.BorrowTransactions
                .GroupBy(x => x.MemberName)
                .Select(g => new
                {
                    MemberName = g.Key,
                    TotalTransactions = g.Count()
                })
                .OrderByDescending(x => x.TotalTransactions)
                .Take(5)
                .ToList();

            ViewBag.OverdueBooks = _context.BorrowTransactions
                .Where(x => x.Status == "Overdue")
                .ToList();

            return View();
        }
    }
}