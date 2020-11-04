using Microsoft.EntityFrameworkCore;
using user.Models;

namespace user.Services
{
    public class UserDbContext: DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            Utility.CreatePasswordHash("123", out byte[] passwordHash, out byte[] passwordSalt);

            modelBuilder.Entity<User>().HasData(
                new User { Id = "sis", Email="sis@user.com", PasswordHash = passwordHash, PasswordSalt = passwordSalt },
                new User { Id = "sisempty", Email="", PasswordHash = passwordHash, PasswordSalt = passwordSalt },
                new User { Id = "admin", Name = "admin", Email="admin@user.com", PasswordHash = passwordHash, PasswordSalt = passwordSalt }
            );
        }

    }
}
