using learn.it.Models;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Database
{
    public static class DataSeeder
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = 1, Name = "Admin" },
                new Permission { PermissionId = 2, Name = "User" }
                );
        }
    }
}