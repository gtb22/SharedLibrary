// add.js - Add Movie Logic

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('add-movie-form');
    form.addEventListener('submit', handleSubmit);
});

async function handleSubmit(event) {
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
        await apiFetch('/api/v1/movies', {
            method: 'POST',
            body: JSON.stringify(movie)
        });

        showMessage('Movie added successfully!');
        setTimeout(() => window.location.href = 'index.html', 2000);
    } catch (error) {
        showMessage(`Error adding movie: ${error.message}`, 'error');
    }
}

function showMessage(message, type = 'success') {
    const msgDiv = document.getElementById('message');
    msgDiv.textContent = message;
    msgDiv.className = type;
}
