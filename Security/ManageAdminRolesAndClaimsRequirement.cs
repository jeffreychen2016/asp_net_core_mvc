using Microsoft.AspNetCore.Authorization;

// to create custom authorization requirement
// we must inherit from IAuthorizationRequirement
public class ManageAdminRolesAndClaimsRequirement : IAuthorizationRequirement
{ }