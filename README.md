# 🚀 InterviewPrep

InterviewPrep is an AI-powered mock interview platform built with **.NET 9 (Clean Architecture)**, **Next.js 14**, and **SQLite**, with backend hosted on **Azure App Service**.

It simulates real interview sessions by generating tailored questions, capturing answers, and returning structured AI feedback and scoring using **Google Gemini**.

---

# 🧠 What it does

- Create interview sessions (CV + job spec + company context)
- Generate AI interview questions using Gemini
- Conduct interactive interview sessions
- Submit and store answers
- Evaluate performance with score + feedback

---

# 🏗️ Architecture (High Level)

### Backend (.NET 9 – Azure Hosted)
- Clean Architecture (API / Application / Domain / Infrastructure)
- REST API for interview sessions
- AI integration using **Google Gemini**
- SQLite database for local/dev persistence
- Hosted on **Azure App Service**

### Frontend (Next.js 14)
- App Router structure
- Session setup flow
- Live interview room UI
- Results and feedback screen

---

# 🔄 Core Flow

1. Create interview session  
2. Generate questions (Gemini)  
3. Conduct interview  
4. Submit answers  
5. Get AI evaluation + score  

---

# ⚙️ Tech Stack

- .NET 9 Web API (Azure Hosted)
- Next.js 14 (App Router)
- TypeScript
- SQLite
- Entity Framework Core
- Google Gemini API

---

# 🎯 Purpose

A realistic interview simulator designed to help candidates practice structured interviews with AI-generated questions and feedback.
