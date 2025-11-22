// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Theme Management
(function () {
    const THEME_KEY = 'theme-preference';
    const LIGHT_THEME = 'light';
    const DARK_THEME = 'dark';

    // Get saved theme or system preference
    function getThemePreference() {
        // Check localStorage
        const savedTheme = localStorage.getItem(THEME_KEY);
        if (savedTheme) {
            return savedTheme;
        }

        // Check system preference
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return DARK_THEME;
        }

        return LIGHT_THEME;
    }

    // Apply theme to document
    function applyTheme(theme) {
        const html = document.documentElement;
        html.setAttribute('data-theme', theme);
        localStorage.setItem(THEME_KEY, theme);
        updateThemeButton(theme);
    }

    // Update button appearance
    function updateThemeButton(theme) {
        const btn = document.getElementById('theme-toggle-btn');
        if (btn) {
            btn.textContent = theme === DARK_THEME ? '☀️' : '🌙';
            btn.title = theme === DARK_THEME ? 'Switch to Light Mode' : 'Switch to Dark Mode';
            btn.setAttribute('aria-label', btn.title);
        }
    }

    // Toggle theme
    function toggleTheme() {
        const currentTheme = document.documentElement.getAttribute('data-theme') || LIGHT_THEME;
        const newTheme = currentTheme === LIGHT_THEME ? DARK_THEME : LIGHT_THEME;
        applyTheme(newTheme);
    }

    // Initialize on page load
    document.addEventListener('DOMContentLoaded', function () {
        const theme = getThemePreference();
        applyTheme(theme);

        // Attach event listener to toggle button
        const btn = document.getElementById('theme-toggle-btn');
        if (btn) {
            btn.addEventListener('click', toggleTheme);
        }
    });

    // Listen to system theme changes
    if (window.matchMedia) {
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            if (!localStorage.getItem(THEME_KEY)) {
                applyTheme(e.matches ? DARK_THEME : LIGHT_THEME);
            }
        });
    }

    // Expose toggle function globally (if needed from other scripts)
    window.toggleTheme = toggleTheme;
})();
