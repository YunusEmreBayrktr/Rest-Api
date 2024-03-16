public class BusinessLayer
{
    private readonly DataAccess _dataAccess;

    public BusinessLayer(DataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public List<Employee> GetAllEmployees()
    {
        return _dataAccess.GetAllEmployees();
    }

    public bool CreateEmployee(Employee newEmployee)
    {

        if (!IsOverAge18(newEmployee.DateOfBirth)){
            return false;
        }
        else{
            _dataAccess.CreateEmployee(newEmployee);
            return true;
        }
    }

    private bool IsOverAge18(string dateOfBirthString)
    {
        // Convert date string to DateTime
        if (!DateTime.TryParse(dateOfBirthString, out DateTime dateOfBirth)){
            return false;
        }

        // Calculate age
        int age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Today.AddYears(-age)){
            age--;
        }

        return age >= 18;
    }
}

