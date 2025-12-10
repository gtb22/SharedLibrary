import { API_BASE, fetchJson, createActorCard } from './common.js';

let currentPage = 1;
const pageSize = 5;

async function loadActors() {
    const container = document.getElementById('actors-container');
    container.innerHTML = '<div class="loading">Loading actors...</div>';

    try {
        const data = await fetchJson(`${API_BASE}/actors?page=${currentPage}&size=${pageSize}`);
        
        if (data.success && data.data) {
            const actors = data.data;
            if (actors.length === 0) {
                container.innerHTML = '<p>No actors found.</p>';
            } else {
                container.innerHTML = actors.map(createActorCard).join('');
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
        container.innerHTML = `<p class="error">Error loading actors: ${error.message}</p>`;
    }
}

window.deleteActor = async function(id) {
    if (!confirm('Are you sure?')) return;
    try {
        const result = await fetchJson(`${API_BASE}/actors/${id}`, { method: 'DELETE' });
        if (result.success) {
            loadActors();
        } else {
            alert('Error deleting actor: ' + result.message);
        }
    } catch (error) {
        alert('Error: ' + error.message);
    }
};

document.addEventListener('DOMContentLoaded', () => {
    loadActors();

    const prevBtn = document.getElementById('prev-btn');
    const nextBtn = document.getElementById('next-btn');

    if (prevBtn) {
        prevBtn.addEventListener('click', () => {
            if (currentPage > 1) {
                currentPage--;
                loadActors();
            }
        });
    }

    if (nextBtn) {
        nextBtn.addEventListener('click', () => {
            currentPage++;
            loadActors();
        });
    }
});