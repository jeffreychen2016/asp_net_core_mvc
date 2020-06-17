using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // we need to create property for each type in our project
        // this will be used to query and save employee data

        // when running db imigration, tables will be created for each of these properties
        public DbSet<Employee> Employees { get; set; }

        // seed/initialize data in the database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // calling the method on the base class 
            // to fix the error:
            // The entity type 'IdentityUserLogin<string>' requires a primary key to be defined. If you intended to use a keyless entity type call 'HasNoKey()'.
            base.OnModelCreating(modelBuilder);

            // create and use modlBuilder extension method to seed data
            modelBuilder.Seed();
        }
    }
}