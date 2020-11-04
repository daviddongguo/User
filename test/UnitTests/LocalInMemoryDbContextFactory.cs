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

            var usersList = new List<User>{
                new User { Id = "sis", Email="sis@user.com", PasswordHash = passwordHash, PasswordSalt = passwordSalt },
                new User { Id = "sisempty", Email="", PasswordHash = passwordHash, PasswordSalt = passwordSalt },
                new User { Id = "admin", Name = "admin", Email="admin@user.com", PasswordHash = passwordHash, PasswordSalt = passwordSalt }

                };
            for (int i = 0; i < 20; i++)
            {
                usersList.Add(new User
                {
                    Id = "000sis" + i + 123,
                    Name = "Sis000" + i,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                });
            }


            dbContext.Users.AddRange(usersList);

            dbContext.SaveChangesAsync().GetAwaiter();
        }

    }
}
