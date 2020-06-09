using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // we need to create property for each type in our project
        // this will be used to query and save employee data
        public DbSet<Employee> Employees { get; set; }
    }
}