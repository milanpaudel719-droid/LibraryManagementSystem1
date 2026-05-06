using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class BorrowingSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowingSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BorrowingSettings
        public async Task<IActionResult> Index()
        {
              return _context.BorrowingSettings != null ? 
                          View(await _context.BorrowingSettings.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.BorrowingSettings'  is null.");
        }

        // GET: BorrowingSettings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BorrowingSettings == null)
            {
                return NotFound();
            }

            var borrowingSetting = await _context.BorrowingSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrowingSetting == null)
            {
                return NotFound();
            }

            return View(borrowingSetting);
        }

        // GET: BorrowingSettings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BorrowingSettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LoanDurationDays,RenewalLimit,OverduePenalty,MaxBorrowableItems")] BorrowingSetting borrowingSetting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(borrowingSetting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(borrowingSetting);
        }

        // GET: BorrowingSettings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BorrowingSettings == null)
            {
                return NotFound();
            }

            var borrowingSetting = await _context.BorrowingSettings.FindAsync(id);
            if (borrowingSetting == null)
            {
                return NotFound();
            }
            return View(borrowingSetting);
        }

        // POST: BorrowingSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LoanDurationDays,RenewalLimit,OverduePenalty,MaxBorrowableItems")] BorrowingSetting borrowingSetting)
        {
            if (id != borrowingSetting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowingSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowingSettingExists(borrowingSetting.Id))
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
            return View(borrowingSetting);
        }

        // GET: BorrowingSettings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BorrowingSettings == null)
            {
                return NotFound();
            }

            var borrowingSetting = await _context.BorrowingSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrowingSetting == null)
            {
                return NotFound();
            }

            return View(borrowingSetting);
        }

        // POST: BorrowingSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BorrowingSettings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BorrowingSettings'  is null.");
            }
            var borrowingSetting = await _context.BorrowingSettings.FindAsync(id);
            if (borrowingSetting != null)
            {
                _context.BorrowingSettings.Remove(borrowingSetting);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowingSettingExists(int id)
        {
          return (_context.BorrowingSettings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
