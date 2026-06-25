# STAR Air ADM — Project Deliverables

---

## Deliverable 2: LinkedIn Post

---

Most aviation accidents aren't caused by mechanical failure — they're caused by a pilot who was too tired, too stressed, or too pressured to say "no."

I built STAR Air ADM, a full-stack Aviation Safety Management System that replaces subjective pre-flight self-assessment with algorithmic, auditable risk scoring.

Pilots complete IMSAFE (physiological fitness), PAVE (operational risk), and DECIDE (structured decision-making) assessments before every flight. A deterministic scoring engine enforces hard medical overrides — Alcohol flagged as High? Automatic NoGo. SpO₂ below 90%? Hypoxia warning, flight blocked. No negotiation.

The system also ingests wearable biometric data (heart rate, HRV, stress index, sleep) and produces a composite fitness verdict. Admins get a real-time command center with custom SVG trend charts, pilot performance analytics, and a complete audit trail — aligned with ICAO Annex 19 and FAA SMS standards.

A few technical decisions I'm proud of:

→ Custom SVG charting engine with cubic Bézier curves, gradient fills, and click-to-drill-down — zero chart library dependency for the core analytics
→ Offline-first architecture using IndexedDB (Dexie) with sync queue — critical for pilots operating in low-connectivity environments
→ Pure static `RiskScoringEngine` with no side effects — fully testable, fully auditable

Stack: React 18 + TypeScript + Zustand | ASP.NET Core 9 + EF Core | SQL Server | PWA

Aviation safety shouldn't depend on a pilot's honesty on a bad day. It should depend on data.

Would love feedback from anyone working at the intersection of software and safety-critical systems.

---

## Deliverable 3: LinkedIn Profile — Projects Section

---

**Project Name:** STAR Air ADM — Aviation Safety Management System

**Description:**
Full-stack web platform that digitizes pre-flight risk assessment for pilots using IMSAFE, PAVE, and DECIDE aviation safety frameworks. Replaces paper-based checklists with algorithmic Go/Caution/NoGo scoring, wearable biometric integration, and real-time fleet safety analytics for flight operations teams.

**Technologies Used:**
React 18, TypeScript, Vite, Tailwind CSS, Zustand, ASP.NET Core 9, Entity Framework Core 9, SQL Server, JWT Authentication, MailKit, Swagger/OpenAPI, Dexie (IndexedDB), PWA, Custom SVG Charting

**Key Highlights:**
- Engineered a deterministic risk-scoring engine enforcing FAA-aligned medical overrides (alcohol, fatigue, hypoxia) across 3 aviation assessment frameworks with hard-gate flight clearance logic
- Built a custom SVG analytics dashboard with multi-series Bézier trend charts, interactive drill-down, and per-pilot stacked performance bars — zero third-party chart dependency for core visuals
- Designed an offline-first PWA architecture with IndexedDB persistence and sync queue, enabling full assessment workflows in low-connectivity environments

---

## Deliverable 4: CV / Resume — Project Entry

---

**STAR Air ADM** | React 18 · TypeScript · ASP.NET Core 9 · EF Core · SQL Server · JWT · PWA
- Developed a full-stack aviation SMS platform digitizing 3 pre-flight risk frameworks (IMSAFE, PAVE, DECIDE) across 13 REST endpoints and 15 domain entities
- Engineered a deterministic risk-scoring engine with FAA-aligned hard overrides and wearable biometric analysis (HR, HRV, SpO₂, stress index)
- Architected an offline-first PWA with IndexedDB sync queue and custom SVG analytics dashboard serving role-based views for pilots and administrators
