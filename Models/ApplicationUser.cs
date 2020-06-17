using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // add City column to the user table
    public string City { get; set; }
}