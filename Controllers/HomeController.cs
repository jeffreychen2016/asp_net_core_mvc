using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
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
        public ViewResult Details()
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

            // ViewModel approach
            // use ViewModel to include extra data that Employee model does not have
            // such as PageTitle
            var homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(1),
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);
        }
    };
}