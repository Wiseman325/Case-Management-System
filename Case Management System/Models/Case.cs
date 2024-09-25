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
        public string CaseDescription { get; set; }
        public DateOnly IncidentDate { get; set; }
        public TimeOnly IncidentTime { get; set; }
        public string Location { get; set; }
        public string Severity { get; set; }

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
    }
}