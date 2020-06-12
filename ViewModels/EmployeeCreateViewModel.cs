using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagement.Models
{
    public class EmployeeCreateViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Dept? Department { get; set; }

        // it is used for upload image file
        public IFormFile Photo { get; set; }
    }
}