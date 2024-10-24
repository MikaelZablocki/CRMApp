using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Api
{
    public class Repository
    {
        protected const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CrmDB;Integrated Security=True;";

        // General method to handle database connection and commands
        private T ExecuteCommand<T>(string query, Func<SqlCommand, T> commandFunc, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }
                    return commandFunc(command);
                }
            }
        }

        // Helper method for executing queries that return a single value
        private T ExecuteScalar<T>(string query, Dictionary<string, object> parameters = null)
        {
            return ExecuteCommand(query, cmd => (T)cmd.ExecuteScalar(), parameters);
        }

        // Helper method for executing queries that return rows
        private List<T> ExecuteReader<T>(string query, Func<SqlDataReader, T> mapFunc, Dictionary<string, object> parameters = null)
        {
            return ExecuteCommand(query, cmd =>
            {
                var results = new List<T>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(mapFunc(reader));
                    }
                }
                return results;
            }, parameters);
        }

        // Helper method for non-query commands (INSERT, UPDATE, DELETE)
        private void ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            ExecuteCommand(query, cmd =>
            {
                cmd.ExecuteNonQuery();
                return 0; // No return value for non-query commands
            }, parameters);
        }

        // Get a user by ID
        public string GetUserById(int userId)
        {
            string query = "SELECT Username FROM Users WHERE UserId = @UserId";
            return ExecuteScalar<string>(query, new Dictionary<string, object> { { "@UserId", userId } });
        }

        // Add a new user
        public void AddUser(User user)
        {
            string query = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@Password", user.Password }  // Simple password storage (use hashed/salted passwords in real apps)
            };
            ExecuteNonQuery(query, parameters);
        }

        // Get a user by credentials
        public User GetUserByCredentials(string username, string password)
        {
            string query = "SELECT UserId, Username, Password FROM Users WHERE Username = @Username AND Password = @Password";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", username },
                { "@Password", password }
            };

            var users = ExecuteReader(query, reader =>
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2)
                };
            }, parameters);

            return users.Count > 0 ? users[0] : null;
        }

        // Get a company by ID
        public Company GetCompanyById(int companyId)
        {
            string query = "SELECT CompanyId, CompanyName FROM Companies WHERE CompanyId = @CompanyId";
            var parameters = new Dictionary<string, object>
            {
                { "@CompanyId", companyId }
            };

            var companies = ExecuteReader(query, reader => new Company
            {
                CompanyId = reader.GetInt32(0),
                CompanyName = reader.GetString(1)
            }, parameters);

            return companies.Count > 0 ? companies[0] : null;
        }

        // Get all companies
        public IEnumerable<Company> GetAllCompanies()
        {
            string query = "SELECT CompanyId, CompanyName, Address, Industry, UserId FROM Companies"; // Fetch Address and Industry
            return ExecuteReader(query, reader => new Company
            {
                CompanyId = reader.GetInt32(0),
                CompanyName = reader.GetString(1),
                Address = reader.GetString(2), // Read Address
                Industry = reader.GetString(3), // Read Industry
                UserId = reader.GetInt32(4) // Read UserId
            });
        }

      

        public void AddCompany(Company company)
        {
            string query = "INSERT INTO Companies (CompanyName, Address, Industry, UserId) VALUES (@CompanyName, @Address, @Industry, @UserId)";
            var parameters = new Dictionary<string, object>
    {
        { "@CompanyName", company.CompanyName },
        { "@Address", company.Address },     // Include Address
        { "@Industry", company.Industry },   // Include Industry
        { "@UserId", company.UserId }        // UserId will now be set in the controller
    };
            ExecuteNonQuery(query, parameters);
        }



        // Get a contact by ID
        public Contact GetContactById(int contactId)
        {
            string query = "SELECT ContactId, ContactName FROM Contacts WHERE ContactId = @ContactId";
            var parameters = new Dictionary<string, object>
            {
                { "@ContactId", contactId }
            };

            var contacts = ExecuteReader(query, reader => new Contact
            {
                ContactId = reader.GetInt32(0),
                ContactName = reader.GetString(1)
            }, parameters);

            return contacts.Count > 0 ? contacts[0] : null;
        }

        // Get all contacts
        public IEnumerable<Contact> GetAllContacts()
        {
            string query = "SELECT ContactId, ContactName, PhoneNumber, Email, CompanyId FROM Contacts";
            return ExecuteReader(query, reader => new Contact
            {
                ContactId = reader.GetInt32(0),
                ContactName = reader.GetString(1),
                PhoneNumber = reader.GetString(2),
                Email = reader.GetString(3),
                CompanyId = reader.GetInt32(4)
            });
        }

        // Add a new contact
        public void AddContact(Contact contact)
        {
            string query = "INSERT INTO Contacts (ContactName, PhoneNumber, Email, CompanyId) VALUES (@ContactName, @PhoneNumber, @Email, @CompanyId)";
            var parameters = new Dictionary<string, object>
            {
                { "@ContactName", contact.ContactName },
                { "@PhoneNumber", contact.PhoneNumber },
                { "@Email", contact.Email },
                { "@CompanyId", contact.CompanyId }
            };
            ExecuteNonQuery(query, parameters);
        }

        public void DeleteContact(int contactId)
        {
            // Define the SQL DELETE query
            string query = "DELETE FROM Contacts WHERE ContactId = @ContactId";

            // Create a dictionary to store the parameter for the query
            var parameters = new Dictionary<string, object>
    {
        { "@ContactId", contactId }
    };

            // Execute the DELETE query using the ExecuteNonQuery method
            ExecuteNonQuery(query, parameters);
        }
       

        public class CompanyWithContacts
        {
            public Company Company { get; set; }
            public List<Contact> Contacts { get; set; }
        }

        public IEnumerable<CompanyWithContacts> GetCompaniesWithContacts()
        {
            var companiesWithContacts = new List<CompanyWithContacts>();

            // First, get all companies
            string companyQuery = "SELECT CompanyId, CompanyName, Address, Industry, UserId FROM Companies";
            var companies = ExecuteReader(companyQuery, reader => new Company
            {
                CompanyId = reader.GetInt32(0),
                CompanyName = reader.GetString(1),
                Address = reader.GetString(2),
                Industry = reader.GetString(3),
                UserId = reader.GetInt32(4) // Assuming UserId is a part of the company
            });

            // Now get all contacts
            string contactQuery = "SELECT ContactId, ContactName, PhoneNumber, Email, CompanyId FROM Contacts";
            var contacts = ExecuteReader(contactQuery, reader => new Contact
            {
                ContactId = reader.GetInt32(0),
                ContactName = reader.GetString(1),
                PhoneNumber = reader.GetString(2),
                Email = reader.GetString(3),
                CompanyId = reader.GetInt32(4)
            });

            // Group contacts by CompanyId
            var contactsGrouped = contacts.GroupBy(c => c.CompanyId)
                                          .ToDictionary(g => g.Key, g => g.ToList());

            // Combine companies and their contacts
            foreach (var company in companies)
            {
                companiesWithContacts.Add(new CompanyWithContacts
                {
                    Company = company,
                    Contacts = contactsGrouped.ContainsKey(company.CompanyId) ? contactsGrouped[company.CompanyId] : new List<Contact>()
                });
            }

            return companiesWithContacts;
        }

        public void DeleteCompany(int companyId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // First, delete the contacts associated with the company
                var deleteContactsCommand = new SqlCommand("DELETE FROM Contacts WHERE CompanyId = @CompanyId", connection);
                deleteContactsCommand.Parameters.AddWithValue("@CompanyId", companyId);
                deleteContactsCommand.ExecuteNonQuery();

                // Now, delete the company
                var deleteCompanyCommand = new SqlCommand("DELETE FROM Companies WHERE CompanyId = @CompanyId", connection);
                deleteCompanyCommand.Parameters.AddWithValue("@CompanyId", companyId);
                deleteCompanyCommand.ExecuteNonQuery();
            }
        }


    }
}
