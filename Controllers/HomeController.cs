using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        // make readonly to prevent accidental mutating
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public string Index()
        {
            return _employeeRepository.GetEmployee(1).Name;
        }

        // JsonResult does not respect content negiciation
        // meaning that it will always return json
        // use ObjectResult instead if you do not the above behavior, it return json by default
        // chain AddXmlSerializerFormatters() after AddMvc() in DI container to reutrn xml.
        public ObjectResult Details()
        {
            Employee model = _employeeRepository.GetEmployee(1);
            return new ObjectResult(model);
        }
    };
}