using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Name can not be more than 50 characters.")]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
        ErrorMessage = "Invalid email format")]
        [Display(Name = "Office Email")]
        public string Email { get; set; }

        // use ? to make Department optional
        // Dept is an enum so in order to make optional
        // we have to make it nullable
        // by default, the Numeric type is always required
        // so, we do not need [required] 
        public Dept Department { get; set; }
        // public Dept? Department { get; set; }

    }
}