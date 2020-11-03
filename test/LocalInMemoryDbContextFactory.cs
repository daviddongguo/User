using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using user.Models;
using user.Services;

namespace test
{
    public class LocalInMemoryDbContextFactory
    {
        public UserDbContext GetUserContext()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                        .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                        .Options;
            var dbContext = new UserDbContext(options);

            Seed(dbContext);

            return dbContext;

        }

        private void Seed(UserDbContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            Utility.CreatePasswordHash("123", out byte[] passwordHash, out byte[] passwordSalt);

            dbContext.Users.AddRange(new List<User>{
            new User { Id = "sis", Name = "sis", PasswordHash = passwordHash, PasswordSalt = passwordSalt },
            new User { Id = "puppy", Name = "puppy", PasswordHash = passwordHash, PasswordSalt = passwordSalt }
            });

            dbContext.SaveChangesAsync().GetAwaiter();
        }

    }
}
