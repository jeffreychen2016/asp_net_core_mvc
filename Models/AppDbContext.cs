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

        // when running db imigration, tables will be created for each of these properties
        public DbSet<Employee> Employees { get; set; }

        // seed/initialize data in the database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // create and use modlBuilder extension method to seed data
            modelBuilder.Seed();
        }
    }
}