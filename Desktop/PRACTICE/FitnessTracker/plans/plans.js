document.addEventListener('DOMContentLoaded', function () {
  const plansButton = document.querySelector('[data-target="plansContainer"]');
  const plansContainer = document.getElementById('plansContainer');
  const closeButton = document.getElementById('closePlansContainer');

  if (plansButton && plansContainer) {
    plansButton.addEventListener('click', function (event) {
      event.preventDefault();
      plansContainer.style.display = 'block'; // Afișează containerul
    });
  }

  if (closeButton) {
    closeButton.addEventListener('click', function () {
      plansContainer.style.display = 'none'; // Ascunde containerul
    });
  }
});
