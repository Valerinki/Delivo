document.addEventListener("DOMContentLoaded", () => {
  // Înregistrare
  const registerForm = document.getElementById("registerForm");
  if (registerForm) {
    registerForm.addEventListener("submit", async function (event) {
      event.preventDefault(); // Previne reîncărcarea paginii

      const username = document.getElementById("username").value; // Numele utilizatorului
      const email = document.getElementById("email").value;
      const password = document.getElementById("password").value;
      const errorElement = document.getElementById("error");

      // Validare parolă
      const passwordRegex = /^(?=.*[A-Z])(?=.*[!@#$%^&*])(?=.{8,})/;
      if (!passwordRegex.test(password)) {
        errorElement.textContent =
          "Parola trebuie să conțină minim 8 caractere, o literă mare și un simbol.";
        return;
      }

      try {
        const response = await fetch("/api/register", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ username, email, password }),
        });

        const result = await response.json();

        if (result.success) {
          alert("Înregistrarea a fost realizată cu succes!");
          window.location.href = "/main.html"; // Redirecționează utilizatorul la pagina principală
        } else {
          errorElement.textContent = result.message || "Eroare necunoscută.";
        }
      } catch (error) {
        console.error("Eroare la înregistrare:", error);
        errorElement.textContent = "Eroare la conectarea cu serverul.";
      }
    });
  }

  // Autentificare
  const loginForm = document.getElementById("loginForm");
  if (loginForm) {
    loginForm.addEventListener("submit", async function (event) {
      event.preventDefault();

      const email = document.getElementById("email").value;
      const password = document.getElementById("password").value;
      const errorElement = document.getElementById("error");

      try {
        const response = await fetch("/api/login", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ email, password }),
        });

        const result = await response.json();

        if (result.success) {
          alert("Autentificare reușită!");
          window.location.href = result.redirect; // Redirecționează utilizatorul la pagina principală
        } else {
          errorElement.textContent = result.message || "Email sau parolă incorectă.";
        }
      } catch (error) {
        console.error("Eroare la autentificare:", error);
        errorElement.textContent = "Eroare la conectarea cu serverul.";
      }
    });
  }

  // Afișează numele utilizatorului în navbar
  const usernameElement = document.getElementById("username");
  if (usernameElement) {
    const username = localStorage.getItem("username") || "Nume Utilizator";
    usernameElement.textContent = username; // Setează numele utilizatorului
  }

  // Salvare antrenamente
  const trainingForm = document.getElementById("trainingForm");
  if (trainingForm) {
    trainingForm.addEventListener("submit", (event) => {
      event.preventDefault();

      const exercise = document.getElementById("exercise").value;
      const sets = document.getElementById("sets").value;

      if (!exercise || !sets) {
        alert("Completați toate câmpurile pentru a salva antrenamentul.");
        return;
      }

      const trainings = JSON.parse(localStorage.getItem("trainings")) || [];
      trainings.push({ exercise, sets, date: new Date().toLocaleString() });
      localStorage.setItem("trainings", JSON.stringify(trainings));

      alert("Antrenamentul a fost salvat!");
      trainingForm.reset();

      // Actualizează lista de antrenamente
      displayTrainings();
    });
  }

  // Funcție pentru afișarea antrenamentelor salvate
  function displayTrainings() {
    const trainingList = document.getElementById("trainingList");
    const trainings = JSON.parse(localStorage.getItem("trainings")) || [];

    trainingList.innerHTML = ""; // Golește lista înainte de a o popula

    if (trainings.length === 0) {
      trainingList.innerHTML = "<p>Nu există antrenamente salvate.</p>";
    } else {
      trainings.forEach((training) => {
        const li = document.createElement("li");
        li.textContent = `${training.date} - ${training.exercise}: ${training.sets}`;
        trainingList.appendChild(li);
      });
    }
  }

  // Afișează antrenamentele la încărcarea paginii
  displayTrainings();
});
