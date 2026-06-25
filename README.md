<div align="center">

# ✈️ STAR Air ADM

### Air Decision Manager — Aviation Safety Management System

A full-stack, data-driven pre-flight risk assessment platform that digitizes IMSAFE, PAVE, and DECIDE aviation safety frameworks, integrates wearable biometric data, and provides real-time fleet safety analytics for flight operations teams.

[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React 18](https://img.shields.io/badge/React-18.3-61DAFB?logo=react)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.6-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

</div>

---

## 📖 Overview

STAR Air ADM replaces paper-based pre-flight checklists with an algorithmic, auditable safety gate system. Pilots cannot be cleared to fly until they complete mandatory IMSAFE (personal fitness), PAVE (operational risk), and DECIDE (aeronautical decision-making) assessments — each scored by a deterministic risk engine that enforces civil-aviation medical standards. Administrators monitor fleet health through a real-time command center with custom SVG trend charts, pilot performance analytics, and a tamper-proof audit trail aligned with ICAO Annex 19 and FAA SMS requirements.

---

## 🛠 Tech Stack

| Layer | Technology |
|---|---|
| **Frontend** | React 18, TypeScript 5.6, Vite 6, Tailwind CSS 3.4 |
| **State Management** | Zustand 5 |
| **Charts** | Custom SVG engine (Bézier curves, gradients) + Recharts 3.8 |
| **Offline / PWA** | Dexie (IndexedDB), vite-plugin-pwa, Service Workers |
| **Backend** | ASP.NET Core 9 (Web API), C# 13 |
| **ORM** | Entity Framework Core 9 (Code-First Migrations) |
| **Database** | Microsoft SQL Server |
| **Authentication** | ASP.NET Identity + JWT Bearer tokens (access + refresh) |
| **Email** | MailKit 4.15 via SMTP |
| **API Docs** | Swashbuckle / Swagger (OpenAPI v3) |
| **Logging** | Serilog |
| **Desktop** | Electron (optional build target) |

---

## 🏗 Architecture Overview

The system follows **Clean Architecture** with strict dependency inversion, deployed as two independent units:

```
┌──────────────────────────────────────────────────┐
│                   React PWA                      │
│  (Role-aware SPA: Admin dashboard + Pilot hub)   │
│  Zustand stores │ Axios HTTP client │ Dexie (IDB)│
└────────────────────────┬─────────────────────────┘
                         │ HTTPS / JWT Bearer
┌────────────────────────▼─────────────────────────┐
│              StarAirAdm.Api (.NET 9)             │
│  13 Controllers │ Global Exception Middleware     │
│  CORS │ Swagger │ Role-based Authorization       │
├──────────────────────────────────────────────────┤
│           StarAirAdm.Application                 │
│  Interfaces (contracts) │ DTOs │ Models           │
├──────────────────────────────────────────────────┤
│           StarAirAdm.Infrastructure              │
│  EF Core DbContext │ 14 Services │ DB Seeder     │
│  RiskScoringEngine (IMSAFE + PAVE + SmartWatch)  │
│  AuditInterceptor │ MailKit EmailService          │
├──────────────────────────────────────────────────┤
│              StarAirAdm.Domain                   │
│  15 Entities │ 7 Enums │ Zero external deps       │
└──────────────────────────────────────────────────┘
                         │ EF Core
                ┌────────▼────────┐
                │   SQL Server    │
                └─────────────────┘
```

---

## ✨ Features

### Pilot Capabilities
- **IMSAFE Self-Assessment** — rate 6 physiological/psychological risk factors; algorithmic Go/Caution/NoGo scoring with hard medical overrides (e.g., Alcohol=High → automatic NoGo)
- **PAVE Operational Risk** — assess Pilot readiness, Aircraft airworthiness, enVironment (METAR/TAF weather), and External pressures
- **DECIDE Decision-Making** — structured 6-step aeronautical decision session with full audit trail
- **SmartWatch Biometric Monitor** — submit heart rate, HRV, SpO₂, stress index, and sleep metrics; receive fitness status (Fit / Caution / Not Fit) with plain-English recommendations
- **Live Weather Intelligence** — automatic METAR/TAF fetch per ICAO code with flight-category badges (VFR/MVFR/IFR/LIFR)
- **Flight Preparation Hub** — single-page workflow combining all four mandatory assessments; trip auto-transitions to "Cleared" on completion
- **Digital Kneeboard & Checklists** — in-flight notes and customizable pre-flight checklists
- **Progressive Web App** — installable, offline-capable via Service Worker caching

### Admin Capabilities
- **Command Center Dashboard** — KPI cards, custom SVG multi-series trend charts (7-day paginated, interactive tooltips, drill-down), pilot performance stacked bars, assessment reasons donut chart
- **Flight Operations Control** — create/assign/track flight trips with automated pilot email notifications
- **Pilot Roster Management** — activate/suspend accounts (regulatory gate), view individual assessment histories
- **Fleet Registry** — aircraft management with airworthiness status tracking
- **Audit Log** — searchable, timestamped log of every action for regulatory compliance
- **Notification System** — in-app + email alerts for trip assignments and status changes

### Cross-Cutting
- JWT authentication with refresh token rotation
- Role-based authorization (Admin, Pilot)
- Global exception middleware with structured error responses
- Offline-first desktop mode with IndexedDB persistence (Dexie)
- Automatic database seeding (roles + default admin account)

---

## 📁 Project Structure

```
STAR Air ADM/
├── BackEnd/
│   ├── StarAirAdm.sln                      # Solution file
│   ├── db_script.sql                        # Full database creation script
│   ├── StarAirAdm.Api/                      # ASP.NET Core Web API
│   │   ├── Controllers/                     # 13 REST controllers
│   │   │   ├── AuthController.cs            #   Login, refresh, forgot/reset password
│   │   │   ├── FlightController.cs          #   CRUD flights, link assessments, complete
│   │   │   ├── ImSafeController.cs          #   IMSAFE assessment endpoints
│   │   │   ├── PaveController.cs            #   PAVE assessment endpoints
│   │   │   ├── DecideController.cs          #   DECIDE session management
│   │   │   ├── SmartWatchController.cs      #   Biometric readings + analysis
│   │   │   ├── DashboardController.cs       #   Aggregated KPI stats
│   │   │   ├── UserController.cs            #   User management (admin)
│   │   │   ├── ProfileController.cs         #   Profile CRUD + password change
│   │   │   ├── ChecklistController.cs       #   Checklist CRUD
│   │   │   ├── KneeboardController.cs       #   Kneeboard notes CRUD
│   │   │   ├── NotificationController.cs    #   Notifications + mark-read
│   │   │   └── AuditLogController.cs        #   Audit log retrieval
│   │   ├── Middlewares/                     # Global exception handler
│   │   ├── Services/                        # CurrentUserService (JWT context)
│   │   ├── Program.cs                       # App entry point + pipeline
│   │   └── appsettings.json                 # Connection strings, JWT, Email config
│   ├── StarAirAdm.Application/             # Business logic contracts
│   │   ├── Interfaces/                      # Service contracts (IAuthService, etc.)
│   │   ├── DTOs/                            # Data transfer objects (11 domains)
│   │   └── Models/                          # Configuration models (EmailSettings)
│   ├── StarAirAdm.Domain/                  # Core domain model (zero dependencies)
│   │   ├── Entities/                        # 15 entity classes
│   │   ├── Enums/                           # 7 enumerations
│   │   └── Common/                          # Shared base classes
│   └── StarAirAdm.Infrastructure/          # Implementation layer
│       ├── Data/                            # AppDbContext, DbSeeder, Interceptors
│       ├── Services/                        # 14 service implementations
│       │   ├── RiskScoringEngine.cs         #   IMSAFE + PAVE + SmartWatch scoring
│       │   ├── AuthService.cs               #   JWT auth, refresh tokens, OTP
│       │   ├── FlightService.cs             #   Flight lifecycle + gate enforcement
│       │   ├── DashboardService.cs          #   KPI aggregation
│       │   └── ...                          #   (ImSafe, Pave, Decide, Email, etc.)
│       ├── Migrations/                      # EF Core migration history
│       └── DependencyInjection.cs           # IoC container registration
│
├── FrontEnd/
│   ├── package.json                         # Dependencies + scripts
│   ├── vite.config.ts                       # Vite + PWA configuration
│   ├── tailwind.config.js                   # Tailwind CSS customization
│   ├── index.html                           # SPA entry point
│   └── src/
│       ├── App.tsx                           # Root router (role-aware + offline routes)
│       ├── main.tsx                          # React entry point
│       ├── index.css                        # Global styles
│       ├── api/                             # API client helpers
│       ├── components/
│       │   ├── guards/                      # RequireAuth route guard
│       │   ├── layout/                      # AppLayout, Sidebar, Header, OfflineLayout
│       │   └── shared/                      # Reusable components
│       │       ├── AdvancedCharts.tsx        #   Custom SVG trend + donut charts (35KB)
│       │       ├── WeatherWidget.tsx         #   AVWX METAR/TAF weather card
│       │       ├── SmartWatchPanel.tsx       #   Embedded biometric panel (24KB)
│       │       └── TripAssessmentBadge.tsx   #   Go/Caution/NoGo badge
│       ├── pages/
│       │   ├── admin/                       # Admin: Dashboard, Users, Flights, Audit
│       │   ├── pilot/                       # Pilot: Dashboard, FlightPrep, SmartWatch, etc.
│       │   ├── auth/                        # Login, ForgotPassword, SetPassword
│       │   ├── offline/                     # Offline/Desktop mode pages
│       │   ├── TripDetailPage.tsx           # Full trip assessment report (36KB)
│       │   ├── ProfilePage.tsx              # User profile management
│       │   └── LandingPage.tsx              # Public marketing page
│       ├── stores/                          # Zustand state stores
│       │   ├── authStore.ts                 #   JWT + user context
│       │   ├── notificationStore.ts         #   Notification list + unread count
│       │   ├── uiStore.ts                   #   Sidebar + theme preferences
│       │   └── modeStore.ts                 #   Online/Offline mode toggle
│       └── lib/                             # Utilities + data
│           ├── apiClient.ts                 #   Axios instance with interceptors
│           ├── offlineBridge.ts             #   IndexedDB ↔ API sync logic (26KB)
│           ├── db.ts                        #   Dexie database schema
│           ├── weather.ts                   #   AVWX weather API integration
│           ├── aerodromeData.ts             #   ICAO airport reference data
│           ├── aircraftData.ts              #   Aircraft type reference data
│           ├── syncQueue.ts                 #   Offline action queue
│           └── types.ts                     #   TypeScript type definitions
│
└── STAR_Air_ADM_Description.md              # Detailed system documentation
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/) with npm
- [SQL Server](https://www.microsoft.com/sql-server) (local or remote)
- (Optional) [AVWX API key](https://avwx.rest/) for live weather data

### 1. Clone the Repository

```bash
git clone https://github.com/<your-org>/star-air-adm.git
cd star-air-adm
```

### 2. Backend Setup

```bash
cd BackEnd

# Restore packages
dotnet restore

# Update connection string in appsettings.json (see Environment Variables below)
# Then apply migrations:
cd StarAirAdm.Api
dotnet ef database update --project ../StarAirAdm.Infrastructure

# Run the API
dotnet run
```

The API will start at `https://localhost:5001` (or `http://localhost:5000`).  
Swagger UI is available at `/swagger`.

### 3. Frontend Setup

```bash
cd FrontEnd

# Install dependencies
npm install

# Update API base URL in src/lib/apiClient.ts if needed

# Start dev server
npm run dev
```

The app will open at `http://localhost:3000`.

### 4. Default Credentials

| Role | Email | Password |
|---|---|---|
| Admin | `admin@starair.com` | `Admin@123!` |
| Pilot | `pilot@starair.com` | `Pilot@123!` |

These are seeded automatically on first run.

---

## 🔐 Environment Variables

All backend configuration is managed via `appsettings.json`:

| Variable | Section | Description | Required |
|---|---|---|---|
| `DefaultConnection` | `ConnectionStrings` | SQL Server connection string | ✅ |
| `Secret` | `JwtSettings` | Symmetric key for signing JWT tokens (≥32 chars) | ✅ |
| `Issuer` | `JwtSettings` | JWT issuer claim value | ✅ |
| `Audience` | `JwtSettings` | JWT audience claim value | ✅ |
| `AccessTokenExpirationMinutes` | `JwtSettings` | Access token lifetime in minutes | ✅ |
| `RefreshTokenExpirationDays` | `JwtSettings` | Refresh token lifetime in days | ✅ |
| `Host` | `Email` | SMTP server hostname (e.g., `smtp.gmail.com`) | ✅ |
| `Port` | `Email` | SMTP port (e.g., `587`) | ✅ |
| `Username` | `Email` | SMTP authentication username | ✅ |
| `Password` | `Email` | SMTP authentication password / app password | ✅ |
| `FromEmail` | `Email` | Sender email address | ✅ |
| `FromName` | `Email` | Sender display name | ❌ |
| `EnableSsl` | `Email` | Whether to use TLS for SMTP | ✅ |
| `FrontendUrl` | Root | Frontend origin URL (for email links) | ✅ |
| `AdminEmail` | `SeedData` | Default admin account email | ❌ |
| `AdminPassword` | `SeedData` | Default admin account password | ❌ |

---

## 📡 API Overview

All endpoints are prefixed with `/api`. Protected routes require a `Bearer <token>` header.

| Method | Path | Description | Auth |
|---|---|---|---|
| `POST` | `/api/auth/login` | Authenticate and receive JWT tokens | Public |
| `POST` | `/api/auth/refresh-token` | Refresh expired access token | Public |
| `POST` | `/api/auth/forgot-password` | Request password reset OTP via email | Public |
| `POST` | `/api/auth/reset-password` | Reset password with OTP | Public |
| `POST` | `/api/auth/set-password` | Set password for invited user | Public |
| `POST` | `/api/auth/check-email` | Check if email exists in system | Public |
| `POST` | `/api/flight` | Create a new flight trip | Admin |
| `GET` | `/api/flight` | List all flights | Admin |
| `GET` | `/api/flight/my` | List authenticated pilot's flights | Pilot |
| `PUT` | `/api/flight/{id}` | Update flight details | Admin |
| `PATCH` | `/api/flight/{id}/link` | Link assessments to a flight | Auth |
| `PATCH` | `/api/flight/{id}/complete` | Mark flight as completed | Auth |
| `DELETE` | `/api/flight/{id}` | Delete a flight | Admin |
| `POST` | `/api/imsafe` | Submit IMSAFE assessment | Auth |
| `GET` | `/api/imsafe` | List all IMSAFE assessments | Admin |
| `GET` | `/api/imsafe/my` | List pilot's own assessments | Auth |
| `GET` | `/api/imsafe/{id}` | Get assessment by ID | Auth |
| `POST` | `/api/pave` | Submit PAVE assessment | Auth |
| `GET` | `/api/pave` | List all PAVE assessments | Admin |
| `GET` | `/api/pave/my` | List pilot's own assessments | Auth |
| `GET` | `/api/pave/{id}` | Get assessment by ID | Auth |
| `POST` | `/api/decide` | Start a DECIDE session | Auth |
| `POST` | `/api/decide/{id}/step` | Add a step to session | Auth |
| `PATCH` | `/api/decide/{id}/complete` | Complete DECIDE session | Auth |
| `GET` | `/api/decide/my` | List pilot's DECIDE sessions | Auth |
| `POST` | `/api/smartwatch` | Submit smartwatch reading | Auth |
| `GET` | `/api/smartwatch` | List pilot's readings | Auth |
| `GET` | `/api/smartwatch/analysis` | Get biometric analysis | Auth |
| `GET` | `/api/dashboard` | Get aggregated KPI stats | Admin |
| `GET` | `/api/user` | List all users | Admin |
| `GET` | `/api/user/{id}` | Get user details | Admin |
| `PATCH` | `/api/user/{id}/activate` | Activate a pilot account | Admin |
| `PATCH` | `/api/user/{id}/suspend` | Suspend a pilot account | Admin |
| `GET` | `/api/profile` | Get authenticated user profile | Auth |
| `PUT` | `/api/profile` | Update profile | Auth |
| `POST` | `/api/profile/change-password` | Change password | Auth |
| `POST` | `/api/checklist` | Create checklist | Auth |
| `GET` | `/api/checklist` | List checklists | Auth |
| `PUT` | `/api/checklist/{id}` | Update checklist | Auth |
| `DELETE` | `/api/checklist/{id}` | Delete checklist | Auth |
| `POST` | `/api/kneeboard` | Create kneeboard note | Auth |
| `GET` | `/api/kneeboard` | List kneeboard notes | Auth |
| `DELETE` | `/api/kneeboard/{id}` | Delete kneeboard note | Auth |
| `GET` | `/api/notification` | List notifications | Auth |
| `PATCH` | `/api/notification/{id}/read` | Mark notification as read | Auth |
| `GET` | `/api/auditlog` | List audit log entries | Admin |

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/your-feature-name`
3. **Commit** your changes with descriptive messages: `git commit -m "feat: add weather alert thresholds"`
4. **Push** to your fork: `git push origin feature/your-feature-name`
5. **Open** a Pull Request against `main`

### Guidelines

- Follow existing code conventions (C# naming, TypeScript strict mode)
- Write descriptive commit messages using [Conventional Commits](https://www.conventionalcommits.org/)
- Ensure the backend builds with `dotnet build` and frontend with `npm run build`
- Update documentation for any new endpoints or features
- All aviation-related scoring changes must be reviewed by a domain expert

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<div align="center">
<sub>Built with ❤️ for aviation safety</sub>
</div>
