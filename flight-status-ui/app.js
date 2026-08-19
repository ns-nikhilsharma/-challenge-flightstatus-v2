const API_BASE_URL = "http://localhost:5000";

const form = document.getElementById("searchForm");
const flightNumberInput = document.getElementById("flightNumber");
const dateInput = document.getElementById("date");
const searchButton = document.getElementById("searchButton");
const state = document.getElementById("state");
const result = document.getElementById("result");

dateInput.value = new Date().toISOString().slice(0, 10);

form.addEventListener("submit", async (event) => {
  event.preventDefault();

  const flightNumber = flightNumberInput.value.trim().toUpperCase();
  const date = dateInput.value;

  if (!flightNumber || !date) {
    showState("Please enter both a flight number and date.", "error");
    return;
  }

  showLoading();

  try {
    const response = await fetch(
      `${API_BASE_URL}/flights/status?flightNumber=${encodeURIComponent(flightNumber)}&date=${encodeURIComponent(date)}`
    );

    const body = await response.json();

    if (!response.ok) {
      throw new Error(body.message || "The API request failed.");
    }

    if (body.status === "Unknown") {
      showState(body.message || "No usable status was returned.", "empty");
      hideResult();
      return;
    }

    renderResult(body);
    hideState();
  } catch (error) {
    hideResult();
    showState(
      `Unable to retrieve flight status. ${error.message}`,
      "error"
    );
  } finally {
    searchButton.disabled = false;
    searchButton.textContent = "Search status";
  }
});

function renderResult(data) {
  result.className = "card result";
  result.innerHTML = `
    <div class="result-header">
      <div>
        <p class="eyebrow">${escapeHtml(data.provider || "Provider")}</p>
        <h2>${escapeHtml(data.flightNumber)}</h2>
        <p>${escapeHtml(data.date)}</p>
      </div>
      <span class="status status-${data.status.toLowerCase()}">${escapeHtml(data.status)}</span>
    </div>

    <div class="details">
      ${detail("Scheduled departure", data.scheduledDepartureUtc)}
      ${detail("Actual departure", data.actualDepartureUtc)}
      ${detail("Scheduled arrival", data.scheduledArrivalUtc)}
      ${detail("Actual arrival", data.actualArrivalUtc)}
      ${detail("Terminal", data.terminal)}
      ${detail("Gate", data.gate)}
      ${detail("Delay reason", data.delayReason)}
      ${detail("Last updated", data.lastUpdatedUtc)}
    </div>
  `;
}

function detail(label, value) {
  if (!value) return "";
  return `<div><dt>${escapeHtml(label)}</dt><dd>${escapeHtml(formatValue(value))}</dd></div>`;
}

function formatValue(value) {
  if (typeof value !== "string") return String(value);
  const parsed = new Date(value);
  return Number.isNaN(parsed.getTime()) ? value : parsed.toLocaleString();
}

function showLoading() {
  searchButton.disabled = true;
  searchButton.textContent = "Searching...";
  hideResult();
  showState("Querying both flight-status providers...", "loading");
}

function showState(message, type) {
  state.className = `state ${type}`;
  state.textContent = message;
}

function hideState() {
  state.className = "state hidden";
}

function hideResult() {
  result.className = "card result hidden";
  result.innerHTML = "";
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}
