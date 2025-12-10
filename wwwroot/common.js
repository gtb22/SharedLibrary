export const API_BASE = 'http://localhost:5000/api/v1';

export async function fetchJson(url, options = {}) {
    const response = await fetch(url, {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...options.headers
        }
    });
    return response.json();
}

export function showMessage(elementId, message, type = 'success') {
    const el = document.getElementById(elementId);
    if (el) {
        el.textContent = message;
        el.className = `alert alert-${type}`;
        el.style.display = 'block';
    }
}

export function hideMessage(elementId) {
    const el = document.getElementById(elementId);
    if (el) {
        el.style.display = 'none';
    }
}

export function createMovieCard(movie) {
    return `
        <div class="card">
            <h3>${movie.title}</h3>
            <p class="meta">Year: ${movie.year}</p>
            ${movie.rating > 0 ? `<p class="rating">Rating: ${movie.rating}/10</p>` : ''}
            <p>${movie.description}</p>
            <div class="card-actions">
                <a href="/edit-movie.html?id=${movie.id}" class="btn btn-primary btn-sm">Edit</a>
                <button onclick="deleteMovie(${movie.id})" class="btn btn-danger btn-sm">Delete</button>
            </div>
        </div>
    `;
}

export function createActorCard(actor) {
    return `
        <div class="card">
            <h3>${actor.name}</h3>
            <p class="meta">Birth Year: ${actor.birthYear}</p>
            ${actor.bio ? `<p>${actor.bio}</p>` : ''}
            <div class="card-actions">
                <a href="/edit-actor.html?id=${actor.id}" class="btn btn-primary btn-sm">Edit</a>
                <button onclick="deleteActor(${actor.id})" class="btn btn-danger btn-sm">Delete</button>
            </div>
        </div>
    `;
}