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
        ViewBag.NewArrivals = _context.Books
            .OrderByDescending(b => b.Id)
            .Take(4)
            .ToList();

        ViewBag.MostBorrowed = _context.BorrowTransactions
            .GroupBy(t => t.BookTitle)
            .Select(g => new
            {
                BookTitle = g.Key,
                BorrowCount = g.Count()
            })
            .OrderByDescending(x => x.BorrowCount)
            .Take(4)
            .ToList();

        ViewBag.RecommendedBooks = _context.Books
            .Where(b => b.AvailableCopies > 0)
            .OrderBy(b => Guid.NewGuid())
            .Take(4)
            .ToList();

        ViewBag.AvailableBooks = _context.Books
            .Where(b => b.AvailableCopies > 0)
            .Take(4)
            .ToList();

        return View();
    }

    public IActionResult Catalogue(string searchString)
    {
        var books = _context.Books.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            books = books.Where(b =>
                b.Title.Contains(searchString) ||
                b.Author.Contains(searchString) ||
                b.Genre.Contains(searchString) ||
                b.ISBN.Contains(searchString));
        }

        ViewData["CurrentFilter"] = searchString;

        return View(books.ToList());
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