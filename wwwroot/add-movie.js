import { API_BASE, fetchJson, showMessage, hideMessage } from './common.js';

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('add-movie-form');
    
    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            const movie = {
                title: document.getElementById('title').value,
                year: parseInt(document.getElementById('year').value),
                description: document.getElementById('description').value,
                genre: document.getElementById('genre').value,
                rating: parseFloat(document.getElementById('rating').value) || 0
            };

            try {
                const result = await fetchJson(`${API_BASE}/movies`, {
                    method: 'POST',
                    body: JSON.stringify(movie)
                });

                if (result.success) {
                    showMessage('message', 'Movie added successfully!', 'success');
                    form.reset();
                    setTimeout(() => {
                        window.location.href = '/index.html';
                    }, 1500);
                } else {
                    showMessage('message', result.message || 'Error adding movie', 'error');
                }
            } catch (error) {
                showMessage('message', 'Error: ' + error.message, 'error');
            }
        });
    }
});