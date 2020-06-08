using System.Collections.Generic;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    // Attribute Routing
    // [Route("Home")]
    // [Route("[controller]/[action]")] // we can use token all in controller level so that we do not have specify the [action] for each methods down below.
    [Route("[controller]")] // use token here so when we rename controller, we do not have to rename the route
    public class HomeController : Controller
    {
        // make readonly to prevent accidental mutating
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // [Route("Index")] // /home/index
        [Route("/")] // root
        [Route("")] // /home
        [Route("[action]")] // /home/index
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        // JsonResult does not respect content negiciation
        // meaning that it will always return json
        // use ObjectResult instead if you do not the above behavior, it return json by default
        // chain AddXmlSerializerFormatters() after AddMvc() in DI container to reutrn xml.
        // [Route("Details/{id?}")] // /home/details/1
        [Route("[action]/{id?}")] // /home/details/1
        public ViewResult Details(int id)
        {
            // 1. ViewData approach. Not recommended
            // Employee model = _employeeRepository.GetEmployee(1);
            // ViewData["Employee"] = model;
            // ViewData["PageTitle"] = "Employee Details";
            // return View();

            // 2. ViewBag approach, Not recommended
            // Employee model = _employeeRepository.GetEmployee(1);
            // ViewBag.Employee = model;
            // ViewBag.PageTitle = "Employee Details";
            // return View()

            // 3. Strongly typed view approach, recommended
            // Employee model = _employeeRepository.GetEmployee(1);
            // ViewBag.PageTitle = "Employee Details";
            // return View(model);

            // 4. ViewModel approach
            // use ViewModel to include extra data that Employee model does not have
            // such as PageTitle
            var homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id),
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);
        }


        // *** we need HttpGet to display form first
        [Route("[action]")]
        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        // *** and HttpPost to handle the form submission
        [Route("[action]")]
        [HttpPost]
        public RedirectToActionResult Create(Employee employee)
        {
            Employee newEmployee = _employeeRepository.Add(employee);
            return RedirectToAction("details", new { id = newEmployee.Id });
        }
    };
}