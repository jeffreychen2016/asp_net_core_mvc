using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        [Route("[action]")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}