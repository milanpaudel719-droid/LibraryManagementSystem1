using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // REGISTER PAGE
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER SAVE
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // LOGIN PAGE
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN CHECK
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Email == email && x.Password == password);

            if (user != null)
            {
                return RedirectToAction("Index", "Books");
            }

            ViewBag.Message = "Invalid Email or Password";
            return View();
        }
    }
}