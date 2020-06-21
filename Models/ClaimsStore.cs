using System.Collections.Generic;
using System.Security.Claims;

// this class can be replaced with database table
public static class ClaimsStore
{
    public static List<Claim> AllClaims = new List<Claim>()
    {
        // Type = 1st parameter, Value = 2nd parameter
        new Claim("Create Role", "Create Role"),
        new Claim("Edit Role","Edit Role"),
        new Claim("Delete Role","Delete Role")
    };
}