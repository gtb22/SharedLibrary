import { ApiClient, UI } from '/api.js';

let currentPage = 1;
let pageSize = 15;
let totalPages = 1;

document.addEventListener('DOMContentLoaded', () => {
  setupEventListeners();
  loadMovies();
});

function setupEventListeners(){
  const pageSizeSelect = document.getElementById('page-size-select');
  document.getElementById('btn-first').addEventListener('click', ()=>{ currentPage = 1; loadMovies(); });
  document.getElementById('btn-prev').addEventListener('click', ()=>{ if(currentPage>1){ currentPage--; loadMovies(); }});
  document.getElementById('btn-next').addEventListener('click', ()=>{ if(currentPage<totalPages){ currentPage++; loadMovies(); }});
  document.getElementById('btn-last').addEventListener('click', ()=>{ currentPage = totalPages; loadMovies(); });
  pageSizeSelect.addEventListener('change', (e)=>{ pageSize = parseInt(e.target.value); currentPage = 1; loadMovies(); });
}

async function loadMovies(){
  UI.showMessage('error-message','', 'success'); UI.hideMessage('error-message'); UI.hideMessage('success-message');
  const loadingEl = document.getElementById('loading'); const grid = document.getElementById('movies-grid');
  loadingEl.classList.remove('hidden'); grid.innerHTML = '';
  try{
    const result = await ApiClient.fetchMovies(currentPage, pageSize);
    // result expected: { Success: true, Data: [...], Page, Size, Total }
    let movies = result.data || result.Data || result;
    const total = result.total || result.Total || (result.TotalCount || movies.length || 0);
    if (!Array.isArray(movies)) movies = [];
    totalPages = Math.max(1, Math.ceil((total||movies.length)/pageSize));
    renderMovies(movies);
    updatePaginationControls();
    loadingEl.classList.add('hidden');
  }catch(err){
    loadingEl.classList.add('hidden'); UI.showMessage('error-message', `Error loading movies: ${err.message}`, 'error');
  }
}

function renderMovies(movies){
  const grid = document.getElementById('movies-grid');
  grid.innerHTML = '';
  if(!movies || movies.length===0){ grid.innerHTML = '<p class="text-center">No movies found.</p>'; return; }
  movies.forEach(movie=>{
    const card = document.createElement('div'); card.className='movie-card';
    card.innerHTML = `\n      <h3>${escapeHtml(movie.title)}</3>\n      <div class="year">${movie.year||''}</div>\n      <div class="description">${escapeHtml(movie.description||'')}</div>\n      <div class="card-buttons">\n        <button class="btn btn-primary view-btn" data-id="${movie.id}">View</button>\n        <button class="btn btn-primary edit-btn" data-id="${movie.id}">Edit</button>\n        <button class="btn btn-danger delete-btn" data-id="${movie.id}">Remove</button>\n      </div>`;
    // fix malformed tag closure above after template injection
    // attach handlers
    card.querySelector('.view-btn').addEventListener('click', ()=> window.location.href = `/view-movie.html?id=${movie.id}`);
    card.querySelector('.edit-btn').addEventListener('click', ()=> window.location.href = `/edit-movie.html?id=${movie.id}`);
    card.querySelector('.delete-btn').addEventListener('click', async ()=>{
      if(!confirm(`Delete "${movie.title}"?`)) return;
      try{ await ApiClient.deleteMovie(movie.id); UI.showMessage('success-message','Movie removed successfully.','success'); loadMovies(); }
      catch(err){ UI.showMessage('error-message',`Error deleting movie: ${err.message}`,'error'); }
    });
    // ensure tag is correct: repair unintended characters
    // replace the malformed closing tag sequence
    card.innerHTML = card.innerHTML.replace('</\u007f3>','</h3>');
    grid.appendChild(card);
  });
}

function updatePaginationControls(){
  document.getElementById('page-label').textContent = `Page ${currentPage}`;
  document.getElementById('btn-first').disabled = currentPage===1;
  document.getElementById('btn-prev').disabled = currentPage===1;
  document.getElementById('btn-next').disabled = currentPage>=totalPages;
  document.getElementById('btn-last').disabled = currentPage>=totalPages;
}

function escapeHtml(s){ if(!s) return ''; return String(s).replace(/[&<>"']/g, c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":"&#039;"})[c]); }

export { loadMovies };
