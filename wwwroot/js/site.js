// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    function filterDropdown(inputId, dropdownId) {
        const input = document.getElementById(inputId);
        const dropdownItems = document.querySelectorAll(`#${dropdownId} .dropdown-item`);

        input.addEventListener('keyup', function () {
            const filter = input.value.toLowerCase();
            dropdownItems.forEach(function (item) {
                const text = item.textContent.toLowerCase();
                if (text.includes(filter)) {
                    item.style.display = '';
                } else {
                    item.style.display = 'none';
                }
            });
        });
    }

    function setupDropdownButton(dropdownId, buttonId) {
        const dropdownItems = document.querySelectorAll(`#${dropdownId} .dropdown-item`);
        const button = document.getElementById(buttonId);

        dropdownItems.forEach(function (item) {
            item.addEventListener('click', function () {
                button.textContent = this.textContent;
                button.setAttribute('data-selected-value', this.getAttribute('data-value'));
            });
        });
    }

    filterDropdown('dropdownSearchInputBooks', 'dropdownMenuButtonBooks');
    filterDropdown('dropdownSearchInputStudents', 'dropdownMenuButtonStudents');

    setupDropdownButton('dropdownMenuButtonBooks', 'dropdownMenuButtonBooks');
    setupDropdownButton('dropdownMenuButtonStudents', 'dropdownMenuButtonStudents');
});