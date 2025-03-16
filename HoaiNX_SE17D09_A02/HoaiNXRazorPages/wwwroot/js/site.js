"use strict";

// Single shared connection object for SignalR - check if it's already defined first
var connection = window.connection || null;

// Function to initialize SignalR connection
function initializeSignalR() {
    console.log("Initializing SignalR connection...");

    // Only initialize if connection doesn't already exist or is in a disconnected state
    if (!connection || connection.state === signalR.HubConnectionState.Disconnected) {
        // Create a new connection if needed
        if (!connection) {
            connection = new signalR.HubConnectionBuilder()
                .withUrl("/newsHub")
                .withAutomaticReconnect()
                .build();

            // Make the connection globally available
            window.connection = connection;

            // Set up SignalR event handlers
            setupSignalRHandlers();
        }

        // Start the connection
        connection.start()
            .then(() => {
                console.log("SignalR Connected!");
            })
            .catch(function (err) {
                console.error("SignalR Connection Error:", err.toString());
            });
    } else {
        console.log("SignalR connection already established, state:", connection.state);
    }

    return connection;
}

// Set up event handlers for SignalR
function setupSignalRHandlers() {
    // Handle receiving a new news article
    connection.on("ReceiveNewNewsArticle", function (newsArticle) {
        console.log("Received new article:", newsArticle);

        // Add the new news article to the table if it exists
        var table = document.getElementById("newsTable");
        if (!table) {
            console.log("Table element not found, cannot display new article");
            return;
        }

        var tbody = table.querySelector("tbody");
        var row = tbody.insertRow(0);

        // Populate the row with news article data
        row.setAttribute("data-id", newsArticle.newsArticleId);
        row.insertCell(0).innerHTML = newsArticle.newsTitle;
        row.insertCell(1).innerHTML = newsArticle.headline;
        row.insertCell(2).innerHTML = new Date(newsArticle.createdDate).toLocaleDateString();
        row.insertCell(3).innerHTML = newsArticle.categoryName;
        var statusCell = row.insertCell(4);
        statusCell.innerHTML = `<span class="badge ${newsArticle.status ? "bg-success" : "bg-secondary"}">${newsArticle.status ? "active" : "inactive"}</span>`;
        // Add action buttons
        var actionsCell = row.insertCell(5);
        actionsCell.innerHTML = `
            <button class="btn btn-sm btn-primary edit-news" data-id="${newsArticle.newsArticleId}">
                <i class="fas fa-edit"></i> Edit
            </button>
            <button class="btn btn-sm btn-danger delete-news" data-id="${newsArticle.newsArticleId}">
                <i class="fas fa-trash"></i> Delete
            </button>
        `;

        // Attach event handlers to the new buttons
        attachEventHandlers();
    });





    // Handle updates to news articles
    connection.on("ReceiveUpdatedNewsArticle", function (newsArticle) {
        console.log("Received updated article:", newsArticle);

        var row = document.querySelector(`tr[data-id="${newsArticle.newsArticleId}"]`);
        if (row) {
            row.cells[0].innerHTML = newsArticle.newsTitle;
            row.cells[1].innerHTML = newsArticle.headline;
            row.cells[2].innerHTML = new Date(newsArticle.createdDate).toLocaleDateString();
            row.cells[3].innerHTML = newsArticle.categoryName;
            row.cells[4].innerHTML = `<span class="badge ${newsArticle.status ? "bg-success" : "bg-secondary"}">${newsArticle.status ? "active" : "inactive"}</span>`;
        }
    });

    // Handle deleted news articles
    connection.on("ReceiveDeletedNewsArticle", function (newsArticleId) {
        var row = document.querySelector(`tr[data-id="${newsArticleId}"]`);
        if (row) {
            row.remove();
        }
    });
}

// Function to handle creating a news article
//function createNewsArticle(newsArticle) {
//    if (connection) {
//        connection.invoke("CreateNewsArticle", newsArticle).catch(function (err) {
//            console.error("Error invoking CreateNewsArticle:", err.toString());
//        });
//    } else {
//        console.error("Cannot create news article - SignalR connection not established");
//    }
//}

// Function to handle updating a news article
//function updateNewsArticle(newsArticle) {
//    if (connection) {
//        connection.invoke("UpdateNewsArticle", newsArticle).catch(function (err) {
//            console.error("Error invoking UpdateNewsArticle:", err.toString());
//        });
//    } else {
//        console.error("Cannot update news article - SignalR connection not established");
//    }
//}

// Function to handle deleting a news article


// Attach event handlers to buttons
function attachEventHandlers() {
    // Edit news button
    document.querySelectorAll('.edit-news').forEach(function (button) {
        button.removeEventListener('click', editNewsHandler); // Remove first to prevent duplicates
        button.addEventListener('click', editNewsHandler);
    });

}

// Click handlers defined separately to avoid recreation
function editNewsHandler(e) {
    const button = e.currentTarget; // Use currentTarget instead of target to ensure we get the button
    const newsId = button.getAttribute('data-id');
    if (newsId) {
        openEditDialog(newsId);
    }
}

// Initialize the page
document.addEventListener('DOMContentLoaded', function () {
    // Initialize SignalR
    initializeSignalR();

    // Attach handlers to existing buttons
    attachEventHandlers();

    // Add news button (if it exists)
    const addNewsButton = document.getElementById('addNewsButton');
    if (addNewsButton) {
        addNewsButton.addEventListener('click', function () {
            openAddDialog();
        });
    }
});

function confirmLogout(event) {
    if (!confirm('Are you sure you want to log out?')) {
        event.preventDefault();
    }
}
