// API base URL - direct to API server (CORS enabled)
const API_BASE = 'http://localhost:5000/api/v1';

let movieId = null;

// Show message banner
function showMessage(text, type) {
  const container = document.getElementById('message-container');
  const div = document.createElement('div');
  div.className = `message ${type}`;
  div.textContent = text;
  container.innerHTML = '';
  container.appendChild(div);
}

// Get query parameter
function getQueryParam(name) {
  const params = new URLSearchParams(window.location.search);
  return params.get(name);
}

// Load movie data
async function loadMovie() {
  movieId = getQueryParam('id');

  if (!movieId) {
    showMessage('No movie ID provided. Redirecting...', 'error');
    setTimeout(() => {
      window.location.href = '/movies.html';
    }, 1500);
    return;
  }

  try {
    const response = await fetch(`${API_BASE}/movies/${movieId}`);
    const data = await response.json();

    if (!response.ok || !data.Success) {
      throw new Error(data.Message || 'Failed to load movie');
    }

    const movie = data.Data;
    document.getElementById('title').value = movie.Title || '';
    document.getElementById('year').value = movie.Year || '';
    document.getElementById('description').value = movie.Description || '';
  } catch (error) {
    showMessage(`Error loading movie: ${error.message}`, 'error');
    // Disable form inputs on error
    document.getElementById('title').disabled = true;
    document.getElementById('year').disabled = true;
    document.getElementById('description').disabled = true;
  }
}

// Handle form submission
async function handleSubmit(e) {
  e.preventDefault();

  const title = document.getElementById('title').value.trim();
  const year = document.getElementById('year').value.trim();
  const description = document.getElementById('description').value.trim();

  if (!title || !year) {
    showMessage('Please fill in Title and Year fields.', 'error');
    return;
  }

  try {
    const response = await fetch(`${API_BASE}/movies/${movieId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        Title: title,
        Year: parseInt(year),
        Description: description
      })
    });

    const data = await response.json();

    if (!response.ok || !data.Success) {
      throw new Error(data.Message || 'Failed to update movie');
    }

    showMessage('Movie updated successfully. Redirecting...', 'success');
    setTimeout(() => {
      window.location.href = '/movies.html';
    }, 1000);
  } catch (error) {
    showMessage(`Error: ${error.message}`, 'error');
  }
}

// Cancel button handler
function goBack() {
  window.location.href = '/movies.html';
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
  loadMovie();

  const form = document.getElementById('movie-form');
  form.addEventListener('submit', handleSubmit);

  const cancelBtn = document.getElementById('cancel-btn');
  if (cancelBtn) {
    cancelBtn.addEventListener('click', goBack);
  }
});
