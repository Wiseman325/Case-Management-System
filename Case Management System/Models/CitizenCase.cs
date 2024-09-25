using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Case_Management_System.Models
{
    public class CitizenCase
    {
        [Key]
        public int CitizenCaseId { get; set; }

        [Display(Name = "Citezen Id")]
        public string CitizenId { get; set; }
        [ForeignKey("CitizenId")]
        [ValidateNever]
        public ApplicationUser Citizen { get; set; } 
        
        [Display(Name = "Case Number")]
        public int CaseNum { get; set; }
        [ForeignKey("CaseNum")]
        [ValidateNever]
        public Case Case { get; set; }

        public DateOnly DateReported { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
