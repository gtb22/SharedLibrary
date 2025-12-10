import { ApiClient, UI } from '/api.js';
let movieId = null;

document.addEventListener('DOMContentLoaded', ()=>{
  movieId = UI.getQueryParam('id');
  const form = document.getElementById('movie-form');
  if(!movieId){ UI.showMessage('status-message','Missing ?id in URL.','error'); form.querySelectorAll('input,textarea').forEach(i=>i.disabled=true); return; }
  loadMovie();
  form.addEventListener('submit', handleSubmit);
});

async function loadMovie(){
  try{
    const movie = await ApiClient.getMovie(movieId);
    document.getElementById('title').value = movie.title || '';
    document.getElementById('year').value = movie.year || '';
    document.getElementById('description').value = movie.description || '';
    UI.showMessage('status-message','Loaded movie. You can edit and save.','success');
  }catch(err){
    UI.showMessage('status-message',`Failed to load movie ${movieId}: Could not read movie with id ${movieId}.`,'error');
    document.getElementById('movie-form').querySelectorAll('input,textarea').forEach(i=>i.disabled=true);
  }
}

async function handleSubmit(e){
  e.preventDefault();
  const title = document.getElementById('title').value.trim();
  const year = document.getElementById('year').value.trim();
  const description = document.getElementById('description').value.trim();
  if(!title||!year){ UI.showMessage('status-message','Please fill required fields.','error'); return; }
  try{
    await ApiClient.updateMovie(movieId, title, year, description);
    UI.showMessage('status-message','Movie updated successfully.','success');
  }catch(err){ UI.showMessage('status-message',`Error: ${err.message}`,'error'); }
}
