import { API_BASE, fetchJson, showMessage, hideMessage } from './common.js';

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('add-actor-form');
    
    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            const actor = {
                name: document.getElementById('name').value,
                birthYear: parseInt(document.getElementById('birthYear').value),
                bio: document.getElementById('bio').value
            };

            try {
                const result = await fetchJson(`${API_BASE}/actors`, {
                    method: 'POST',
                    body: JSON.stringify(actor)
                });

                if (result.success) {
                    showMessage('message', 'Actor added successfully!', 'success');
                    form.reset();
                    setTimeout(() => {
                        window.location.href = '/actors.html';
                    }, 1500);
                } else {
                    showMessage('message', result.message || 'Error adding actor', 'error');
                }
            } catch (error) {
                showMessage('message', 'Error: ' + error.message, 'error');
            }
        });
    }
});