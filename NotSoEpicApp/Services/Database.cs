using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace NotSoEpicApp
{
    public static class Database
    {
        private static string connectionString;

        static Database()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            connectionString = config.GetConnectionString("DefaultConnection");
        }

        // Async method to save a transaction to the database
        public static async Task<bool> SaveTransactionToDatabase(Transaction transaction)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Transactions (title, description, amount, date, is_income, type, user_id) VALUES (@title, @description, @amount, @date, @is_income, @type, @user_id)";
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("title", transaction.Title);
                        command.Parameters.AddWithValue("description", transaction.Description);
                        command.Parameters.AddWithValue("amount", transaction.Amount);
                        command.Parameters.AddWithValue("date", transaction.Date);
                        command.Parameters.AddWithValue("is_income", transaction.IsIncome);
                        command.Parameters.AddWithValue("type", transaction.Type.ToString());
                        command.Parameters.AddWithValue("user_id", Login.CurrentUserId);

                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return false;
                }
            }
        }

        // Async method to load all transactions from the database
        public static async Task<List<Transaction>> LoadTransactionsFromDatabase(int userId)
        {
            List<Transaction> transactions = new List<Transaction>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT id, title, description, amount, date, is_income, type FROM Transactions WHERE user_id = @userId ORDER BY date DESC";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("userId", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                TransactionType transactionType = Enum.TryParse(reader.GetString(6), out TransactionType parsedType) ? parsedType : TransactionType.Other;

                                var transaction = new Transaction
                                {
                                    Title = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Amount = reader.GetDecimal(3),
                                    Date = reader.GetDateTime(4),
                                    IsIncome = reader.GetBoolean(5),
                                    Type = transactionType
                                };

                                transactions.Add(transaction);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return transactions;
        }


        // Async method to calculate totals for a period
        public static async Task<(decimal TotalIncome, decimal TotalExpenses)> CalculateTotalsForPeriod(DateTime startDate, DateTime endDate)
        {
           

            decimal totalIncome = 0;
            decimal totalExpenses = 0;

            foreach (var transaction in Login.transactions)
            {
                if (transaction.Date >= startDate && transaction.Date <= endDate)
                {
                    if (transaction.IsIncome)
                    {
                        totalIncome += transaction.Amount;
                    }
                    else
                    {
                        totalExpenses += transaction.Amount;
                    }
                }
            }

            return (totalIncome, totalExpenses);
        }




        public static async Task<bool> SaveUserToDatabase(user newUser)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO public.users (username, email, password_hash) VALUES (@name, @email, @passwordHash)";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("name", newUser.Name);
                        command.Parameters.AddWithValue("email", newUser.Email);
                        command.Parameters.AddWithValue("passwordHash", newUser.PasswordHash);
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return false;
                }
            }
        }




        public static async Task<int?> CheckUserCredentials(string username, byte[] passwordHash)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT id FROM public.users WHERE username = @username AND password_hash = @passwordHash";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("username", username);
                        command.Parameters.AddWithValue("passwordHash", passwordHash);

                        object result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return null;
                }
            }
        }

        public static async Task<bool> UpdateUserCurrentMoney(decimal currentMoneyUpdate)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "UPDATE public.users SET current_money = @currentMoney WHERE id = @userId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("currentMoney", currentMoneyUpdate);
                        command.Parameters.AddWithValue("userId", Login.CurrentUserId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return false;
                }
            }
        }

        public static async Task<decimal?> GetUserCurrentMoney()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT current_money FROM public.users WHERE id = @userId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("userId", Login.CurrentUserId);

                        object result = await command.ExecuteScalarAsync();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToDecimal(result);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return 0;
                }
            }
        }


        public static async Task<List<string>> GetAllOtherUsers()
        {
            List<string> usernames = new List<string>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT username FROM public.users WHERE id != @userId ORDER BY username";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("userId", Login.CurrentUserId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                usernames.Add(reader.GetString(0));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return usernames;
        }

        public static async Task<SupervisorInfo> GetSupervisorInfo(string selectedUser)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    // Query to check if the current user is supervised by the selected user
                    string query = @"
    SELECT charts_access, transaction_access, supervisors_access, transactions_add, supervisors_add
    FROM public.supervisors
    WHERE supervisor_id = (SELECT id FROM public.users WHERE username = @selectedUser)
    AND user_id = @currentUserId"; 

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("selectedUser", selectedUser);  // Supervisor
                        command.Parameters.AddWithValue("currentUserId", Login.CurrentUserId);  // Current user

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new SupervisorInfo
                                {
                                    AllowViewCharts = reader.GetBoolean(0),
                                    AllowViewTransactions = reader.GetBoolean(1),
                                    AllowViewSupervisors = reader.GetBoolean(2),
                                    AllowAddTransactions = reader.GetBoolean(3),
                                    AllowAddSupervisors = reader.GetBoolean(4)
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error retrieving supervisor information: {ex.Message}");
                }
            }

            return null;
        }

        // Adds or updates supervisor with correct supervisor and user assignment
        public static async Task<bool> AddOrUpdateSupervisor(string username, bool allowViewTransactions, bool allowViewSupervisors, bool allowViewCharts, bool allowAddTransactions, bool allowAddSupervisors)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string checkQuery = @"
SELECT COUNT(*)
FROM public.supervisors
WHERE user_id = @currentUserId AND supervisor_id = (SELECT id FROM public.users WHERE username = @username)";

                using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("username", username);  // Supervisor
                    checkCommand.Parameters.AddWithValue("currentUserId", Login.CurrentUserId);  // Current user

                    long count = (long)await checkCommand.ExecuteScalarAsync();

                    if (count > 0)
                    {
                        await UpdateSupervisor(username, allowViewTransactions, allowViewSupervisors, allowViewCharts, allowAddTransactions, allowAddSupervisors);
                        return true; // was update
                    }
                    else
                    {
                        string insertQuery = @"
        INSERT INTO public.supervisors (user_id, supervisor_id, transaction_access, supervisors_access, charts_access, transactions_add, supervisors_add)
        VALUES (@currentUserId, (SELECT id FROM public.users WHERE username = @username), @allowViewTransactions, @allowViewSupervisors, @allowViewCharts, @transactions_add, @supervisors_add)";

                        using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("username", username);  // Supervisor
                            insertCommand.Parameters.AddWithValue("currentUserId", Login.CurrentUserId);  // Current user
                            insertCommand.Parameters.AddWithValue("allowViewTransactions", allowViewTransactions);
                            insertCommand.Parameters.AddWithValue("allowViewSupervisors", allowViewSupervisors);
                            insertCommand.Parameters.AddWithValue("allowViewCharts", allowViewCharts);
                            insertCommand.Parameters.AddWithValue("transactions_add", allowAddTransactions);
                            insertCommand.Parameters.AddWithValue("supervisors_add", allowAddSupervisors);

                            await insertCommand.ExecuteNonQueryAsync();
                        }
                        return false;
                    }
                }
            }
        }

        public static async Task UpdateSupervisor(string username, bool allowViewTransactions, bool allowViewSupervisors, bool allowViewCharts, bool allowAddTransactions, bool allowAddSupervisors)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = @"
UPDATE public.supervisors
SET transaction_access = @allowViewTransactions, supervisors_access = @allowViewSupervisors, charts_access = @allowViewCharts, transactions_add = @transactions_add, supervisors_add = @supervisors_add
WHERE user_id = @currentUserId AND supervisor_id = (SELECT id FROM public.users WHERE username = @username)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("username", username);  // Supervisor
                    command.Parameters.AddWithValue("currentUserId", Login.CurrentUserId);  // Current user
                    command.Parameters.AddWithValue("allowViewTransactions", allowViewTransactions);
                    command.Parameters.AddWithValue("allowViewSupervisors", allowViewSupervisors);
                    command.Parameters.AddWithValue("allowViewCharts", allowViewCharts);
                    command.Parameters.AddWithValue("transactions_add", allowAddTransactions);
                    command.Parameters.AddWithValue("supervisors_add", allowAddSupervisors);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // Deletes a supervisor if the relationship is removed
        public static async Task DeleteSupervisor(string username)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = @"
DELETE FROM public.supervisors
WHERE user_id = @currentUserId AND supervisor_id = (SELECT id FROM public.users WHERE username = @username)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("username", username);  // Supervisor
                    command.Parameters.AddWithValue("currentUserId", Login.CurrentUserId);  // Current user

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task<List<SupervisedUser>> GetSupervisedUsersWithPermissions()
        {
            List<SupervisedUser> users = new List<SupervisedUser>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string query = @"
SELECT u.username, s.charts_access, s.transaction_access, s.supervisors_access, s.transactions_add, s.supervisors_add
FROM public.supervisors s
JOIN public.users u ON s.user_id = u.id
WHERE s.supervisor_id = @currentUserId
ORDER BY u.username";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("currentUserId", Login.CurrentUserId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(new SupervisedUser
                                {
                                    Username = reader.GetString(0),
                                    AllowViewCharts = reader.GetBoolean(1),
                                    AllowViewTransactions = reader.GetBoolean(2),
                                    AllowViewSupervisors = reader.GetBoolean(3),
                                    AllowAddTransactions = reader.GetBoolean(4),
                                    AllowAddSupervisors = reader.GetBoolean(5)
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving supervised users: {ex.Message}");
                }
            }

            return users;
        }



    }
}
