const API_BASE = '/api/v1';

//Show message banner
function showMessage(text, type) {
  const container = document.getElementById('message-container');
  const div = document.createElement('div');
  div.className = `message ${type}`;
  div.textContent = text;
  container.innerHTML = '';
  container.appendChild(div);
}

//Get query parameter
function getQueryParam(name) {
  const params = new URLSearchParams(window.location.search);
  return params.get(name);
}

//Load movie data
async function loadMovie() {
  const movieId = getQueryParam('id');

  if (!movieId) {
    showMessage('No movie ID provided.', 'error');
    return;
  }

  try {
    const response = await fetch(`${API_BASE}/movies/${movieId}`);
    const data = await response.json();

    if (!response.ok || !data.Success) {
      throw new Error(data.Message || 'Failed to load movie');
    }

    const movie = data.Data;
    document.getElementById('movie-id').textContent = movie.Id || 'N/A';
    document.getElementById('movie-title').textContent = movie.Title || 'Unknown';
    document.getElementById('movie-year').textContent = movie.Year || 'N/A';
    document.getElementById('movie-description').textContent = movie.Description || 'No description available.';

    //Show success message
    showMessage('Movie loaded successfully.', 'success');
  } catch (error) {
    showMessage(`Error: ${error.message}`, 'error');
  }
}

//Edit button handler
window.editMovie = function() {
  const movieId = getQueryParam('id');
  if (movieId) {
    window.location.href = `/edit-movie.html?id=${movieId}`;
  }
};

//Home button handler
window.goHome = function() {
  window.location.href = '/index.html';
};

//All Movies button handler
window.goToMovies = function() {
  window.location.href = '/movies.html';
};

//Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
  loadMovie();
});
