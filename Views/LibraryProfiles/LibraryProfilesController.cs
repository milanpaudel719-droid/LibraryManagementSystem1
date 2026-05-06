using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Views.LibraryProfiles
{
    public class LibraryProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibraryProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LibraryProfiles
        public async Task<IActionResult> Index()
        {
              return _context.LibraryProfiles != null ? 
                          View(await _context.LibraryProfiles.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.LibraryProfiles'  is null.");
        }

        // GET: LibraryProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LibraryProfiles == null)
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

        // GET: LibraryProfiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LibraryProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: LibraryProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LibraryProfiles == null)
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

        // POST: LibraryProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                try
                {
                    _context.Update(libraryProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryProfileExists(libraryProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(libraryProfile);
        }

        // GET: LibraryProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LibraryProfiles == null)
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

        // POST: LibraryProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LibraryProfiles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.LibraryProfiles'  is null.");
            }
            var libraryProfile = await _context.LibraryProfiles.FindAsync(id);
            if (libraryProfile != null)
            {
                _context.LibraryProfiles.Remove(libraryProfile);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibraryProfileExists(int id)
        {
          return (_context.LibraryProfiles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
