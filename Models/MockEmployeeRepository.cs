using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        // mock data from database
        private List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee() {Id = 1, Name = "Mary", Department = "HR", Email= "mary@google.com"},
                new Employee() {Id = 2, Name = "Jim", Department = "IT", Email= "jim@google.com"},
                new Employee() {Id = 3, Name = "Keth", Department = "IT", Email= "keth@google.com"}
            };
        }

        // method that interact with database
        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == Id);
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeeList;
        }

    }
}