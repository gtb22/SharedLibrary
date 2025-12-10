const API_BASE = 'http://localhost:5000/api/v1';

let currentPage = 1;
let currentPageSize = 10;
let totalMovies = 0;

// Helper: Show message banner
function showMessage(text, type) {
  const container = document.getElementById('message-container');
  const div = document.createElement('div');
  div.className = `message ${type}`;
  div.textContent = text;
  container.innerHTML = '';
  container.appendChild(div);
}

// Helper: Clear message
function clearMessage() {
  document.getElementById('message-container').innerHTML = '';
}

// Fetch and render movies
async function renderMovies() {
  document.getElementById('loading').style.display = 'block';
  clearMessage();

  try {
    const response = await fetch(
      `${API_BASE}/movies?page=${currentPage}&pageSize=${currentPageSize}`
    );
    const data = await response.json();

    if (!response.ok || !data.Success) {
      throw new Error(data.Message || 'Failed to load movies');
    }

    totalMovies = data.Total;
    const movies = data.Data || [];

    // Render movie cards
    const grid = document.getElementById('movies-grid');
    grid.innerHTML = '';

    if (movies.length === 0) {
      grid.innerHTML = '<p style="grid-column: 1/-1; text-align: center; color: #666; padding: 20px;">No movies found.</p>';
    } else {
      movies.forEach((movie) => {
        const card = document.createElement('div');
        card.className = 'movie-card';
        card.innerHTML = `
          <h3>${movie.Title || 'Unknown'}</h3>
          <div class="year">${movie.Year || 'N/A'}</div>
          <div class="description">${movie.Description || 'No description available.'}</div>
          <div class="card-buttons">
            <button class="btn btn-secondary" onclick="window.location.href='/view-movie.html?id=${movie.Id}'">View</button>
            <button class="btn btn-primary" onclick="window.location.href='/edit-movie.html?id=${movie.Id}'">Edit</button>
            <button class="btn btn-danger" onclick="deleteMovie(${movie.Id})">Remove</button>
          </div>
        `;
        grid.appendChild(card);
      });
    }

    // Update pagination bar
    const bar = document.getElementById('pagination-bar');
    const hasNextPage = data.HasNextPage || false;
    const hasPreviousPage = data.HasPreviousPage || false;

    bar.innerHTML = `
      <div class="pagination-controls">
        <span class="page-info">Page ${currentPage}</span>
        <button class="btn btn-secondary" onclick="goToFirstPage()" ${!hasPreviousPage ? 'disabled' : ''}>First</button>
        <button class="btn btn-secondary" onclick="goToPreviousPage()" ${!hasPreviousPage ? 'disabled' : ''}>Prev</button>
        <button class="btn btn-secondary" onclick="goToNextPage()" ${!hasNextPage ? 'disabled' : ''}>Next</button>
        <button class="btn btn-secondary" onclick="goToLastPage()" ${!hasNextPage ? 'disabled' : ''}>Last</button>
      </div>
      <div class="items-per-page">
        <label>Items per page:</label>
        <select onchange="changePageSize(parseInt(this.value))">
          <option value="5" ${currentPageSize === 5 ? 'selected' : ''}>5</option>
          <option value="10" ${currentPageSize === 10 ? 'selected' : ''}>10</option>
          <option value="15" ${currentPageSize === 15 ? 'selected' : ''}>15</option>
          <option value="20" ${currentPageSize === 20 ? 'selected' : ''}>20</option>
        </select>
      </div>
    `;
  } catch (error) {
    showMessage(`Error: ${error.message}`, 'error');
  } finally {
    document.getElementById('loading').style.display = 'none';
  }
}

// Pagination handlers (exposed globally for inline onclick)
window.goToFirstPage = () => {
  currentPage = 1;
  renderMovies();
};

window.goToPreviousPage = () => {
  if (currentPage > 1) {
    currentPage--;
    renderMovies();
  }
};

window.goToNextPage = () => {
  currentPage++;
  renderMovies();
};

window.goToLastPage = () => {
  const estimatedLast = Math.ceil(totalMovies / currentPageSize);
  currentPage = estimatedLast || currentPage + 1;
  renderMovies();
};

window.changePageSize = (size) => {
  currentPageSize = size;
  currentPage = 1;
  renderMovies();
};

window.deleteMovie = (id) => {
  if (!confirm('Are you sure you want to delete this movie?')) {
    return;
  }

  fetch(`${API_BASE}/movies/${id}`, { method: 'DELETE' })
    .then((response) => response.json())
    .then((data) => {
      if (data.Success) {
        showMessage('Movie removed successfully.', 'success');
        setTimeout(() => renderMovies(), 500);
      } else {
        showMessage(`Error: ${data.Message || 'Failed to delete movie'}`, 'error');
      }
    })
    .catch((error) => {
      showMessage(`Error: ${error.message}`, 'error');
    });
};

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
  renderMovies();
});
