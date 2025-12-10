const API_BASE = 'http://localhost:5000/api/v1';

// Show message banner
function showMessage(text, type) {
  const container = document.getElementById('message-container');
  const div = document.createElement('div');
  div.className = `message ${type}`;
  div.textContent = text;
  container.innerHTML = '';
  container.appendChild(div);
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
    const response = await fetch(`${API_BASE}/movies`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        Title: title,
        Year: parseInt(year),
        Description: description
      })
    });

    const data = await response.json();

    if (!response.ok || !data.Success) {
      throw new Error(data.Message || 'Failed to create movie');
    }

    showMessage('Movie created successfully. Redirecting...', 'success');
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
  const form = document.getElementById('movie-form');
  form.addEventListener('submit', handleSubmit);
  
  const cancelBtn = document.getElementById('cancel-btn');
  if (cancelBtn) {
    cancelBtn.addEventListener('click', goBack);
  }
});
