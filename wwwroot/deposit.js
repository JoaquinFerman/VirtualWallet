document.getElementById('depositForm').addEventListener('submit', async function(event) {
    event.preventDefault();

    const token = document.cookie.replace(/(?:(?:^|.*;\s*)token\s*=\s*([^;]*).*$)|^.*$/, "$1");

    if (!token) {
        alert("No se encontró el token, por favor inicie sesión.");
        window.location.href = "login.html";
        return;
    }

    const amount = document.getElementById('amount').value;

    try {
        const response = await fetch(`http://localhost:5000/api/v0/deposit?amount=${amount}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
        });
        const result = await response.json();
        const message = result.message;
        alert(message);

    } catch (error) {
        alert("Error during transfer.");
    }
});
