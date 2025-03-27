let mediaRecorder;
let audioChunks = [];
let audioBlob;

const recordButton = document.getElementById("recordButton").onclick = () => {
  chrome.tabs.create({ url: "http://localhost:3000" });
};
const status = document.getElementById("status");

recordButton.onclick = async () => {
  const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
  mediaRecorder = new MediaRecorder(stream);
  audioChunks = [];

  mediaRecorder.ondataavailable = (event) => {
    if (event.data.size > 0) audioChunks.push(event.data);
  };

  mediaRecorder.onstop = () => {
    audioBlob = new Blob(audioChunks, { type: 'audio/webm' });
    uploadButton.disabled = false;
  };

  mediaRecorder.start();
  status.textContent = "Recording...";
  recordButton.disabled = true;
  stopButton.disabled = false;
};