// site.js
document.addEventListener("DOMContentLoaded", function () {
    // Session timeout handling
    let sessionTimeoutMinutes = 30; // Match this with your server-side session timeout
    let warningMinutes = 5;

    // Convert to milliseconds
    let sessionTimeout = sessionTimeoutMinutes * 60 * 1000;
    let warningTimeout = (sessionTimeoutMinutes - warningMinutes) * 60 * 1000;

    // Set timers if user is logged in
    if (document.querySelector('[data-user-logged-in="true"]')) {
        // Warning timer
        setTimeout(function () {
            showSessionWarning();
        }, warningTimeout);

        // Logout timer
        setTimeout(function () {
            window.location.href = '/Auth/Logout';
        }, sessionTimeout);
    }

    function showSessionWarning() {
        // Create and show session timeout warning
        let warningDiv = document.createElement('div');
        warningDiv.className = 'alert alert-warning alert-dismissible fade show session-warning';
        warningDiv.setAttribute('role', 'alert');
        warningDiv.innerHTML = `
            <strong>Session Timeout Warning!</strong> Your session will expire in ${warningMinutes} minutes.
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            <div class="mt-2">
                <button type="button" class="btn btn-sm btn-primary" onclick="window.location.reload()">Stay Logged In</button>
            </div>
        `;

        document.body.appendChild(warningDiv);
    }
});

