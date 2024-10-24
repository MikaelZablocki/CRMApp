
let companies = []; // Global variable to hold the company data

window.onload = function() {
    if (window.location.pathname.endsWith('login.html')) {
        setupLoginForm();  // Set up the login form functionality
    } else if (window.location.pathname.endsWith('index.html')) {
        setupDashboard();   // Set up the dashboard functionality
    }
};

// Function to handle login form submission
function setupLoginForm() {
    document.getElementById("login-form").addEventListener("submit", async function(event) {
        event.preventDefault(); // Prevent the default form submission

        const username = document.getElementById("username").value;
        const password = document.getElementById("password").value;

        try {
            const response = await fetch(`https://localhost:7110/api/Users/login?username=${username}&password=${password}`, {
                method: 'GET',
                headers: {
                    'Accept': '*/*'
                }
            });

            if (response.ok) {
                const user = await response.json();
                console.log("Login successful:", user);

                // Store user ID in local storage
                localStorage.setItem("userId", user.userId);

                // Redirect to the dashboard
                window.location.href = "index.html"; 
            } else {
                // Handle invalid login
                document.getElementById("login-error-message").style.display = 'block'; 
            }
        } catch (error) {
            console.error("Error logging in:", error);
            document.getElementById("login-error-message").style.display = 'block'; 
        }
    });
}




