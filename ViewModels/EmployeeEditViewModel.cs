using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagement.Models
{
    public class EmployeeEditViewModel : EmployeeCreateViewModel
    {
        public int Id { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}