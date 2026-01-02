using System.Collections.Concurrent;

namespace SmartHospitalSystem.Models
{
    public static class HospitalData
    {
        private static readonly ConcurrentDictionary<string, Patient> Patients = new();
        private static readonly ConcurrentDictionary<string, Bed> Beds = new();
        private static readonly ConcurrentDictionary<string, Alert> Alerts = new();
        private static Timer? _monitoringTimer;

        static HospitalData()
        {
            InitializeSampleData();
            StartMonitoring();
        }

        // Patient Operations
        public static List<Patient> GetAllPatients() => Patients.Values.OrderByDescending(p => p.AdmissionDate).ToList();

        public static Patient? GetPatient(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return Patients.TryGetValue(id, out var patient) ? patient : null;
        }

        public static void AddPatient(Patient patient)
        {
            // Validate and set critical status
            patient.IsCritical = patient.OxygenLevel < 92;
            Patients[patient.PatientID] = patient;

            // Check for critical alerts
            CheckForCriticalAlerts(patient);
        }

        public static void UpdatePatient(Patient patient)
        {
            if (Patients.ContainsKey(patient.PatientID))
            {
                patient.IsCritical = patient.OxygenLevel < 92;
                Patients[patient.PatientID] = patient;
                CheckForCriticalAlerts(patient);
            }
        }

        public static bool DeletePatient(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            return Patients.TryRemove(id, out _);
        }

        // Bed Operations
        public static List<Bed> GetAllBeds() => Beds.Values.OrderBy(b => b.WardName).ThenBy(b => b.BedType).ToList();

        public static Bed? GetBed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return Beds.TryGetValue(id, out var bed) ? bed : null;
        }

        public static void AddBed(Bed bed) => Beds[bed.BedID] = bed;

        public static void UpdateBed(Bed bed)
        {
            if (Beds.ContainsKey(bed.BedID))
                Beds[bed.BedID] = bed;
        }

        public static bool DeleteBed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            if (Beds.TryGetValue(id, out var bed) && !bed.IsOccupied)
                return Beds.TryRemove(id, out _);
            return false;
        }

        // Alert Operations
        public static List<Alert> GetAllAlerts() => Alerts.Values.OrderByDescending(a => a.CreatedAt).ToList();
        public static List<Alert> GetUnreadAlerts() => Alerts.Values.Where(a => !a.IsRead).OrderByDescending(a => a.CreatedAt).ToList();

        public static void AddAlert(Alert alert) => Alerts[alert.AlertID] = alert;

        public static void MarkAlertAsRead(string alertId)
        {
            if (string.IsNullOrEmpty(alertId))
                return;

            if (Alerts.TryGetValue(alertId, out var alert))
            {
                alert.IsRead = true;
                Alerts[alertId] = alert;
            }
        }

        // Business Logic
        public static bool AllocateBedToPatient(string patientId, string bedId)
        {
            if (string.IsNullOrEmpty(patientId) || string.IsNullOrEmpty(bedId))
                return false;

            if (!Patients.TryGetValue(patientId, out var patient) || !Beds.TryGetValue(bedId, out var bed))
                return false;

            // VALIDATION RULES
            // 1. Bed must be sanitized within 48 hours
            if ((DateTime.Now - bed.LastSanitized).TotalHours > 48)
            {
                AddAlert(new Alert
                {
                    AlertType = AlertType.BedAllocationFailure,
                    Message = $"Bed needs sanitation (last: {bed.LastSanitized:MM/dd HH:mm})",
                    PatientName = patient.PatientName,
                    BedInfo = $"{bed.WardName} - {bed.BedType}"
                });
                return false;
            }

            // 2. Prevent two patients in same bed
            if (bed.IsOccupied)
            {
                AddAlert(new Alert
                {
                    AlertType = AlertType.BedAllocationFailure,
                    Message = "Bed is already occupied",
                    PatientName = patient.PatientName,
                    BedInfo = $"{bed.WardName} - {bed.BedType}"
                });
                return false;
            }

            // 3. ICU bed only for critical patients
            if (bed.BedType == BedType.ICU && !patient.IsCritical)
            {
                AddAlert(new Alert
                {
                    AlertType = AlertType.BedAllocationFailure,
                    Message = "ICU bed only for critical patients",
                    PatientName = patient.PatientName,
                    BedInfo = $"{bed.WardName} - {bed.BedType}"
                });
                return false;
            }

            // 4. Critical patients only in ICU
            if (bed.BedType == BedType.General && patient.IsCritical)
            {
                AddAlert(new Alert
                {
                    AlertType = AlertType.BedAllocationFailure,
                    Message = "Critical patients need ICU beds",
                    PatientName = patient.PatientName,
                    BedInfo = $"{bed.WardName} - {bed.BedType}"
                });
                return false;
            }

            // Allocate bed
            bed.IsOccupied = true;
            bed.PatientID = patientId;
            patient.AssignedBedID = bedId;

            Beds[bedId] = bed;
            Patients[patientId] = patient;

            AddAlert(new Alert
            {
                AlertType = AlertType.BedAllocationSuccess,
                Message = $"Bed allocated to {patient.PatientName}",
                PatientName = patient.PatientName,
                BedInfo = $"{bed.WardName} - {bed.BedType}"
            });

            return true;
        }

        public static bool DeallocateBed(string bedId)
        {
            if (string.IsNullOrEmpty(bedId))
                return false;

            if (!Beds.TryGetValue(bedId, out var bed) || !bed.IsOccupied)
                return false;

            // Find patient
            var patient = Patients.Values.FirstOrDefault(p => p.AssignedBedID == bedId);
            if (patient != null)
            {
                patient.AssignedBedID = null;
                Patients[patient.PatientID] = patient;
            }

            bed.IsOccupied = false;
            bed.PatientID = null;
            bed.LastSanitized = DateTime.Now;
            Beds[bedId] = bed;

            return true;
        }

        public static bool SanitizeBed(string bedId)
        {
            if (string.IsNullOrEmpty(bedId))
                return false;

            if (!Beds.TryGetValue(bedId, out var bed))
                return false;

            bed.LastSanitized = DateTime.Now;
            Beds[bedId] = bed;

            return true;
        }

        // Filter Methods
        public static List<Patient> GetCriticalPatients() =>
            Patients.Values.Where(p => p.IsCritical).OrderByDescending(p => p.AdmissionDate).ToList();

        public static List<Patient> GetPatientsRequiringICU() =>
            Patients.Values.Where(p => p.IsCritical && string.IsNullOrEmpty(p.AssignedBedID)).ToList();

        public static List<Patient> GetUnallocatedPatients() =>
            Patients.Values.Where(p => string.IsNullOrEmpty(p.AssignedBedID)).ToList();

        public static List<Bed> GetAvailableBeds() =>
            Beds.Values.Where(b => !b.IsOccupied && (DateTime.Now - b.LastSanitized).TotalHours <= 48).ToList();

        public static List<Bed> GetAvailableICUBeds() =>
            Beds.Values.Where(b => b.BedType == BedType.ICU && !b.IsOccupied && (DateTime.Now - b.LastSanitized).TotalHours <= 48).ToList();

        public static List<Bed> GetSanitationOverdueBeds() =>
            Beds.Values.Where(b => (DateTime.Now - b.LastSanitized).TotalHours > 48).ToList();

        // Private Methods
        private static void CheckForCriticalAlerts(Patient patient)
        {
            if (patient.OxygenLevel < 92)
            {
                AddAlert(new Alert
                {
                    AlertType = AlertType.CriticalVital,
                    Message = $"Critical oxygen: {patient.OxygenLevel}%",
                    PatientName = patient.PatientName
                });
            }

            if (patient.Temperature < 35 || patient.Temperature > 42)
            {
                AddAlert(new Alert
                {
                    AlertType = AlertType.CriticalVital,
                    Message = $"Abnormal temperature: {patient.Temperature}°C",
                    PatientName = patient.PatientName
                });
            }

            if (patient.PulseRate < 60 || patient.PulseRate > 100)
            {
                AddAlert(new Alert
                {
                    AlertType = AlertType.CriticalVital,
                    Message = $"Abnormal pulse: {patient.PulseRate} bpm",
                    PatientName = patient.PatientName
                });
            }
        }

        private static void CheckSanitationAlerts(object? state)
        {
            try
            {
                var overdueBeds = GetSanitationOverdueBeds();
                foreach (var bed in overdueBeds)
                {
                    AddAlert(new Alert
                    {
                        AlertType = AlertType.SanitationOverdue,
                        Message = $"{bed.WardName} bed needs sanitation",
                        BedInfo = $"{bed.WardName} - {bed.BedType}"
                    });
                }
            }
            catch (Exception)
            {
                // Ignore timer errors
            }
        }

        private static void StartMonitoring()
        {
            _monitoringTimer = new Timer(CheckSanitationAlerts, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private static void InitializeSampleData()
        {
            // Clear any existing data
            Patients.Clear();
            Beds.Clear();
            Alerts.Clear();

            // Sample beds
            var beds = new List<Bed>
            {
                new Bed { WardName = "Emergency", BedType = BedType.ICU, LastSanitized = DateTime.Now.AddHours(-24) },
                new Bed { WardName = "Emergency", BedType = BedType.ICU, LastSanitized = DateTime.Now.AddHours(-12) },
                new Bed { WardName = "General", BedType = BedType.General, LastSanitized = DateTime.Now.AddHours(-36) },
                new Bed { WardName = "General", BedType = BedType.General, LastSanitized = DateTime.Now.AddHours(-48) },
                new Bed { WardName = "ICU", BedType = BedType.ICU, LastSanitized = DateTime.Now.AddHours(-6), IsOccupied = true }
            };

            foreach (var bed in beds)
                Beds[bed.BedID] = bed;

            // Sample patients
            var patients = new List<Patient>
            {
                new Patient { PatientName = "John Smith", Age = 45, DiseaseCategory = "Pneumonia",
                             Temperature = 38.5, PulseRate = 95, OxygenLevel = 91 },
                new Patient { PatientName = "Mary Johnson", Age = 62, DiseaseCategory = "Heart Condition",
                             Temperature = 37.2, PulseRate = 85, OxygenLevel = 96 },
                new Patient { PatientName = "Robert Brown", Age = 33, DiseaseCategory = "COVID-19",
                             Temperature = 39.1, PulseRate = 110, OxygenLevel = 88 },
                new Patient { PatientName = "Sarah Wilson", Age = 28, DiseaseCategory = "Broken Arm",
                             Temperature = 36.8, PulseRate = 72, OxygenLevel = 98 }
            };

            foreach (var patient in patients)
            {
                patient.IsCritical = patient.OxygenLevel < 92;
                Patients[patient.PatientID] = patient;
            }

            // Allocate one bed
            if (patients[2].IsCritical && beds[4].BedType == BedType.ICU)
            {
                patients[2].AssignedBedID = beds[4].BedID;
                Patients[patients[2].PatientID] = patients[2];
            }

            // Sample alerts
            AddAlert(new Alert
            {
                AlertType = AlertType.CriticalVital,
                Message = "Patient Robert Brown has critical oxygen level: 88%",
                PatientName = "Robert Brown"
            });

            AddAlert(new Alert
            {
                AlertType = AlertType.BedAllocationSuccess,
                Message = "ICU bed allocated to patient Robert Brown",
                PatientName = "Robert Brown",
                BedInfo = "ICU - ICU"
            });
        }
    }
}