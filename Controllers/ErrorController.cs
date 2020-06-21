using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[AllowAnonymous]
public class ErrorController : Controller
{
    private ILogger<ErrorController> _logger;

    // specify ErrorController in ILogger
    // so that the error will be grouped.
    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    // If there is 404 status code, the route path will become Error/404
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        switch (statusCode)
        {
            case 404:

                ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                // ViewBag.Path = statusCodeResult.OriginalPath;
                // ViewBag.QS = statusCodeResult.OriginalQueryString;
                _logger.LogWarning($"404 Error Occured. Path = {statusCodeResult.OriginalPath} + and QueryString = {statusCodeResult.OriginalQueryString}");
                break;
        }

        return View("NotFound");
    }

    // handle unhandled exception from other action methods
    [AllowAnonymous]
    [Route("Error")]
    public IActionResult Error()
    {
        var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        ViewBag.ExceptionPath = exceptionDetails.Path;
        ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
        ViewBag.Stacktrace = exceptionDetails.Error.StackTrace;

        // in real world, we should log it internally instead of print it to user.
        // return View("Error");

        // log exception details
        _logger.LogError($"The path {exceptionDetails.Path} threw an exception {exceptionDetails.Error}");

        return View("Error");
    }
}