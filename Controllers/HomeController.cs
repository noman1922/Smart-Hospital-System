using Microsoft.AspNetCore.Mvc;
using SmartHospitalSystem.Models;

namespace SmartHospitalSystem.Controllers
{
    public class HomeController : Controller
    {
        // Dashboard
        public IActionResult Index()
        {
            ViewBag.TotalPatients = HospitalData.GetAllPatients().Count;
            ViewBag.CriticalPatients = HospitalData.GetCriticalPatients().Count;
            ViewBag.AvailableBeds = HospitalData.GetAvailableBeds().Count;
            ViewBag.OverdueBeds = HospitalData.GetSanitationOverdueBeds().Count;
            ViewBag.UnreadAlerts = HospitalData.GetUnreadAlerts().Count;

            return View();
        }

        // Patient Management
        public IActionResult Patients()
        {
            return View(HospitalData.GetAllPatients());
        }

        public IActionResult CreatePatient() => View();

        [HttpPost]
        public IActionResult CreatePatient(Patient patient)
        {
            if (ModelState.IsValid)
            {
                HospitalData.AddPatient(patient);
                TempData["Message"] = "Patient added successfully!";
                return RedirectToAction("Patients");
            }
            return View(patient);
        }

        public IActionResult EditPatient(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Patient ID is required!";
                return RedirectToAction("Patients");
            }

            var patient = HospitalData.GetPatient(id);
            if (patient == null)
            {
                TempData["Error"] = "Patient not found!";
                return RedirectToAction("Patients");
            }

            return View(patient);
        }

        [HttpPost]
        public IActionResult EditPatient(string id, Patient patient)
        {
            if (string.IsNullOrEmpty(id) || id != patient.PatientID)
            {
                TempData["Error"] = "Invalid patient ID!";
                return RedirectToAction("Patients");
            }

            if (ModelState.IsValid)
            {
                HospitalData.UpdatePatient(patient);
                TempData["Message"] = "Patient updated successfully!";
                return RedirectToAction("Patients");
            }
            return View(patient);
        }

        public IActionResult DeletePatient(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Patient ID is required!";
                return RedirectToAction("Patients");
            }

            var patient = HospitalData.GetPatient(id);
            if (patient == null)
            {
                TempData["Error"] = "Patient not found!";
                return RedirectToAction("Patients");
            }

            return View(patient);
        }

        [HttpPost, ActionName("DeletePatient")]
        public IActionResult DeletePatientConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Patient ID is required!";
                return RedirectToAction("Patients");
            }

            HospitalData.DeletePatient(id);
            TempData["Message"] = "Patient deleted successfully!";
            return RedirectToAction("Patients");
        }

        // Bed Management
        public IActionResult Beds() => View(HospitalData.GetAllBeds());

        public IActionResult CreateBed() => View();

        [HttpPost]
        public IActionResult CreateBed(Bed bed)
        {
            if (ModelState.IsValid)
            {
                HospitalData.AddBed(bed);
                TempData["Message"] = "Bed added successfully!";
                return RedirectToAction("Beds");
            }
            return View(bed);
        }

        public IActionResult EditBed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Bed ID is required!";
                return RedirectToAction("Beds");
            }

            var bed = HospitalData.GetBed(id);
            if (bed == null)
            {
                TempData["Error"] = "Bed not found!";
                return RedirectToAction("Beds");
            }

            return View(bed);
        }

        [HttpPost]
        public IActionResult EditBed(string id, Bed bed)
        {
            if (string.IsNullOrEmpty(id) || id != bed.BedID)
            {
                TempData["Error"] = "Invalid bed ID!";
                return RedirectToAction("Beds");
            }

            if (ModelState.IsValid)
            {
                HospitalData.UpdateBed(bed);
                TempData["Message"] = "Bed updated successfully!";
                return RedirectToAction("Beds");
            }
            return View(bed);
        }

        public IActionResult DeleteBed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Bed ID is required!";
                return RedirectToAction("Beds");
            }

            var bed = HospitalData.GetBed(id);
            if (bed == null)
            {
                TempData["Error"] = "Bed not found!";
                return RedirectToAction("Beds");
            }

            return View(bed);
        }

        [HttpPost, ActionName("DeleteBed")]
        public IActionResult DeleteBedConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Bed ID is required!";
                return RedirectToAction("Beds");
            }

            if (HospitalData.DeleteBed(id))
                TempData["Message"] = "Bed deleted successfully!";
            else
                TempData["Error"] = "Cannot delete occupied bed!";
            return RedirectToAction("Beds");
        }

        // Bed Allocation
        public IActionResult AllocateBed(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                TempData["Error"] = "Patient ID is required!";
                return RedirectToAction("Patients");
            }

            var patient = HospitalData.GetPatient(patientId);
            if (patient == null)
            {
                TempData["Error"] = "Patient not found!";
                return RedirectToAction("Patients");
            }

            ViewBag.Patient = patient;

            // Get appropriate beds based on patient status
            List<Bed> availableBeds;
            if (patient.IsCritical)
            {
                availableBeds = HospitalData.GetAvailableICUBeds();
            }
            else
            {
                availableBeds = HospitalData.GetAvailableBeds();
            }

            ViewBag.AvailableBeds = availableBeds ?? new List<Bed>();

            return View();
        }

        [HttpPost]
        public IActionResult AllocateBed(string patientId, string bedId)
        {
            if (string.IsNullOrEmpty(patientId) || string.IsNullOrEmpty(bedId))
            {
                TempData["Error"] = "Invalid patient or bed selection!";
                return RedirectToAction("Patients");
            }

            if (HospitalData.AllocateBedToPatient(patientId, bedId))
                TempData["Message"] = "Bed allocated successfully!";
            else
                TempData["Error"] = "Bed allocation failed! Check validation rules.";

            return RedirectToAction("Patients");
        }

        [HttpPost]
        public IActionResult DeallocateBed(string bedId)
        {
            if (string.IsNullOrEmpty(bedId))
            {
                TempData["Error"] = "Bed ID is required!";
                return RedirectToAction("Beds");
            }

            if (HospitalData.DeallocateBed(bedId))
                TempData["Message"] = "Bed deallocated successfully!";
            else
                TempData["Error"] = "Failed to deallocate bed!";

            return RedirectToAction("Beds");
        }

        [HttpPost]
        public IActionResult SanitizeBed(string bedId)
        {
            if (string.IsNullOrEmpty(bedId))
            {
                TempData["Error"] = "Bed ID is required!";
                return RedirectToAction("Beds");
            }

            if (HospitalData.SanitizeBed(bedId))
                TempData["Message"] = "Bed sanitized successfully!";
            else
                TempData["Error"] = "Failed to sanitize bed!";

            return RedirectToAction("Beds");
        }

        // Filter Views
        public IActionResult CriticalPatients() => View(HospitalData.GetCriticalPatients());
        public IActionResult PatientsRequiringICU() => View(HospitalData.GetPatientsRequiringICU());
        public IActionResult UnallocatedPatients() => View(HospitalData.GetUnallocatedPatients());
        public IActionResult SanitationOverdue() => View(HospitalData.GetSanitationOverdueBeds());
        public IActionResult Alerts() => View(HospitalData.GetAllAlerts());

        [HttpPost]
        public IActionResult MarkAlertAsRead(string alertId)
        {
            if (string.IsNullOrEmpty(alertId))
            {
                TempData["Error"] = "Alert ID is required!";
                return RedirectToAction("Alerts");
            }

            HospitalData.MarkAlertAsRead(alertId);
            TempData["Message"] = "Alert marked as read!";
            return RedirectToAction("Alerts");
        }

        // Update Vitals
        public IActionResult UpdateVitals(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Patient ID is required!";
                return RedirectToAction("Patients");
            }

            var patient = HospitalData.GetPatient(id);
            if (patient == null)
            {
                TempData["Error"] = "Patient not found!";
                return RedirectToAction("Patients");
            }

            return View(patient);
        }

        [HttpPost]
        public IActionResult UpdateVitals(string id, double temperature, int pulseRate, double oxygenLevel)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Patient ID is required!";
                return RedirectToAction("Patients");
            }

            var patient = HospitalData.GetPatient(id);
            if (patient == null)
            {
                TempData["Error"] = "Patient not found!";
                return RedirectToAction("Patients");
            }

            patient.Temperature = temperature;
            patient.PulseRate = pulseRate;
            patient.OxygenLevel = oxygenLevel;

            HospitalData.UpdatePatient(patient);
            TempData["Message"] = "Vitals updated successfully!";

            return RedirectToAction("Patients");
        }

    }
}