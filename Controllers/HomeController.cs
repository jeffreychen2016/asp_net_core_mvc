using System;
using System.Collections.Generic;
using System.IO;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository, IHostingEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
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

        [Route("[action]")]
        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(employeeEditViewModel);
        }

        // *** and HttpPost to handle the form submission
        [Route("[action]")]
        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photo != null)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }


                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                // save the new employee to db
                _employeeRepository.Add(newEmployee);

                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }
    };
}