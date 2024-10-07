using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NuGet.ProjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Case_Management_System.Models
{
    public class Case
    {
        [Key]
        public int CaseNum { get; set; }
        [Required]
        public string CaseDescription { get; set; }
        [Required]
        public DateOnly IncidentDate { get; set; }
        [Required]
        public TimeOnly IncidentTime { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [ValidateNever]
        public string? Evidence { get; set; }
        [ValidateNever]

        public DateOnly DateReported { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Display(Name = "Case Type")]

        public int CaseTypeId { get; set; }
        [ForeignKey("CaseTypeId")]
        [ValidateNever]
        public CaseType CaseType { get; set; }

        [Display(Name = "Citezen Id")]
        public string CitizenId { get; set; }
        [ForeignKey("CitizenId")]
        [ValidateNever]
        public ApplicationUser Citizen { get; set; }

        [Display(Name = "Officer Id")]
        public string? OfficerId { get; set; }
        [ForeignKey("OfficerId")]
        [ValidateNever]
        public ApplicationUser Officer { get; set; }

        public string Status { get; set; } = "Pending";

        public string? StatusReason { get; set; }

    }
}