namespace SmartHospitalSystem.Models
{
    public enum AlertType
    {
        CriticalVital,
        BedAllocationSuccess,
        BedAllocationFailure,
        SanitationOverdue,
        Emergency
    }

    public class Alert
    {
        public string AlertID { get; set; } = Guid.NewGuid().ToString();
        public AlertType AlertType { get; set; }
        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; }
        public string PatientName { get; set; } = "";
        public string BedInfo { get; set; } = "";
    }
}