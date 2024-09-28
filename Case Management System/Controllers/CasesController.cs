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
using Microsoft.AspNetCore.Authorization;

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
                pcase.Status = "Pending";
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

            ViewData["Status"] = new SelectList(new List<string> { "Pending", "In Progress", "Resolved" }, pcase.Status);

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
        public async Task<IActionResult> Edit(int id, [Bind("CaseNum,CaseDescription,IncidentDate,IncidentTime,Location,Severity,DateReported,Status,CaseTypeId,CitizenId,OfficerId")] Case pcase)
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

                return RedirectToAction(nameof(OfficerCases));
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

            ViewData["Status"] = new SelectList(new List<string> { "Pending", "In Progress", "Resolved" }, pcase.Status);

            return View(pcase);
        }


        public async Task<IActionResult> AssignOfficer(int? id)
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

            // Populate the Severity dropdown
            ViewData["Severity"] = new SelectList(new List<string> { "Low", "Medium", "High" }, pcase.Severity);

            // Populate CaseType dropdown
            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeName", pcase.CaseTypeId);

            // Populate Citizen dropdown
            var citizenRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Citizen")?.Id;
            var citizenUserIds = _context.UserRoles.Where(ur => ur.RoleId == citizenRoleId).Select(ur => ur.UserId).ToList();
            var citizenUsers = _context.applicationUsers.Where(u => citizenUserIds.Contains(u.Id)).ToList();
            ViewData["CitizenId"] = new SelectList(citizenUsers, "Id", "UserName", pcase.CitizenId);

            // Populate Officer dropdown
            var officerRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Officer")?.Id;
            var officerUserIds = _context.UserRoles.Where(ur => ur.RoleId == officerRoleId).Select(ur => ur.UserId).ToList();
            var officerUsers = _context.applicationUsers.Where(u => officerUserIds.Contains(u.Id)).ToList();
            ViewData["OfficerId"] = new SelectList(officerUsers, "Id", "UserName", pcase.OfficerId);

            return View(pcase);
        }

        // POST: Cases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignOfficer(int id, [Bind("CaseNum,CaseDescription,IncidentDate,IncidentTime,Location,Severity,DateReported,CaseTypeId,CitizenId,OfficerId")] Case pcase)
        {
            if (id != pcase.CaseNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pcase.Status = "Officer Assigned";
                    _context.Update(pcase);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Officer assigned to the case Successfully";
                    return RedirectToAction(nameof(Index));
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
                //return RedirectToAction(nameof(Index));

            }

            // If ModelState is invalid, repopulate the SelectLists to re-render the form correctly
            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeName", pcase.CaseTypeId);

            var citizenRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Citizen")?.Id;
            var citizenUserIds = _context.UserRoles.Where(ur => ur.RoleId == citizenRoleId).Select(ur => ur.UserId).ToList();
            var citizenUsers = _context.applicationUsers.Where(u => citizenUserIds.Contains(u.Id)).ToList();
            ViewData["CitizenId"] = new SelectList(citizenUsers, "Id", "UserName", pcase.CitizenId);

            var officerRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Officer")?.Id;
            var officerUserIds = _context.UserRoles.Where(ur => ur.RoleId == officerRoleId).Select(ur => ur.UserId).ToList();
            var officerUsers = _context.applicationUsers.Where(u => officerUserIds.Contains(u.Id)).ToList();
            ViewData["OfficerId"] = new SelectList(officerUsers, "Id", "UserName", pcase.OfficerId);

            ViewData["Severity"] = new SelectList(new List<string> { "Low", "Medium", "High" }, pcase.Severity);

            ViewData["Status"] = new SelectList(new List<string> { "Pending", "In Progress", "Resolved" }, pcase.Status);

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

        public async Task<IActionResult> OfficerCases()
        {
            // Get the currently logged-in officer's ID
            var officerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the cases assigned to this officer
            var assignedCases = await _context.cases
                .Include(c => c.CaseType) // Include related case type information
                .Include(c => c.Citizen)  // Include citizen information
                .Where(c => c.OfficerId == officerId) // Filter by the officer's ID
                .ToListAsync();

            // Return the view with the assigned cases
            return View(assignedCases);
        }


        public async Task<IActionResult> TrackCases()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationDBContext = await _context.cases
                .Where(c => c.CitizenId == loggedInUserId)  // Only fetch cases for the logged-in citizen
                .Include(p => p.CaseType)
                .Include(p => p.Officer)
                .ToListAsync();

            return View(applicationDBContext);
        }



        //[HttpGet]
        //[Authorize(Roles = "Citizen")]
        //public async Task<IActionResult> TrackCases()
        //{
        //    // Retrieve the current logged-in citizen's ID
        //    var citizenId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (string.IsNullOrEmpty(citizenId))
        //    {
        //        TempData["error"] = "You must be logged in to view your cases.";
        //        return RedirectToAction("Login", "Account");
        //    }

        //    try
        //    {
        //        // Fetch all cases reported by this citizen
        //        var citizenCases = await _context.citizenCases
        //            .Where(c => c.CitizenId == citizenId)
        //            .Include(c => c.Case)  // Include related case details
        //            .ToListAsync();

        //        // Check if any cases were found for the citizen
        //        if (!citizenCases.Any())
        //        {
        //            TempData["info"] = "No cases found. You haven't reported any cases yet.";
        //            return RedirectToAction("Index", "Cases");  // Redirect to Cases index if no cases exist
        //        }

        //        // Display the cases for the citizen
        //        return View(citizenCases);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (logging logic to be implemented based on your logging framework)
        //        TempData["error"] = "An error occurred while fetching your cases. Please try again later.";
        //        return RedirectToAction("Index", "Cases");  // Redirect to Cases index on error
        //    }
        //}


    }

}
