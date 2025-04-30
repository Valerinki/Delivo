document.addEventListener('DOMContentLoaded', function () {
  const userData = JSON.parse(localStorage.getItem('userData'));
  if (!userData) {
    window.location.href = "../registrarea/register.html";
  }

  const logoutButton = document.getElementById('logoutButton');
  if (logoutButton) {
    logoutButton.addEventListener('click', function (event) {
      event.preventDefault();
      localStorage.removeItem('userData');
      // Redirecționează utilizatorul la pagina de autentificare
      window.location.href = "autentificarea/auth.html";
    });
  } else {
    console.error('Elementul logoutButton nu există în DOM.');
  }
});