// Esperamos que el DOM esté completamente cargado antes de agregar los eventos
document.addEventListener("DOMContentLoaded", function() {

    function redirectToPage(page) {
        window.location.href = `${page}.html`;
    }

    document.getElementById('balanceBtn').addEventListener('click', function() {
        redirectToPage("balance");
    });

    document.getElementById('depositBtn').addEventListener('click', function() {
        redirectToPage("deposit");
    });

    document.getElementById('transferBtn').addEventListener('click', function() {
        redirectToPage("transfer");
    });

    document.getElementById('loansBtn').addEventListener('click', function() {
        redirectToPage("loans");
    });

});
