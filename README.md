# 🚀 InterviewPrep

InterviewPrep is an AI-powered mock interview platform built with **.NET 9 (Clean Architecture)** and **Next.js 14**, using **SQLite** as the database.

It simulates real interview sessions by generating AI-driven questions, capturing answers, and returning structured feedback and scoring.

---

# 🧠 What it does

- Create interview sessions (CV + job + company context)
- Generate tailored interview questions using AI
- Conduct interactive interview sessions
- Submit and store answers
- Generate final evaluation with score + feedback

---

# 🏗️ Architecture (High Level)

### Backend (.NET 9)
- Clean Architecture (API / Application / Domain / Infrastructure)
- RESTful API for session management
- AI service abstraction (OpenAI / Gemini / Mock)
- SQLite database for persistence

### Frontend (Next.js 14)
- App Router structure
- Interview session flow (setup → interview → results)
- Component-based UI design

---

# 🔄 Core Flow

1. Create session  
2. Generate AI questions  
3. Conduct interview  
4. Submit answers  
5. Evaluate results  

---

# ⚙️ Tech Stack

- .NET 9 Web API  
- Next.js 14 (App Router)  
- TypeScript  
- SQLite  
- Entity Framework Core  
- AI APIs (pluggable providers)

---

# 🎯 Purpose

A realistic AI interview simulator designed for practice, evaluation, and preparation using structured, adaptive questioning and feedback.
