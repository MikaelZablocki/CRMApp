let companies = []; // Global variable to hold the company data
let currentSortColumn = null;
let currentSortOrder = 'asc';

// Call this function on window load or when appropriate
window.onload = function() {
    if (window.location.pathname.endsWith('login.html')) {
        setupLoginForm();  // Set up the login form functionality
    } else if (window.location.pathname.endsWith('index.html')) {
        fetchCompaniesWithContacts(); // Fetch companies for the logged-in user
        setupSearchFunctionality(); // Set up search functionality
    }
};

async function fetchCompaniesWithContacts() {
    const userId = localStorage.getItem("userId"); // Get the logged-in user's ID
    if (!userId) {
        alert("User not logged in. Redirecting to login page.");
        window.location.href = "login.html"; // Redirect if not logged in
        return;
    }

    try {
        const response = await fetch(`https://localhost:7110/api/Companies/WithCompany?userId=${userId}`);
        
        if (!response.ok) {
            throw new Error('Network response was not ok ' + response.statusText);
        }

        const companiesWithContacts = await response.json();
        console.log(companiesWithContacts); // Log the response for inspection
        companies = companiesWithContacts; // Store companies globally
        populateCompanyTable(companies); // Initial population of the table
    } catch (error) {
        console.error('Error fetching companies:', error);
    }
}

// Function to check if the search term is in sequence
function isInSequence(companyName, searchTerm) {
    let companyLower = companyName.toLowerCase();
    let termLower = searchTerm.toLowerCase();
    
    let termIndex = 0;

    for (let char of companyLower) {
        if (char === termLower[termIndex]) {
            termIndex++;
            if (termIndex === termLower.length) {
                return true; // Found all characters in sequence
            }
        }
    }
    return false; // Not found in sequence
}

function sortTable(columnIndex) {
    // Determine the sort order based on the current sorting state
    if (currentSortColumn === columnIndex) {
        currentSortOrder = (currentSortOrder === 'asc') ? 'desc' : 'asc'; // Toggle order
    } else {
        currentSortOrder = 'asc'; // Default to ascending if a new column is clicked
    }

    currentSortColumn = columnIndex; // Update the current sorted column

    // Sort the companies array based on the column index
    companies.sort((a, b) => {
        const aValue = getValueForSorting(a, columnIndex);
        const bValue = getValueForSorting(b, columnIndex);

        if (currentSortOrder === 'asc') {
            return aValue.localeCompare(bValue);
        } else {
            return bValue.localeCompare(aValue);
        }
    });

    // Refresh the company table with the sorted data
    populateCompanyTable(companies);
}

// Function to get the value for sorting based on column index
function getValueForSorting(companyWithContacts, columnIndex) {
    switch (columnIndex) {
        case 0: // Profiles
            return companyWithContacts.contacts[0]?.contactName || ""; // Get the first contact name
        case 1: // Contacts
            return companyWithContacts.contacts.length > 0 ? companyWithContacts.contacts.map(contact => contact.contactName).join(', ') : ""; // Join contact names
        case 2: // Company
            return companyWithContacts.company?.companyName || "";
        case 3: // Address
            return companyWithContacts.company?.address || "";
        case 4: // Industry
            return companyWithContacts.company?.industry || "";
        default:
            return ""; // Default return for invalid index
    }
}

function setupSearchFunctionality() {
    const filterInput = document.getElementById("filterInput");
    
    filterInput.addEventListener("input", function() {
        const searchTerm = filterInput.value; // Get the input value

        // If the search term is empty, show all companies
        if (searchTerm.trim() === "") {
            populateCompanyTable(companies); // Display all companies
            return; // Exit the function
        }

        // Filter the companies based on the search term
        const filteredCompanies = companies.filter(company => 
            startsWithIgnoreCase(company.company.companyName, searchTerm) // Check if company name starts with search term
        );

        // Update the company table with filtered results
        populateCompanyTable(filteredCompanies);
    });
}

// Function to check if a string starts with another string (case-insensitive)
function startsWithIgnoreCase(companyName, searchTerm) {
    return companyName.toLowerCase().startsWith(searchTerm.toLowerCase());
}

function populateCompanyTable(companies) {
    const tableBody = document.getElementById('companyTableBody');
    tableBody.innerHTML = ''; // Clear existing rows

    companies.forEach(companyWithContacts => {
        const row = document.createElement('tr');

        // Create a cell for the contact names (Profile)
        const profileCell = document.createElement('td');
        const contacts = companyWithContacts.contacts || []; // Default to an empty array if undefined
        
        // Create a detailed string for all contact names
        if (contacts.length > 0) {
            profileCell.innerHTML = contacts.map(contact => 
                `${contact.contactName}`
            ).join('<hr>'); // Join with horizontal line for separation
        } else {
            profileCell.textContent = "No Contacts"; // Fallback if there are no contacts
        }
        row.appendChild(profileCell);

        // Create a cell for all contacts' details
        const contactsCell = document.createElement('td');
        
        if (contacts.length > 0) {
            contactsCell.innerHTML = contacts.map(contact => 
                `Phone: ${contact.phoneNumber} <br> Email: ${contact.email}`
            ).join('<hr>'); // Join with horizontal line for separation
        } else {
            contactsCell.textContent = "No Contacts"; // Fallback if there are no contacts
        }
        row.appendChild(contactsCell);

        // Create a cell for company name
        const companyCell = document.createElement('td');
        companyCell.textContent = companyWithContacts.company?.companyName || "No Company"; // Fallback for company name
        row.appendChild(companyCell);

        // Create a cell for address
        const addressCell = document.createElement('td');
        addressCell.textContent = companyWithContacts.company?.address || "No Address"; // Fallback for address
        row.appendChild(addressCell);

        // Create a cell for industry
        const industryCell = document.createElement('td');
        industryCell.textContent = companyWithContacts.company?.industry || "No Industry"; // Fallback for industry
        row.appendChild(industryCell);

        // Create an action cell with Edit and Delete buttons
        const actionCell = document.createElement('td');

    

        // Delete button for the company
        const deleteButton = document.createElement('button');
        deleteButton.textContent = 'Delete Company';
        deleteButton.onclick = function() {
            deleteCompany(companyWithContacts.company?.companyId); // Call the delete function
        };
        actionCell.appendChild(deleteButton);

        // Add Contact button
        const addContactButton = document.createElement('button');
        addContactButton.textContent = 'Add Contact';
        addContactButton.onclick = function() {
            addContact(companyWithContacts.company?.companyId); // Call the function to add contact
        };
        actionCell.appendChild(addContactButton);

        // Button to delete contact (this will trigger the dropdown)
        const deleteContactButton = document.createElement('button');
        deleteContactButton.textContent = 'Delete Contact';
        deleteContactButton.onclick = function() {
            // Show the dropdown for selecting a contact to delete
            dropdown.style.display = 'block'; // Show the dropdown
        };
        actionCell.appendChild(deleteContactButton);

        // Create a dropdown for selecting contact to delete (hidden initially)
        const dropdown = document.createElement('select');
        dropdown.style.display = 'none'; // Hide by default
        dropdown.innerHTML = '<option value="">Select a Contact</option>'; // Default option

        contacts.forEach(contact => {
            const option = document.createElement('option');
            option.value = contact.contactId; // Set contact ID as the value
            option.textContent = contact.contactName; // Display contact name
            dropdown.appendChild(option);
        });

        actionCell.appendChild(dropdown); // Add dropdown to action cell

        // Button to confirm deletion of selected contact
        const confirmDeleteContactButton = document.createElement('button');
        confirmDeleteContactButton.textContent = 'Confirm Delete';
        confirmDeleteContactButton.style.display = 'none'; // Hidden initially

        confirmDeleteContactButton.onclick = function() {
            const selectedContactId = dropdown.value; // Get selected contact ID
            if (selectedContactId) {
                confirmDeleteContact(selectedContactId); // Call the confirm delete function
                dropdown.style.display = 'none'; // Hide dropdown after deletion
                confirmDeleteContactButton.style.display = 'none'; // Hide confirm button after deletion
            } else {
                alert("Please select a contact to delete.");
            }
        };
        actionCell.appendChild(confirmDeleteContactButton); // Add confirm button to action cell

        // Show confirm button when dropdown is displayed
        dropdown.onchange = function() {
            confirmDeleteContactButton.style.display = dropdown.value ? 'inline' : 'none'; // Show only if an option is selected
        };

        row.appendChild(actionCell);

        // Append the row to the table body
        tableBody.appendChild(row);
    });
}



// Function to confirm and delete a contact by its ID
function confirmDeleteContact(contactId) {
    if (confirm('Are you sure you want to delete this contact?')) {
        deleteContact(contactId); // Call the delete function if confirmed
    }
}

// Function to delete a contact by its ID
async function deleteContact(contactId) {
    try {
        const response = await fetch(`https://localhost:7110/api/Contacts/${contactId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            console.log("Contact deleted successfully.");
            // Refresh the company table to reflect the changes
            fetchCompaniesWithContacts(); // Re-fetch the data to update the UI
        } else {
            console.error("Failed to delete contact.");
            alert("Failed to delete contact. Please try again.");
        }
    } catch (error) {
        console.error("Error deleting contact:", error);
        alert("An error occurred while deleting the contact.");
    }
}

// Function to setup the Add Company Form
function setupAddCompanyForm() {
    document.getElementById("addCompanyForm").addEventListener("submit", async function(event) {
        event.preventDefault(); // Prevent the default form submission

        // Get the input values
        const companyName = document.getElementById("companyName").value;
        const companyAddress = document.getElementById("companyAddress").value;
        const companyIndustry = document.getElementById("companyIndustry").value;

        // Get the logged-in user's ID from localStorage
        const userId = localStorage.getItem("userId");
        
        // Create a new company object without companyId
        const newCompany = {
            companyName: companyName,
            address: companyAddress,
            industry: companyIndustry,
            userId: parseInt(userId) // Ensure userId is an integer
        };

        try {
            // Send the POST request to add the company
            const response = await fetch('https://localhost:7110/api/Companies', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': '*/*'
                },
                body: JSON.stringify(newCompany) // Convert the JS object to JSON string
            });

            if (response.ok) {
                const createdCompany = await response.json();
                console.log("Company added successfully:", createdCompany);
                // Optionally, refresh the company list or redirect
                fetchCompaniesWithContacts(); 
            } else {
                // Handle error response
                const error = await response.json();
                console.error("Error adding company:", error);
                alert("Failed to add company. Please try again.");
            }
        } catch (error) {
            console.error("Error in fetch:", error);
            alert("Error occurred. Please check the console.");
        }
    });
}

// Function to add a contact
function addContact(companyId) {
    const contactName = prompt("Enter the contact name:");
    const phoneNumber = prompt("Enter the phone number:");
    const email = prompt("Enter the email:");

    if (contactName && phoneNumber && email) {
        // Create a new contact object
        const newContact = {
            contactName: contactName,
            phoneNumber: phoneNumber,
            email: email,
            companyId: companyId // Associate with the company
        };

        // Call your API to add the contact
        fetch('https://localhost:7110/api/Contacts', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': '*/*'
            },
            body: JSON.stringify(newContact)
        })
        .then(response => {
            if (response.ok) {
                alert("Contact added successfully!");
                // Optionally, refresh the company table or re-fetch companies
                fetchCompaniesWithContacts(); // Re-fetch the data to update the UI
            } else {
                alert("Failed to add contact.");
            }
        })
        .catch(error => {
            console.error("Error adding contact:", error);
            alert("An error occurred while adding the contact.");
        });
    } else {
        alert("All fields are required.");
    }
}

// Function to delete a company
async function deleteCompany(companyId) {
    if (!confirm('Are you sure you want to delete this company?')) {
        return; // Exit if the user cancels
    }

    try {
        const response = await fetch(`https://localhost:7110/api/Companies/${companyId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Network response was not ok ' + response.statusText);
        }

        // Refresh the table after deletion
        fetchCompaniesWithContacts();
    } catch (error) {
        console.error('Error deleting company:', error);
    }
}
