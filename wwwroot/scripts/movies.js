//movies.js - Movies list with pagination, items-per-page, and CRUD operations
//All functions exposed globally for onclick handlers

const API_BASE = '/api/v1';

let currentPage = 1;
let currentPageSize = 10;
let totalPages = 1;
let totalItems = 0;

/**
 * Show a message banner (success or error)
 */
function showMessage(text, type) {
  const container = document.getElementById('message-container');
  if (!container) return;
  const div = document.createElement('div');
  div.className = `message ${type}`;
  div.textContent = text;
  container.innerHTML = '';
  container.appendChild(div);
}

/**
 * Clear the message banner
 */
function clearMessage() {
  const container = document.getElementById('message-container');
  if (container) container.innerHTML = '';
}

/**
 * Fetch movies from the API and render the page
 */
async function renderMovies() {
  const loadingEl = document.getElementById('loading');
  const gridEl = document.getElementById('movies-grid');
  
  if (loadingEl) loadingEl.style.display = 'block';
  clearMessage();

  try {
    const url = `${API_BASE}/movies?page=${currentPage}&pageSize=${currentPageSize}`;
    const response = await fetch(url);
    
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }
    
    const data = await response.json();
    const movies = data.Data || data.items || [];
    totalItems = data.Total || data.totalItems || 0;
    const page = data.Page || data.page || currentPage;
    const pageSize = data.Size || data.pageSize || currentPageSize;
    const hasNext = data.HasNextPage !== undefined ? data.HasNextPage : (page * pageSize < totalItems);
    const hasPrev = data.HasPreviousPage !== undefined ? data.HasPreviousPage : (page > 1);

    totalPages = Math.ceil(totalItems / currentPageSize);
    if (totalPages < 1) totalPages = 1;

    if (!gridEl) throw new Error('movies-grid element not found');
    gridEl.innerHTML = '';

    if (movies.length === 0) {
      gridEl.innerHTML = '<p style="grid-column: 1/-1; text-align: center; color: #666; padding: 40px 20px;">No movies found.</p>';
    } else {
      movies.forEach((movie) => {
        const card = document.createElement('div');
        card.className = 'movie-card';
        card.innerHTML = `
          <h3>${escapeHtml(movie.Title || 'Unknown')}</h3>
          <div class="year">${movie.Year || 'N/A'}</div>
          <div class="description">${escapeHtml(movie.Description || 'No description available.')}</div>
          <div class="card-buttons">
            <button class="btn btn-secondary" onclick="goToViewMovie(${movie.Id})">View</button>
            <button class="btn btn-primary" onclick="goToEditMovie(${movie.Id})">Edit</button>
            <button class="btn btn-danger" onclick="deleteMovie(${movie.Id})">Remove</button>
          </div>
        `;
        gridEl.appendChild(card);
      });
    }

    updatePaginationBar(hasPrev, hasNext);

  } catch (error) {
    console.error('Error loading movies:', error);
    showMessage(`Error loading movies: ${error.message}`, 'error');
    if (gridEl) gridEl.innerHTML = '';
  } finally {
    if (loadingEl) loadingEl.style.display = 'none';
  }
}

/**
 * Update the pagination bar with First, Prev, Next, Last buttons and items-per-page dropdown
 */
function updatePaginationBar(hasPrev, hasNext) {
  const barEl = document.getElementById('pagination-bar');
  if (!barEl) return;

  const isFirstPage = (currentPage === 1);
  const isLastPage = (currentPage >= totalPages);

  barEl.innerHTML = `
    <div class="pagination-controls">
      <span class="page-info">Page ${currentPage} of ${totalPages}</span>
      <button class="btn btn-secondary" onclick="goToFirstPage()" ${isFirstPage ? 'disabled' : ''}>First</button>
      <button class="btn btn-secondary" onclick="goToPreviousPage()" ${isFirstPage ? 'disabled' : ''}>Prev</button>
      <button class="btn btn-secondary" onclick="goToNextPage()" ${isLastPage ? 'disabled' : ''}>Next</button>
      <button class="btn btn-secondary" onclick="goToLastPage()" ${isLastPage ? 'disabled' : ''}>Last</button>
    </div>
    <div class="items-per-page">
      <label for="page-size">Items per page:</label>
      <select id="page-size" onchange="changePageSize(parseInt(this.value))">
        <option value="5" ${currentPageSize === 5 ? 'selected' : ''}>5</option>
        <option value="10" ${currentPageSize === 10 ? 'selected' : ''}>10</option>
        <option value="15" ${currentPageSize === 15 ? 'selected' : ''}>15</option>
        <option value="20" ${currentPageSize === 20 ? 'selected' : ''}>20</option>
      </select>
    </div>
  `;
}

/**
 * Go to the first page
 */
window.goToFirstPage = function() {
  if (currentPage !== 1) {
    currentPage = 1;
    renderMovies();
  }
};

/**
 * Go to the previous page
 */
window.goToPreviousPage = function() {
  if (currentPage > 1) {
    currentPage--;
    renderMovies();
  }
};

/**
 * Go to the next page
 */
window.goToNextPage = function() {
  if (currentPage < totalPages) {
    currentPage++;
    renderMovies();
  }
};

/**
 * Go to the last page
 */
window.goToLastPage = function() {
  if (currentPage !== totalPages) {
    currentPage = totalPages;
    renderMovies();
  }
};

/**
 * Change the page size and reset to page 1
 */
window.changePageSize = function(size) {
  currentPageSize = size;
  currentPage = 1;
  renderMovies();
};

/**
 * Delete a movie with confirmation
 */
window.deleteMovie = function(id) {
  if (!confirm('Are you sure you want to delete this movie?')) {
    return;
  }

  const loadingEl = document.getElementById('loading');
  if (loadingEl) loadingEl.style.display = 'block';

  fetch(`${API_BASE}/movies/${id}`, { method: 'DELETE' })
    .then((response) => {
      if (!response.ok) throw new Error(`HTTP ${response.status}`);
      return response.json();
    })
    .then((data) => {
      if (data.Success) {
        showMessage('Movie deleted successfully.', 'success');
        setTimeout(() => {
          renderMovies();
        }, 500);
      } else {
        showMessage(`Error: ${data.Message || 'Failed to delete movie'}`, 'error');
      }
    })
    .catch((error) => {
      console.error('Delete error:', error);
      showMessage(`Error deleting movie: ${error.message}`, 'error');
    })
    .finally(() => {
      if (loadingEl) loadingEl.style.display = 'none';
    });
};

/**
 * Navigate to view a movie
 */
window.goToViewMovie = function(id) {
  window.location.href = `/view-movie.html?id=${id}`;
};

/**
 * Navigate to edit a movie
 */
window.goToEditMovie = function(id) {
  window.location.href = `/edit-movie.html?id=${id}`;
};

/**
 * Simple HTML escape to prevent XSS
 */
function escapeHtml(text) {
  if (!text) return '';
  const div = document.createElement('div');
  div.textContent = text;
  return div.innerHTML;
}

/**
 * Initialize the page on load
 */
document.addEventListener('DOMContentLoaded', () => {
  renderMovies();
});
