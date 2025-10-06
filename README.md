# 🕒 Timesheet Attendance App (.NET)

Aplikasi **Timesheet Attendance** berbasis **.NET** untuk mencatat kehadiran dan aktivitas kerja karyawan secara efisien.  
Dilengkapi dengan fitur login, pencatatan jam kerja (Clock In / Clock Out), serta laporan harian dan bulanan.

---

## 🚀 Fitur Utama

- 🕘 **Pencatatan Absensi** (Clock In / Clock Out)
- 📅 **Laporan Timesheet Bulanan**
- 📊 **Ekspor Laporan ke Excel/PDF**

---

## 🧩 Teknologi yang Digunakan

| Komponen | Teknologi |
|-----------|------------|
| **Framework** | .NET 8.0 |
| **Database** | SQL Server / MongoDB |
| **ORM** | Entity Framework Core / Dapper |
| **Dependency Injection** | .NET Built-in DI Container |
| **Logging** | Serilog |
| **Testing** | xUnit / NUnit |
| **Frontend (opsional)** | WPF / Blazor / ASP.NET MVC |

---

## 📁 Struktur Proyek

```plaintext
TimesheetApp/
├── TimesheetApp.API/           # API utama
│   ├── Controllers/             # Endpoint untuk absensi & users
│   ├── Models/                  # Model dan DTO
│   ├── Services/                # Logika bisnis
│   ├── Data/                    # DbContext & konfigurasi EF Core
│   ├── Repositories/            # Akses data
│   ├── Mappings/                # AutoMapper profiles
│   └── Program.cs               # Entry point aplikasi
│
├── TimesheetApp.Tests/          # Unit tests
│   ├── Services/
│   ├── Handlers/
│   └── Repositories/
│
└── README.md                    # Dokumentasi proyek
