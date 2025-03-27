// Root file structure assumes you're inside the Next.js project
// File: app/page.tsx

"use client";

import React, { useState, useRef } from "react";
import axios from "axios";

export default function Home() {
  const [recording, setRecording] = useState(false);
  const [audioBlob, setAudioBlob] = useState<Blob | null>(null);
  const [feedback, setFeedback] = useState<string | null>(null);
  const mediaRecorderRef = useRef<MediaRecorder | null>(null);
  const chunks = useRef<Blob[]>([]);

  const startRecording = async () => {
    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    const mediaRecorder = new MediaRecorder(stream);
    mediaRecorderRef.current = mediaRecorder;

    mediaRecorder.ondataavailable = (event) => {
      if (event.data.size > 0) chunks.current.push(event.data);
    };

    mediaRecorder.onstop = () => {
      const blob = new Blob(chunks.current, { type: "audio/webm" });
      setAudioBlob(blob);
      chunks.current = [];
    };

    mediaRecorder.start();
    setRecording(true);
  };

  const stopRecording = () => {
    mediaRecorderRef.current?.stop();
    setRecording(false);
  };

  const uploadAudio = async () => {
    if (!audioBlob) return;
    const formData = new FormData();
    formData.append("file", audioBlob, "interview.webm");

    try {
      const response = await axios.post("http://localhost:5117/api/audio/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });
      setFeedback(response.data); // Assume server returns a string of feedback
    } catch (error) {
      console.error("Upload failed", error);
    }
  };

  return (
    <main className="flex flex-col items-center justify-center min-h-screen p-4">
      <h1 className="text-2xl font-bold mb-4">Interview Analyzer</h1>
      <div className="flex gap-4">
        {!recording ? (
          <button className="bg-blue-500 text-white px-4 py-2 rounded" onClick={startRecording}>
            Start Recording
          </button>
        ) : (
          <button className="bg-red-500 text-white px-4 py-2 rounded" onClick={stopRecording}>
            Stop Recording
          </button>
        )}
        {audioBlob && (
          <button className="bg-green-600 text-white px-4 py-2 rounded" onClick={uploadAudio}>
            Upload & Analyze
          </button>
        )}
      </div>

      {feedback && (
        <div className="mt-6 p-4 border rounded bg-gray-100 w-full max-w-xl" style={{color: 'black'}}>
          <h2 className="font-semibold mb-2">AI Feedback</h2>
          <p>{feedback}</p>
        </div>
      )}
    </main>
  );
}
