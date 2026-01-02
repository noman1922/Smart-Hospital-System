using System.ComponentModel.DataAnnotations;

namespace SmartHospitalSystem.Models
{
    public class Patient
    {
        public string PatientID { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Patient name is required")]
        [Display(Name = "Patient Name")]
        public string PatientName { get; set; } = "";

        [Required(ErrorMessage = "Age is required")]
        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Disease category is required")]
        [Display(Name = "Disease Category")]
        public string DiseaseCategory { get; set; } = "";

        [Required(ErrorMessage = "Temperature is required")]
        [Range(35.0, 42.0, ErrorMessage = "Temperature must be between 35°C and 42°C")]
        public double Temperature { get; set; }

        [Required(ErrorMessage = "Pulse rate is required")]
        [Range(20, 250, ErrorMessage = "Pulse rate must be between 20 and 250")]
        [Display(Name = "Pulse Rate")]
        public int PulseRate { get; set; }

        [Required(ErrorMessage = "Oxygen level is required")]
        [Range(0, 100, ErrorMessage = "Oxygen level must be between 0% and 100%")]
        [Display(Name = "Oxygen Level (%)")]
        public double OxygenLevel { get; set; }

        [Display(Name = "Critical Status")]
        public bool IsCritical { get; set; }

        public string? AssignedBedID { get; set; }
        public DateTime AdmissionDate { get; set; } = DateTime.Now;

        // Navigation property
        public Bed? AssignedBed { get; set; }
    }
}