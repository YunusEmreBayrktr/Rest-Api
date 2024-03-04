using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
DataAccess dataAccess = new DataAccess();



app.MapGet("/", () => @"REST API to perform Create, Read, Update and Delete methods.


                                        ***Usage***

+--------+----------------------+-----------------------------+---------------+----------------+
|        | API                  | Description                 | Request Body  | Response Body  |
+--------+----------------------+-----------------------------+---------------+----------------+
| CREATE | /AddEmployee         | Add a new employee          | None          | Employee item  |
+--------+----------------------+-----------------------------+---------------+----------------+
| READ   | /ListEmployee/{id}   | Get employee by id          | None          | Employee item  |
+--------+----------------------+-----------------------------+---------------+----------------+
| UDATE  | /UpdateEmployee/{id} | Update an existing employee | None          | Employee item  |
+--------+----------------------+-----------------------------+---------------+----------------+
| DELETE | /DeleteEmployee/{id} | Delete an employee          | None          | None           |
+--------+----------------------+-----------------------------+---------------+----------------+
| LIST   | /ListEmployee        | List all employees          | None          | Employee items |
+--------+----------------------+-----------------------------+---------------+----------------+


Example Employee object:

{
  ""ID"": 23,
  ""FirstName"": ""Emre"",
  ""LastName"": ""Bayraktar"",
  ""DateOfBirth"": ""2001-08-16"",
  ""Gender"": ""Male"",
  ""Salary"": 66000
}


http://localhost:5000/AddEmployee?FirstName=Emre&LastName=Bayraktar&DateOfBirth=2001-08-16&Gender=Male&Salary=66000

");


// To get all employees
app.MapGet("/ListEmployee", () => {

    List<Employee> allEmployees = dataAccess.GetAllEmployees();
    return JsonSerializer.Serialize(allEmployees, new JsonSerializerOptions { WriteIndented = true });
});



// To get specified employee
app.MapGet("/ListEmployee/{id}", async (HttpContext context, int id) => {

    var employee = dataAccess.GetEmployee(id);

    if (employee == null)
    {
        context.Response.StatusCode = 404; // Not Found
        await context.Response.WriteAsync("Employee not found");
        return;
    }

    // Return the employee data
    context.Response.Headers.Add("Content-Type", "application/json");

    var jsonResponse = JsonSerializer.Serialize(employee, new JsonSerializerOptions{WriteIndented = true});

    await context.Response.WriteAsync(jsonResponse);
});


// To add a new employee
app.MapGet("/AddEmployee", async (HttpContext context) =>
{
    // Retrieve employee details
    var firstName = context.Request.Query["FirstName"].ToString();
    var lastName = context.Request.Query["LastName"].ToString();
    var dateOfBirth = context.Request.Query["DateOfBirth"].ToString();
    var gender = context.Request.Query["Gender"].ToString();
    var salaryStr = context.Request.Query["Salary"].ToString();

    int salary = int.TryParse(salaryStr, out int parsedSalary) ? parsedSalary : 0;

    // Create a new employee object
    var newEmployee = new Employee
    {
        FirstName = firstName,
        LastName = lastName,
        DateOfBirth = dateOfBirth,
        Gender = gender,
        Salary = salary
    };

    // Save the new employee data
    dataAccess.CreateEmployee(newEmployee);

    // Return the created employee data
    context.Response.StatusCode = 201; // Created
    context.Response.Headers.Add("Content-Type", "application/json");

    var jsonResponse = JsonSerializer.Serialize(newEmployee, new JsonSerializerOptions { WriteIndented = true });

    await context.Response.WriteAsync(jsonResponse);
});




app.MapGet("/UpdateEmployee/{id}", async (HttpContext context, int id) =>
{
    // Retrieve update fields
    var updatedFields = new Dictionary<string, object>();

    foreach (var queryParameter in context.Request.Query)
    {
        updatedFields.Add(queryParameter.Key, queryParameter.Value);
    }

    // Update the employee data
    var isSuccess = dataAccess.UpdateEmployee(id, updatedFields);

    if (!isSuccess)
    {
        context.Response.StatusCode = 404; // Not Found
        await context.Response.WriteAsync("Employee not found");
        return;
    }

    // Return the updated employee data
    context.Response.Headers.Add("Content-Type", "application/json");

    var updatedEmployee = dataAccess.GetEmployee(id);

    var jsonResponse = JsonSerializer.Serialize(updatedEmployee, new JsonSerializerOptions { WriteIndented = true });

    await context.Response.WriteAsync(jsonResponse);
});




// To delete specified employee
app.MapGet("/DeleteEmployee/{id}", async (HttpContext context, int id) =>
{
    // Delete the employee
    var isSuccess = dataAccess.DeleteEmployee(id);

    if (!isSuccess)
    {
        context.Response.StatusCode = 404; // Not Found
        await context.Response.WriteAsync("Employee not found");
        return;
    }

    // Return a response indicating successful deletion
    context.Response.StatusCode = 200; // OK 
    await context.Response.WriteAsync("Employee deleted successfully");
});



app.Run();