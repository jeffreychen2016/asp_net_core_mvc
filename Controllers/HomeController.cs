using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Controllers
{
    // Attribute Routing
    // [Route("Home")]
    // [Route("[controller]/[action]")] // we can use token all in controller level so that we do not have specify the [action] for each methods down below.
    [Route("[controller]")] // use token here so when we rename controller, we do not have to rename the route
    [Authorize]
    public class HomeController : Controller
    {
        // make readonly to prevent accidental mutating
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly IDataProtector _protector;

        public HomeController(IEmployeeRepository employeeRepository,
            IHostingEnvironment hostingEnvironment,
            ILogger<HomeController> logger,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;

            // add it to encrypt and decrypt query string
            _protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // use using statement to properly dispose the resource
                // so that we can delete the photo later
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        // [Route("Index")] // /home/index
        [Route("/")] // root
        [Route("")] // /home
        [Route("[action]")] // /home/index
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee()
                        .Select(e =>
                        {
                            // encrypt employee id
                            e.EncryptedId = _protector.Protect(e.Id.ToString());
                            return e;
                        });
            return View(model);
        }

        // JsonResult does not respect content negiciation
        // meaning that it will always return json
        // use ObjectResult instead if you do not the above behavior, it return json by default
        // chain AddXmlSerializerFormatters() after AddMvc() in DI container to reutrn xml.
        // [Route("Details/{id?}")] // /home/details/1
        [Route("[action]/{id?}")] // /home/details/1
        public ViewResult Details(string id)
        {
            // throw new Exception("aaaaa");
            // _logger.LogTrace("Trace Log");
            // _logger.LogDebug("Debug Log");
            // _logger.LogInformation("Information Log");
            // _logger.LogWarning("Warning Log");
            // _logger.LogError("Error Log");
            // _logger.LogCritical("Critical Log");

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

            // decrypt the id
            int employeeId = Convert.ToInt32(_protector.Unprotect(id));

            // check if the employee exits for the incoming id
            // if not, redirect to EmployeeNotFound page.
            Employee employee = _employeeRepository.GetEmployee(employeeId);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", employeeId);
            }

            var homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
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
            // when there is no validation error
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);

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

            // if there is validation error, then we want to re-render the same page
            // so the user can fix the error
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // get employee from db
                // and then change the property of the employee using udpated info
                // model contains the new employee info
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                // is the Photo is not null, means the user uploaded a new photo
                if (model.Photo != null)
                {
                    // delete existing photo
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                // save the new employee to db
                _employeeRepository.Update(employee);

                return RedirectToAction("index");
            }

            return View(model);
        }
    };
}