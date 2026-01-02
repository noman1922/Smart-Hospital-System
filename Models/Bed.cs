using System.ComponentModel.DataAnnotations;

namespace SmartHospitalSystem.Models
{
    public enum BedType
    {
        General,
        ICU
    }

    public class Bed
    {
        public string BedID { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Ward name is required")]
        [Display(Name = "Ward Name")]
        public string WardName { get; set; } = "";

        [Required(ErrorMessage = "Bed type is required")]
        [Display(Name = "Bed Type")]
        public BedType BedType { get; set; }

        [Display(Name = "Occupied")]
        public bool IsOccupied { get; set; }

        [Required(ErrorMessage = "Last sanitized date is required")]
        [Display(Name = "Last Sanitized")]
        public DateTime LastSanitized { get; set; } = DateTime.Now;

        public string? PatientID { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable => !IsOccupied && (DateTime.Now - LastSanitized).TotalHours <= 48;

        [Display(Name = "Sanitation Status")]
        public string SanitationStatus => (DateTime.Now - LastSanitized).TotalHours > 48 ? "OVERDUE" : "OK";
    }
}