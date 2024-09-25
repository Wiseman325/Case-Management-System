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
    public class CitizenCasesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public CitizenCasesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: CitizenCases
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.citizenCases.Include(c => c.Case).Include(c => c.Citizen);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: CitizenCases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizenCase = await _context.citizenCases
                .Include(c => c.Case)
                .Include(c => c.Citizen)
                .FirstOrDefaultAsync(m => m.CitizenCaseId == id);
            if (citizenCase == null)
            {
                return NotFound();
            }

            return View(citizenCase);
        }

        // GET: CitizenCases/Create
        public IActionResult Create()
        {
            ViewData["CaseNum"] = new SelectList(_context.cases, "CaseNum", "CaseNum");
            ViewData["CitizenId"] = new SelectList(_context.applicationUsers, "Id", "Id");
            return View();
        }

        // POST: CitizenCases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CitizenCaseId,CitizenId,CaseNum,DateReported,Status")] CitizenCase citizenCase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(citizenCase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CaseNum"] = new SelectList(_context.cases, "CaseNum", "CaseNum", citizenCase.CaseNum);
            ViewData["CitizenId"] = new SelectList(_context.applicationUsers, "Id", "Id", citizenCase.CitizenId);
            return View(citizenCase);
        }

        // GET: CitizenCases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizenCase = await _context.citizenCases.FindAsync(id);
            if (citizenCase == null)
            {
                return NotFound();
            }
            ViewData["CaseNum"] = new SelectList(_context.cases, "CaseNum", "CaseNum", citizenCase.CaseNum);
            ViewData["CitizenId"] = new SelectList(_context.applicationUsers, "Id", "Id", citizenCase.CitizenId);
            return View(citizenCase);
        }

        // POST: CitizenCases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CitizenCaseId,CitizenId,CaseNum,DateReported,Status")] CitizenCase citizenCase)
        {
            if (id != citizenCase.CitizenCaseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(citizenCase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitizenCaseExists(citizenCase.CitizenCaseId))
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
            ViewData["CaseNum"] = new SelectList(_context.cases, "CaseNum", "CaseNum", citizenCase.CaseNum);
            ViewData["CitizenId"] = new SelectList(_context.applicationUsers, "Id", "Id", citizenCase.CitizenId);
            return View(citizenCase);
        }

        // GET: CitizenCases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizenCase = await _context.citizenCases
                .Include(c => c.Case)
                .Include(c => c.Citizen)
                .FirstOrDefaultAsync(m => m.CitizenCaseId == id);
            if (citizenCase == null)
            {
                return NotFound();
            }

            return View(citizenCase);
        }

        // POST: CitizenCases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var citizenCase = await _context.citizenCases.FindAsync(id);
            if (citizenCase != null)
            {
                _context.citizenCases.Remove(citizenCase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CitizenCaseExists(int id)
        {
            return _context.citizenCases.Any(e => e.CitizenCaseId == id);
        }
    }
}
