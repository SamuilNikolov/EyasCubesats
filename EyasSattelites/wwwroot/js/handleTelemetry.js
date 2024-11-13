var real = true;
async function fetchTelemetry() {
    var response = 0;
    if (real) {
        response = await fetch('/real-telemetry');
    }
    else {
        response = await fetch('/fake-telemetry');
    }
    if (!response.ok) {
        handleTelemetryError('Error fetching telemetry data.');
        return;
    }
    const data = await response.json();
    parseTelemetry(data.telemetry);
}

function handleTelemetryError(message) {
    document.getElementById('comms').innerText = message;
    applyErrorStyles();
}

function applyErrorStyles() {
    const sections = document.getElementsByClassName('telemetry-section');
    for (let i = 0; i < sections.length; i++) {
        sections[i].classList.add('error', 'glow-red');
    }
}

function removeErrorStyles() {
    const sections = document.getElementsByClassName('telemetry-section');
    for (let i = 0; i < sections.length; i++) {
        sections[i].classList.remove('error', 'glow-red');
    }
}