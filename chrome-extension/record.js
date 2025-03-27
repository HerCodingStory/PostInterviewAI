window.onload = () => {
    let mediaRecorder;
    let audioChunks = [];
    let audioBlob;
  
    const recordButton = document.getElementById("recordButton");
    const stopButton = document.getElementById("stopButton");
    const uploadButton = document.getElementById("uploadButton");
    const status = document.getElementById("status");
  
    recordButton.onclick = async () => {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            console.log("Microphone access granted");
    
            mediaRecorder = new MediaRecorder(stream);
            audioChunks = [];
    
            mediaRecorder.ondataavailable = (event) => {
            if (event.data.size > 0) {
                audioChunks.push(event.data);
                console.log("Chunk received:", event.data);
            }
            };
    
            mediaRecorder.onstop = () => {
            audioBlob = new Blob(audioChunks, { type: 'audio/webm' });
            uploadButton.disabled = false;
            console.log("Audio blob ready:", audioBlob);
            };
    
            mediaRecorder.start();
            status.textContent = "Recording...";
            recordButton.disabled = true;
            stopButton.disabled = false;
        } catch (err) {
            console.error("Microphone access error:", err);
            status.textContent = "Microphone access denied or dismissed.";
        }
    };
  
    stopButton.onclick = () => {
        mediaRecorder.stop();
        recordButton.disabled = false;
        stopButton.disabled = true;
        status.textContent = "Recording stopped.";
    };
  
    uploadButton.onclick = async () => {
        const formData = new FormData();
        formData.append("file", audioBlob, "interview.webm");
  
        try {
            const response = await fetch("http://localhost:5117/api/audio/upload", {
                method: "POST",
                body: formData
            });

            const contentType = response.headers.get("Content-Type") || "";
            if (contentType.includes("application/json")) {
                const result = await response.json();
                status.innerHTML = `
                    <h3>✅ Feedback</h3>
                    <ul style="text-align:left; padding-left: 20px;">
                    <li><strong>Clarity:</strong> ${result.clarity}</li>
                    <li><strong>Structure:</strong> ${result.structure}</li>
                    <li><strong>Content:</strong> ${result.content}</li>
                    <li><strong>Technical Accuracy:</strong> ${result.technicalAccuracy}</li>
                    </ul>
                `;
            } else {
                const text = await response.text();
                status.textContent = `⚠️ Response was not JSON: ${text}`;
            }

        } catch (err) {
            status.textContent = "❌ Upload failed.";
            console.error(err);
        }
    };
};