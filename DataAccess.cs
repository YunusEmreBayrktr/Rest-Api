using System.Data.SQLite;
using System.Text;

public class DataAccess
{
    private readonly string connectionString = "Data Source=SampleDB.db;Version=3;";



    public List<Employee> GetAllEmployees(){

        List<Employee> employees = new List<Employee>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Employee";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            DateOfBirth = reader["DateOfBirth"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Salary = Convert.ToInt32(reader["Salary"]),
                        };
                        employees.Add(employee);
                    }
                }
            }
        }

        return employees;
    }



    public Employee GetEmployee(int id){

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Employee WHERE ID = @Id";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Employee
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            DateOfBirth = reader["DateOfBirth"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Salary = Convert.ToInt32(reader["Salary"]),
                        };
                    }
                    else
                    {
                        return null; // Employee not found
                    }
                }
            }
        }
    }



    public void CreateEmployee(Employee newEmployee){

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO Employee (FirstName, LastName, DateOfBirth, Gender, Salary) " +
                           "VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @Salary)";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@FirstName", newEmployee.FirstName);
                command.Parameters.AddWithValue("@LastName", newEmployee.LastName);
                command.Parameters.AddWithValue("@DateOfBirth", newEmployee.DateOfBirth);
                command.Parameters.AddWithValue("@Gender", newEmployee.Gender);
                command.Parameters.AddWithValue("@Salary", newEmployee.Salary);

                // Execute the query
                command.ExecuteNonQuery();
            }
        }
    }



    public bool UpdateEmployee(int id, Dictionary<string, object> updatedFields){

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            StringBuilder queryBuilder = new StringBuilder("UPDATE Employee SET ");

            foreach (var field in updatedFields)
            {
                queryBuilder.Append($"{field.Key} = @{field.Key}, ");
            }

            queryBuilder.Remove(queryBuilder.Length - 2, 2); // Remove the last comma and space
            queryBuilder.Append($" WHERE ID = @Id");

            string query = queryBuilder.ToString();

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                foreach (var field in updatedFields)
                {
                    command.Parameters.AddWithValue($"@{field.Key}", field.Value);
                }

                // Execute the update query
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
    }


    public bool DeleteEmployee(int id){
        
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "DELETE FROM Employee WHERE ID = @Id";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                // Execute the delete query
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
    }

}