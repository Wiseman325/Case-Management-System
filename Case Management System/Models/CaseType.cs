using System.ComponentModel.DataAnnotations;

namespace Case_Management_System.Models
{
    public class CaseType
    {
        [Key]
        public int CaseTypeId { get; set; }
        public string CaseTypeName { get; set; }    
    }
}
