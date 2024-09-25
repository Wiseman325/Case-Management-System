using Case_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Case_Management_System.DB
{
    public class ApplicationDBContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<Case> cases { get; set; }
        public DbSet<CaseType> casesType { get; set; }
        public DbSet<CitizenCase> citizenCases { get; set; }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
    }
}
