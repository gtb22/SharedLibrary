import { ApiClient, UI } from '/api.js';
let movieId = null;

document.addEventListener('DOMContentLoaded', ()=>{
  movieId = UI.getQueryParam('id');
  if(!movieId){ UI.showMessage('status-message','Missing ?id in URL.','error'); return; }
  document.getElementById('edit-btn').href = `/edit-movie.html?id=${movieId}`;
  loadMovie();
});

async function loadMovie(){
  try{
    const movie = await ApiClient.getMovie(movieId);
    document.getElementById('movie-id').textContent = movie.id || '-';
    document.getElementById('movie-title').textContent = movie.title || '-';
    document.getElementById('movie-year').textContent = movie.year || '-';
    document.getElementById('movie-description').textContent = movie.description || '-';
    UI.showMessage('status-message','Movie loaded successfully.','success');
  }catch(err){
    document.getElementById('movie-id').textContent = '-';
    document.getElementById('movie-title').textContent = '-';
    document.getElementById('movie-year').textContent = '-';
    document.getElementById('movie-description').textContent = '-';
    UI.showMessage('status-message',`Failed to load movie ${movieId}: Could not read movie with id ${movieId}.`,'error');
  }
}
