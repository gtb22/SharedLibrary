import { ApiClient, UI } from '/api.js';

document.addEventListener('DOMContentLoaded', ()=>{
  const form = document.getElementById('movie-form');
  form.addEventListener('submit', handleSubmit);
});

async function handleSubmit(e){
  e.preventDefault();
  const title = document.getElementById('title').value.trim();
  const year = document.getElementById('year').value.trim();
  const description = document.getElementById('description').value.trim();
  if(!title || !year){ UI.showMessage('status-message','Please fill in required fields.','error'); return; }
  try{
    await ApiClient.createMovie(title, year, description);
    UI.showMessage('status-message','Movie created successfully. Redirecting...','success');
    setTimeout(()=> window.location.href='/movies.html',1200);
  }catch(err){ UI.showMessage('status-message',`Error: ${err.message}`,'error'); }
}
