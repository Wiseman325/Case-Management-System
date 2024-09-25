using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Case_Management_System.DB;
using Case_Management_System.Models;
using System.Security.Claims;

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
        // GET: Cases/Create
        public IActionResult Create()
        {

            ViewData["Severity"] = new SelectList(new List<string> { "Low", "Medium", "High" });

            // Filter and populate CaseType dropdown with CaseTypeName
            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeName");

            // Filter users with the "Citizen" role and populate dropdown with UserName
            var citizenRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Citizen")?.Id;
            var citizenUserIds = _context.UserRoles.Where(ur => ur.RoleId == citizenRoleId).Select(ur => ur.UserId).ToList();
            var citizenUsers = _context.applicationUsers.Where(u => citizenUserIds.Contains(u.Id)).ToList();
            ViewData["CitizenId"] = new SelectList(citizenUsers, "Id", "UserName");

            // Filter users with the "Officer" role and populate dropdown with UserName
            var officerRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Officer")?.Id;
            var officerUserIds = _context.UserRoles.Where(ur => ur.RoleId == officerRoleId).Select(ur => ur.UserId).ToList();
            var officerUsers = _context.applicationUsers.Where(u => officerUserIds.Contains(u.Id)).ToList();
            ViewData["OfficerId"] = new SelectList(officerUsers, "Id", "UserName");

            return View();
        }

        // POST: Cases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CaseNum,CaseDescription,IncidentDate,IncidentTime,Location,Severity,DateReported,CaseTypeId,CitizenId,OfficerId")] Case pcase)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            pcase.CitizenId = loggedInUserId;

            //if (pcase.IncidentDate > DateTime.Today)
            //{
            //    ModelState.AddModelError("IncidentDate", "Incident date cannot be in the future.");
            //}

            if (!ModelState.IsValid)
            {
                _context.Add(pcase);
                await _context.SaveChangesAsync();
                TempData["success"] = "Case Reported Successfully";

                if (User.IsInRole(SD.Role_StationCommander) || User.IsInRole(SD.Role_Officer))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // Re-populate dropdowns if the form needs to be reloaded after validation errors
            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeName", pcase.CaseTypeId);

            var citizenRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Citizen")?.Id;
            var citizenUserIds = _context.UserRoles.Where(ur => ur.RoleId == citizenRoleId).Select(ur => ur.UserId).ToList();
            var citizenUsers = _context.applicationUsers.Where(u => citizenUserIds.Contains(u.Id)).ToList();
            ViewData["CitizenId"] = new SelectList(citizenUsers, "Id", "UserName", pcase.CitizenId);

            var officerRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Officer")?.Id;
            var officerUserIds = _context.UserRoles.Where(ur => ur.RoleId == officerRoleId).Select(ur => ur.UserId).ToList();
            var officerUsers = _context.applicationUsers.Where(u => officerUserIds.Contains(u.Id)).ToList();
            ViewData["OfficerId"] = new SelectList(officerUsers, "Id", "UserName", pcase.OfficerId);
            ViewData["Severity"] = new SelectList(new List<string> { "Low", "Medium", "High" });

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
            ViewData["Severity"] = new SelectList(new List<string> { "Low", "Medium", "High" });

            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeName", pcase.CaseTypeId);

            var citizenRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Citizen")?.Id;
            var citizenUserIds = _context.UserRoles.Where(ur => ur.RoleId == citizenRoleId).Select(ur => ur.UserId).ToList();
            var citizenUsers = _context.applicationUsers.Where(u => citizenUserIds.Contains(u.Id)).ToList();
            ViewData["CitizenId"] = new SelectList(citizenUsers, "Id", "UserName", pcase.CitizenId);

            var officerRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Officer")?.Id;
            var officerUserIds = _context.UserRoles.Where(ur => ur.RoleId == officerRoleId).Select(ur => ur.UserId).ToList();
            var officerUsers = _context.applicationUsers.Where(u => officerUserIds.Contains(u.Id)).ToList();
            ViewData["OfficerId"] = new SelectList(officerUsers, "Id", "UserName", pcase.OfficerId);

            return View(pcase);
        }

        // POST: Cases/Edit/5
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
                    TempData["success"] = "Case Updated Successfully";
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

            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeName", pcase.CaseTypeId);

            var citizenRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Citizen")?.Id;
            var citizenUserIds = _context.UserRoles.Where(ur => ur.RoleId == citizenRoleId).Select(ur => ur.UserId).ToList();
            var citizenUsers = _context.applicationUsers.Where(u => citizenUserIds.Contains(u.Id)).ToList();
            ViewData["CitizenId"] = new SelectList(citizenUsers, "Id", "UserName", pcase.CitizenId);

            var officerRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Officer")?.Id;
            var officerUserIds = _context.UserRoles.Where(ur => ur.RoleId == officerRoleId).Select(ur => ur.UserId).ToList();
            var officerUsers = _context.applicationUsers.Where(u => officerUserIds.Contains(u.Id)).ToList();
            ViewData["OfficerId"] = new SelectList(officerUsers, "Id", "UserName", pcase.OfficerId);
            ViewData["Severity"] = new SelectList(new List<string> { "Low", "Medium", "High" });

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
            TempData["success"] = "Case Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool CaseExists(int id)
        {
            return _context.cases.Any(e => e.CaseNum == id);
        }
    }
}
