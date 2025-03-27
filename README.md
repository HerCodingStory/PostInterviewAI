# PostInterviewAI

**PostInterviewAI** is a full-stack web application and Chrome extension that helps users practice and improve their interview skills by analyzing their responses using AI.

- Record audio responses to mock interviews
- Automatically transcribe them using **OpenAI Whisper**
- Analyze clarity, structure, and technical depth using **ChatGPT**
- ~~Detect emotional tone using~~ **~~AWS Comprehend~~ {To Be Develop}**
- Use as a web app or Chrome extension

---

## Features

- Record interviews in-browser or via Chrome extension
- Upload audio to backend (.NET Web API)
- AI-generated feedback on structure, content, and clarity
- ~~Emotion analysis to measure tone and delivery~~ {To Be Develop}
- ~~Audio and feedback stored for future reference~~ {To Be Develop}

---

## Tech Stack

| Layer            | Technology                    |
| ---------------- | ----------------------------- |
| Frontend         | React + TypeScript + Next.js  |
| Backend          | ASP.NET Core Web API (.NET 9) |
| Transcription    | OpenAI Whisper (self-hosted)  |
| AI Feedback      | OpenAI ChatGPT API            |
| ~~Emotion NLP~~ | ~~AWS Comprehend~~           |
| Chrome Ext       | HTML + JS + Web APIs          |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node.js + npm](https://nodejs.org/)
- [Python 3](https://www.python.org/) with `pip`
- [OpenAI account](https://platform.openai.com/account/api-keys)
- [AWS account](https://aws.amazon.com/)

---

## Chrome Extension Setup

### Usage:

- Dowload extension from chrome extensions page
- Click the extension icon
- Record → Stop → Upload
- See AI-powered feedback directly in the popup

---

## Web App Workflow

1. Record audio in the React app
2. Upload triggers backend
3. Whisper transcribes the audio
4. ChatGPT analyzes the text and returns feedback
5. ~~AWS Comprehend detects emotional tone~~
6. Final report displayed in the dashboard

---

## Future Features

- User authentication (AWS Cognito or Auth0)
- Metrics on confidence level and filler words
- Save Interview question and create templates

---

## License

MIT License © 2025 Cristina Villarroel
