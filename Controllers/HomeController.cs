using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;

namespace LibraryManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var books = _context.Books.ToList();
        var transactions = _context.BorrowTransactions.ToList();

        ViewBag.NewArrivals = books
            .OrderByDescending(b => b.Id)
            .Take(4)
            .ToList();

        ViewBag.MostBorrowed = transactions
            .GroupBy(t => t.BookTitle)
            .Select(g => new
            {
                BookTitle = g.Key,
                BorrowCount = g.Count()
            })
            .OrderByDescending(x => x.BorrowCount)
            .Take(4)
            .ToList();

        ViewBag.RecommendedBooks = books
            .Where(b => b.AvailableCopies > 0)
            .Take(4)
            .ToList();

        ViewBag.AvailableBooks = books
            .Where(b => b.AvailableCopies > 0)
            .Take(4)
            .ToList();

        return View();
    }

    public IActionResult Catalogue(string searchString)
    {
        var books = _context.Books.ToList();

        if (!string.IsNullOrEmpty(searchString))
        {
            books = books.Where(b =>
                b.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                b.Genre.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                b.ISBN.Contains(searchString, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        ViewData["CurrentFilter"] = searchString;
        return View(books);
    }

    public IActionResult NewArrivals()
    {
        var books = _context.Books
            .OrderByDescending(b => b.Id)
            .Take(20)
            .ToList();

        return View("Catalogue", books);
    }

    public IActionResult MostBorrowed()
    {
        var transactions = _context.BorrowTransactions.ToList();

        var popularTitles = transactions
            .GroupBy(t => t.BookTitle)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        var books = _context.Books
            .ToList()
            .Where(b => popularTitles.Contains(b.Title))
            .ToList();

        return View("Catalogue", books);
    }

    public IActionResult RecommendedBooks()
    {
        var books = _context.Books
            .ToList()
            .Where(b => b.AvailableCopies > 0)
            .Take(20)
            .ToList();

        return View("Catalogue", books);
    }

    public IActionResult AvailableBooks()
    {
        var books = _context.Books
            .ToList()
            .Where(b => b.AvailableCopies > 0)
            .ToList();

        return View("Catalogue", books);
    }

    public IActionResult Notifications()
    {
        return View();
    }

    public IActionResult BorrowingGuidelines()
    {
        return View();
    }

    public IActionResult ContactSupport()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}