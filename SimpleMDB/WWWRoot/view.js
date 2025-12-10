// view.js - View Movie Logic

document.addEventListener('DOMContentLoaded', () => {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('id');

    if (!id) {
        showMessage('No movie ID provided', 'error');
        return;
    }

    loadMovie(id);

    const editLink = document.getElementById('edit-link');
    editLink.href = `edit.html?id=${id}`;
});

async function loadMovie(id) {
    try {
        const movie = await apiFetch(`/api/v1/movies?id=${id}`);
        document.getElementById('movie-title').textContent = movie.title;
        document.getElementById('movie-year').textContent = movie.year;
        document.getElementById('movie-rating').textContent = `${movie.rating}/10`;
        document.getElementById('movie-description').textContent = movie.description;
    } catch (error) {
        showMessage(`Error loading movie: ${error.message}`, 'error');
    }
}

function showMessage(message, type = 'success') {
    const msgDiv = document.getElementById('message');
    msgDiv.textContent = message;
    msgDiv.className = type;
}
