using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Api
{
    public class Repository
    {
        protected const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CrmDB;Integrated Security=True;";

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

        private T ExecuteScalar<T>(string query, Dictionary<string, object> parameters = null)
        {
            return ExecuteCommand(query, cmd => (T)cmd.ExecuteScalar(), parameters);
        }

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

        private void ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            ExecuteCommand(query, cmd =>
            {
                cmd.ExecuteNonQuery();
                return 0;
            }, parameters);
        }

        //////////////////////
        /// Users here
        ////////////////////

        public User GetUserById(int userId)
        {
            string query = "SELECT UserId, Username, Password FROM Users WHERE UserId = @UserId";
            var parameters = new Dictionary<string, object> { { "@UserId", userId } };

            var users = ExecuteReader(query, reader => new User
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            }, parameters);

            return users.Count > 0 ? users[0] : null;
        }

        public void AddUser(User user)
        {
            string query = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@Password", user.Password }
            };
            ExecuteNonQuery(query, parameters);
        }

        public User GetUserByCredentials(string username, string password)
        {
            string query = "SELECT UserId, Username, Password FROM Users WHERE Username = @Username AND Password = @Password";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", username },
                { "@Password", password }
            };

            var users = ExecuteReader(query, reader => new User
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            }, parameters);

            return users.Count > 0 ? users[0] : null;
        }

        //////////////////////
        /// Contacts here
        ////////////////////

        public Contact GetContactById(int contactId)
        {
            string query = "SELECT ContactId, ContactName FROM Contacts WHERE ContactId = @ContactId";
            var parameters = new Dictionary<string, object> { { "@ContactId", contactId } };

            var contacts = ExecuteReader(query, reader => new Contact
            {
                ContactId = reader.GetInt32(0),
                ContactName = reader.GetString(1)
            }, parameters);

            return contacts.Count > 0 ? contacts[0] : null;
        }

        public IEnumerable<Contact> GetAllContacts()
        {
            string query = "SELECT ContactId, ContactName, PhoneNumber, Email, CompanyId FROM Contacts";
            return ExecuteReader(query, reader => new Contact
            {
                ContactId = reader.GetInt32(0),
                ContactName = reader.GetString(1),
                PhoneNumber = reader.GetString(2),
                Email = reader.GetString(3),
                Company = new Company { Id = reader.GetInt32(4) }
            });
        }

        public void AddContact(Contact contact)
        {
            string query = "INSERT INTO Contacts (ContactName, PhoneNumber, Email, CompanyId) VALUES (@ContactName, @PhoneNumber, @Email, @CompanyId)";
            var parameters = new Dictionary<string, object>
            {
                { "@ContactName", contact.ContactName },
                { "@PhoneNumber", contact.PhoneNumber },
                { "@Email", contact.Email },
                { "@Id", contact.Company.Id }
            };
            ExecuteNonQuery(query, parameters);
        }

        //////////////////////
        /// Companies here
        ////////////////////

        // Method to get all contacts for a specific company
        public List<Contact> GetContactsByCompany(int companyId)
        {
            string query = "SELECT * FROM Contacts WHERE CompanyId = @CompanyId";
            var parameters = new Dictionary<string, object>
            {
                { "@CompanyId", companyId }
            };

            return ExecuteReader(query, reader => new Contact
            {
                ContactId = (int)reader["ContactId"],
                ContactName = reader["ContactName"].ToString(),
                PhoneNumber = reader["PhoneNumber"].ToString(),
                Email = reader["Email"].ToString(),
                // No need to set Company here, since you will fetch it based on the CompanyId later if needed
            }, parameters);
        }


        // Example method to delete a company from the database
        public void DeleteCompany(int companyId)
        {
            string query = "DELETE FROM Companies WHERE CompanyId = @CompanyId";

            var parameters = new Dictionary<string, object>
        {
            { "@CompanyId", companyId }
        };

            ExecuteNonQuery(query, parameters); // Call your method to execute the SQL command
        }



        public Company GetCompanyById(int companyId)
        {
            string query = "SELECT CompanyId, CompanyName FROM Companies WHERE CompanyId = @CompanyId";
            var parameters = new Dictionary<string, object> { { "@CompanyId", companyId } };

            var companies = ExecuteReader(query, reader => new Company
            {
                Id = reader.GetInt32(0),
                CompanyName = reader.GetString(1)
            }, parameters);

            return companies.Count > 0 ? companies[0] : null;
        }

        public IEnumerable<Company> GetAllCompanies()
        {
            string query = "SELECT CompanyId, CompanyName, Address, Industry, UserId FROM Companies";
            return ExecuteReader(query, reader => new Company
            {
                Id = reader.GetInt32(0),
                CompanyName = reader.GetString(1),
                Address = reader.GetString(2),
                Industry = reader.GetString(3),
                User = new User { UserId = reader.GetInt32(4) }
            });
        }

        public void AddCompany(Company company)
        {
            string query = "INSERT INTO Companies (CompanyName, Address, Industry, UserId) VALUES (@CompanyName, @Address, @Industry, @UserId)";

            var parameters = new Dictionary<string, object>
        {
            { "@CompanyName", company.CompanyName },
            { "@Address", company.Address },
            { "@Industry", company.Industry },
            { "@UserId", company.User.UserId } // Assuming User is not null
        };

            ExecuteNonQuery(query, parameters);
        }

        //////////////////////
        /// Meetings here
        ////////////////////

        public Meeting GetMeetingById(int meetingId)
        {
            string query = "SELECT MeetingId, MeetingName FROM Meetings WHERE MeetingId = @MeetingId";
            var parameters = new Dictionary<string, object> { { "@MeetingId", meetingId } };

            var meetings = ExecuteReader(query, reader => new Meeting
            {
                MeetingId = reader.GetInt32(0),
                MeetingName = reader.GetString(1)
            }, parameters);

            return meetings.Count > 0 ? meetings[0] : null;
        }

        // Method to add a meeting
        public void AddMeeting(Meeting meeting)
        {
            string query = "INSERT INTO Meetings (MeetingName, MeetingDescription, MeetingTime, UserId, ContactId) VALUES (@MeetingName, @MeetingDescription, @MeetingTime, @UserId, @ContactId)";

            var parameters = new Dictionary<string, object>
        {
            { "@MeetingName", meeting.MeetingName },
            { "@MeetingDescription", meeting.MeetingDescription },
            { "@MeetingTime", meeting.MeetingTime },
            { "@UserId", meeting.User.UserId }, // Assuming User is set correctly
            { "@ContactId", meeting.Contact.ContactId } // Assuming Contact is set correctly
        };

            ExecuteNonQuery(query, parameters);
        }

        public List<Meeting> GetMeetingsByDate(int userId, string dateString)
        {
            DateTime meetingDate;

            // Parse the date string to DateTime
            if (!DateTime.TryParseExact(dateString, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out meetingDate))
            {
                throw new ArgumentException("Invalid date format. Please use DD-MM-YYYY.");
            }

            string query = "SELECT * FROM Meetings WHERE UserId = @UserId AND CAST(MeetingTime AS DATE) = @MeetingDate";

            var parameters = new Dictionary<string, object>
    {
        { "@UserId", userId },
        { "@MeetingDate", meetingDate.Date } // Use .Date to match only the date part
    };

            return ExecuteReader(query, reader => new Meeting
            {
                MeetingId = (int)reader["MeetingId"],
                MeetingName = reader["MeetingName"].ToString(),
                MeetingDescription = reader["MeetingDescription"].ToString(),
                MeetingTime = (DateTime)reader["MeetingTime"],
                User = new User // If you need to map user info, do so here
                {
                    UserId = (int)reader["UserId"] // Adjust according to your database schema
                },
                Contact = new Contact // If you need to map contact info, do so here
                {
                    ContactId = (int)reader["ContactId"] // Adjust according to your database schema
                }
            }, parameters);
        }



        // Method to get all meetings for a user
        public IEnumerable<Meeting> GetAllMeetingsByUserId(int userId)
        {
            var meetings = new List<Meeting>();
            string query = "SELECT * FROM Meetings WHERE UserId = @UserId";

            var parameters = new Dictionary<string, object>
        {
            { "@UserId", userId }
        };

            using (var connection = new SqlConnection("YourConnectionStringHere"))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var meeting = new Meeting
                            {
                                MeetingId = (int)reader["MeetingId"],
                                MeetingName = reader["MeetingName"].ToString(),
                                MeetingDescription = reader["MeetingDescription"].ToString(),
                                MeetingTime = (DateTime)reader["MeetingTime"],
                                // You would need to fetch User and Contact details if necessary
                            };
                            meetings.Add(meeting);
                        }
                    }
                }
            }

            return meetings;
        }

        // Method to delete a meeting
        public void DeleteMeeting(int meetingId)
        {
            string query = "DELETE FROM Meetings WHERE MeetingId = @MeetingId";

            var parameters = new Dictionary<string, object>
        {
            { "@MeetingId", meetingId }
        };

            ExecuteNonQuery(query, parameters);
        }

        
    }
}
