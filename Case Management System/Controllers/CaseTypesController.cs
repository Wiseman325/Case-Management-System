using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Case_Management_System.DB;
using Case_Management_System.Models;

namespace Case_Management_System.Controllers
{
    public class CaseTypesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public CaseTypesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: CaseTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.casesType.ToListAsync());
        }

        // GET: CaseTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caseType = await _context.casesType
                .FirstOrDefaultAsync(m => m.CaseTypeId == id);
            if (caseType == null)
            {
                return NotFound();
            }

            return View(caseType);
        }

        // GET: CaseTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CaseTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CaseTypeId,CaseTypeName")] CaseType caseType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(caseType);
                await _context.SaveChangesAsync();
                TempData["success"] = "Case Type Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(caseType);
        }

        // GET: CaseTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caseType = await _context.casesType.FindAsync(id);
            if (caseType == null)
            {
                return NotFound();
            }
            return View(caseType);
        }

        // POST: CaseTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CaseTypeId,CaseTypeName")] CaseType caseType)
        {
            if (id != caseType.CaseTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(caseType);
                    TempData["success"] = "Case Type Updated Successfully";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CaseTypeExists(caseType.CaseTypeId))
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
            return View(caseType);
        }

        // GET: CaseTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caseType = await _context.casesType
                .FirstOrDefaultAsync(m => m.CaseTypeId == id);
            if (caseType == null)
            {
                return NotFound();
            }

            return View(caseType);
        }

        // POST: CaseTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var caseType = await _context.casesType.FindAsync(id);
            if (caseType != null)
            {
                _context.casesType.Remove(caseType);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Case Type Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool CaseTypeExists(int id)
        {
            return _context.casesType.Any(e => e.CaseTypeId == id);
        }
    }
}
