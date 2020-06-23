using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

// create custom AuthorizationHandler<T> where T is our requirement
public class CanEditOnlyOtherAdminRolesAndClaimsHandler :
    AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
    {
        // resouce will return the action methods we are trying to protect
        var authFilterContext = context.Resource as AuthorizationFilterContext;
        if (authFilterContext == null)
        {
            return Task.CompletedTask;
        }

        // get current logged-in user id
        string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

        // get the admin id that is being edited
        string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];

        // if the current logged-in user is Admin and has Edit Role claim
        // AND not editing its own role
        // then move on
        // otherwise, do not move on
        if (context.User.IsInRole("admin") &&
            context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
            adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}