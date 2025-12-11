// API base URL - direct to API server (CORS enabled)
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
  console.log('Form submitted');
  
  const title = document.getElementById('title').value.trim();
  const year = document.getElementById('year').value.trim();
  const description = document.getElementById('description').value.trim();
  
  console.log('Form data:', { title, year, description });
  
  if (!title) {
    showMessage('Please fill in the Title field.', 'error');
    return;
  }
  
  if (!year) {
    showMessage('Please fill in the Year field.', 'error');
    return;
  }

  try {
    const payload = {
      Title: title,
      Year: parseInt(year),
      Description: description || ''
    };
    
    console.log('Payload:', JSON.stringify(payload));
    showMessage('Creating movie...', 'info');
    
    console.log(`Fetching: ${API_BASE}/movies`);
    const response = await fetch(`${API_BASE}/movies`, {
      method: 'POST',
      headers: { 
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      },
      body: JSON.stringify(payload)
    });

    console.log('Response received:', response.status, response.statusText);
    
    // Always try to parse as text first to see what we got
    const responseText = await response.text();
    console.log('Response text:', responseText);
    
    let data;
    try {
      data = JSON.parse(responseText);
    } catch (e) {
      console.error('Failed to parse response as JSON:', e);
      throw new Error(`Invalid response format: ${responseText.substring(0, 100)}`);
    }

    console.log('Parsed data:', data);

    if (data && data.Success) {
      console.log('Success! Movie created with ID:', data.Data.Id);
      showMessage(`Movie "${data.Data.Title}" created successfully! Redirecting...`, 'success');
      setTimeout(() => {
        console.log('Redirecting to /movies.html');
        window.location.href = '/movies.html';
      }, 2000);
    } else {
      const errorMsg = data?.Message || 'Server returned error';
      console.error('Server error:', errorMsg);
      throw new Error(errorMsg);
    }
  } catch (error) {
    console.error('Caught error:', error);
    // Distinguish network errors to guide the user
    if (error.message.includes('Failed to fetch') || error.message.includes('NetworkError')) {
      showMessage('Cannot reach API at port 5000. Please ensure the API server is running.', 'error');
    } else {
      showMessage(`Error: ${error.message}`, 'error');
    }
  }
}

// Cancel button handler
function goBack() {
  window.location.href = '/movies.html';
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
  console.log('Page loaded, initializing form...');
  const form = document.getElementById('movie-form');
  if (!form) {
    console.error('Form not found!');
    return;
  }
  
  console.log('Form found, attaching submit handler');
  form.addEventListener('submit', handleSubmit);
  
  const cancelBtn = document.getElementById('cancel-btn');
  if (cancelBtn) {
    cancelBtn.addEventListener('click', goBack);
    console.log('Cancel button handler attached');
  }
  
  console.log('Initialization complete');
});
