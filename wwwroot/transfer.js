document.getElementById('transferForm').addEventListener('submit', async function(event) {
    event.preventDefault();

    const token = document.cookie.replace(/(?:(?:^|.*;\s*)token\s*=\s*([^;]*).*$)|^.*$/, "$1");

    if (!token) {
        alert("Token not found, please re login.");
        window.location.href = "login.html";
        return;
    }

    const username = document.getElementById('target').value;
    const amount = document.getElementById('amount').value;

    const transferData = { username, amount };

    try {
        const response = await fetch("http://localhost:5000/api/v0/transfer", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(transferData)
        });
        if (response.ok) {
            const result = await response.json();
            alert("Transfer successful.");
        } else {
            const errorData = await response.text();
            alert(`Error: ${errorData}`);
        }
    } catch (error) {
        alert("Error during transfer.");
    }
});
