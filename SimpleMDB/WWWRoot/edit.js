// edit.js - Edit Movie Logic

document.addEventListener('DOMContentLoaded', () => {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('id');

    if (!id) {
        showMessage('No movie ID provided', 'error');
        return;
    }

    loadMovie(id);

    const form = document.getElementById('edit-movie-form');
    form.addEventListener('submit', (event) => handleSubmit(event, id));
});

async function loadMovie(id) {
    try {
        const movie = await apiFetch(`/api/v1/movies?id=${id}`);
        document.getElementById('title').value = movie.title;
        document.getElementById('year').value = movie.year;
        document.getElementById('rating').value = movie.rating;
        document.getElementById('description').value = movie.description;
    } catch (error) {
        showMessage(`Error loading movie: ${error.message}`, 'error');
    }
}

async function handleSubmit(event, id) {
    event.preventDefault();

    // Client-side validation
    const title = document.getElementById('title').value.trim();
    const year = parseInt(document.getElementById('year').value);
    const rating = parseFloat(document.getElementById('rating').value);
    const description = document.getElementById('description').value.trim();

    if (!title || !description) {
        showMessage('Title and description are required', 'error');
        return;
    }

    if (isNaN(year) || year < 1888 || year > new Date().getFullYear() + 1) {
        showMessage('Invalid year', 'error');
        return;
    }

    if (isNaN(rating) || rating < 0 || rating > 10) {
        showMessage('Rating must be between 0 and 10', 'error');
        return;
    }

    const movie = { title, year, rating, description };

    try {
        await apiFetch(`/api/v1/movies?id=${id}`, {
            method: 'PUT',
            body: JSON.stringify(movie)
        });

        showMessage('Movie updated successfully!');
        setTimeout(() => window.location.href = 'index.html', 2000);
    } catch (error) {
        showMessage(`Error updating movie: ${error.message}`, 'error');
    }
}

function showMessage(message, type = 'success') {
    const msgDiv = document.getElementById('message');
    msgDiv.textContent = message;
    msgDiv.className = type;
}
