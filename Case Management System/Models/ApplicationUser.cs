using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Case_Management_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "Postal code must be exactly 4 characters long.")]
        public string? PostalCode { get; set; }
    }
}
