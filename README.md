# PostInterviewAI

The Interview Analysis App is a full-stack web application designed to help job seekers evaluate and improve their interview performance by getting AI feedback after the interview. It allows candidates to record interviews, automatically transcribes the audio using OpenAI Whisper, analyzes their answers using ChatGPT, and provides emotional tone analysis using AWS Comprehend.

---

## ✨ Features

- 🎙️ Record interviews in-browser (React + MediaRecorder)
- 📤 Upload audio to backend (.NET Web API)
- 🧠 AI-generated feedback on structure, content, and clarity
- 😃 Emotion analysis to measure tone and delivery
- 🗃️ Audio and feedback stored for future reference

---

## 🧱 Tech Stack

| Layer         | Technology                    |
| ------------- | ----------------------------- |
| Frontend      | React + TypeScript + Next.js  |
| Backend       | ASP.NET Core Web API (.NET 9) |
| Transcription | OpenAI Whisper                |
| AI Feedback   | OpenAI ChatGPT API            |
| Emotion NLP   | AWS Comprehend                |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node.js + npm](https://nodejs.org/)
- [Python 3+](https://www.python.org/) with `pip`
- [OpenAI account](https://platform.openai.com/account/api-keys)
- [AWS account](https://aws.amazon.com/)

---

### 🔧 Setup Instructions

#### 1. Clone the repository

```bash
git clone https://github.com/your-username/PostInterviewAI.git
cd PostInterviewAI
```
