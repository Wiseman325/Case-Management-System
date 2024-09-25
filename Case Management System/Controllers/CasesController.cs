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
    public class CasesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public CasesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Cases
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.cases.Include(p => p.CaseType).Include(p => p.Citizen).Include(p => p.Officer);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Cases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pcase = await _context.cases
                .Include(p => p.CaseType)
                .Include(p => p.Citizen)
                .Include(p => p.Officer)
                .FirstOrDefaultAsync(m => m.CaseNum == id);
            if (pcase == null)
            {
                return NotFound();
            }

            return View(pcase);
        }

        // GET: Cases/Create
        public IActionResult Create()
        {
            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeId");
            ViewData["CitizenId"] = new SelectList(_context.applicationUsers, "Id", "Id");
            ViewData["OfficerId"] = new SelectList(_context.applicationUsers, "Id", "Id");
            return View();
        }

        // POST: Cases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CaseNum,CaseDescription,IncidentDate,IncidentTime,Location,Severity,DateReported,CaseTypeId,CitizenId,OfficerId")] Case pcase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pcase);
                await _context.SaveChangesAsync();
                if (User.IsInRole(SD.Role_StationCommander) || User.IsInRole(SD.Role_Officer))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }


            // Same filtering logic for the Foreman dropdown in case the form is reloaded
            var citizenRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Citizen")?.Id;
            var citizen = _context.UserRoles
                                  .Where(ur => ur.RoleId == citizenRoleId)
                                  .Select(ur => ur.UserId).ToList();

            var foremanUsers = _context.applicationUsers
                                       .Where(u => citizen.Contains(u.Id))
                                       .Select(u => new { u.Id, u.UserName })
                                       .ToList();

            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeId", pcase.CaseTypeId);
            ViewData["CitizenId"] = new SelectList(_context.applicationUsers, "Id", "Id", pcase.CitizenId);
            ViewData["OfficerId"] = new SelectList(_context.applicationUsers, "Id", "Id", pcase.OfficerId);
            return View(pcase);
        }

        // GET: Cases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pcase = await _context.cases.FindAsync(id);
            if (pcase == null)
            {
                return NotFound();
            }
            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeId", pcase.CaseTypeId);
            ViewData["CitizenId"] = new SelectList(_context.applicationUsers, "Id", "Id", pcase.CitizenId);
            ViewData["OfficerId"] = new SelectList(_context.applicationUsers, "Id", "Id", pcase.OfficerId);
            return View(pcase);
        }

        // POST: Cases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CaseNum,CaseDescription,IncidentDate,IncidentTime,Location,Severity,DateReported,CaseTypeId,CitizenId,OfficerId")] Case pcase)
        {
            if (id != pcase.CaseNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pcase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CaseExists(pcase.CaseNum))
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
            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeId", pcase.CaseTypeId);
            ViewData["CitizenId"] = new SelectList(_context.applicationUsers, "Id", "Id", pcase.CitizenId);
            ViewData["OfficerId"] = new SelectList(_context.applicationUsers, "Id", "Id", pcase.OfficerId);
            return View(pcase);
        }

        // GET: Cases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pcase = await _context.cases
                .Include(p => p.CaseType)
                .Include(p => p.Citizen)
                .Include(p => p.Officer)
                .FirstOrDefaultAsync(m => m.CaseNum == id);
            if (pcase == null)
            {
                return NotFound();
            }

            return View(pcase);
        }

        // POST: Cases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pcase = await _context.cases.FindAsync(id);
            if (pcase != null)
            {
                _context.cases.Remove(pcase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CaseExists(int id)
        {
            return _context.cases.Any(e => e.CaseNum == id);
        }
    }
}
