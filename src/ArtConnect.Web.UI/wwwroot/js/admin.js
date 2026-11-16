(function () {
    var toggles = document.querySelectorAll('[data-toggle-sidebar]');
    var sidebar = document.getElementById('sidebar');
    var backdrop = document.querySelector('.sidebar-backdrop');

    toggles.forEach(function (el) {
        el.addEventListener('click', function () {
            var open = sidebar.classList.toggle('open');
            if (backdrop) { backdrop.classList.toggle('show', open); }
        });
    });
})();
