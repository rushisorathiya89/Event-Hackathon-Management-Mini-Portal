# 🏆 Hackathon Portal

A web-based mini-portal built for the **CodeMind AI Hackathon** to digitize event operations — from team registration to judge scoring and leaderboard display.

---

## 📌 Problem Statement

Develop a portal for event details, team registration, and judging/leaderboard operations.  
**Goal:** Digitize event operations from registration to scoring with a reliable, role-aware interface.

---

## 🚀 Features

### 👤 Role-Based Access
- **Admin** — Create & manage hackathons, assign judges, view all participants and results
- **Participant** — Browse events, register a team, track hackathon status and scores
- **Judge** — View assigned hackathons, enter scores for teams, see live rankings

### 🗂️ Core Modules
- Event overview with status (Draft / Published / Cancelled)
- Team registration with unique team name validation
- Team member management
- Judge score entry per hackathon
- Ranked leaderboard / results view
- Notification bell for in-app alerts
- Registration deadline enforcement

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| Database | SQL Server (via EF Core) |
| ORM | Entity Framework Core 8 |
| Auth | Session-based (BCrypt password hashing) |
| Frontend | Razor Views + Bootstrap |
| Validation | jQuery Validation / Unobtrusive |

---

## 📁 Project Structure

```
Hackathon_Portal/
├── Controllers/
│   ├── AccountController.cs      # Login & Register
│   ├── AdminController.cs        # Admin dashboard & hackathon management
│   ├── JudgeController.cs        # Score entry & judge views
│   ├── ParticipantController.cs  # Team registration & participant views
│   └── NotificationController.cs # Notifications
├── Models/
│   ├── User.cs                   # Roles: Admin, Participant, Judge
│   ├── Hackathon.cs              # Event details & status
│   ├── Team.cs                   # Team + project info
│   ├── TeamMember.cs             # Members linked to a team
│   ├── Score.cs                  # Judge scores per team
│   ├── Notification.cs           # In-app alerts
│   └── ViewModels/               # Form-specific models
├── Views/
│   ├── Admin/                    # Admin-specific pages
│   ├── Judge/                    # Judge-specific pages
│   ├── Participant/              # Participant-specific pages
│   ├── Account/                  # Login & Register pages
│   └── Shared/                   # Layouts & partials
├── Data/
│   └── AppDbContext.cs           # EF Core DbContext
├── Filters/
│   └── AuthRequired.cs           # Session-based auth filter
├── Migrations/                   # EF Core migrations
├── appsettings.json              # Config & connection string
└── Program.cs                    # App startup & middleware
```

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or SQL Server LocalDB

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/rushisorathiya89/Event-Hackathon-Management-Mini-Portal.git
   cd Hackathon_Portal/Hackathon_Portal
   ```

2. **Configure the database connection**  
   Edit `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HackathonPortalDb;Trusted_Connection=True;"
   }
   ```

3. **Run the application**  
   Migrations are applied automatically on startup.
   ```bash
   dotnet run
   ```

4. **Open in browser**  
   Navigate to `https://localhost:5001` or the URL shown in your terminal.

---

## 🧪 Default Roles & Usage

1. **Register** a new account (default role: Participant)
2. To access Admin features, manually set `Role = "Admin"` in the database for your user
3. Admin can then assign Judge roles from the portal

---

## 📊 Hackathon Checkpoints

| Checkpoint | Time | Features |
|---|---|---|
| T+4 | Architecture & Skeleton | Event overview, team registration, team list |
| T+8 | Core Features | Registration validation, key team details |
| T+16 | UX & Stability | Score entry, ranking view |
| T+24 | Final Demo | Multi-judge support, display-ready leaderboard |

---

## 📦 NuGet Packages

- `Microsoft.EntityFrameworkCore.SqlServer` v8.0.0
- `Microsoft.EntityFrameworkCore.Tools` v8.0.0
- `BCrypt.Net-Next` v4.0.3

---

## 👨‍💻 Author

Built for the **CodeMind AI Hackathon** (24-hour challenge).
