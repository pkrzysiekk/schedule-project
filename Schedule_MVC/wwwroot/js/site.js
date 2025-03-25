document.addEventListener('DOMContentLoaded', function () {
    const toggleButton = document.getElementById('theme');
    const body = document.body;

    //Sprawdzamy, czy uzytkownik ma zapisany preferowany tryb w przeglądarce
    if (localStorage.getItem('theme') === 'dark') {
        body.classList.add('dark-mode');
        toggleButton.textContent = 'Switch to light mode';
    }
    else {
        toggleButton.textContent = 'Switch to dark mode';
    }

    toggleButton.addEventListener('click', function () {
        if (body.classList.contains('dark-mode')) {
            body.classList.remove('dark-mode');
            toggleButton.textContent = 'Switch to dark mode';
            localStorage.setItem('theme', 'light');
        } else {
            body.classList.add('dark-mode');
            toggleButton.textContent = 'Switch to light mode';
            localStorage.setItem('theme', 'dark');
        }
    });
});