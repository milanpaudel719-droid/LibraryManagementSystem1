using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class BorrowTransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowTransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            if (userRole == "Admin" || userRole == "Staff")
            {
                return View(await _context.BorrowTransactions.ToListAsync());
            }

            return View(await _context.BorrowTransactions
                .Where(t => t.MemberName == userName)
                .ToListAsync());
        }

        [HttpPost]
        public IActionResult CreateFromCatalogue(int bookId)
        {
            var memberName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(memberName))
            {
                return RedirectToAction("Login", "Account");
            }

            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                return NotFound();
            }

            if (book.AvailableCopies <= 0)
            {
                TempData["BorrowLimitMessage"] = "This book is currently not available. Please reserve it instead.";
                return RedirectToAction("Catalogue", "Home");
            }

            var activeBorrowCount = _context.BorrowTransactions
                .Count(t => t.MemberName == memberName && t.Status == "Borrowed");

            if (activeBorrowCount >= 5)
            {
                TempData["BorrowLimitMessage"] = "You cannot borrow more than 5 books at one time.";
                return RedirectToAction("Catalogue", "Home");
            }

            var alreadyBorrowed = _context.BorrowTransactions
                .Any(t => t.MemberName == memberName &&
                          t.BookTitle == book.Title &&
                          t.Status == "Borrowed");

            if (alreadyBorrowed)
            {
                TempData["BorrowLimitMessage"] = "You have already borrowed this book.";
                return RedirectToAction("Catalogue", "Home");
            }

            var transaction = new BorrowTransaction
            {
                MemberName = memberName,
                BookTitle = book.Title,
                BorrowDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(14),
                Status = "Borrowed",
                FineAmount = 0
            };

            _context.BorrowTransactions.Add(transaction);

            book.AvailableCopies--;

            if (book.AvailableCopies < 0)
            {
                book.AvailableCopies = 0;
            }

            book.AvailabilityStatus = book.AvailableCopies > 0 ? "Available" : "Borrowed";

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ReserveFromCatalogue(int bookId)
        {
            var memberName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(memberName))
            {
                return RedirectToAction("Login", "Account");
            }

            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                return NotFound();
            }

            var alreadyReserved = _context.BorrowTransactions
                .Any(t => t.MemberName == memberName &&
                          t.BookTitle == book.Title &&
                          t.Status == "Reserved");

            if (alreadyReserved)
            {
                TempData["BorrowLimitMessage"] = "You have already reserved this book.";
                return RedirectToAction("Catalogue", "Home");
            }

            var reservation = new BorrowTransaction
            {
                MemberName = memberName,
                BookTitle = book.Title,
                BorrowDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(14),
                Status = "Reserved",
                FineAmount = 0
            };

            _context.BorrowTransactions.Add(reservation);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ReturnBook(int id)
        {
            var transaction = _context.BorrowTransactions.FirstOrDefault(t => t.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            if (transaction.Status == "Returned")
            {
                return RedirectToAction(nameof(Index));
            }

            var oldStatus = transaction.Status;

            transaction.Status = "Returned";

            if (DateTime.Now > transaction.ReturnDate)
            {
                var overdueDays = (DateTime.Now.Date - transaction.ReturnDate.Date).Days;
                transaction.FineAmount = overdueDays * 1;
            }

            if (oldStatus == "Borrowed")
            {
                var book = _context.Books.FirstOrDefault(b => b.Title == transaction.BookTitle);

                if (book != null)
                {
                    book.AvailableCopies++;

                    if (book.AvailableCopies > book.TotalCopies)
                    {
                        book.AvailableCopies = book.TotalCopies;
                    }

                    book.AvailabilityStatus = book.AvailableCopies > 0 ? "Available" : "Borrowed";
                }
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowTransaction = await _context.BorrowTransactions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (borrowTransaction == null)
            {
                return NotFound();
            }

            return View(borrowTransaction);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MemberName,BookTitle,BorrowDate,ReturnDate,Status,FineAmount")] BorrowTransaction borrowTransaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(borrowTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(borrowTransaction);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowTransaction = await _context.BorrowTransactions.FindAsync(id);

            if (borrowTransaction == null)
            {
                return NotFound();
            }

            return View(borrowTransaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberName,BookTitle,BorrowDate,ReturnDate,Status,FineAmount")] BorrowTransaction borrowTransaction)
        {
            if (id != borrowTransaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(borrowTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(borrowTransaction);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowTransaction = await _context.BorrowTransactions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (borrowTransaction == null)
            {
                return NotFound();
            }

            return View(borrowTransaction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowTransaction = await _context.BorrowTransactions.FindAsync(id);

            if (borrowTransaction != null)
            {
                _context.BorrowTransactions.Remove(borrowTransaction);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}