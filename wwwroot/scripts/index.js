//index.js - Home page module

document.addEventListener('DOMContentLoaded', async () => {
  //Fetch and display movie count
  const movieCountEl = document.getElementById('movieCount');
  
  try {
    const response = await fetch('/api/v1/movies?page=1&pageSize=1');
    
    if (response.ok) {
      const data = await response.json();
      console.log('Movie data received:', data);
      
      if (movieCountEl) {
        const totalCount = data.totalCount || data.total || 0;
        console.log('Total movie count:', totalCount);
        
        if (totalCount > 0) {
          //Animate the count
          animateCount(movieCountEl, 0, totalCount, 1500);
        } else {
          movieCountEl.textContent = '0';
        }
      }
    } else {
      console.error('Response not OK:', response.status);
      if (movieCountEl) movieCountEl.textContent = '0';
    }
  } catch (error) {
    console.error('Failed to fetch movie count:', error);
    if (movieCountEl) movieCountEl.textContent = '0';
  }
});

//Animate counting from start to end
function animateCount(element, start, end, duration) {
  const startTime = performance.now();
  
  function update(currentTime) {
    const elapsed = currentTime - startTime;
    const progress = Math.min(elapsed / duration, 1);
    
    //Easing function for smooth animation
    const easeOutQuad = progress * (2 - progress);
    const current = Math.floor(start + (end - start) * easeOutQuad);
    
    element.textContent = current;
    
    if (progress < 1) {
      requestAnimationFrame(update);
    } else {
      element.textContent = end;
    }
  }
  
  requestAnimationFrame(update);
}