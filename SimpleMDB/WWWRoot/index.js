// index.js - Main Movie List Logic

document.addEventListener('DOMContentLoaded', () => {
    loadMovies();

    // Pagination event listeners can be added here
});

let allMovies = [];

async function loadMovies(page = 1, size = 10) {
    try {
        const data = await apiFetch(`/api/v1/movies?page=${page}&size=${size}`);
        allMovies = data.items;
        displayMovies(allMovies);
        displayPagination(data.totalCount, page, size);
        showMessage('Movies loaded successfully');
    } catch (error) {
        showMessage(`Error loading movies: ${error.message}`, 'error');
    }
}

function filterMovies(query) {
    const filtered = allMovies.filter(movie =>
        movie.title.toLowerCase().includes(query.toLowerCase())
    );
    displayMovies(filtered);
}

function displayMovies(movies) {
    const list = document.getElementById('movie-list');
    list.innerHTML = '';

    const template = document.getElementById('movie-card-template');

    movies.forEach(movie => {
        const clone = template.content.cloneNode(true);
        clone.querySelector('.movie-title').textContent = movie.title;
        clone.querySelector('.movie-year').textContent = `Year: ${movie.year}`;
        clone.querySelector('.movie-rating').textContent = `Rating: ${movie.rating}/10`;
        clone.querySelector('.movie-description').textContent = movie.description;

        const viewLink = clone.querySelector('.view-link');
        viewLink.href = `view.html?id=${movie.id}`;

        const editLink = clone.querySelector('.edit-link');
        editLink.href = `edit.html?id=${movie.id}`;

        const deleteBtn = clone.querySelector('.delete-btn');
        deleteBtn.addEventListener('click', () => deleteMovie(movie.id));

        list.appendChild(clone);
    });
}

function displayPagination(totalCount, currentPage, pageSize) {
    const pagination = document.getElementById('pagination');
    pagination.innerHTML = '';

    const totalPages = Math.ceil(totalCount / pageSize);

    for (let i = 1; i <= totalPages; i++) {
        const btn = document.createElement('button');
        btn.textContent = i;
        btn.disabled = i === currentPage;
        btn.addEventListener('click', () => loadMovies(i, pageSize));
        pagination.appendChild(btn);
    }
}

async function deleteMovie(id) {
    if (!confirm('Are you sure you want to delete this movie?')) return;

    try {
        await apiFetch(`/api/v1/movies?id=${id}`, { method: 'DELETE' });
        showMessage('Movie deleted successfully');
        loadMovies();
    } catch (error) {
        showMessage(`Error deleting movie: ${error.message}`, 'error');
    }
}

function showMessage(message, type = 'success') {
    const msgDiv = document.getElementById('message');
    msgDiv.textContent = message;
    msgDiv.className = type;
}
