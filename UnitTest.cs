using Xunit;
using System.Data.SQLite;


public class DataAccessTests : IDisposable
{
    private readonly string testDbPath;
    private readonly string connectionString;
    private readonly DataAccess dataAccess;

    public DataAccessTests(){
        // Temporary test database
        testDbPath = Path.GetTempFileName();
        connectionString = $"Data Source={testDbPath};Version=3;";
        dataAccess = new DataAccess(connectionString);

        InitializeDatabase();
    }



    private void InitializeDatabase(){
        // Create Employee table
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = @"CREATE TABLE Employee (
                                ID INTEGER PRIMARY KEY,
                                FirstName TEXT,
                                LastName TEXT,
                                DateOfBirth TEXT,
                                Gender TEXT,
                                Salary INTEGER
                            );";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }



    public void Dispose(){
        // Clean up resources
        File.Delete(testDbPath);
    }



    [Fact]
    public void GetAllEmployees_ReturnsAllEmployees(){
        // Arrange
        dataAccess.CreateEmployee(new Employee { FirstName = "John", LastName = "Doe", DateOfBirth = "1990-01-01", Gender = "Male", Salary = 50000 });
        dataAccess.CreateEmployee(new Employee { FirstName = "Jane", LastName = "Doe", DateOfBirth = "1995-01-01", Gender = "Female", Salary = 60000 });

        // Act
        List<Employee> employees = dataAccess.GetAllEmployees();

        // Assert
        Assert.Equal(2, employees.Count);
    }



    [Fact]
    public void GetEmployee_WithValidId_ReturnsEmployee(){
        // Arrange
        dataAccess.CreateEmployee(new Employee { FirstName = "John", LastName = "Doe", DateOfBirth = "1990-01-01", Gender = "Male", Salary = 50000 });
        int employeeId = 1;

        // Act
        Employee employee = dataAccess.GetEmployee(employeeId);

        // Assert
        Assert.NotNull(employee);
        Assert.Equal("John", employee.FirstName);
    }



    [Fact]
    public void GetEmployee_WithInvalidId_ReturnsNull(){
        // Arrange
        int employeeId = 999;

        // Act
        Employee employee = dataAccess.GetEmployee(employeeId);

        // Assert
        Assert.Null(employee);
    }



    [Fact]
    public void CreateEmployee_AddsNewEmployee(){
        // Arrange
        Employee newEmployee = new Employee { FirstName = "Test", LastName = "User", DateOfBirth = "2000-01-01", Gender = "Male", Salary = 70000 };

        // Act
        dataAccess.CreateEmployee(newEmployee);
        Employee retrievedEmployee = dataAccess.GetEmployee(1);

        // Assert
        Assert.NotNull(retrievedEmployee);
        Assert.Equal(newEmployee.FirstName, retrievedEmployee.FirstName);
    }



    [Fact]
    public void UpdateEmployee_WithValidId_UpdatesEmployee(){
        // Arrange
        Employee newEmployee = new Employee { FirstName = "Test", LastName = "User", DateOfBirth = "2000-01-01", Gender = "Male", Salary = 70000 };
        dataAccess.CreateEmployee(newEmployee);

        Dictionary<string, object> updatedFields = new Dictionary<string, object>
        {
            { "FirstName", "UpdatedTest" }
        };

        // Act
        bool result = dataAccess.UpdateEmployee(1, updatedFields);
        Employee updatedEmployee = dataAccess.GetEmployee(1);

        // Assert
        Assert.True(result);
        Assert.Equal("UpdatedTest", updatedEmployee.FirstName);
    }



    [Fact]
    public void DeleteEmployee_WithValidId_DeletesEmployee(){
        // Arrange
        Employee newEmployee = new Employee { FirstName = "Test", LastName = "User", DateOfBirth = "2000-01-01", Gender = "Male", Salary = 70000 };
        dataAccess.CreateEmployee(newEmployee);

        // Act
        bool result = dataAccess.DeleteEmployee(1);
        Employee deletedEmployee = dataAccess.GetEmployee(1);

        // Assert
        Assert.True(result);
        Assert.Null(deletedEmployee);
    }
}
