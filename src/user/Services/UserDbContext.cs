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
                new User { Id = "sis", Name = "sis", PasswordHash = passwordHash, PasswordSalt = passwordSalt },
                new User { Id = "puppy", Name = "puppy", PasswordHash = passwordHash, PasswordSalt = passwordSalt }
            );

        }

    }
}
