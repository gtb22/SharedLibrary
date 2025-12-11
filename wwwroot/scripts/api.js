//api.js - API client and UI helpers
const API_BASE = '/api/v1';

export const ApiClient = {
  async fetchMovies(page=1, pageSize=15){
    const url = `${API_BASE}/movies?page=${page}&pageSize=${pageSize}`;
    const res = await fetch(url);
    if(!res.ok) throw new Error(`HTTP ${res.status}`);
    return res.json();
  },
  async getMovie(id){
    const res = await fetch(`${API_BASE}/movies/${id}`);
    if(!res.ok) throw new Error(`Could not read movie with id ${id}.`);
    const json = await res.json();
    return json.data || json;
  },
  async createMovie(title, year, description){
    const res = await fetch(`${API_BASE}/movies`,{
      method:'POST', headers:{'Content-Type':'application/json'},
      body: JSON.stringify({title, year: parseInt(year), description})
    });
    if(!res.ok){ const err = await res.json().catch(()=>({message:res.statusText})); throw new Error(err.message||res.statusText); }
    return res.json();
  },
  async updateMovie(id, title, year, description){
    const res = await fetch(`${API_BASE}/movies/${id}`,{
      method:'PUT', headers:{'Content-Type':'application/json'},
      body: JSON.stringify({title, year: parseInt(year), description})
    });
    if(!res.ok){ const err = await res.json().catch(()=>({message:res.statusText})); throw new Error(err.message||res.statusText); }
    return res.json();
  },
  async deleteMovie(id){
    const res = await fetch(`${API_BASE}/movies/${id}`, { method:'DELETE' });
    if(!res.ok) throw new Error(`HTTP ${res.status}`);
    return res.json().catch(()=>({success:true}));
  }
}

export const UI = {
  showMessage(id, text, type='success'){
    const el = document.getElementById(id); if(!el) return;
    el.textContent = text; el.classList.remove('hidden'); el.classList.remove('success','error');
    el.classList.add(type==='success'?'message success':'message error');
  },
  hideMessage(id){ const el=document.getElementById(id); if(!el) return; el.classList.add('hidden'); },
  getQueryParam(name){ return new URLSearchParams(window.location.search).get(name); }
}
