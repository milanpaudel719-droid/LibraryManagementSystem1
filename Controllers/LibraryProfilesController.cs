using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class LibraryProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibraryProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.LibraryProfiles.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryProfile = await _context.LibraryProfiles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (libraryProfile == null)
            {
                return NotFound();
            }

            return View(libraryProfile);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LibraryName,Location,OperatingHours,ContactDetails")] LibraryProfile libraryProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libraryProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(libraryProfile);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryProfile = await _context.LibraryProfiles.FindAsync(id);

            if (libraryProfile == null)
            {
                return NotFound();
            }

            return View(libraryProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LibraryName,Location,OperatingHours,ContactDetails")] LibraryProfile libraryProfile)
        {
            if (id != libraryProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(libraryProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(libraryProfile);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryProfile = await _context.LibraryProfiles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (libraryProfile == null)
            {
                return NotFound();
            }

            return View(libraryProfile);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libraryProfile = await _context.LibraryProfiles.FindAsync(id);

            if (libraryProfile != null)
            {
                _context.LibraryProfiles.Remove(libraryProfile);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}