document.addEventListener("DOMContentLoaded", async function () {
    const loansContainer = document.getElementById("LoansContainer");
    const token = document.cookie.replace(/(?:(?:^|.*;\s*)token\s*=\s*([^;]*).*$)|^.*$/, "$1");

    if (!token) {
        alert("Token not found, please re login.");
        window.location.href = "login.html";
        return;
    }

    async function fetchLoans() {
        try {
            const response = await fetch("http://localhost:5000/api/v0/loans", {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                }
            });
            
            if (!response.ok) {
                const errorMessage = await response.text();
                loansContainer.innerHTML = `<p style='color: red;'>Error: ${errorMessage}</p>`;
                return;
            }
            const result = await response.json();
            const loans = result.loans;
            displayLoans(loans);

        } catch (error) {
            console.error("Error fetching loans:", error);
            loansContainer.innerHTML = "<p style='color: red;'>Failed to load loans.</p>";
        }
    }

    function displayLoans(loans) {
        loansContainer.innerHTML = "<h2>Active Loans</h2>";

        if (loans.length === 0) {
            loansContainer.innerHTML += "<p>No active loans found.</p>";
            return;
        }

        const ul = document.createElement("ul");

        loans.forEach(loan => {
            const li = document.createElement("li");

            li.innerHTML = `
                Amount: $${loan.remainingValue} - Type: ${loan.loanType} - Interest: ${loan.interest}% - Status: ${loan.defaulting ? "Defaulting" : "Active"}
                <input type="number" id="payAmount-${loan.id}" min="1" placeholder="Enter amount">
                <button onclick="payLoan(${loan.id})">Pay</button>
            `;

            ul.appendChild(li);
        });

        loansContainer.appendChild(ul);
    }

    window.payLoan = async function (id) {
        const payInput = document.getElementById(`payAmount-${id}`);
        const amount = parseFloat(payInput.value);

        if (isNaN(amount) || amount <= 0) {
            alert("Please enter a valid payment amount.");
            return;
        }

        try {
            const response = await fetch(`http://localhost:5000/api/v0/loans`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                },
                body: JSON.stringify({ id, amount })
            });

            if (!response.ok) {
                const errorMessage = await response.text();
                alert(`Error: ${errorMessage}`);
                return;
            }

            alert("Payment successful!");
            fetchLoans();
        } catch (error) {
            console.error("Error processing payment:", error);
            alert("Failed to process payment.");
        }
    };

    window.requestLoan = async function () {
        const value = document.getElementById("loanValue").value;
        const type = document.getElementById("loanType").value;
        const interest = document.getElementById("loanInterest").value;
        try {
            const response = await fetch(`http://localhost:5000/api/v0/loans`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                },
                body: JSON.stringify({ value: value, loantype: type, interest: interest})
            });

            if (!response.ok) {
                const errorMessage = await response.text();
                alert(`Error: ${errorMessage}`);
                return;
            }

            alert("Loan requested successfully!");
            fetchLoans();
        } catch (error) {
            alert("Failed to process request.");
        }
    }
    fetchLoans();
});