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
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Case_Management_System.Controllers
{
    public class CasesController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CasesController(ApplicationDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CaseNum,CaseDescription,IncidentDate,IncidentTime,Location,StreetAddress,DateReported,CaseTypeId,CitizenId,OfficerId,StatusReason")] Case pcase, IFormFile file)
        {
            //string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            //if (!Directory.Exists(uploadsFolder))
            //{
            //    Directory.CreateDirectory(uploadsFolder);
            //}
            //string fileName = Path.GetFileName(file.FileName);
            //string fileSavedPath = Path.Combine(uploadsFolder, fileName);

            //using (FileStream stream = new FileStream(fileSavedPath, FileMode.Create))
            //{
            //    await file.CopyToAsync(stream);
            //}
            //ViewBag.Message = fileName + " uploaded successfully!";

            
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            pcase.CitizenId = loggedInUserId;

            if (!ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string evidencePath = Path.Combine(wwwRootPath, @"files\evidence");

                    if (!string.IsNullOrEmpty(pcase.Evidence))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, pcase.Evidence.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(evidencePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    pcase.Evidence = @"files\evidence\" + fileName;
                }

                // Set Priority based on CaseNum
                pcase.SetPriority();
                pcase.Status = "Pending";

                _context.Add(pcase);
                await _context.SaveChangesAsync();

                TempData["success"] = "Case Reported Successfully";
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdowns if validation fails
            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeName", pcase.CaseTypeId);
            ViewData["CitizenId"] = new SelectList(_context.applicationUsers, "Id", "UserName", pcase.CitizenId);
            ViewData["OfficerId"] = new SelectList(_context.applicationUsers, "Id", "UserName", pcase.OfficerId);

            return View(pcase);
        }


        // GET: Cases/Edit/5
        public async Task<IActionResult> Edit(int? id, IFormFile? file)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CaseNum,CaseDescription,IncidentDate,IncidentTime,Location,StreetAddress,Evidence,DateReported,CaseTypeId,CitizenId,OfficerId,Status,StatusReason")] Case pcase, IFormFile? file)
        {
            if (id != pcase.CaseNum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string evidencePath = Path.Combine(wwwRootPath, @"files\evidence");

                        if (!string.IsNullOrEmpty(pcase.Evidence))
                        {
                            var oldImagePath = Path.Combine(wwwRootPath, pcase.Evidence.TrimStart('\\'));

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
S
                        using (var fileStream = new FileStream(Path.Combine(evidencePath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        pcase.Evidence = @"files\evidence\" + fileName;
                    }
                    // Set Priority based on CaseNum
                    pcase.SetPriority();
                    //if(pcase.CaseNum == 0)
                    //{

                    //}

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

            // Repopulate dropdowns if validation fails
            ViewData["CaseTypeId"] = new SelectList(_context.casesType, "CaseTypeId", "CaseTypeName", pcase.CaseTypeId);
            ViewData["CitizenId"] = new SelectList(_context.applicationUsers, "Id", "UserName", pcase.CitizenId);
            ViewData["OfficerId"] = new SelectList(_context.applicationUsers, "Id", "UserName", pcase.OfficerId);

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
            //ViewData["Severity"] = new SelectList(new List<string> { "Low", "Medium", "High" }, pcase.Severity);

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

            //ViewData["Severity"] = new SelectList(new List<string> { "Low", "Medium", "High" }, pcase.Severity);

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

    }

}
