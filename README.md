# 🏥 Smart Hospital Management System

A web-based **Smart Hospital Management System** built with **ASP.NET Core MVC** to manage patients, beds, alerts, sanitation, and critical care operations efficiently.

This project focuses on **real-time hospital workflow**, including patient vitals monitoring, ICU allocation, bed sanitation tracking, and automated alerts.

---

## 📌 Features

### 👤 Patient Management
- Add, edit, delete patients
- Track vital signs (Temperature, Pulse Rate, Oxygen Level)
- Automatic **critical status detection** (O₂ < 92%)
- View all patients, critical patients, and unallocated patients

### 🛏️ Bed Management
- Add, edit, delete beds
- Support for **General** and **ICU** beds
- Allocate and deallocate beds
- Prevent deletion of occupied beds

### 🚨 Alert System
- Automatic alerts for:
  - Critical vital signs
  - ICU bed requirement
  - Bed sanitation overdue
- Mark alerts as read
- View unread alert count in sidebar

### 🧼 Sanitation Tracking
- Track last sanitized time for beds
- Automatic detection of beds overdue (>48 hours)
- One-click sanitation update

### 📊 Dashboards & Filters
- Dashboard overview
- Filters for:
  - Critical patients
  - Patients requiring ICU
  - Unallocated patients
  - Sanitation overdue beds

---

## 🛠️ Technology Stack

| Layer        | Technology |
|--------------|------------|
| Frontend     | Razor Views, Bootstrap 5, Font Awesome |
| Backend      | ASP.NET Core MVC |
| Language     | C# |
| Architecture | MVC (Model–View–Controller) |
| Styling      | Custom CSS + Bootstrap |
| Version Control | Git & GitHub |

---

## 🗂️ Project Structure

```text
SmartHospitalSystem/
│
├── Controllers/
│   └── HomeController.cs
│
├── Models/
│   ├── Patient.cs
│   ├── Bed.cs
│   ├── Alert.cs
│   └── Enums/
│
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml
│   │   ├── Patients.cshtml
│   │   ├── CriticalPatients.cshtml
│   │   ├── AllocateBed.cshtml
│   │   ├── Alerts.cshtml
│   │   └── ...
│   └── Shared/
│       └── _Layout.cshtml
│
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   └── js/
│
├── Program.cs
├── appsettings.json
└── SmartHospitalSystem.csproj
⚙️ How to Run the Project
✅ Prerequisites
.NET SDK (6 or later)

Visual Studio / VS Code

Git (optional)

▶️ Steps
Clone the repository:

bash
Copy code
git clone https://github.com/noman1922/Smart-Hospital-System.git
Open the project in Visual Studio

Restore dependencies:

bash
Copy code
dotnet restore
Run the project:

bash
Copy code
dotnet run
Open browser:

arduino
Copy code
https://localhost:xxxx
📖 System Rules Implemented
Oxygen level < 92% → Patient becomes CRITICAL

CRITICAL patients must be allocated ICU beds

Beds require sanitation every 48 hours

Alerts are generated automatically for violations

🎯 Use Case Scenarios
Emergency patient admission

ICU bed shortage management

Sanitation compliance monitoring

Real-time alert-driven hospital operations

👨‍💻 Author
Noman Ahmed
Student, Computer Science & Engineering
Project: Academic / Learning Purpose

📜 License
This project is created for educational purposes.
You are free to study, modify, and extend it.

⭐ If you like this project, consider giving it a star!

yaml
Copy code

---

## ✅ Next Recommended Step
After adding this file:

```bash
git add README.md
git commit -m "Add project README"
git push
