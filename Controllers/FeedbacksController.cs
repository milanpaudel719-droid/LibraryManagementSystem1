using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbacksController(ApplicationDbContext context)
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
                return View(await _context.Feedbacks.ToListAsync());
            }

            return View(await _context.Feedbacks
                .Where(f => f.MemberName == userName)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var borrowedBooks = _context.BorrowTransactions
                .Where(t => t.MemberName == userName &&
                            (t.Status == "Borrowed" || t.Status == "Returned"))
                .Select(t => t.BookTitle)
                .Distinct()
                .ToList();

            if (!borrowedBooks.Any())
            {
                TempData["FeedbackMessage"] = "You can only give feedback for books you have borrowed.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BookTitle = new SelectList(borrowedBooks);

            var feedback = new Feedback
            {
                MemberName = userName
            };

            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            feedback.MemberName = userName;

            var hasBorrowedBook = _context.BorrowTransactions.Any(t =>
                t.MemberName == userName &&
                t.BookTitle == feedback.BookTitle &&
                (t.Status == "Borrowed" || t.Status == "Returned"));

            if (!hasBorrowedBook)
            {
                ModelState.AddModelError("BookTitle", "You can only give feedback for books you have borrowed.");
            }

            if (feedback.Rating < 1 || feedback.Rating > 5)
            {
                ModelState.AddModelError("Rating", "Rating must be between 1 and 5.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var borrowedBooks = _context.BorrowTransactions
                .Where(t => t.MemberName == userName &&
                            (t.Status == "Borrowed" || t.Status == "Returned"))
                .Select(t => t.BookTitle)
                .Distinct()
                .ToList();

            ViewBag.BookTitle = new SelectList(borrowedBooks, feedback.BookTitle);

            return View(feedback);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(m => m.Id == id);

            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "Admin" && userRole != "Staff")
            {
                return RedirectToAction(nameof(Index));
            }

            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.FindAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Feedback feedback)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "Admin" && userRole != "Staff")
            {
                return RedirectToAction(nameof(Index));
            }

            if (id != feedback.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(feedback);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "Admin" && userRole != "Staff")
            {
                return RedirectToAction(nameof(Index));
            }

            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(m => m.Id == id);

            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "Admin" && userRole != "Staff")
            {
                return RedirectToAction(nameof(Index));
            }

            var feedback = await _context.Feedbacks.FindAsync(id);

            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}