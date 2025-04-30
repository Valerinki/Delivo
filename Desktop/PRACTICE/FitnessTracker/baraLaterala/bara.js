document.addEventListener('DOMContentLoaded', function () {
  const menuButton = document.getElementById('menuButton');
  const sidebar = document.getElementById('sidebar');
  const contentFrame = document.getElementById('contentFrame');

  // Activează/dezactivează bara laterală
  if (menuButton && sidebar) {
    menuButton.addEventListener('click', function () {
      sidebar.classList.toggle('active');
    });
  } else {
    console.error('Elementele menuButton sau sidebar nu au fost găsite.');
  }

  // Gestionarea link-urilor din bara laterală
  const sidebarLinks = document.querySelectorAll('#sidebar ul li a');
  sidebarLinks.forEach(link => {
    link.addEventListener('click', function (event) {
      event.preventDefault();
      const targetHref = this.getAttribute('href'); // Obține calea fișierului HTML
      if (contentFrame && targetHref) {
        contentFrame.src = targetHref; // Încarcă fișierul HTML în iframe
      } else {
        console.error(`Fișierul ${targetHref} nu a putut fi încărcat.`);
      }
    });
  });
});
