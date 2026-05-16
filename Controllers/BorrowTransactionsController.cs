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
            if (_context.BorrowTransactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BorrowTransactions' is null.");
            }

            var transactions = await _context.BorrowTransactions.ToListAsync();

            return View(transactions);
        }

        [HttpPost]
        public IActionResult CreateFromCatalogue(int bookId)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                return NotFound();
            }

            if (book.AvailableCopies <= 0)
            {
                return RedirectToAction("Catalogue", "Home");
            }

            var memberName = HttpContext.Session.GetString("UserName") ?? "Library Member";

            var activeBorrowCount = _context.BorrowTransactions
                .Count(t => t.MemberName == memberName && t.Status == "Borrowed");

            if (activeBorrowCount >= 5)
            {
                TempData["BorrowLimitMessage"] = "You cannot borrow more than 5 books at one time.";
                return RedirectToAction("Catalogue", "Home");
            }

            var transaction = new BorrowTransaction
            {
                BookTitle = book.Title,
                MemberName = memberName,
                BorrowDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(14),
                Status = "Borrowed",
                FineAmount = 0
            };

            _context.BorrowTransactions.Add(transaction);

            book.AvailableCopies--;

            book.AvailabilityStatus = book.AvailableCopies == 0 ? "Borrowed" : "Available";

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ReturnBook(int id)
        {
            var transaction = _context.BorrowTransactions
                .FirstOrDefault(t => t.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            if (transaction.Status == "Returned")
            {
                return RedirectToAction(nameof(Index));
            }

            transaction.Status = "Returned";

            var book = _context.Books
                .FirstOrDefault(b => b.Title == transaction.BookTitle);

            if (book != null)
            {
                book.AvailableCopies++;

                if (book.AvailableCopies > book.TotalCopies)
                {
                    book.AvailableCopies = book.TotalCopies;
                }

                book.AvailabilityStatus = book.AvailableCopies > 0 ? "Available" : "Borrowed";
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BorrowTransactions == null)
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
            if (id == null || _context.BorrowTransactions == null)
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
                try
                {
                    _context.Update(borrowTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowTransactionExists(borrowTransaction.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(borrowTransaction);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BorrowTransactions == null)
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
            if (_context.BorrowTransactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BorrowTransactions' is null.");
            }

            var borrowTransaction = await _context.BorrowTransactions.FindAsync(id);

            if (borrowTransaction != null)
            {
                _context.BorrowTransactions.Remove(borrowTransaction);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool BorrowTransactionExists(int id)
        {
            return (_context.BorrowTransactions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}