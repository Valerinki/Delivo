document.addEventListener('DOMContentLoaded', function () {
  const calendarButton = document.querySelector('[data-target="calendarContainer"]');
  const calendarContainer = document.getElementById('calendarContainer');
  const closeButton = document.getElementById('closeCalendarContainer');

  if (calendarButton && calendarContainer) {
    calendarButton.addEventListener('click', function (event) {
      event.preventDefault();
      calendarContainer.style.display = 'block'; // Afișează containerul
    });
  }

  if (closeButton) {
    closeButton.addEventListener('click', function () {
      calendarContainer.style.display = 'none'; // Ascunde containerul
    });
  }
});
