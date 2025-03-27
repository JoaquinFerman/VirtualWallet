document.addEventListener("DOMContentLoaded", async function() {
    const token = document.cookie.replace(/(?:(?:^|.*;\s*)token\s*=\s*([^;]*).*$)|^.*$/, "$1");

    if (!token) {
        alert("No se encontró el token, por favor inicie sesión.");
        window.location.href = "login.html";
        return;
    }

    try {
        const response = await fetch("http://localhost:5000/api/v0/balance", {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.ok) {            
            const result = await response.json();
            document.getElementById("balance").textContent = `Tu balance es: $${result.balance}`;
        } else {
            const errorData = await response.text();
            console.error("Error al obtener el balance:", errorData);
            document.getElementById("balance").textContent = "Hubo un error al obtener tu balance.";
        }
    } catch (error) {
        console.error("Error al obtener el balance:", error);
        document.getElementById("balance").textContent = "Hubo un error al obtener tu balance.";
    }

    document.getElementById("backBtn").addEventListener("click", function() {
        window.location.href = "main.html";
    });
});
