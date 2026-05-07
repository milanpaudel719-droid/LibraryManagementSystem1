using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult UserDashboard()
        {
            return View();
        }

        public IActionResult StaffDashboard()
        {
            return View();
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}