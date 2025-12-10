import { API_BASE, fetchJson, createMovieCard } from './common.js';

let currentPage = 1;
const pageSize = 5;

async function loadMovies() {
    const container = document.getElementById('movies-container');
    container.innerHTML = '<div class="loading">Loading movies...</div>';

    try {
        const data = await fetchJson(`${API_BASE}/movies?page=${currentPage}&size=${pageSize}`);
        
        if (data.success && data.data) {
            const movies = data.data;
            if (movies.length === 0) {
                container.innerHTML = '<p>No movies found.</p>';
            } else {
                container.innerHTML = movies.map(createMovieCard).join('');
            }

            // Update pagination
            const pageInfo = document.getElementById('page-info');
            if (pageInfo) {
                pageInfo.textContent = `Page ${data.page}`;
            }

            const prevBtn = document.getElementById('prev-btn');
            const nextBtn = document.getElementById('next-btn');
            if (prevBtn) prevBtn.disabled = currentPage === 1;
            if (nextBtn) nextBtn.disabled = !data.hasNextPage;
        }
    } catch (error) {
        container.innerHTML = `<p class="error">Error loading movies: ${error.message}</p>`;
    }
}

window.deleteMovie = async function(id) {
    if (!confirm('Are you sure?')) return;
    try {
        const result = await fetchJson(`${API_BASE}/movies/${id}`, { method: 'DELETE' });
        if (result.success) {
            loadMovies();
        } else {
            alert('Error deleting movie: ' + result.message);
        }
    } catch (error) {
        alert('Error: ' + error.message);
    }
};

document.addEventListener('DOMContentLoaded', () => {
    loadMovies();

    const prevBtn = document.getElementById('prev-btn');
    const nextBtn = document.getElementById('next-btn');

    if (prevBtn) {
        prevBtn.addEventListener('click', () => {
            if (currentPage > 1) {
                currentPage--;
                loadMovies();
            }
        });
    }

    if (nextBtn) {
        nextBtn.addEventListener('click', () => {
            currentPage++;
            loadMovies();
        });
    }
});