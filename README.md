# ⚔️ RiftRoster

RiftRoster helps League of Legends squads actually play like squads.

Instead of checking stats individually across multiple sites, RiftRoster brings a team’s performance into one shared dashboard — recent matches, collective win rates, playstyle patterns, and trends that reveal a squad’s identity.

It gives players a way to improve together, celebrate progress, and understand how they perform as a group rather than as five isolated players. Built with a modern full-stack architecture (React + TypeScript + .NET 9 + PostgreSQL), RiftRoster combines Riot API data with clean UI and squad-level analytics to help teams grow and play more intentionally.

---

## ✨ Core Features (MVP)

> **A detailed multi-phase roadmap is available below.**

| Feature | Description | Status |
|--------|-------------|--------|
| Squad Management | Create and manage squads persistently | ✅ Complete |
| Member Management | Add/remove squad members using Riot ID lookup | ✅ Complete |
| Match Data Sync | Pull and store recent matches for all squad members | ✅ Complete |
| Player Data Sync | Retrieve and update player details via Riot API | ✅ Complete |
| Squad Dashboard | View roster, matches, and basic team metrics | 🚧 In Progress |
| Modern Frontend UI | React + Mantine + React Query | 🚧 In Progress |
| Authentication | Optional Google or guest login | ⏳ Planned |

---

## 🏗️ Architecture Overview

### **Backend — .NET 9**
- Minimal API with a clean, service-oriented design  
- Strongly typed DTOs for requests/responses  
- PostgreSQL relational models for players, squads, matches, and participants  
- Riot Games API integration for player and match data  

### **Frontend — React + TypeScript**
- Vite development environment  
- React Query for server state management  
- Mantine UI for styling and components  
- Axios-based typed API client  
- React Router for navigation  

---

## 📡 API Overview

### **Squads**
- `GET /squads`
- `POST /squads`
- `GET /squads/{id}`
- `PUT /squads/{id}`
- `DELETE /squads/{id}`

### **Members**
- `POST /squads/{id}/members`
- `GET /squads/{id}/members`
- `DELETE /squads/{id}/members/{memberId}`

### **Matches**
- `GET /squads/{id}/matches`

### **Players**
- `GET /players/{puuid}`

---

## 🗺️ RiftRoster — Feature Roadmap

### 🎯 Vision
A platform where squads can track shared stats, analyze matches together, and discover their team identity through meaningful insights and analytics.

---

## Phase 1 — MVP: Core Squad Analytics
- Squad creation & Riot ID member linking  
- Fetch and store match + timeline data  
- Shared squad dashboard (recent matches, basic stats)  
- Clean UI for squads and members  

---

## Phase 2 — Insights & Playstyle Analysis
- Playstyle tags based on timeline metrics  
- Weekly squad summaries  
- Basic match visualizations (gold/XP graphs)  
- Early insights system for identifying strengths and weaknesses  

---

## Phase 3 — Social & Gamified Experience
- Squad challenges (e.g., “Win 3 games this week”)  
- Progression badges  
- Public/private squad profiles  
- Comparative leaderboards  

---

## Phase 4 — Advanced Analytics & AI (Stretch Goals)
- AI-generated match summaries  
- Synergy analytics (“Your jungle-mid duo performs best together”)  
- Champion/role recommendations  
- End-of-season “Squad Wrapped” summaries  

---

## 🧰 Tech Stack

**Frontend:**  
React • TypeScript • Vite • Mantine • React Query • Axios  

**Backend:**  
.NET 9 • C# • PostgreSQL • Entity Framework Core • Riot Games API  

---

## ⚙️ Running the Project (Local)

### Backend
```bash
cd backend
dotnet run
